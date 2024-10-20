using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public class Game
    {
        private Timer gameTimer;
        private Snake snake;
        private List<Desktop.DesktopObject> foodObjects;
        private int score;
        private Random random;
        private Font scoreFont;
        private Font gameOverFont; // Шрифт для табло
        private Point foodPosition;
        private Desktop desktop;
        private bool isGameOver;
        private const int SegmentSize = 50;
        private SoundManager soundManager;
        private Food currentFood;
        private Form form; // Поле для хранения ссылки на форму

        public Game(Form form)
        {
            this.form = form; // Сохраняем форму в поле класса
            soundManager = new SoundManager();
            InitializeGame();
        }

        private void InitializeGame()
        {
            isGameOver = false;
            desktop = new Desktop();
            foodObjects = new List<Desktop.DesktopObject>();
            snake = new Snake();
            random = new Random();
            scoreFont = new Font("Arial", 24, FontStyle.Bold);
            gameOverFont = new Font("Arial", 32, FontStyle.Bold); // Инициализация шрифта для табло

            gameTimer = new Timer();
            gameTimer.Interval = 10;
            gameTimer.Tick += (sender, e) => GameTimer_Tick();
            gameTimer.Start();

            score = 0;
            currentFood = new Food(SegmentSize); // Инициализируем еду
            GenerateFood();
        }

        private void GameTimer_Tick()
        {
            snake.Move();
            CheckForFood();
            CheckCollisions();
            form.Invalidate();
        }

        private void GenerateFood()
        {
            currentFood.GenerateFood(SegmentSize); // Генерация новой еды
        }

        public void Draw(Graphics g)
        {
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, SegmentSize, SegmentSize);
            }

            g.FillRectangle(Brushes.Red, currentFood.Position.X, currentFood.Position.Y, SegmentSize, SegmentSize);

            if (!isGameOver)
            {
                string scoreText = $"Счёт: {score}";
                SizeF textSize = g.MeasureString(scoreText, scoreFont);
                float xPosition = (SystemInformation.VirtualScreen.Width - textSize.Width) / 2;
                g.DrawString(scoreText, scoreFont, Brushes.White, new PointF(xPosition, 10));
            }
            else
            {
                // Отображение табло при завершении игры
                string gameOverText = $"Игра окончена! Счёт: {score}\nНажмите R для рестарта";
                SizeF gameOverSize = g.MeasureString(gameOverText, gameOverFont);
                float xPosition = (form.ClientSize.Width - gameOverSize.Width) / 2;
                float yPosition = (form.ClientSize.Height - gameOverSize.Height) / 2;
                g.DrawString(gameOverText, gameOverFont, Brushes.Red, new PointF(xPosition, yPosition));
            }
        }

        public void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    if (snake.CurrentDirection != Direction.Down) snake.CurrentDirection = Direction.Up;
                    break;
                case Keys.Down:
                    if (snake.CurrentDirection != Direction.Up) snake.CurrentDirection = Direction.Down;
                    break;
                case Keys.Left:
                    if (snake.CurrentDirection != Direction.Right) snake.CurrentDirection = Direction.Left;
                    break;
                case Keys.Right:
                    if (snake.CurrentDirection != Direction.Left) snake.CurrentDirection = Direction.Right;
                    break;
                case Keys.R:
                    if (isGameOver) RestartGame();
                    break;
                case Keys.Escape: // Обработка нажатия клавиши ESC
                    if (!isGameOver) GameOver(); // Вызываем завершение игры
                    break;
            }
        }

        private void CheckForFood()
        {
            var head = snake.Body[0];

            double distance = Math.Sqrt(Math.Pow(head.X - currentFood.CenterX, 2) + Math.Pow(head.Y - currentFood.CenterY, 2));

            const int eatRadius = 50;
            if (distance < eatRadius)
            {
                snake.Grow();
                score++;
                soundManager.PlayEatSound();
                GenerateFood();
            }
        }

        private void CheckCollisions()
        {
            var head = snake.Body.First();

            if (head.X < 0) head.X = form.ClientSize.Width - 10;
            else if (head.X >= form.ClientSize.Width) head.X = 0;
            if (head.Y < 0) head.Y = form.ClientSize.Height - 10;
            else if (head.Y >= form.ClientSize.Height) head.Y = 0;

            snake.Body[0] = head;

            for (int i = 1; i < snake.Body.Count; i++)
            {
                if (head == snake.Body[i])
                {
                    GameOver();
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            soundManager.PlayGameOverSound();
            isGameOver = true;
            form.Invalidate();
        }

        private void RestartGame()
        {
            isGameOver = false;
            score = 0;
            snake = new Snake();
            GenerateFood();
            gameTimer.Start();
        }
    }
}
