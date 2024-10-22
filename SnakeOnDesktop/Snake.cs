using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SnakeOnDesktop
{
    /// <summary>
    /// Представляет змею в игре, включая ее тело и направление движения.
    /// </summary>
    public class Snake
    {
        public List<Point> Body { get; private set; }
        public Direction CurrentDirection { get; set; }

        public Snake()
        {
            Body = new List<Point>
            {
                new Point(500, 500),
                new Point(490, 500),
                new Point(480, 500)
            };
            CurrentDirection = Direction.Right;
        }

        public void Move()
        {
            var head = Body.First();
            Point newHead = head;

            switch (CurrentDirection)
            {
                case Direction.Up:
                    newHead.Y -= 10;
                    break;
                case Direction.Down:
                    newHead.Y += 10;
                    break;
                case Direction.Left:
                    newHead.X -= 10;
                    break;
                case Direction.Right:
                    newHead.X += 10;
                    break;
            }
            Body.Insert(0, newHead);
            Body.RemoveAt(Body.Count - 1);

            Console.WriteLine($"Координаты головы: {newHead.X}, {newHead.Y}");
        }

        /// <summary>
        /// Увеличивает длину змеи, добавляя новый сегмент в конец тела.
        /// </summary>
        public void Grow()
        {
            Point lastSegment = Body[Body.Count - 1];
            Body.Add(lastSegment);
        }

    }
}
