using System;
using NAudio.Wave;

namespace SnakeOnDesktop
{
    public class SoundManager
    {
        private IWavePlayer backgroundPlayer;
        private AudioFileReader backgroundMusic;
        private IWavePlayer eatPlayer;
        private AudioFileReader eatSound;
        private IWavePlayer gameOverPlayer;
        private AudioFileReader gameOverSound;
        private string[] backgroundMusicFiles; // Массив файлов фоновой музыки
        private Random random;

        private string currentBackgroundMusic; // Для отслеживания текущей фоновой музыки

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

            PlayRandomBackgroundMusic(); // Запускаем случайную фоновую музыку
        }

        private void PlayRandomBackgroundMusic()
        {
            // Останавливаем предыдущую музыку, если она играла
            StopBackgroundMusic();

            // Выбираем случайный файл фоновой музыки, который не совпадает с текущим
            string randomFile;
            do
            {
                randomFile = backgroundMusicFiles[random.Next(backgroundMusicFiles.Length)];
            } while (randomFile == currentBackgroundMusic);

            currentBackgroundMusic = randomFile;

            backgroundMusic = new AudioFileReader(randomFile);
            backgroundPlayer = new WaveOutEvent();
            backgroundPlayer.Init(backgroundMusic);
            backgroundPlayer.Volume = 0.5f; // Уровень громкости фоновой музыки

            // Подписываемся на событие окончания воспроизведения
            backgroundPlayer.PlaybackStopped += (s, e) => PlayRandomBackgroundMusic();

            backgroundPlayer.Play(); // Воспроизводим выбранную фоновую музыку
        }

        public void StopBackgroundMusic()
        {
            backgroundPlayer?.Stop();
            backgroundPlayer?.Dispose();
            backgroundMusic?.Dispose();
        }

        public void PlayEatSound()
        {
            // Уменьшаем громкость фоновой музыки
            if (backgroundPlayer != null)
            {
                backgroundPlayer.Volume = 0.1f; // Уменьшаем громкость фоновой музыки
            }

            // Создаем новый экземпляр AudioFileReader для звука поедания
            var newEatSound = new AudioFileReader("Source/-jojo-nigerundayo.wav");
            eatPlayer = new WaveOutEvent();
            eatPlayer.Init(newEatSound);
            eatPlayer.Play();

            // После завершения воспроизведения звука, возвращаем громкость фоновой музыки
            eatPlayer.PlaybackStopped += (s, e) =>
            {
                if (backgroundPlayer != null)
                {
                    backgroundPlayer.Volume = 0.5f; // Возвращаем громкость фоновой музыки
                }
                eatPlayer.Dispose(); // Освобождаем ресурсы
                newEatSound.Dispose(); // Освобождаем ресурсы для нового звука
            };
        }

        public void PlayGameOverSound()
        {
            // Останавливаем все звуки, кроме звука завершения игры
            StopAllSounds();

            // Создаем новый экземпляр для звука завершения игры
            gameOverPlayer = new WaveOutEvent();
            gameOverPlayer.Init(new AudioFileReader("Source/jojos-bizarre-adventure-ay-ay-ay-ay-_-sound-effect.wav"));
            gameOverPlayer.Play();

            // Очищаем ресурсы после завершения воспроизведения
            gameOverPlayer.PlaybackStopped += (s, e) =>
            {
                gameOverPlayer.Dispose(); // Освобождаем ресурсы
            };
        }

        // Метод для остановки всех звуков
        public void StopAllSounds()
        {
            // Останавливаем фоновую музыку
            StopBackgroundMusic();

            // Останавливаем звук поедания, если он воспроизводится
            eatPlayer?.Stop();
            eatPlayer?.Dispose();
        }
    }
}
