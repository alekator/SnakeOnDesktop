using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;

public class Leaderboard
{
    private string connectionString;

    public Leaderboard(string connectionString)
    {
        this.connectionString = connectionString;
    }

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
                return result != null ? Convert.ToInt32(result) : 0; // Возвращаем 0, если игрок не найден
            }
        }
    }

}

public class LeaderboardEntry
{
    public string Username { get; set; }
    public int MaxScore { get; set; }
}
