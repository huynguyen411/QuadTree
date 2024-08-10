using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net.Http;
using System.Net.WebSockets;

namespace ConsoleApp1
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public class Rectangle
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public Rectangle(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        // Check if the space contains the point
        public bool Contains(Point point)
        {
            return (point.X >= MinX && point.X <= MaxX &&
                    point.Y >= MinY && point.Y <= MaxY);
        }

        // Check the intersection between 2 space
        public bool Intersects(Rectangle range)
        {
            return !(range.MinX > MaxX || range.MaxX < MinX ||
                     range.MinY > MaxY || range.MaxY < MinY);
        }
    }
    public class QuadTree
    {
        private const int Capacity = 4;
        private List<Point> Points;
        private Rectangle Boundary;
        private bool Divided;
        private QuadTree NE, NW, SE, SW;

        public QuadTree(Rectangle boundary)
        {
            Boundary = boundary;
            Points = new List<Point>();
            Divided = false;
        }

        // Sub divide current space to 4 children space
        private void Subdivide()
        {
            double xMid = (Boundary.MinX + Boundary.MaxX) / 2;
            double yMid = (Boundary.MinY + Boundary.MaxY) / 2;

            Rectangle ne = new Rectangle(xMid, yMid, Boundary.MaxX, Boundary.MaxY);
            NE = new QuadTree(ne);

            Rectangle nw = new Rectangle(Boundary.MinX, yMid, xMid, Boundary.MaxY);
            NW = new QuadTree(nw);

            Rectangle se = new Rectangle(xMid, Boundary.MinY, Boundary.MaxX, yMid);
            SE = new QuadTree(se);

            Rectangle sw = new Rectangle(Boundary.MinX, Boundary.MinY, xMid, yMid);
            SW = new QuadTree(sw);

            Divided = true;
        }

        // Insert point
        public bool Insert(Point point)
        {
            if (!Boundary.Contains(point))
            {
                return false;
            }

            if (Points.Count < Capacity)
            {
                Points.Add(point);
                return true;
            }
            else
            {
                if (!Divided)
                {
                    Subdivide();
                }

                if (NE.Insert(point)) return true;
                if (NW.Insert(point)) return true;
                if (SE.Insert(point)) return true;
                if (SW.Insert(point)) return true;
            }

            return false;
        }

        // Query to get all points in the range
        public List<Point> Query(Rectangle range, List<Point> found)
        {
            if (!Boundary.Intersects(range))
            {
                return found;
            }

            foreach (var point in Points)
            {
                if (range.Contains(point))
                {
                    found.Add(point);
                }
            }

            if (Divided)
            {
                NE.Query(range, found);
                NW.Query(range, found);
                SE.Query(range, found);
                SW.Query(range, found);
            }

            return found;
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            Rectangle boundary = new Rectangle(0, 0, 400, 400);
            QuadTree qt = new QuadTree(boundary);

            // Insert points
            qt.Insert(new Point(100, 100));
            qt.Insert(new Point(150, 150));
            qt.Insert(new Point(200, 200));
            qt.Insert(new Point(250, 250));
            qt.Insert(new Point(300, 300));

            // init rectangle
            Rectangle range = new Rectangle(100, 100, 250, 250);
            List<Point> found = qt.Query(range, new List<Point>());

            // Query
            foreach (var point in found)
            {
                Console.WriteLine($"({point.X}, {point.Y})");
            }
        }
    }
}

