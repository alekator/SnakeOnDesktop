using System;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Форма для входа и регистрации пользователя.
    /// Позволяет пользователю вводить имя пользователя и пароль для входа или регистрации в игре.
    /// </summary>
    public partial class LoginForm : Form
    {
        private Random random = new Random();
        private DatabaseManager dbManager;

        /// <summary>
        /// Получает имя пользователя после успешного входа.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LoginForm"/>.
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            dbManager = new DatabaseManager(@"CE3HU7L\SQLEXPRESS", "SnakeGameDB");
            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;
        }

        /// <summary>
        /// Обработчик события нажатия кнопки входа.
        /// Проверяет введенные учетные данные и выполняет вход пользователя.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Пожалуйста, введите имя пользователя и пароль.";
                return;
            }

            if (IsValidCredentials(username, password))
            {
                MessageBox.Show($"Добро пожаловать, {username}!");
                this.DialogResult = DialogResult.OK;
                this.Username = username;
                this.Close();
            }
            else
            {
                lblMessage.Text = "Неверное имя пользователя или пароль.";
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопки регистрации.
        /// Позволяет пользователю зарегистрировать новую учетную запись.
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Пожалуйста, введите имя пользователя и пароль.";
                return;
            }

            if (RegisterUser(username, password))
            {
                MessageBox.Show("Регистрация успешна! Вы можете войти.");
            }
            else
            {
                lblMessage.Text = "Ошибка при регистрации. Попробуйте другое имя пользователя.";
            }
        }

        /// <summary>
        /// Регистрирует нового пользователя.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Возвращает true, если регистрация успешна; в противном случае false.</returns>
        private bool RegisterUser(string username, string password)
        {
            if (dbManager.UserExists(username))
            {
                return false;
            }

            dbManager.InsertPlayer(username, 0);
            return true;
        }

        /// <summary>
        /// Проверяет, являются ли введенные учетные данные допустимыми.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Возвращает true, если учетные данные допустимы; в противном случае false.</returns>
        private bool IsValidCredentials(string username, string password)
        {
            return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
