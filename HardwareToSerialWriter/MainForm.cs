#define OutputTextbox
//#define OutputSerial

namespace HardwareToSerialWriter
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Windows.Forms;
    using OpenHardwareMonitor.Hardware;
    using Computer = OpenHardwareMonitor.Hardware.Computer;

    public partial class MainForm : Form
    {
        private readonly Computer _myComputer;
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _gpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        private readonly IDictionary<IHardware, IEnumerable<ISensor>> _cpuTempByHardware = new Dictionary<IHardware, IEnumerable<ISensor>>();
        
        private ShowDataKinds _showDataKinds = ShowDataKinds.CpuLoadAndRam;

        public MainForm()
        {
            InitializeComponent();
            ConfigureComponents();
            SetUpEventHandlers();
            ResetSerialPort();

            _myComputer = new Computer
            {
                GPUEnabled = true,
                CPUEnabled = true
            };
            _myComputer.Open();

            GetGpuTemps();
            GetCpuTemps();
        }

        private void ConfigureComponents()
        {
            cbComPort.Items.AddRange(new[] {"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9"});
            cbComPort.SelectedItem = "COM5";

            groupBoxPortAndTest.Visible = false;

            #if OutputSerial
                groupBoxPortAndTest.Visible = true;
            #endif
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
            var ramValue = (float) computer.Info.AvailablePhysicalMemory/(1024*1024);
            var ramMb = Math.Round(ramValue, 2);
            var cpuLoad = Math.Truncate(PerformanceCounter1.NextValue() * 100)/100;

            // ` First line
            // * Second line
            var loadString = string.Format("RAM: {0} MB free`CPU: {1}%*", ramMb, cpuLoad);

            ClearDisplay();
            WriteSerial(loadString);
        }

        private void SetUpEventHandlers()
        {
            btnStartDisplayLoadAndRam.Click += btnStartDisplayLoadAndRam_Click;
            btnStartDisplayTemps.Click += btnStartDisplayTemps_Click;
            btnSendCustomText.Click += btnSendCustomText_Click;
            DisplayHardwareValuesTimer.Tick += DisplayHardwareValuesTimer_Tick;
            btnStopDisplay.Click += btnStopDisplay_Click;
            tbCustomText.TextChanged += tbCustomText_TextChanged;
            ClearDisplayTimer.Tick += ClearDisplayTimer_Tick;
            btnConnect.Click += btnConnect_Click;
            btnLine1.Click += btnLine1_Click;
            btnLine2.Click += btnLine2_Click;
            btnClearDisplay.Click += btnClearDisplay_Click;
        }

        private void ResetSerialPort()
        {
            #if OutputSerial
                DestinationSerialPort.Close();
                DestinationSerialPort.PortName = (string)cbComPort.SelectedItem;
                DestinationSerialPort.BaudRate = 9600;
                DestinationSerialPort.DataBits = 8;
                DestinationSerialPort.Parity = Parity.None;
                DestinationSerialPort.StopBits = StopBits.One;
                DestinationSerialPort.Handshake = Handshake.None;
                DestinationSerialPort.Encoding = System.Text.Encoding.Default;
            #endif
        }

        private void WriteSerial(string message)
        {
            WriteSerial(new List<string> { message });
        }

        private void WriteSerial(IEnumerable<string> messages)
        {
            #if OutputTextbox
                var newLines = messages.ToList();
                newLines.AddRange(tbTempReadout.Lines);
                tbTempReadout.Lines = newLines.ToArray();
            #endif

            #if OutputSerial
                DestinationSerialPort.Open();
                foreach (var message in messages)
                {
                    DestinationSerialPort.Write(message);
                }
                DestinationSerialPort.Close();
            #endif
        }

        private void ClearDisplay()
        {

            #if OutputTextbox
                tbTempReadout.Lines = new string[0];
            #endif

            #if OutputSerial
                WriteSerial("~");
            #endif
        }
        
        private void GetGpuTemps() {
            var gpus = from hardwareItem in _myComputer.Hardware
                       where hardwareItem.HardwareType == HardwareType.GpuAti | hardwareItem.HardwareType == HardwareType.GpuNvidia
                       select hardwareItem;
            
            foreach (IHardware hardwareItem in gpus) {
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

        private void btnStartDisplayLoadAndRam_Click(System.Object sender, System.EventArgs e)
        {
            _showDataKinds = ShowDataKinds.CpuLoadAndRam;
            DisplayHardwareValuesTimer.Start();
            UpdateDisplay();

            btnStartDisplayTemps.Enabled = false;
            btnStartDisplayLoadAndRam.Enabled = false;
        }

        void btnStartDisplayTemps_Click(object sender, EventArgs e)
        {
            _showDataKinds = ShowDataKinds.CpuAndGpuTemperature;
            DisplayHardwareValuesTimer.Start();
            UpdateDisplay();

            btnStartDisplayTemps.Enabled = false;
            btnStartDisplayLoadAndRam.Enabled = false;
        }

        private void btnStopDisplay_Click(System.Object sender, System.EventArgs e)
        {
            DisplayHardwareValuesTimer.Stop();
            const string loadString = "Monitoring" + "`" + "stopped!";
            WriteSerial(new List<string> {"~", loadString});
            ClearDisplayTimer.Start();

            btnStartDisplayTemps.Enabled = true;
            btnStartDisplayLoadAndRam.Enabled = true;
        }

        private void DisplayHardwareValuesTimer_Tick(System.Object sender, System.EventArgs e)
        {
            UpdateDisplay();
        }

        private void tbCustomText_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (tbCustomText.Text.Length >= 16)
            {
                MessageBox.Show("This line is full!");
            }
        }

        private void ClearDisplayTimer_Tick(System.Object sender, System.EventArgs e)
        {
            ClearDisplay();
            ClearDisplayTimer.Stop();
        }

        private void btnConnect_Click(System.Object sender, System.EventArgs e)
        {
            ClearDisplay();
            WriteSerial("LCD is working!");
            ClearDisplayTimer.Start();
        }

        private void btnSendCustomText_Click(System.Object sender, System.EventArgs e)
        {
            WriteSerial(tbCustomText.Text);
        }

        private void btnLine1_Click(System.Object sender, System.EventArgs e)
        {
            WriteSerial("*");
        }

        private void btnLine2_Click(System.Object sender, System.EventArgs e)
        {
            WriteSerial("`");
        }

        private void btnClearDisplay_Click(System.Object sender, System.EventArgs e)
        {
            ClearDisplay();
        }

        private enum ShowDataKinds { CpuLoadAndRam, CpuAndGpuTemperature }
    }
}
