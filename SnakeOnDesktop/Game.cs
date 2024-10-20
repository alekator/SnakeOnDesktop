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
        private Point foodPosition;
        private Desktop desktop;
        private bool isGameOver;
        private const int SegmentSize = 50;

        public Game(Form form)
        {
            InitializeGame(form);
        }

        private void InitializeGame(Form form)
        {
            isGameOver = false;
            desktop = new Desktop();
            foodObjects = new List<Desktop.DesktopObject>();
            snake = new Snake();
            random = new Random();
            scoreFont = new Font("Arial", 24, FontStyle.Bold);

            gameTimer = new Timer();
            gameTimer.Interval = 10;
            gameTimer.Tick += (sender, e) => GameTimer_Tick(form);
            gameTimer.Start();

            score = 0;
            GenerateFood();
        }

        private void GameTimer_Tick(Form form)
        {
            snake.Move();
            CheckForFood();
            CheckCollisions(form);
            form.Invalidate();
        }

        private void GenerateFood()
        {
            int maxX = (SegmentSize * (int)(SystemInformation.VirtualScreen.Width / SegmentSize));
            int maxY = (SegmentSize * (int)(SystemInformation.VirtualScreen.Height / SegmentSize));
            foodPosition = new Point(random.Next(0, maxX / SegmentSize) * SegmentSize, random.Next(0, maxY / SegmentSize) * SegmentSize);
        }

        public void Draw(Graphics g)
        {
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, SegmentSize, SegmentSize);
            }

            g.FillRectangle(Brushes.Red, foodPosition.X, foodPosition.Y, SegmentSize, SegmentSize);

            if (!isGameOver)
            {
                string scoreText = $"Счёт: {score}";
                SizeF textSize = g.MeasureString(scoreText, scoreFont);
                float xPosition = (SystemInformation.VirtualScreen.Width - textSize.Width) / 2;
                g.DrawString(scoreText, scoreFont, Brushes.White, new PointF(xPosition, 10));
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
            }
        }

        private void CheckForFood()
        {
            if (snake.Body[0] == foodPosition)
            {
                snake.Grow();
                score++;
                GenerateFood();
            }
        }

        private void CheckCollisions(Form form)
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
                    GameOver(form);
                }
            }
        }

        private void GameOver(Form form)
        {
            gameTimer.Stop();
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
