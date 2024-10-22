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
        private GameDifficulty difficulty;
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
        private List<Obstacle> obstacles; // Добавь это поле в класс Game
        private Portal portal; // Поле для хранения портала
        private const int PortalWidth = 50; // Ширина портала
        private int portalPositionX; // Позиция по оси X для портала
        private Leaderboard leaderboard;
        private List<LeaderboardEntry> topEntries;
        private DatabaseManager dbManager;
        string serverName = @"CE3HU7L\SQLEXPRESS"; // Укажите имя сервера
        string databaseName = "SnakeGameDB"; // Укажите имя базы данных
        private string currentUsername;  // Добавьте это поле
        public Game(Form form, GameDifficulty difficulty, string connectionString, string username)
        {
            this.form = form; // Сохраняем форму в поле класса
            this.currentUsername = username; // Сохраняем имя пользователя
            soundManager = new SoundManager();
            this.difficulty = difficulty;
            InitializeGame();
            leaderboard = new Leaderboard(connectionString);
            topEntries = leaderboard.GetTopEntries(5); // Получаем 5 лучших записей
            dbManager = new DatabaseManager(serverName, databaseName);
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
            gameTimer.Interval = difficulty.SnakeSpeed; // Скорость движения змейки в зависимости от сложности
            gameTimer.Tick += (sender, e) => GameTimer_Tick();
            gameTimer.Start();
            obstacles = new List<Obstacle>(); // Инициализируй список препятствий
            score = 0;
            currentFood = new Food(); // Инициализируем еду
            GenerateFood(); // Генерируем начальную позицию еды
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
            // Генерируем случайное положение еды
            currentFood.GenerateRandomFood(form.ClientSize.Width, form.ClientSize.Height);

            // Проверяем, не попадает ли еда на змею, препятствия или портал
            while (obstacles != null && obstacles.Any(o => o.Bounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize))) ||
                                                            snake.Body.Contains(currentFood.Position)) ||
                   portal != null && (portal.Bounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize))) ||
                                      portal.BottomBounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize)))))
            {
                // Генерируем снова, если еда попадает на змею, препятствия или портал
                currentFood.GenerateRandomFood(form.ClientSize.Width, form.ClientSize.Height);
            }
        }

        private void GenerateObstacle()
        {
            Point obstaclePosition;
            Rectangle newObstacle;
            int obstacleType = random.Next(0, 3); // Случайный выбор типа препятствия

            int width, height;

            switch (obstacleType)
            {
                case 0: // Прямоугольное препятствие
                    width = SegmentSize; // Фиксированная ширина в 1 сегмент
                    height = random.Next(1, 4) * SegmentSize; // Размер от 1 до 3 сегментов высотой
                    break;

                case 1: // "Г" образное препятствие (горизонтальное)
                    width = 2 * SegmentSize; // Ширина 2 сегмента
                    height = SegmentSize; // Высота 1 сегмент
                    break;

                case 2: // "Г" образное препятствие (вертикальное)
                    width = SegmentSize; // Ширина 1 сегмент
                    height = 2 * SegmentSize; // Высота 2 сегмента
                    break;

                default:
                    throw new InvalidOperationException("Invalid obstacle type");
            }

            do
            {
                int x = random.Next(0, form.ClientSize.Width / SegmentSize) * SegmentSize; // Случайная позиция по горизонтали
                int y = random.Next(0, (form.ClientSize.Height - height) / SegmentSize) * SegmentSize; // Случайная позиция по вертикали
                obstaclePosition = new Point(x, y);
                newObstacle = new Rectangle(obstaclePosition, new Size(width, height));
            } while (obstacles.Any(o => o.Bounds.IntersectsWith(newObstacle) ||
                                        snake.Body.Any(segment => newObstacle.Contains(segment)))); // Проверяем на пересечение

            obstacles.Add(new Obstacle(obstaclePosition, new Size(width, height))); // Добавляем новое препятствие в список
        }


        public void Draw(Graphics g)
        {
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, SegmentSize, SegmentSize);
            }

            g.FillRectangle(Brushes.Red, currentFood.Position.X, currentFood.Position.Y, SegmentSize, SegmentSize);


            // Рисуем верхнюю часть портала
            if (portal != null)
            {
                // Рисуем верхнюю часть
                g.FillRectangle(Brushes.Blue, portal.Bounds.X, portal.Bounds.Y, portal.Bounds.Width, portal.Bounds.Height);

                // Рисуем нижнюю часть
                g.FillRectangle(Brushes.Blue, portal.BottomBounds.X, portal.BottomBounds.Y, portal.BottomBounds.Width, portal.BottomBounds.Height);

                // Рисуем проход (не закрашиваем его)
                Rectangle passageBounds = new Rectangle(portal.Bounds.X, portal.Bounds.Y + portal.Bounds.Height, portal.Bounds.Width, SegmentSize * 2);
                g.FillRectangle(Brushes.Transparent, passageBounds); // Этот прямоугольник оставляем прозрачным
            }

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
            foreach (var obstacle in obstacles)
            {
                g.FillRectangle(Brushes.Gray, obstacle.Bounds); // Рисуем препятствие
            }
            DrawLeaderboard(g);
        }
        private void DrawLeaderboard(Graphics g)
        {
            Font leaderboardFont = new Font("Arial", 16, FontStyle.Bold);
            int startY = 50; // Начальная позиция по Y для лидерборда

            for (int i = 0; i < topEntries.Count; i++)
            {
                string entryText = $"{i + 1}. {topEntries[i].Username}: {topEntries[i].MaxScore}";
                g.DrawString(entryText, leaderboardFont, Brushes.White, new PointF(10, startY + (i * 20)));
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


                if (score == 10) // Проверяем, набрано ли 50 очков
                {
                    CreatePortal();
                }

                if (score % 5 == 0 && score > 0) // Проверяем, набрано ли 10 очков
                {
                    GenerateObstacle(); // Генерируем новое препятствие
                }
                if (score % 1 == 0 && score > 0) // Каждые 5 очков увеличиваем скорость
                {
                    int newInterval = (int)(gameTimer.Interval * 0.9); // Уменьшаем интервал на 10%

                    // Убедимся, что интервал не становится меньше минимального значения, например, 50 миллисекунд
                    gameTimer.Interval = Math.Max(newInterval, 1);

                    Console.WriteLine($"Score: {score}, New Interval: {gameTimer.Interval}"); // Вывод текущего интервала
                }




            }
        }


        private void CreatePortal()
        {
            // Устанавливаем позицию портала в центре экрана
            portalPositionX = (form.ClientSize.Width - PortalWidth) / 2;

            // Высота портала от верхней границы экрана до середины, не доходя 1 сегмента
            int topPortalHeight = (form.ClientSize.Height / 2) - SegmentSize;

            // Высота портала от нижней границы экрана до середины, не доходя 1 сегмента
            int bottomPortalHeight = (form.ClientSize.Height / 2) - SegmentSize;

            // Создаем портал
            portal = new Portal(portalPositionX, PortalWidth, form.ClientSize.Height);

            // Обновляем границы портала
            portal.Bounds = new Rectangle(portalPositionX, 0, PortalWidth, topPortalHeight); // Верхняя часть портала
            portal.PassageBounds = new Rectangle(portalPositionX, topPortalHeight, PortalWidth, SegmentSize * 2); // Проход в центре (2 сегмента в высоту)

            // Добавляем нижнюю часть портала
            portal.BottomBounds = new Rectangle(portalPositionX, form.ClientSize.Height - bottomPortalHeight, PortalWidth, bottomPortalHeight); // Нижняя часть портала
        }




        private void CheckCollisions()
        {
            var head = snake.Body.First();
            Rectangle headBounds = new Rectangle(head.X, head.Y, SegmentSize, SegmentSize);

            // Проверка выхода за границы экрана
            if (head.X < 0)
            {
                head.X = form.ClientSize.Width - SegmentSize;
            }
            else if (head.X >= form.ClientSize.Width)
            {
                head.X = 0;
            }
            if (head.Y < 0)
            {
                head.Y = form.ClientSize.Height - SegmentSize;
            }
            else if (head.Y >= form.ClientSize.Height)
            {
                head.Y = 0; // Появление сверху
            }

            // Проверка столкновения с верхней частью портала
            if (portal != null && headBounds.IntersectsWith(portal.Bounds))
            {
                // Логика для телепортации в противоположную сторону
                switch (snake.CurrentDirection)
                {
                    case Direction.Right:
                        head.X = form.ClientSize.Width - SegmentSize;
                        snake.CurrentDirection = Direction.Left;
                        break;
                    case Direction.Left:
                        head.X = 0;
                        snake.CurrentDirection = Direction.Right;
                        break;
                    case Direction.Down:
                        head.Y = form.ClientSize.Height - SegmentSize;
                        snake.CurrentDirection = Direction.Up;
                        break;
                    case Direction.Up:
                        head.Y = 0;
                        snake.CurrentDirection = Direction.Down;
                        break;
                }
            }

            if (portal != null && headBounds.IntersectsWith(portal.BottomBounds))
            {
                switch (snake.CurrentDirection)
                {
                    case Direction.Right:
                        head.X = form.ClientSize.Width - SegmentSize;
                        snake.CurrentDirection = Direction.Left;
                        break;
                    case Direction.Left:
                        head.X = 0;
                        snake.CurrentDirection = Direction.Right;
                        break;
                    case Direction.Down:
                        head.Y = 0;
                        snake.CurrentDirection = Direction.Up;
                        break;
                    case Direction.Up:
                        head.Y = form.ClientSize.Height - SegmentSize;
                        snake.CurrentDirection = Direction.Down;
                        break;
                }
            }

            snake.Body[0] = head;

            // Проверка столкновения с телом змеи
            for (int i = 1; i < snake.Body.Count; i++)
            {
                if (head == snake.Body[i])
                {
                    GameOver();
                }
            }

            // Проверка столкновения с препятствиями
            foreach (var obstacle in obstacles)
            {
                if (headBounds.IntersectsWith(obstacle.Bounds))
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

            if (!string.IsNullOrEmpty(currentUsername)) // Используем более безопасную проверку
            {
                // Получаем текущий максимальный счет из базы данных
                int maxScore = leaderboard.GetMaxScore(currentUsername); // Убедитесь, что метод существует

                // Если текущий счет больше, чем максимальный, обновляем в базе данных
                if (score > maxScore)
                {
                    dbManager.UpdateMaxScore(currentUsername, score);
                }
                else
                {
                    Console.WriteLine($"Текущий счет ({score}) не превышает максимальный счет ({maxScore}).");
                }
            }
            else
            {
                Console.WriteLine("Имя игрока не установлено. Обновление максимального счета невозможно.");
            }

            form.Invalidate();
        }



        private void RestartGame()
        {
            isGameOver = false;
            score = 0;
            snake = new Snake();
            obstacles.Clear();
            portal = null;
            GenerateFood();
            gameTimer.Start();
        }

    }
}
