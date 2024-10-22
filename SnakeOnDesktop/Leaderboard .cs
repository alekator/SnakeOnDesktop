using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;

/// <summary>
/// Класс для работы с таблицей лидеров в базе данных.
/// Позволяет получать топ записи и максимальные очки пользователя.
/// </summary>
public class Leaderboard
{
    private string connectionString;

    /// <summary>
    /// Конструктор класса Leaderboard.
    /// Инициализирует объект с заданной строкой подключения к базе данных.
    /// </summary>
    /// <param name="connectionString">Строка подключения к базе данных.</param>
    public Leaderboard(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>
    /// Получает топ записей из таблицы лидеров.
    /// </summary>
    /// <param name="count">Количество записей для получения.</param>
    /// <returns>Список записей лидеров.</returns>
    public List<LeaderboardEntry> GetTopEntries(int count)
    {
        var entries = new List<LeaderboardEntry>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT TOP(@Count) Username, MaxScore FROM Leaderboard ORDER BY MaxScore DESC";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Count", count);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
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

    /// <summary>
    /// Получает максимальное количество очков для заданного пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <returns>Максимальное количество очков.</returns>
    public int GetMaxScore(string username)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT MaxScore FROM Leaderboard WHERE Username = @Username";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}

/// <summary>
/// Представляет запись в таблице лидеров, содержащую имя пользователя и максимальный счет.
/// </summary>
public class LeaderboardEntry
{
    public string Username { get; set; }
    public int MaxScore { get; set; }
}
