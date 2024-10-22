using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Управляет взаимодействием с базой данных для игры "Змейка".
    /// </summary>
    public class DatabaseManager
    {
        private string connectionString;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DatabaseManager"/>.
        /// </summary>
        /// <param name="serverName">Имя сервера базы данных.</param>
        /// <param name="databaseName">Имя базы данных.</param>
        public DatabaseManager(string serverName, string databaseName)
        {
            connectionString = $"Server={serverName};Database={databaseName};Trusted_Connection=True; Connect Timeout=60;";
        }

        /// <summary>
        /// Проверяет, существует ли пользователь с указанным именем в базе данных.
        /// </summary>
        /// <param name="username">Имя пользователя для проверки.</param>
        /// <returns>Возвращает true, если пользователь существует; в противном случае false.</returns>
        public bool UserExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Leaderboard WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Вставляет нового игрока в таблицу Leaderboard с начальным максимальным счетом.
        /// </summary>
        /// <param name="username">Имя пользователя, которого нужно вставить.</param>
        /// <param name="maxScore">Начальный максимальный счет для пользователя.</param>
        public void InsertPlayer(string username, int maxScore)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Leaderboard (Username, MaxScore) VALUES (@Username, @MaxScore)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@MaxScore", maxScore);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновляет максимальный счет для указанного пользователя, если новый счет больше текущего.
        /// </summary>
        /// <param name="username">Имя пользователя, для которого нужно обновить максимальный счет.</param>
        /// <param name="newMaxScore">Новый максимальный счет.</param>
        public void UpdateMaxScore(string username, int newMaxScore)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Leaderboard SET MaxScore = @MaxScore WHERE Username = @Username AND MaxScore < @MaxScore";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaxScore", newMaxScore);
                    command.Parameters.AddWithValue("@Username", username);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Запись для пользователя '{username}' обновлена. Новый максимальный счет: {newMaxScore}.");
                    }
                    else
                    {
                        Console.WriteLine($"Запись для пользователя '{username}' не обновлена. Возможно, новый максимальный счет меньше или равен текущему.");
                    }
                }
            }
        }
    }
}
