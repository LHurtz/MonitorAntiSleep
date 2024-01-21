using NAudio.Wave;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WindowsService2
{
    public partial class AntiSleepService : ServiceBase
    {
        private const string soundFilePath = @"D:\Entwicklung\MonitorAntiSleep\AntiSleepService\bin\Debug\10Hz.wav";
        private Timer timer;

        public AntiSleepService()
        {
            InitializeComponent();
            this.ServiceName = "MonitorAntiSleep";
        }

        protected override void OnStart(string[] args)
        {
            Task loopTask = new Task(() =>
            {
                timer = new Timer(60 * 1000 * 15);
                timer.Elapsed += OnTimerElapsed;
                timer.Start();

                PlaySound();
            });

            loopTask.Start();
        }

        protected override void OnStop()
        {
            DisposeTimer();
        }

        private static int FindCorrectDeviceNumber()
        {
            for (int deviceNumber = -1; deviceNumber < WaveOut.DeviceCount; deviceNumber++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceNumber);
                if (capabilities.ProductName.Contains("Lautsprecher"))
                {
                    return deviceNumber;
                }
            }

            return -1;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            PlaySound();
        }

        private void PlaySound()
        {
            using (var audioFile = new AudioFileReader(soundFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                int deviceNumber = FindCorrectDeviceNumber();
                outputDevice.DeviceNumber = deviceNumber;
                outputDevice.Init(audioFile);
                outputDevice.Volume = 1;
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void DisposeTimer()
        {
            timer.Elapsed -= OnTimerElapsed;
            timer.Stop();
            timer.Dispose();
        }
    }
}