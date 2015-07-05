namespace HardwareToSerialWriter.Viewmodels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Annotations;
    using CommandHandlers;
    using OpenHardwareMonitor.Hardware;

    public class MainViewModel : INotifyPropertyChanged
    {
        private const bool SHALL_USE_SERIAL_PORT = true;

        private readonly PerformanceCounter _performanceCounter = new PerformanceCounter();
        private readonly Computer _myComputer;
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _gpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _cpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        private readonly DispatcherTimer _displayHardwareValuesTimer;
        private readonly DispatcherTimer _clearDisplayTimer;

        private readonly SerialPort _destinationSerialPort = new SerialPort();

        //
        // Bound properties
        //
        public bool SerialCommsActive { get { return SHALL_USE_SERIAL_PORT; } }

        private bool _isDisplaying;
        public bool IsDisplaying
        {
            get { return _isDisplaying; }
            set
            {
                if (value.Equals(_isDisplaying))
                {
                    return;
                }
                _isDisplaying = value;
                OnPropertyChanged("IsDisplaying");
            }
        }

        private string _textOutput;
        public string TextOutput
        {
            get { return _textOutput; }
            set
            {
                if (value == _textOutput)
                {
                    return;
                }
                _textOutput = value;
                OnPropertyChanged("TextOutput");
            }
        }

        private string _customText;
        public string CustomText
        {
            get { return _customText; }
            set
            {
                if (value == _customText)
                {
                    return;
                }
                _customText = value;
                OnPropertyChanged("CustomText");
            }
        }

        private string _selectedPort;
        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _selectedPort = value;
                _destinationSerialPort.PortName = _selectedPort;
            }
        }

        private readonly ObservableCollection<string> _comPortNames = new ObservableCollection<string>();
        public ObservableCollection<string> ComPortNames
        {
            get { return _comPortNames; }
        }

        private double _updateFrequency;
        public double UpdateFrequency
        {
            get { return _updateFrequency; }
            set
            {
                _updateFrequency = value;
                _displayHardwareValuesTimer.Interval = TimeSpan.FromSeconds(value);
            }
        }

        private readonly ObservableCollection<double> _updateFrequencies = new ObservableCollection<double>();
        public ObservableCollection<double> UpdateFrequencies
        {
            get { return _updateFrequencies; }
        }
        //
        // /Bound properties
        //

        //
        // Commands
        //
        private ICommand _startDisplay;
        public ICommand StartDisplay
        {
            get
            {
                return _startDisplay ?? (_startDisplay = new CommandHandler(StartDisplayHandler));
            }
        }

        private ICommand _stopDisplay;
        public ICommand StopDisplay
        {
            get
            {
                return _stopDisplay ?? (_stopDisplay = new CommandHandler(StopDisplayHandler));
            }
        }

        private ICommand _connectComAndTest;
        public ICommand ConnectComAndTest
        {
            get
            {
                return _connectComAndTest ?? (_connectComAndTest = new CommandHandler(ConnectComAndTestHandler));
            }
        }
        private ICommand _sendCustomText;

        public ICommand SendCustomText
        {
            get
            {
                return _sendCustomText ?? (_sendCustomText = new StringCommandHandler(SendCustomTextHandler));
            }
        }
        //
        // /Commands
        //

        public MainViewModel()
        {
            _displayHardwareValuesTimer = new DispatcherTimer();
            _clearDisplayTimer = new DispatcherTimer();

            _myComputer = new Computer
            {
                GPUEnabled = true,
                CPUEnabled = true
            };
            _myComputer.Open();

            ConfigureComponents();

            ResetSerialPort();

            GetGpuTemps();
            GetCpuTemps();
        }

        private void ConfigureComponents()
        {
            _performanceCounter.CategoryName = "processor";
            _performanceCounter.CounterName = "% Processor Time";
            _performanceCounter.InstanceName = "_Total";

            _displayHardwareValuesTimer.Tick += (sender, args) => UpdateDisplay();
            _displayHardwareValuesTimer.Interval = TimeSpan.FromMilliseconds(500);

            _clearDisplayTimer.Interval = TimeSpan.FromSeconds(4);
            _clearDisplayTimer.Tick += (sender, args) =>
            {
                ClearDisplay();
                _clearDisplayTimer.Stop();
            };

            var portNames = SerialPort.GetPortNames();

            foreach (var portName in portNames)
            {
                _comPortNames.Add(portName);
            }

            foreach (var frequency in new List<double> { 0.5, 1, 5, 10 })
            {
                _updateFrequencies.Add(frequency);
            }

            IsDisplaying = false;
            TextOutput = string.Empty;
            SelectedPort = _comPortNames.FirstOrDefault() ?? "No ports found :(";
        }

        private void StartDisplayHandler()
        {
            _displayHardwareValuesTimer.Start();
            UpdateDisplay();

            IsDisplaying = true;
        }

        private void StopDisplayHandler()
        {
            _displayHardwareValuesTimer.Stop();

            ClearDisplay();
            WriteSerial("Monitoring" + "*" + "stopped!");

            _clearDisplayTimer.Start();
            IsDisplaying = false;
        }

        private void ResetSerialPort()
        {
            if (SHALL_USE_SERIAL_PORT)
            {
                try
                {
                    _destinationSerialPort.Close();
                    _destinationSerialPort.PortName = SelectedPort;
                    _destinationSerialPort.BaudRate = 9600;
                    _destinationSerialPort.DataBits = 8;
                    _destinationSerialPort.Parity = Parity.None;
                    _destinationSerialPort.StopBits = StopBits.One;
                    _destinationSerialPort.Handshake = Handshake.None;
                    _destinationSerialPort.Encoding = System.Text.Encoding.Default;
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Error resetting port: {0} ({1})", e.Message, e.GetType()));
                    return;
                }
            }
        }

        private void GetGpuTemps()
        {
            var gpus = from hardwareItem in _myComputer.Hardware
                       where hardwareItem.HardwareType == HardwareType.GpuAti | hardwareItem.HardwareType == HardwareType.GpuNvidia
                       select hardwareItem;

            foreach (IHardware hardwareItem in gpus)
            {
                var tempSensors = from sensor in hardwareItem.Sensors
                                  where sensor.SensorType == SensorType.Temperature
                                  select sensor;

                _gpuTempByHardware.Add(hardwareItem, tempSensors);
            }
        }

        private void GetCpuTemps()
        {
            var cpus = from hardwareItem in _myComputer.Hardware
                       where hardwareItem.HardwareType == HardwareType.CPU
                       select hardwareItem;

            foreach (var hardwareItem in cpus)
            {
                var tempSensors = from sensor in hardwareItem.Sensors
                                  where sensor.SensorType == SensorType.Temperature
                                  select sensor;

                _cpuTempByHardware.Add(hardwareItem, tempSensors);
            }
        }

        /// <summary>
        /// This is the main method updating the values displayed. Modify stuff in here.
        /// </summary>
        private void UpdateDisplay()
        {
            foreach (var hardware in _cpuTempByHardware.Keys)
            {
                hardware.Update();
            }

            foreach (var hardware in _gpuTempByHardware.Keys)
            {
                hardware.Update();
            }

            // Line 1
            var cpuTemp = _cpuTempByHardware.Values
                .SelectMany(s => s)
                .First(t => t.Name.Contains("Package"));

            var cpuLoad = Math.Round(_performanceCounter.NextValue(), 0);

            // Line 2
            var gpuTemp = _gpuTempByHardware.Values.SelectMany(s => s).First();

            var computer = new Microsoft.VisualBasic.Devices.Computer();
            var ramPerc = Math.Round((double)100 * computer.Info.AvailablePhysicalMemory / computer.Info.TotalPhysicalMemory, 0);

            // * First line
            // ` Second line

            /*
             * 1234567890123456
             * CPU 51C Load 27%
             * GPU 46C RAM  44%
             */

            var displayString =
                  string.Format("*CPU {0}C Load {1,2}%", cpuTemp.Value, cpuLoad)
                + string.Format("`GPU {0}C RAM  {1,2}%", gpuTemp.Value, ramPerc);

            ClearDisplay();
            WriteSerial(displayString);
        }

        private void WriteSerial(string message)
        {
            WriteSerial(new List<string> { message });
        }

        private void WriteSerial(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                if (message == "~")
                {
                    TextOutput = string.Empty;
                }
                else
                {
                    TextOutput = TextOutput += message.Replace("`", Environment.NewLine).Replace("*", string.Empty);
                }
            }

            if (SHALL_USE_SERIAL_PORT)
            {
                try
                {
                    _destinationSerialPort.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Error opening port: {0} ({1})", e.Message, e.GetType()));
                    return;
                }

                try
                {
                    foreach (var message in messages)
                    {
                        _destinationSerialPort.Write(message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Error writing messages to port: {0} ({1})", e.Message, e.GetType()));
                    return;
                }

                try
                {
                    _destinationSerialPort.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Error closing port: {0} ({1})", e.Message, e.GetType()));
                    return;
                }
            }
        }

        private void ClearDisplay()
        {
            TextOutput = string.Empty;
            WriteSerial("~");
        }

        private void ConnectComAndTestHandler()
        {
            ClearDisplay();
            WriteSerial("*LCD is working!`Port: " + SelectedPort);
            _clearDisplayTimer.Start();
        }

        private void SendCustomTextHandler(string parameter)
        {
            switch (parameter)
            {
                case "Line1":
                    WriteSerial("*");
                    WriteSerial(CustomText);
                    break;
                case "Line2":
                    WriteSerial("`");
                    WriteSerial(CustomText);
                    break;
                case "Clear":
                    WriteSerial("~");
                    break;
                default:
                    throw new ArgumentException(string.Format("Unexpected parameter: '{0}'. I don't know what to do. :(", parameter));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}