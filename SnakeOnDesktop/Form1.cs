using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Snake snake;
        private List<Desktop.DesktopObject> foodObjects; // Список подходящих объектов для еды
        private int score;
        private Random random;
        private Font scoreFont;
        private Point foodPosition; // Позиция еды
        private Desktop desktop; // Объявите переменную desktop
        private bool isGameOver = false; // Переменная для отслеживания окончания игры
        private const int SegmentSize = 50; // Новый размер сегмента змейки и еды

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
            this.KeyDown += new KeyEventHandler(Form1_KeyDown); // Добавляем обработчик нажатий клавиш

            MinimizeAllWindows(); // Сворачиваем все окна при запуске
        }

        private void MinimizeAllWindows()
        {
            // Импортируем необходимые методы из библиотеки user32.dll
            ShowWindow(GetForegroundWindow(), SW_MINIMIZE);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Обрабатываем клавиши стрелок для изменения направления
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (snake.CurrentDirection != Direction.Down) // Чтобы змейка не могла развернуться на 180 градусов
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
                case Keys.R: // Проверка нажатия R для перезапуска
                    if (isGameOver)
                    {
                        RestartGame(); // Вызов метода перезапуска
                    }
                    break;
            }
        }

        private void RestartGame()
        {
            isGameOver = false; // Сбрасываем состояние игры
            score = 0; // Сбрасываем счёт
            snake = new Snake(); // Создаем новую змейку
            GenerateFood(); // Генерируем новую еду
            gameTimer.Start(); // Запускаем таймер
            Invalidate(); // Обновляем отрисовку формы
        }


        private void InitializeGame()
        {
            isGameOver = false; // Сброс флага окончания игры
            desktop = new Desktop();
            foodObjects = new List<Desktop.DesktopObject>(); // Инициализация списка объектов для еды

            // Инициализация змейки
            snake = new Snake();
            random = new Random();
            // Настройка таймера игры
            gameTimer = new Timer();
            gameTimer.Interval = 10; // Интервал обновления игры в миллисекундах
            gameTimer.Tick += GameTimer_Tick; // Привязка события для обновления игры
            gameTimer.Start();
            score = 0; // Начальный счёт
            scoreFont = new Font("Arial", 24, FontStyle.Bold); // Инициализация шрифта для счета

            GenerateFood(); // Генерация начальной еды
        }

        private void GenerateFood()
        {
            // Генерация случайных координат для еды
            int maxX = (this.ClientSize.Width / SegmentSize) * SegmentSize;
            int maxY = (this.ClientSize.Height / SegmentSize) * SegmentSize;
            foodPosition = new Point(random.Next(0, maxX / SegmentSize) * SegmentSize, random.Next(0, maxY / SegmentSize) * SegmentSize);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            snake.Move(); // Двигаем змейку
            CheckForFood(); // Проверяем на съеденные папки
            CheckCollisions(); // Проверяем на столкновения
            Invalidate(); // Обновляем отрисовку формы
        }

        private void CheckForFood()
        {
            // Проверяем, если голова змейки на позиции еды
            if (snake.Body[0] == foodPosition) // Если голова змейки на еде
            {
                // Получаем последний сегмент (хвост) и добавляем его в конец тела
                Point lastSegment = snake.Body[snake.Body.Count - 1]; // Хвост
                snake.Body.Add(lastSegment); // Добавляем новый сегмент в конец
                score++; // Увеличиваем счёт

                GenerateFood(); // Генерируем новую еду
            }
        }

        private void CheckCollisions()
        {
            // Проверка на столкновение с телом
            var head = snake.Body.First();

            // Проверка выхода за границы
            if (head.X < 0)
            {
                head.X = this.ClientSize.Width - 10; // Появление справа
            }
            else if (head.X >= this.ClientSize.Width)
            {
                head.X = 0; // Появление слева
            }

            if (head.Y < 0)
            {
                head.Y = this.ClientSize.Height - 10; // Появление снизу
            }
            else if (head.Y >= this.ClientSize.Height)
            {
                head.Y = 0; // Появление сверху
            }

            // Обновляем голову в списке
            snake.Body[0] = head;

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
            isGameOver = true; // Устанавливаем флаг окончания игры
            Invalidate(); // Обновляем форму для отображения сообщения
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем тело змейки
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, SegmentSize, SegmentSize); // Рисуем змейку
            }

            // Рисуем еду
            g.FillRectangle(Brushes.Red, foodPosition.X, foodPosition.Y, SegmentSize, SegmentSize); // Рисуем еду

            // Рисуем счёт только если игра не окончена
            if (!isGameOver)
            {
                string scoreText = $"Счёт: {score}"; // Текст счёта
                SizeF textSizes = g.MeasureString(scoreText, scoreFont); // Измеряем размер текста
                float xPosition = (this.ClientSize.Width - textSizes.Width) / 2; // Вычисляем координату X для центрирования
                g.DrawString(scoreText, scoreFont, Brushes.White, new PointF(xPosition, 10)); // Рисуем текст счёта
            }
            else
            {
                // Отображаем сообщение о конце игры
                string gameOverText = $"Игра окончена!\nВаш счёт: {score}\nНажмите R для перезапуска.";
                SizeF textSize = g.MeasureString(gameOverText, scoreFont);
                PointF textPosition = new PointF((this.ClientSize.Width - textSize.Width) / 2, (this.ClientSize.Height - textSize.Height) / 2);
                g.DrawString(gameOverText, scoreFont, Brushes.White, textPosition);
            }
        }

        // Методы и структуры для работы с Windows API
        private const string SHELLDLL_DEFVIEW = "SHELLDLL_DefView";
        private const string PROGMAN = "Progman";
        private const int SW_HIDE = 0; // Код для скрытия окна
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
    }
}
