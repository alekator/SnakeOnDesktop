using System.Drawing;

public class Portal
{
    public Rectangle Bounds { get; set; } // Границы верхней части портала
    public Rectangle PassageBounds { get; set; } // Границы прохода
    public Rectangle BottomBounds { get; set; } // Границы нижней части портала

    public Portal(int x, int width, int height)
    {
        Bounds = new Rectangle(x, 0, width, height);
        BottomBounds = new Rectangle(x, height - (height / 2), width, height / 2); // Инициализируем нижнюю часть
    }
}
