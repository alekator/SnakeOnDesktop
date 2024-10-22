using System;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public partial class LoginForm : Form
    {
        private Random random = new Random(); // Для генерации уникального значения
        private DatabaseManager dbManager; // Объявление экземпляра DatabaseManager
        public string Username { get; private set; }


        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // Установка положения окна

            // Инициализация DatabaseManager с указанием имени сервера и базы данных
            dbManager = new DatabaseManager(@"CE3HU7L\SQLEXPRESS", "SnakeGameDB"); // Укажите имя сервера

            btnLogin.Click += btnLogin_Click; // Подписка на событие кнопки Вход
            btnRegister.Click += btnRegister_Click; // Подписка на событие кнопки Регистрация
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Пожалуйста, введите имя пользователя и пароль.";
                return;
            }

            // Логика проверки учетных данных
            if (IsValidCredentials(username, password))
            {
                MessageBox.Show($"Добро пожаловать, {username}!");
                this.DialogResult = DialogResult.OK; // Закрываем форму с результатом OK
                this.Username = username; // Возвращаем имя пользователя
                this.Close();
            }
            else
            {
                lblMessage.Text = "Неверное имя пользователя или пароль.";
            }
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Пожалуйста, введите имя пользователя и пароль.";
                return;
            }

            // Логика регистрации нового пользователя
            if (RegisterUser(username, password))
            {
                MessageBox.Show("Регистрация успешна! Вы можете войти.");
            }
            else
            {
                lblMessage.Text = "Ошибка при регистрации. Попробуйте другое имя пользователя.";
            }
        }

        private bool RegisterUser(string username, string password)
        {
            // Проверяем, существует ли пользователь с таким именем
            if (dbManager.UserExists(username))
            {
                return false; // Если пользователь существует, возвращаем false
            }

            // Если пользователь не существует, вставляем нового пользователя с начальным счетом 0
            dbManager.InsertPlayer(username, 0); // Запись нового пользователя с начальным счетом 0
            return true; // Возвращаем true для успешной регистрации
        }


        private bool IsValidCredentials(string username, string password)
        {
            // Здесь должна быть логика для проверки учетных данных
            return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
        }

        
    }
}
