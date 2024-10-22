using System;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    internal static class Program
    {
        /// <summary>
        /// Статический класс, содержащий точку входа в приложение.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new GameForm());
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}
