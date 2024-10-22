using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeOnDesktop

{ /// <summary>
  /// Класс, представляющий основную логику игры "Змейка". Управляет процессом игры, обновлением состояния и отрисовкой элементов.
  /// </summary>
    public class Game
    {
        private Timer gameTimer;
        private Snake snake;
        private GameDifficulty difficulty;
        private List<Desktop.DesktopObject> foodObjects;
        private int score;
        private Random random;
        private Font scoreFont;
        private Font gameOverFont;
        private Point foodPosition;
        private Desktop desktop;
        private bool isGameOver;
        private const int SegmentSize = 50;
        private SoundManager soundManager;
        private Food currentFood;
        private Form form;
        private List<Obstacle> obstacles;
        private Portal portal;
        private const int PortalWidth = 50;
        private int portalPositionX;
        private Leaderboard leaderboard;
        private List<LeaderboardEntry> topEntries;
        private DatabaseManager dbManager;
        string serverName = @"CE3HU7L\SQLEXPRESS";
        string databaseName = "SnakeGameDB";
        private string currentUsername;

        /// <summary>
        /// Инициализирует новый экземпляр игры.
        /// </summary>
        /// <param name="form">Форма, на которой отображается игра.</param>
        /// <param name="difficulty">Уровень сложности игры.</param>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        /// <param name="username">Имя текущего пользователя.</param>
        public Game(Form form, GameDifficulty difficulty, string connectionString, string username)
        {
            this.form = form;
            this.currentUsername = username;
            soundManager = new SoundManager();
            this.difficulty = difficulty;
            InitializeGame();
            leaderboard = new Leaderboard(connectionString);
            topEntries = leaderboard.GetTopEntries(5);
            dbManager = new DatabaseManager(serverName, databaseName);
        }

        /// <summary>
        /// Инициализирует параметры игры и запускает игровой процесс.
        /// </summary>
        private void InitializeGame()
        {
            isGameOver = false;
            desktop = new Desktop();
            foodObjects = new List<Desktop.DesktopObject>();
            snake = new Snake();
            random = new Random();
            scoreFont = new Font("Arial", 24, FontStyle.Bold);
            gameOverFont = new Font("Arial", 32, FontStyle.Bold);

            gameTimer = new Timer();
            gameTimer.Interval = difficulty.SnakeSpeed;
            gameTimer.Tick += (sender, e) => GameTimer_Tick();
            gameTimer.Start();
            obstacles = new List<Obstacle>();
            score = 0;
            currentFood = new Food();
            GenerateFood();
        }

        /// <summary>
        /// Метод, вызываемый таймером для обновления состояния игры.
        /// </summary>
        private void GameTimer_Tick()
        {
            snake.Move();
            CheckForFood();
            CheckCollisions();
            form.Invalidate();
        }

        /// <summary>
        /// Генерирует новую случайную позицию для еды.
        /// </summary>
        private void GenerateFood()
        {
            currentFood.GenerateRandomFood(form.ClientSize.Width, form.ClientSize.Height);

            while (obstacles != null && obstacles.Any(o => o.Bounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize))) ||
                                                            snake.Body.Contains(currentFood.Position)) ||
                   portal != null && (portal.Bounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize))) ||
                                      portal.BottomBounds.IntersectsWith(new Rectangle(currentFood.Position, new Size(SegmentSize, SegmentSize)))))
            {
                currentFood.GenerateRandomFood(form.ClientSize.Width, form.ClientSize.Height);
            }
        }
        /// <summary>
        /// Генерирует препятствия на игрвоом поле.
        /// </summary>
        private void GenerateObstacle()
        {
            Point obstaclePosition;
            Rectangle newObstacle;
            int obstacleType = random.Next(0, 3);

            int width, height;

            switch (obstacleType)
            {
                case 0:
                    width = SegmentSize;
                    height = random.Next(1, 4) * SegmentSize;
                    break;

                case 1:
                    width = 2 * SegmentSize;
                    height = SegmentSize;
                    break;

                case 2:
                    width = SegmentSize;
                    height = 2 * SegmentSize;
                    break;

                default:
                    throw new InvalidOperationException("Invalid obstacle type");
            }

            do
            {
                int x = random.Next(0, form.ClientSize.Width / SegmentSize) * SegmentSize;
                int y = random.Next(0, (form.ClientSize.Height - height) / SegmentSize) * SegmentSize;
                obstaclePosition = new Point(x, y);
                newObstacle = new Rectangle(obstaclePosition, new Size(width, height));
            } while (obstacles.Any(o => o.Bounds.IntersectsWith(newObstacle) ||
                                        snake.Body.Any(segment => newObstacle.Contains(segment))));

            obstacles.Add(new Obstacle(obstaclePosition, new Size(width, height)));
        }

        /// <summary>
        /// Рисует все игровые элементы на экране.
        /// </summary>
        /// <param name="g">Графический объект для отрисовки.</param>
        public void Draw(Graphics g)
        {
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, SegmentSize, SegmentSize);
            }

            g.FillRectangle(Brushes.Red, currentFood.Position.X, currentFood.Position.Y, SegmentSize, SegmentSize);

            if (portal != null)
            {
                g.FillRectangle(Brushes.Blue, portal.Bounds.X, portal.Bounds.Y, portal.Bounds.Width, portal.Bounds.Height);

                g.FillRectangle(Brushes.Blue, portal.BottomBounds.X, portal.BottomBounds.Y, portal.BottomBounds.Width, portal.BottomBounds.Height);

                Rectangle passageBounds = new Rectangle(portal.Bounds.X, portal.Bounds.Y + portal.Bounds.Height, portal.Bounds.Width, SegmentSize * 2);
                g.FillRectangle(Brushes.Transparent, passageBounds);
            }

            if (!isGameOver)
            {
                string scoreText = $"                    Счёт: {score} \n Для выхода нажмите ESC \n Для перезапуска нажмите F1";
                SizeF textSize = g.MeasureString(scoreText, scoreFont);
                float xPosition = (SystemInformation.VirtualScreen.Width - textSize.Width) / 2;
                g.DrawString(scoreText, scoreFont, Brushes.White, new PointF(xPosition, 10));
            }
            else
            {
                string gameOverText = $"Игра окончена! Счёт: {score}\nНажмите R для рестарта";
                SizeF gameOverSize = g.MeasureString(gameOverText, gameOverFont);
                float xPosition = (form.ClientSize.Width - gameOverSize.Width) / 2;
                float yPosition = (form.ClientSize.Height - gameOverSize.Height) / 2;
                g.DrawString(gameOverText, gameOverFont, Brushes.Red, new PointF(xPosition, yPosition));
            }
            foreach (var obstacle in obstacles)
            {
                g.FillRectangle(Brushes.Gray, obstacle.Bounds);
            }
            DrawLeaderboard(g);
        }

        /// <summary>
        /// Отрисовывает таблицу лидеров на экране.
        /// </summary>
        /// <param name="g">Графический объект для отрисовки.</param>
        private void DrawLeaderboard(Graphics g)
        {
            Font leaderboardFont = new Font("Arial", 16, FontStyle.Bold);
            int startY = 50;

            for (int i = 0; i < topEntries.Count; i++)
            {
                string entryText = $"{i + 1}. {topEntries[i].Username}: {topEntries[i].MaxScore}";
                g.DrawString(entryText, leaderboardFont, Brushes.White, new PointF(10, startY + (i * 20)));
            }
        }

        /// <summary>
        /// Обрабатывает нажатие клавиш для управления змейкой и игровым процессом.
        /// </summary>
        /// <param name="key">Клавиша, нажатая пользователем.</param>
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
                case Keys.F1:
                    if (!isGameOver) GameOver();
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
            }
        }

        /// <summary>
        /// Проверяет, съел ли змейка еду. Если еда съедена, увеличивается счёт, 
        /// змейка растёт, создаётся новая еда. Также создаётся портал при достижении определённого счёта 
        /// и генерируются препятствия каждые 10 очков.
        /// </summary>
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


                if (score == 50)
                {
                    CreatePortal();
                }

                if (score % 10 == 0 && score > 0)
                {
                    GenerateObstacle();
                }
                if (score % 10 == 0 && score > 0)
                {
                    int newInterval = (int)(gameTimer.Interval * 0.9);

                    gameTimer.Interval = Math.Max(newInterval, 1);

                    Console.WriteLine($"Score: {score}, New Interval: {gameTimer.Interval}");
                }
            }
        }

        /// <summary>
        /// Создаёт портал на игровом поле при достижении определённого счёта. 
        /// Портал состоит из верхней и нижней частей, и его размеры подстраиваются 
        /// в зависимости от размеров окна.
        /// </summary>
        private void CreatePortal()
        {
            portalPositionX = (form.ClientSize.Width - PortalWidth) / 2;

            int topPortalHeight = (form.ClientSize.Height / 2) - SegmentSize;

            int bottomPortalHeight = (form.ClientSize.Height / 2) - SegmentSize;

            portal = new Portal(portalPositionX, PortalWidth, form.ClientSize.Height);

            portal.Bounds = new Rectangle(portalPositionX, 0, PortalWidth, topPortalHeight);

            portal.PassageBounds = new Rectangle(portalPositionX, topPortalHeight, PortalWidth, SegmentSize * 2);

            portal.BottomBounds = new Rectangle(portalPositionX, form.ClientSize.Height - bottomPortalHeight, PortalWidth, bottomPortalHeight);
        }

        /// <summary>
        /// Проверяет столкновения головы змейки с границами окна, телом змейки, препятствиями, 
        /// а также с порталом. Если происходит столкновение, игра заканчивается.
        /// </summary>
        private void CheckCollisions()
        {
            var head = snake.Body.First();
            Rectangle headBounds = new Rectangle(head.X, head.Y, SegmentSize, SegmentSize);

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
                head.Y = 0;
            }

            if (portal != null && headBounds.IntersectsWith(portal.Bounds))
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

            for (int i = 1; i < snake.Body.Count; i++)
            {
                if (head == snake.Body[i])
                {
                    GameOver();
                }
            }

            foreach (var obstacle in obstacles)
            {
                if (headBounds.IntersectsWith(obstacle.Bounds))
                {
                    GameOver();
                }
            }
        }

        /// <summary>
        /// Завершает игру. Останавливает таймер, воспроизводит звук окончания игры.
        /// Если счёт игрока выше предыдущего максимального, обновляет данные в базе.
        /// </summary>
        private void GameOver()
        {
            gameTimer.Stop();
            soundManager.PlayGameOverSound();
            isGameOver = true;

            if (!string.IsNullOrEmpty(currentUsername))
            {
                int maxScore = leaderboard.GetMaxScore(currentUsername);

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

        /// <summary>
        /// Перезапускает игру, сбрасывает все параметры к начальному состоянию. 
        /// Обнуляет счёт, создаёт новую змейку, очищает препятствия и заново генерирует еду.
        /// </summary>
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
