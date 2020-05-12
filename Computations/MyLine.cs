using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LORA.Computations
{
    public class MyLine 
    {
        LineGeometry line;
        public Point _start, _end;
        Path _myPath;
        public MyLine(Point start, Point end, Canvas canvas)
        {
            line = new LineGeometry(start, end);
            _start = start;
            _end = end;
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 0.5;
            myPath.Data = line;
            canvas.Children.Add(myPath);
            _myPath = myPath;
        } 
        public Path GetMyPath()
        {
            return _myPath;
        }
    }


    public class CreateRectangle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public List<Point> Create(Point p1, Point p2, double w)
        {
            List<Point> reL = new List<Point>();

            double x1 = p1.X;
            double x2 = p2.X;
            double y1 = p1.Y;
            double y2 = p2.Y;
            double d = w / 2;
            double D = Operations.DistanceBetweenTwoPoints(p1, p2);
            double DelateY = d * ((x2 - x1) / D);
            double DeltaX = d * ((y1 - y2) / D);

            Point p3 = new Point(x1 + DeltaX, y1 + DelateY);
            Point p4 = new Point(x1 - DeltaX, y1 - DelateY);
            Point p5 = new Point(x2 + DeltaX, y2 + DelateY);
            Point p6 = new Point(x2 - DeltaX, y2 - DelateY);

            reL.Add(p3);
            reL.Add(p4);
            reL.Add(p5);
            reL.Add(p6);

            return reL;


        }

        public void DrawCreate(Point p1, Point p2, double w, Canvas canvas) 
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double y1 = p1.Y;
            double y2 = p2.Y;
            double d = w / 2;
            double D = Operations.DistanceBetweenTwoPoints(p1, p2);
            double DelateY = d * ((x2 - x1) / D);
            double DeltaX = d * ((y1 - y2) / D);

            Point p3 = new Point(x1 + DeltaX, y1 + DelateY);
            Point p4 = new Point(x1 - DeltaX, y1 - DelateY);
            Point p5 = new Point(x2 + DeltaX, y2 + DelateY);
            Point p6 = new Point(x2 - DeltaX, y2 - DelateY);


            MyLine l34 = new Computations.MyLine(p3, p4, canvas);
            MyLine l56 = new Computations.MyLine(p5, p6, canvas);
            MyLine l35 = new Computations.MyLine(p3, p5, canvas);
            MyLine l46 = new Computations.MyLine(p4, p6, canvas);

        }


    }

    /// <summary>
    /// https://www.researchgate.net/post/Given_a_line_L1p1_p2_how_to_get_a_parallel_line_L2_p3_p4#59018c24eeae3978c77b372f
    /// </summary>
    public class ParalleLine
    {
        /// <summary>
        /// Given L1.
        /// RL=return Line
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="canvas"></param>
        public MyLine DrawParallelines(MyLine L1, double D, Canvas canvas) 
        {
            Point p1 = L1._start;
            Point p2 = L1._end;
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double perp_x = -dy;
            double perp_y = dx;
            double len = Math.Sqrt(perp_x * perp_x + perp_y * perp_y);
            perp_x /= len;
            perp_y /= len;
            perp_x *= D;
            perp_y *= D;

            Point p3 = new Point();
            p3.X = p1.X + perp_x;
            p3.Y = p1.Y + perp_y;

            Point p4 = new Point();
            p4.X = p2.X + perp_x;
            p4.Y = p3.Y + perp_y;

            MyLine relIne = new MyLine(p4, p3, canvas);

            return relIne;
        }
    }
}
