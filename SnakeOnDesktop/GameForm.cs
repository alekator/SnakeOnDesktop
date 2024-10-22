using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
 /// <summary>
 /// Класс формы игры. Отвечает за инициализацию окна игры, 
 /// выбор уровня сложности, перехват событий нажатия клавиш, 
 /// инициализацию и отрисовку игрового процесса.
 /// </summary>
    public partial class GameForm : Form
    {
        private Game game;
        private GameDifficulty selectedDifficulty;
        private string username;

        /// <summary>
        /// Конструктор формы игры. Устанавливает параметры окна и 
        /// добавляет обработчики событий. Также сворачивает все открытые окна.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            ShowDifficultySelection();

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;
            this.TransparencyKey = this.BackColor;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            MinimizeAllWindows();
        }

        /// <summary>
        /// Отображает сообщение для выбора уровня сложности игры на экране.
        /// Добавляет обработчик события Paint для отрисовки выбора.
        /// </summary>
        private void ShowDifficultySelection()
        {
            this.Paint += Form1_SelectDifficulty;
        }

        /// <summary>
        /// Отрисовывает текст выбора уровня сложности на экране.
        /// </summary>
        private void Form1_SelectDifficulty(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            string message = "Выберите уровень сложности:\n1 - Легкий\n2 - Средний\n3 - Сложный";
            g.DrawString(message, new Font("Arial", 24, FontStyle.Bold), Brushes.White, new PointF(100, 100));
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш. Если уровень сложности не выбран, 
        /// выбор происходит в зависимости от нажатой клавиши (1, 2, 3).
        /// Иначе передает нажатие клавиши объекту игры.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (selectedDifficulty == null)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        selectedDifficulty = GameDifficulty.Easy;
                        StartGame(selectedDifficulty);
                        break;
                    case Keys.D2:
                        selectedDifficulty = GameDifficulty.Medium;
                        StartGame(selectedDifficulty);
                        break;
                    case Keys.D3:
                        selectedDifficulty = GameDifficulty.Hard;
                        StartGame(selectedDifficulty);
                        break;
                }
            }
            else
            {
                game.KeyDown(e.KeyCode);
            }
        }

        /// <summary>
        /// Запускает игру, удаляя экран выбора сложности и инициализируя игровой процесс.
        /// </summary>
        private void StartGame(GameDifficulty difficulty)
        {
            this.Paint -= Form1_SelectDifficulty;
            InitializeGame(difficulty);
        }

        /// <summary>
        /// Инициализирует объект игры, используя выбранный уровень сложности и строку подключения к базе данных.
        /// </summary>
        private void InitializeGame(GameDifficulty difficulty)
        {
            string connectionString = @"Server=CE3HU7L\SQLEXPRESS;Database=SnakeGameDB;Trusted_Connection=True; Connect Timeout=60;";
            game = new Game(this, difficulty, connectionString, username);
            Invalidate();
        }

        /// <summary>
        /// Сворачивает все открытые окна, используя WinAPI.
        /// </summary>
        private void MinimizeAllWindows()
        {
            ShowWindow(GetForegroundWindow(), SW_MINIMIZE);
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш во время игры и передает их объекту игры.
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            game.KeyDown(e.KeyCode);
        }

        /// <summary>
        /// Отрисовывает игровой процесс на экране. Если объект игры инициализирован, 
        /// вызывает метод отрисовки игры.
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (game != null)
            {
                game.Draw(g);
            }
        }

        // Константы для работы с оконной системой через WinAPI
        private const string SHELLDLL_DEFVIEW = "SHELLDLL_DefView";
        private const string PROGMAN = "Progman";
        private const int SW_HIDE = 0;
        private const int SW_MINIMIZE = 2;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
