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
        private List<Desktop.DesktopObject> foodObjects; // Список подходящих объектов для еды
        private int score;
        private Random random;
        private Font scoreFont;
        private Point foodPosition; // Позиция еды
        private Desktop desktop; // Объявите переменную desktop
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
            }
        }

        private void InitializeGame()
        {
            desktop = new Desktop();
            foodObjects = new List<Desktop.DesktopObject>(); // Инициализация списка объектов для еды

            // Сканируем объекты на рабочем столе и фильтруем их
            FilterFoodObjects();

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

            GenerateFood(); // Генерация начальной еды
        }

        private void FilterFoodObjects()
        {
            List<Desktop.DesktopObject> desktopObjects = desktop.ScanDesktop();

            // Фильтруем объекты, чтобы оставить только те, которые могут быть едой
            foodObjects = desktopObjects
                .Where(obj => obj.Bounds.Width > 0 && obj.Bounds.Height > 0)
                .GroupBy(obj => new { obj.Bounds.X, obj.Bounds.Y, obj.Bounds.Width, obj.Bounds.Height }) // Группируем по координатам и размеру
                .Select(group => group.First()) // Выбираем только один объект из группы повторяющихся
                .ToList();

            // Логирование объектов для проверки
            foreach (var obj in foodObjects)
            {
                Console.WriteLine($"Объект для еды: {obj.Title}, Позиция: {obj.Bounds}");
            }
        }


        private void GenerateFood()
        {
            // Проверка, есть ли доступные объекты для еды
            if (foodObjects.Count > 0)
            {
                // Выбираем случайный объект
                Desktop.DesktopObject randomObject = foodObjects[random.Next(foodObjects.Count)];

                // Устанавливаем позицию еды в случайный объект
                foodPosition = new Point(randomObject.Bounds.Left, randomObject.Bounds.Top);

                // Скрываем выбранный объект
                HideDesktopObject(randomObject);
            }
            else
            {
                Console.WriteLine("Объектов нет для еды"); // Отладочное сообщение
                // Если объектов нет, генерируем случайные координаты
                int maxX = (this.ClientSize.Width / 10) * 10;
                int maxY = (this.ClientSize.Height / 10) * 10;
                foodPosition = new Point(random.Next(0, maxX / 10) * 10, random.Next(0, maxY / 10) * 10);
            }
        }

        private void HideDesktopObject(Desktop.DesktopObject desktopObject)
        {
            // Скрыть объект на рабочем столе, если это возможно
            IntPtr hwnd = FindWindow(null, desktopObject.Title);
            if (hwnd != IntPtr.Zero)
            {
                ShowWindow(hwnd, SW_HIDE); // Скрываем объект
            }
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
            // Проверяем, если голова змейки на позиции еды
            if (snake.Body[0] == foodPosition) // Если голова змейки на еде
            {
                // Получаем последний сегмент (хвост) и добавляем его в конец тела
                Point lastSegment = snake.Body[snake.Body.Count - 1]; // Хвост
                snake.Body.Add(lastSegment); // Добавляем новый сегмент в конец
                score++; // Увеличиваем счёт
                Console.WriteLine($"Еда съедена! Текущий счёт: {score}"); // Для теста выводим сообщение

                // Проверка, были ли съедены все объекты
                if (foodObjects.Count > 0)
                {
                    GenerateFood(); // Генерируем новую еду
                }
                else
                {
                    GameOver(); // Конец игры, если все объекты съедены
                }

                // Увеличиваем скорость игры
                gameTimer.Interval = Math.Max(10, gameTimer.Interval - 5); // Увеличиваем скорость, уменьшая интервал
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

        // Метод для отрисовки змейки на форме
        // Метод для отрисовки змейки на форме
        // Метод для отрисовки змейки на форме
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем тело змейки
            foreach (var segment in snake.Body)
            {
                g.FillRectangle(Brushes.Green, segment.X, segment.Y, 10, 10); // Рисуем змейку
            }

            // Рисуем еду
            g.FillRectangle(Brushes.Red, foodPosition.X, foodPosition.Y, 10, 10); // Рисуем еду

            // Рисуем счёт
            g.DrawString($"Счёт: {score}", scoreFont, Brushes.White, new PointF(10, 10));

            // Рисуем синие прямоугольники вокруг объектов еды
            foreach (var foodObject in foodObjects)
            {
                Rectangle screenBounds = foodObject.Bounds;

                // Преобразуем координаты окна рабочего стола в координаты окна приложения, если требуется
                Point relativePosition = this.PointToClient(screenBounds.Location);
                Rectangle adjustedBounds = new Rectangle(relativePosition, screenBounds.Size);

                // Рисуем синий прямоугольник по границам объекта
                g.DrawRectangle(Pens.Blue, adjustedBounds);
            }
        }


    }
}
