using System;
using NAudio.Wave;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Управляет звуковыми эффектами и фоновым музыкальным сопровождением в игре.
    /// </summary>
    public class SoundManager
    {
        private IWavePlayer backgroundPlayer;
        private AudioFileReader backgroundMusic;
        private IWavePlayer eatPlayer;
        private AudioFileReader eatSound;
        private IWavePlayer gameOverPlayer;
        private AudioFileReader gameOverSound;
        private string[] backgroundMusicFiles;
        private Random random;

        private string currentBackgroundMusic;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SoundManager"/> и загружает звуковые файлы.
        /// </summary>
        public SoundManager()
        {
            random = new Random();
            backgroundMusicFiles = new string[]
            {
                "Source/electric-dreams-20240608-170535.wav",
                "Source/pixel-dreams-20240608-164310.wav",
                "Source/pixel-dreams-20240608-170002.wav",
                "Source/pixel-love-20240608-164021.wav",
                "Source/retro-adventure-20240608-164956.wav",
                "Source/whispering-waves-20240601-053626.wav"
            };

            eatSound = new AudioFileReader("Source/-jojo-nigerundayo.wav");
            gameOverSound = new AudioFileReader("Source/jojos-bizarre-adventure-ay-ay-ay-ay-_-sound-effect.wav");

            PlayRandomBackgroundMusic();
        }

        private void PlayRandomBackgroundMusic()
        {
            StopBackgroundMusic();

            string randomFile;
            do
            {
                randomFile = backgroundMusicFiles[random.Next(backgroundMusicFiles.Length)];
            } while (randomFile == currentBackgroundMusic);

            currentBackgroundMusic = randomFile;

            backgroundMusic = new AudioFileReader(randomFile);
            backgroundPlayer = new WaveOutEvent();
            backgroundPlayer.Init(backgroundMusic);
            backgroundPlayer.Volume = 0.5f;

            backgroundPlayer.PlaybackStopped += (s, e) => PlayRandomBackgroundMusic();

            backgroundPlayer.Play();
        }

        public void StopBackgroundMusic()
        {
            backgroundPlayer?.Stop();
            backgroundPlayer?.Dispose();
            backgroundMusic?.Dispose();
        }

        public void PlayEatSound()
        {
            if (backgroundPlayer != null)
            {
                backgroundPlayer.Volume = 0.1f;
            }

            var newEatSound = new AudioFileReader("Source/-jojo-nigerundayo.wav");
            eatPlayer = new WaveOutEvent();
            eatPlayer.Init(newEatSound);
            eatPlayer.Play();

            eatPlayer.PlaybackStopped += (s, e) =>
            {
                if (backgroundPlayer != null)
                {
                    backgroundPlayer.Volume = 0.5f;
                }
                eatPlayer.Dispose();
                newEatSound.Dispose();
            };
        }

        public void PlayGameOverSound()
        {
            StopAllSounds();

            gameOverPlayer = new WaveOutEvent();
            gameOverPlayer.Init(new AudioFileReader("Source/jojos-bizarre-adventure-ay-ay-ay-ay-_-sound-effect.wav"));
            gameOverPlayer.Play();

            gameOverPlayer.PlaybackStopped += (s, e) =>
            {
                gameOverPlayer.Dispose();
            };
        }

        public void StopAllSounds()
        {
            StopBackgroundMusic();

            eatPlayer?.Stop();
            eatPlayer?.Dispose();
        }
    }
}
