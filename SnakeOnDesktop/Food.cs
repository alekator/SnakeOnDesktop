using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public class Food
    {
        private Random random;
        public Point Position { get; private set; } // Позиция еды

        public Food(int segmentSize)
        {
            random = new Random();
            GenerateFood(segmentSize);
        }

        public void GenerateFood(int segmentSize)
        {
            // Генерация случайных координат для еды
            int maxX = (Screen.PrimaryScreen.Bounds.Width / segmentSize) * segmentSize;
            int maxY = (Screen.PrimaryScreen.Bounds.Height / segmentSize) * segmentSize;
            Position = new Point(random.Next(0, maxX / segmentSize) * segmentSize, random.Next(0, maxY / segmentSize) * segmentSize);
        }
    }
}
