using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public partial class Form1 : Form
    {
        private Game game; // Объявите переменную game
        private const int SegmentSize = 50; // Новый размер сегмента змейки и еды
        private GameDifficulty selectedDifficulty;
        public Form1()
        {
            InitializeComponent();
            ShowDifficultySelection();

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
        private void ShowDifficultySelection()
        {
            this.Paint += Form1_SelectDifficulty;
        }
        private void Form1_SelectDifficulty(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            string message = "Выберите уровень сложности:\n1 - Легкий\n2 - Средний\n3 - Сложный";
            g.DrawString(message, new Font("Arial", 24, FontStyle.Bold), Brushes.White, new PointF(100, 100));
        }
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
                game.KeyDown(e.KeyCode); // Передаем управление в игру после выбора сложности
            }
        }
        private void StartGame(GameDifficulty difficulty)
        {
            this.Paint -= Form1_SelectDifficulty;
            InitializeGame(difficulty);
        }

        private void InitializeGame(GameDifficulty difficulty)
        {
            game = new Game(this, difficulty); // Передаем выбранную сложность в игру
            Invalidate(); // Перерисовка экрана
        }

        private void MinimizeAllWindows()
        {
            // Импортируем необходимые методы из библиотеки user32.dll
            ShowWindow(GetForegroundWindow(), SW_MINIMIZE);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            game.KeyDown(e.KeyCode); // Передаем нажатую клавишу в игру
        }

        //private void InitializeGame()
        //{
        //    game = new Game(this); // Инициализируем игру
        //}

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (game != null) // Проверяем, инициализирована ли переменная game
            {
                game.Draw(g); // Рисуем игровую логику только если game не равен null
            }
        }


        // Методы и структуры для работы с Windows API
        private const string SHELLDLL_DEFVIEW = "SHELLDLL_DefView";
        private const string PROGMAN = "Progman";
        private const int SW_HIDE = 0; // Код для скрытия окна
        private const int SW_MINIMIZE = 2; // Код для сворачивания окна

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
