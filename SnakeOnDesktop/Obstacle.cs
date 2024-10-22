using System.Drawing;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Представляет препятствие в игре.
    /// Содержит информацию о расположении и размере препятствия.
    /// </summary>
    public class Obstacle
    {
        public Rectangle Bounds { get; private set; }
        public Point Position => Bounds.Location;
        public Obstacle(Point position, Size size)
        {
            Bounds = new Rectangle(position, size);
        }
    }
}
