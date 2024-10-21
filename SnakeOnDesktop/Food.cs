using System.Drawing;
using System;

public class Food
{
    private Random random;
    public Point Position { get; private set; } // Позиция еды

    public int CenterX => Position.X + SegmentSize / 2;
    public int CenterY => Position.Y + SegmentSize / 2;

    private const int SegmentSize = 50; // Размер сегмента еды

    public Food()
    {
        random = new Random();
        GenerateRandomFood(800, 600); // Параметры ширины и высоты формы
    }

    public void GenerateRandomFood(int formWidth, int formHeight)
    {
        Position = new Point(
            random.Next(0, formWidth / SegmentSize) * SegmentSize,
            random.Next(0, formHeight / SegmentSize) * SegmentSize
        );
    }
}
