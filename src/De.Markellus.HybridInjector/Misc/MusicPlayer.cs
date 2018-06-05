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
        private static WaveFileReader _waveReader;

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
            _wavePlayer?.Dispose();
            _waveReader?.Dispose();
        }

        public static void RestartMusic()
        {
            _wavePlayer?.Dispose();
            _waveReader?.Dispose();

            MemoryStream stream = new MemoryStream(ResourceExtractor.Extract(Assembly.GetAssembly(Application.Current.GetType()), "resources/loop.wav"));

            _waveReader = new WaveFileReader(stream);
            LoopStream loop = new LoopStream(_waveReader);
            _wavePlayer = new WaveOutEvent();
            _wavePlayer.Init(loop);
            if (Settings.Default.Music)
            {
                _wavePlayer.Play();
            }
        }
    }

    /// <summary>
    /// Stream for looping playback
    /// </summary>
    internal class LoopStream : WaveStream
    {
        private readonly WaveStream _sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
            EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length => _sourceStream.Length;

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
