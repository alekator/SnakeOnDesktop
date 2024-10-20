using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeOnDesktop
{
    public class Snake
    {
        public List<Point> Body { get; private set; }
        public Direction CurrentDirection { get; set; }

        public Snake()
        {
            // Начальная позиция змейки
            Body = new List<Point>
            {
                new Point(500, 500), // Голова змейки
                new Point(490, 500),  // Тело
                new Point(480, 500)   // Тело
            };
            CurrentDirection = Direction.Right; // По умолчанию движение вправо
        }

        public void Move()
        {
            var head = Body.First();
            Point newHead = head;

            switch (CurrentDirection)
            {
                case Direction.Up:
                    newHead.Y -= 10; // Движение вверх
                    break;
                case Direction.Down:
                    newHead.Y += 10; // Движение вниз
                    break;
                case Direction.Left:
                    newHead.X -= 10; // Движение влево
                    break;
                case Direction.Right:
                    newHead.X += 10; // Движение вправо
                    break;
            }
            // Добавляем новую голову и удаляем хвост
            Body.Insert(0, newHead);
            Body.RemoveAt(Body.Count - 1);

            // Выводим координаты головы в консоль
            Console.WriteLine($"Координаты головы: {newHead.X}, {newHead.Y}");
        }
    }
}
