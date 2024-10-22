using System.Drawing;
using System;

/// <summary>
/// Класс Food управляет созданием и размещением еды для змейки в игре.
/// </summary>
public class Food
{
    private Random random;
    public Point Position { get; private set; }
    public int CenterX => Position.X + SegmentSize / 2;
    public int CenterY => Position.Y + SegmentSize / 2;
    private const int SegmentSize = 50;

    /// <summary>
    /// Конструктор по умолчанию для создания еды с начальным случайным положением.
    /// </summary>
    public Food()
    {
        random = new Random();
        GenerateRandomFood(800, 600); // Параметры ширины и высоты формы
    }

    /// <summary>
    /// Генерирует случайное положение еды на игровом поле в пределах заданных размеров формы.
    /// </summary>
    /// <param name="formWidth">Ширина игровой формы.</param>
    /// <param name="formHeight">Высота игровой формы.</param>
    public void GenerateRandomFood(int formWidth, int formHeight)
    {
        Position = new Point(
            random.Next(0, formWidth / SegmentSize) * SegmentSize,
            random.Next(0, formHeight / SegmentSize) * SegmentSize
        );
    }
}
