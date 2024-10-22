using System.Drawing;

/// <summary>
/// Представляет портал в игре.
/// Содержит информацию о расположении и размере портала.
/// </summary>
public class Portal
{
    public Rectangle Bounds { get; set; }
    public Rectangle PassageBounds { get; set; }
    public Rectangle BottomBounds { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Portal"/> с указанными координатами, шириной и высотой.
    /// </summary>
    /// <param name="x">Координата по оси X верхнего левого угла портала.</param>
    /// <param name="width">Ширина портала.</param>
    /// <param name="height">Высота портала.</param>
    public Portal(int x, int width, int height)
    {
        Bounds = new Rectangle(x, 0, width, height);
        BottomBounds = new Rectangle(x, height - (height / 2), width, height / 2);
    }
}
