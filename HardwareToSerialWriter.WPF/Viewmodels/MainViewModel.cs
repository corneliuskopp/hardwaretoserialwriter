namespace HardwareToSerialWriter.WPF.Viewmodels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Annotations;
    using CommandHandlers;
    using OpenHardwareMonitor.Hardware;

    public class MainViewModel : INotifyPropertyChanged 
    {
        private const bool SHALL_USE_SERIAL_PORT = true;

        private readonly ObservableCollection<string> _comPortNames = new ObservableCollection<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9" };

        private readonly PerformanceCounter _performanceCounter = new PerformanceCounter();
        private readonly Computer _myComputer;
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _gpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _cpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        private readonly DispatcherTimer _displayHardwareValuesTimer;
        private readonly DispatcherTimer _clearDisplayTimer;

        private ShowDataKinds _showDataKinds = ShowDataKinds.CpuLoadAndRam;
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

        public string SelectedPort { get; set; }

        public ObservableCollection<string> ComPortNames
        {
            get { return _comPortNames; }
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
                return _startDisplay ?? (_startDisplay = new StringCommandHandler(StartDisplayHandler));
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

            IsDisplaying = false;
            TextOutput = string.Empty;
            SelectedPort = "COM5";
        }
        
        private void StartDisplayHandler(string parameter)
        {
            if (parameter == "CpuAndRam")
            {
                _showDataKinds = ShowDataKinds.CpuLoadAndRam;
            }
            else if (parameter == "CpuGpuTemp")
            {
                _showDataKinds = ShowDataKinds.CpuAndGpuTemperature;
            }
            else
            {
                throw new ArgumentException(string.Format("Don't know what to do with parameter '{0}'. :(", parameter));
            }

            _displayHardwareValuesTimer.Start();
            UpdateDisplay();

            IsDisplaying = true;
        }

        private void StopDisplayHandler()
        {
            _displayHardwareValuesTimer.Stop();

            const string loadString = "Monitoring" + "`" + "stopped!";
            WriteSerial(new List<string> { "~", loadString });
            
            _clearDisplayTimer.Start();
            IsDisplaying = false;
        }
        
        private void ResetSerialPort()
        {
            if (SHALL_USE_SERIAL_PORT)
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
            if (_showDataKinds == ShowDataKinds.CpuLoadAndRam)
            {
                UpdateWithCpuAndRam();
            }
            else if (_showDataKinds == ShowDataKinds.CpuAndGpuTemperature)
            {
                UpdateWithCpuAndGpuTemps();
            }
            else
            {
                throw new InvalidOperationException("Got unexpected enum value: " + _showDataKinds.ToString() + ". Not sure what to display, so I will exit. :(");
            }
        }

        private void UpdateWithCpuAndGpuTemps()
        {
            foreach (var hardware in _cpuTempByHardware.Keys)
            {
                hardware.Update();
            }
            var cpuTemps = _cpuTempByHardware.Values.SelectMany(s => s).Distinct().OrderBy(s => s.Name).ToList();

            // If the following line is commented out, it shows all cores. Otherwise only the die package temp
            cpuTemps = cpuTemps.Where(t => t.Name.Contains("Package")).ToList();

            string cpuTemp = cpuTemps.Aggregate(string.Empty, (acc, cur) => acc += string.Format("{0}: {1} °C ", cur.Name, cur.Value));


            foreach (var hardware in _gpuTempByHardware.Keys)
            {
                hardware.Update();
            }
            var gpuTemps = _gpuTempByHardware.Values.SelectMany(s => s).Distinct().OrderBy(s => s.Name).ToList();

            string gpuTemp = gpuTemps.Aggregate(string.Empty, (acc, cur) => acc += string.Format("{0}: {1} °C ", cur.Name, cur.Value));

            // ` First line
            // * Second line
            var tempString = gpuTemp + "`" + cpuTemp + "*";

            ClearDisplay();
            WriteSerial(tempString);
        }

        private void UpdateWithCpuAndRam()
        {
            var computer = new Microsoft.VisualBasic.Devices.Computer();
            var ramValue = (float)computer.Info.AvailablePhysicalMemory / (1024 * 1024);
            var ramMb = Math.Round(ramValue, 2);
            var cpuLoad = Math.Truncate(_performanceCounter.NextValue() * 100) / 100;

            // ` First line
            // * Second line
            var loadString = string.Format("RAM: {0} MB free`CPU: {1}%*", ramMb, cpuLoad);

            ClearDisplay();
            WriteSerial(loadString);
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
                    TextOutput = TextOutput += message;
                }
            }

            if (SHALL_USE_SERIAL_PORT)
            {
                _destinationSerialPort.Open();
                foreach (var message in messages)
                {
                    _destinationSerialPort.Write(message);
                }
                _destinationSerialPort.Close();
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

        private enum ShowDataKinds { CpuLoadAndRam, CpuAndGpuTemperature }

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