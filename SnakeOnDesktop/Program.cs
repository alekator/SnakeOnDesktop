using System;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Создаем и показываем LoginForm
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK) // Проверяем, успешна ли аутентификация
                {
                    // Запускаем основную форму, если вход успешен
                    Application.Run(new Form1());
                }
                else
                {
                    // Если аутентификация не удалась, завершаем приложение
                    Application.Exit();
                }
            }
        }
    }
}
