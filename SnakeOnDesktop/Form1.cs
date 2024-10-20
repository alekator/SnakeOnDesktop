using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Snake snake;
        private string[] desktopFolders;
        private int score;
        private Random random;
        private Font scoreFont;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();

            // Установка стиля формы
            this.FormBorderStyle = FormBorderStyle.None; // Без рамки
            this.TopMost = true; // Окно всегда сверху
            this.WindowState = FormWindowState.Maximized; // Разворачиваем окно на весь экран
            this.BackColor = Color.Black; // Цвет фона (можно изменить)
            this.TransparencyKey = this.BackColor; // Делаем фон прозрачным
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            MinimizeAllWindows(); // Сворачиваем все окна при запуске
        }


        private void InitializeGame()
        {
            // Загрузка списка папок на рабочем столе
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktopFolders = Directory.GetDirectories(desktopPath);

            // Инициализация змейки
            snake = new Snake();
            random = new Random();

            // Настройка таймера игры
            gameTimer = new Timer();
            gameTimer.Interval = 100; // Интервал обновления игры в миллисекундах
            gameTimer.Tick += GameTimer_Tick; // Привязка события для обновления игры
            gameTimer.Start();
            score = 0; // Начальный счёт
            scoreFont = new Font("Arial", 24, FontStyle.Bold); // Инициализация шрифта для счета
        }

        private void MinimizeAllWindows()
        {
            // Используем WinAPI для сворачивания всех окон
            ShowWindow(GetForegroundWindow(), SW_MINIMIZE);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            snake.Move(); // Двигаем змейку
            CheckForFood(); // Проверяем на съеденные папки
            CheckCollisions(); // Проверяем на столкновения
            Invalidate(); // Обновляем отрисовку формы
            Console.WriteLine("Обновление игры"); // Отладочное сообщение
        }


        private void CheckForFood()
        {
            foreach (var folder in desktopFolders)
            {
                // Получаем реальные координаты папки
                Point folderPosition = GetFolderPosition(folder);
                if (snake.Body[0] == folderPosition) // Если голова змейки на папке
                {
                    snake.Body.Add(snake.Body.Last()); // Увеличиваем тело змейки
                    score++; // Увеличиваем счёт
                    Console.WriteLine($"Папка {folder} съедена! Текущий счёт: {score}"); // Для теста выводим сообщение
                    // Логика удаления/обновления "съеденной" папки (пока не реализована)
                }
            }
        }

        private void CheckCollisions()
        {
            // Проверка на столкновение с границами
            var head = snake.Body.First();
            if (head.X < 0 || head.X >= this.ClientSize.Width || head.Y < 0 || head.Y >= this.ClientSize.Height)
            {
                GameOver();
            }

            // Проверка на столкновение с телом
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
            MessageBox.Show($"Игра окончена! Ваш счёт: {score}");
            // Можно добавить логику для перезапуска игры
        }

        // Метод для получения реальных координат иконки папки на рабочем столе
        private Point GetFolderPosition(string folder)
        {
            IntPtr progmanHandle = FindWindow(PROGMAN, null);
            if (progmanHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Не удалось найти окно рабочего стола.");
            }

            IntPtr shellViewHandle = FindWindowEx(progmanHandle, IntPtr.Zero, SHELLDLL_DEFVIEW, null);
            if (shellViewHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Не удалось найти окно иконок рабочего стола.");
            }

            // Получение окна папки (в этом примере общее для всех иконок)
            RECT folderRect = new RECT();
            if (!GetWindowRect(shellViewHandle, ref folderRect))
            {
                throw new InvalidOperationException("Не удалось получить координаты папки.");
            }

            return new Point(folderRect.Left, folderRect.Top);
        }

        // Методы и структуры для работы с Windows API
        private const string SHELLDLL_DEFVIEW = "SHELLDLL_DefView";
        private const string PROGMAN = "Progman";
        private const int SW_MINIMIZE = 2; // Код для сворачивания окна

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Метод для отрисовки змейки на форме
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black); // Очистка фона

            // Отрисовка тела змейки
            foreach (Point segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, new Rectangle(segment.X, segment.Y, 10, 10)); // Рисуем каждый сегмент змейки
            }

            // Отрисовка счета
            string scoreText = $"Счет: {score}";
            SizeF textSize = g.MeasureString(scoreText, scoreFont);
            PointF textPosition = new PointF((this.ClientSize.Width - textSize.Width) / 2, 10); // Центрируем текст по горизонтали
            g.DrawString(scoreText, scoreFont, Brushes.White, textPosition); // Отрисовка счета
        }


        // Метод для обработки нажатий клавиш
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    if (snake.CurrentDirection != Direction.Down)
                        snake.CurrentDirection = Direction.Up;
                    break;
                case Keys.Down:
                    if (snake.CurrentDirection != Direction.Up)
                        snake.CurrentDirection = Direction.Down;
                    break;
                case Keys.Left:
                    if (snake.CurrentDirection != Direction.Right)
                        snake.CurrentDirection = Direction.Left;
                    break;
                case Keys.Right:
                    if (snake.CurrentDirection != Direction.Left)
                        snake.CurrentDirection = Direction.Right;
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    // Класс змейки
    public class Snake
    {
        public List<Point> Body { get; private set; }
        public Direction CurrentDirection { get; set; }

        public Snake()
        {
            // Начальная позиция змейки
            Body = new List<Point>
            {
                new Point(100, 100), // Голова змейки
                new Point(90, 100),  // Тело
                new Point(80, 100)   // Тело
            };
            CurrentDirection = Direction.Right; // По умолчанию движение вправо
        }

        public void Move()
        {
            var head = Body.First();
            Point newHead = head;

            switch (CurrentDirection)
            {
                case Direction.Up:
                    newHead.Y -= 10; // Движение вверх
                    break;
                case Direction.Down:
                    newHead.Y += 10; // Движение вниз
                    break;
                case Direction.Left:
                    newHead.X -= 10; // Движение влево
                    break;
                case Direction.Right:
                    newHead.X += 10; // Движение вправо
                    break;
            }

            // Проверка, выходят ли координаты за границы
            if (newHead.X < 0 || newHead.X >= Screen.PrimaryScreen.Bounds.Width ||
                newHead.Y < 0 || newHead.Y >= Screen.PrimaryScreen.Bounds.Height)
            {
                Console.WriteLine("Змейка вышла за пределы экрана!");
            }

            // Добавляем новую голову и удаляем хвост
            Body.Insert(0, newHead);
            Body.RemoveAt(Body.Count - 1);

            // Выводим координаты головы в консоль
            Console.WriteLine($"Координаты головы: {newHead.X}, {newHead.Y}");
        }
    }

    // Перечисление для направления движения
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}