using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LLK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            point1 = new POINT();
            point2 = new POINT();
            numSelected = 0;
            map = new Map(8, 8);             ;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            map.ClearRoute();
            if (!map.Inner(ClientSize.Height, e.X, e.Y))
                return;
            if(numSelected == 0)
            {
                point1.ReSet(-1, -1);
                point2.ReSet(-1, -1);
                POINT point = map.Coordinate(e.X, e.Y, ClientSize.Height);
                point1.ReSet(point.X, point.Y);
                numSelected++;
                Invalidate();
            }
            else if(numSelected == 1)
            {
                POINT point = map.Coordinate(e.X, e.Y, ClientSize.Height);

                if(map.Attempt(point1.X, point1.Y, point.X, point.Y))
                {
                    point2.ReSet(point.X, point.Y);
                    map.Clear(point1.X, point1.Y);
                    map.Clear(point.X, point.Y);
                    numSelected = 0;
                }
                else
                {
                    point1.ReSet(-1, -1);
                    point2.ReSet(-1, -1);
                    numSelected = 0;
                }
                Invalidate();
            }
            if (map.IsOver())
                label1.Text = "You win";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            map.Draw(e.Graphics, ClientSize.Height);
            map.DrawRectangles(e.Graphics, ClientSize.Height, point1, point2);
            map.DrawRoute(e.Graphics, ClientSize.Height);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
