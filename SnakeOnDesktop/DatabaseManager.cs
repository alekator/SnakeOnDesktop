using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SnakeOnDesktop
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string serverName, string databaseName)
        {
            connectionString = $"Server={serverName};Database={databaseName};Trusted_Connection=True; Connect Timeout=60;";
        }

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

        public async Task<List<LeaderboardEntry>> GetTopEntriesAsync(int count)
        {
            var entries = new List<LeaderboardEntry>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT TOP(@Count) Username, MaxScore FROM Leaderboard ORDER BY MaxScore DESC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Count", count);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            entries.Add(new LeaderboardEntry
                            {
                                Username = reader.GetString(0),
                                MaxScore = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }

            return entries;
        }

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
