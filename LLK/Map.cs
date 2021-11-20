using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LLK
{
    class POINT
    {
        public int X { set; get; }
        public int Y { set; get; }
        public void ReSet(int x, int y) { X = x; Y = y; }
        public POINT(int a = -1, int b = -1) { X = a; Y = b; }

    }
    class Map
    {
        private ListBox listBox;

        private List<POINT> route;
        public void SetListBox(ref ListBox listbox)
            {
                listBox = listbox;
            }
        private int[,] metric;
        private int Remained;
        private int length;
        private int typeSize;
        public Map(int _length, int size = 8)
        {
            length = _length; typeSize = size;
            Generate();
            Remained = length * length;
            route = new List<POINT>();
        }

        private void Generate()
        {
            metric = new int[length + 2, length + 2];
            List<int> list = new List<int>();
            Random random = new Random();
            int index;
            int currentPosition = 0;
            for (int i = 0; i < length * length; i++)
                list.Add(i % typeSize);
            Console.WriteLine(list.Count);
            for (int i = 0; i < length + 2; i++)
                for (int j = 0; j < length + 2; j++)
                {
                    if (i == 0 || i == length + 1 || j == 0 || j == length + 1)
                    {
                        metric[i, j] = 0;
                        continue;
                    }
                    index = random.Next() % typeSize;
                    currentPosition = (currentPosition + index > list.Count) ? currentPosition + index - list.Count : currentPosition + index;
                    currentPosition = currentPosition >= list.Count ? currentPosition % list.Count : currentPosition;
                    metric[i, j] = list.ElementAt<int>(currentPosition) + 1;
                    list.RemoveAt(currentPosition);
                }
        }

        public void Print()
        {
            for (int i = 0; i < length + 2; i++)
            {
                for (int j = 0; j < length + 2; j++)
                    if (metric[i, j] == 0)
                        Console.Write("  ");
                    else
                        Console.Write($"{metric[i, j] } ");
                Console.Write("\n");
            }

        }
        public void Clear(int x, int y)
        {
            metric[x, y] = 0;
            Remained--;
        }
        public bool IsOver() { return Remained == 0; }

        //判断
        private bool SingleJudge(int ax, int ay, int bx, int by)
        {
            int direction;
            if (ax == bx && ay == by)
                return true;
            if (ax == bx)
            {
                direction = (by > ay ? 1 : -1);
                for (int i = ay + direction; i != by; i += direction)
                    if (metric[ax, i] != 0)
                        return false;
            }
            if (ay == by)
            {
                direction = (bx > ax ? 1 : -1);
                for (int i = ax + direction; i != bx; i += direction)
                    if (metric[i, ay] != 0)
                        return false;
            }
            return true;
        }
        private bool SingleDiversionJudge(int ax, int ay, int bx, int by)
        {
            if (ax == bx || ay == by)
                return false;
            if (SingleJudge(ax, ay, ax, by) && SingleJudge(bx, by, ax, by) && metric[ax, by] == 0)
            {
                AddRouteNode(ax, ay);
                AddRouteNode(ax, by);
                AddRouteNode(bx, by);
                return true;
            }
            if (SingleJudge(ax, ay, bx, ay) && SingleJudge(bx, by, bx, ay) && metric[bx, ay] == 0)
            {
                AddRouteNode(ax, ay);
                AddRouteNode(bx, ay);
                AddRouteNode(bx, by);
                return true;
            }
            return false;
        }
        private bool DoubleDiversionJudge(int ax, int ay, int bx, int by)
        {
            for (int i = 0; i < length + 2; i = (i + 1 == ax || i + 1 == bx) ? i + 2 : i + 1, i = (i == ax || i == bx) ? i + 1 : i)
                if (SingleJudge(ax, ay, i, ay) && SingleJudge(bx, by, i, by) && SingleJudge(i, ay, i, by) && metric[i, ay] == 0 && metric[i, by] == 0)
                {
                    AddRouteNode(ax, ay);
                    AddRouteNode(i, ay);
                    AddRouteNode(i, by);
                    AddRouteNode(bx, by);
                    return true;
                }
            for (int j = 0; j < length + 2; j = (j + 1 == ay || j + 1 == by) ? j + 2 : j + 1, j = (j == ay || j == by) ? j + 1 : j) 
                if (SingleJudge(ax, ay, ax, j) && SingleJudge(bx, by, bx, j) && SingleJudge(ax, j, bx, j) && metric[ax, j] == 0 && metric[bx, j] == 0)
                {
                    AddRouteNode(ax, ay);
                    AddRouteNode(ax, j);
                    AddRouteNode(bx, j);
                    AddRouteNode(bx, by);
                    return true;
                }
            return false;
        }
        public bool Attempt(int ax, int ay, int bx, int by)
        {
            ClearRoute();
            if (metric[ax, ay] != metric[bx, by])
                return false;
            if (ax == bx && ay == by)
                return false;
            else if (ax == bx || ay == by)
            {
                if (SingleJudge(ax, ay, bx, by))
                {
                    AddRouteNode(ax, ay);
                    AddRouteNode(bx, by);
                    return true;
                }
            }

            if (SingleDiversionJudge(ax, ay, bx, by))
            {
                return true;
            }
            if (DoubleDiversionJudge(ax, ay, bx, by))
            {
                return true;
            }
            return false;
       
        }
        //绘制
        public void DrawRectangles(Graphics graphics, float bianchang, POINT p1, POINT p2)
        {
            float danwei = bianchang / (length + 2);
            Pen pen = new Pen(Color.Black, 3);
            RectangleF rect = new RectangleF();
            rect.Width = rect.Height = danwei;
            if(p1.X != -1)
            {
                rect.X = p1.X * danwei;
                rect.Y = p1.Y * danwei;
                graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
            if (p2.X != -1)
            {
                rect.X = p2.X * danwei;
                rect.Y = p2.Y * danwei;
                graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
        public void Draw(Graphics graphics, float bianchang)
        {
            float danwei = bianchang / (length + 2);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            RectangleF rect = new RectangleF();
            Font font = new Font(FontFamily.GenericSerif, 20);

            for(int i = 0; i < length + 2; i++)
                for(int j = 0; j < length + 2; j++)
                {
                    rect.X = i * danwei;
                    rect.Y = j * danwei;
                    rect.Width = rect.Height = danwei;
                    if (metric[i, j] == 0)
                        graphics.FillRectangle(whiteBrush, rect);
                    else
                    {
                        SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255 * metric[i, j]  / length, 0));
                        graphics.FillRectangle(brush, rect);
                        graphics.DrawString($"{metric[i, j]}", font, whiteBrush, rect.X, rect.Y);
                    }
                }
        }
        public void DrawRoute(Graphics graphics, float bianchang)
        {
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush, 5);
            float danwei = bianchang / (length + 2);
            for(int i = 0; i < route.Count - 1; i++)
            {
                POINT point1 = route[i];
                POINT point2 = route[i + 1];
                graphics.DrawLine(pen, point1.X * danwei + danwei / 2, point1.Y * danwei + danwei / 2, point2.X * danwei + danwei / 2, point2.Y * danwei + danwei / 2);
                graphics.FillEllipse(brush, point1.X * danwei + danwei / 2 - 5, point1.Y * danwei + danwei / 2 - 5, 10, 10);
            }
            if(route.Count > 0)
                graphics.FillEllipse(brush, route.Last().X * danwei + danwei / 2 - 5, route.Last().Y * danwei + danwei / 2 - 5, 10, 10);
        }

        public POINT Coordinate(int x, int y, int danwei)
        {
            danwei /= (length + 2);
            if (x / danwei >= length + 2 || x / danwei < 0 || y / danwei >= length + 2 || y / danwei < 0)
                return new POINT(-1, -1);
            return new POINT(x / danwei, y / danwei);
        }
        public bool Inner(int bianchang, int x, int y)
        {
            int danwei = bianchang / (length + 2);
            if (x > danwei && x < (bianchang - danwei) && y > danwei && y < (bianchang - danwei))
                return true;
            return false;
        }
        //设置路径
        private void AddRouteNode(int ax, int ay)
        {
            route.Add(new POINT(ax, ay));
        }
        private void DeleteRouteNode(int ax, int ay)
        {
            route.RemoveAt(route.Count - 1);
        }
        public void ClearRoute()
        {
            route.Clear();
        }

    }
}