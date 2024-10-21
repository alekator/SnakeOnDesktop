using System.Drawing;

namespace SnakeOnDesktop
{
    public class Obstacle
    {
        public Rectangle Bounds { get; private set; }
        public Point Position => Bounds.Location; // Добавляем это свойство
        public Obstacle(Point position, Size size)
        {
            Bounds = new Rectangle(position, size);
        }
    }
}
