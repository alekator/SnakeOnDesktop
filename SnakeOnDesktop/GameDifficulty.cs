using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnDesktop
{
    public class GameDifficulty
    {
        public int SnakeSpeed { get; private set; }
        public int FoodSpawnRate { get; private set; }

        public GameDifficulty(int snakeSpeed, int foodSpawnRate)
        {
            SnakeSpeed = snakeSpeed;
            FoodSpawnRate = foodSpawnRate;
        }

        public static GameDifficulty Easy => new GameDifficulty(50, 2000);  // Медленное движение змейки, редкая еда
        public static GameDifficulty Medium => new GameDifficulty(20, 1000); // Среднее движение змейки, нормальная частота еды
        public static GameDifficulty Hard => new GameDifficulty(10, 500);    // Быстрое движение змейки, частая еда
    }


}
