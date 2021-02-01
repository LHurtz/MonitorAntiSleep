using NAudio.Wave;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AntiSleepPlayer.Net
{
    class Program
    {
        private const string soundFilePath = "10Hz.wav";
        private static int deviceNumber;

        static void Main(string[] args)
        {
            deviceNumber = FindCorrectDeviceNumber();

            Timer timer = new Timer(60 * 1000 * 15);
            timer.Elapsed += PlaySound;
            timer.Start();

            while(true) { }
        }

        private static void PlaySound(object sender, ElapsedEventArgs e)
        {
            using (var audioFile = new AudioFileReader(soundFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.DeviceNumber = deviceNumber;
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }

            //SoundPlayer soundPlayer = new SoundPlayer(soundFilePath);
            //soundPlayer.PlaySync();
        }

        private static int FindCorrectDeviceNumber()
        {
            for(int deviceNumber = -1; deviceNumber < WaveOut.DeviceCount; deviceNumber++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceNumber);
                if (capabilities.ProductName.Contains("Focusrite"))
                {
                    return deviceNumber;
                }
            }
            
            return -1;
        }
    }
}
