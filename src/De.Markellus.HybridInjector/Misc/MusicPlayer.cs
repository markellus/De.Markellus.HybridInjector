using System.IO;
using System.Reflection;
using System.Windows;
using De.Markellus.HybridInjector.Properties;
using NAudio.Wave;

namespace De.Markellus.HybridInjector.Misc
{
    internal static class MusicPlayer
    {
        private static WaveOutEvent _wavePlayer;
        private static Mp3FileReader _mp3Reader;

        public static void ToggleMusicPlayback()
        {
            if (_wavePlayer.PlaybackState == PlaybackState.Paused ||
                _wavePlayer.PlaybackState == PlaybackState.Stopped)
            {
                _wavePlayer.Play();
                Settings.Default.Music = true;
            }
            else
            {
                _wavePlayer.Pause();
                Settings.Default.Music = false;
            }
        }

        public static void Dispose()
        {
            _wavePlayer.PlaybackStopped -= RestartMusic;
            _wavePlayer?.Dispose();
            _mp3Reader?.Dispose();
        }

        public static void RestartMusic(object sender, StoppedEventArgs e)
        {
            _wavePlayer?.Dispose();
            _mp3Reader?.Dispose();

            MemoryStream stream = new MemoryStream(ResourceExtractor.Extract(Assembly.GetAssembly(Application.Current.GetType()), "resources/music.mp3"));

            _mp3Reader = new Mp3FileReader(stream);
            _wavePlayer = new WaveOutEvent();
            _wavePlayer.Init(_mp3Reader);
            if (Settings.Default.Music)
            {
                _wavePlayer.Play();
            }
            _wavePlayer.PlaybackStopped += RestartMusic;
        }
    }
}
