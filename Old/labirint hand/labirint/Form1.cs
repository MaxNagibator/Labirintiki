using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace labirint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            Global.size = 19;
            Global.step = 20;
            Global.density = 3;
        }
        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            int step = 20;
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Red, 3);
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step - step, i * step + step);
                    }
                    if (Global.lab[i, j + 1] == 0)
                    {
                        g.DrawLine(p, j * step + step, i * step - step, j * step + step, i * step + step);
                    }
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step + step, i * step - step);
                    }
                    if (Global.lab[i + 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step + step, j * step + step, i * step + step);
                    }
                }
            }
        }
         */
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        protected void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            /* //тут ручное перемещение хренотени
            if(Global.x1 == Global.n && Global.y1 == Global.n/2)
            {
                Global.exit = false;
            }

            if (Global.exit)
            {
                int step = 20;
                if (e.KeyCode == Keys.Down)
                {
                    if (Global.lab[Global.x1 + 1, Global.y1] != 0)
                    {
                        Global.x1 += 2;
                        pictureBox1.Location = new System.Drawing.Point(10 + (Global.y1 - 1) * step, 10 + (Global.x1 - 1) * step);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {
                    if (Global.lab[Global.x1 - 1, Global.y1] != 0)
                    {
                        Global.x1 -= 2;
                        pictureBox1.Location = new System.Drawing.Point(10 + (Global.y1 - 1) * step, 10 + (Global.x1 - 1) * step);
                    }
                }
                if (e.KeyCode == Keys.Left)
                {
                    if (Global.lab[Global.x1, Global.y1 - 1] != 0)
                    {
                        Global.y1 -= 2;
                        pictureBox1.Location = new System.Drawing.Point(10 + (Global.y1 - 1) * step, 10 + (Global.x1 - 1) * step);
                    }
                }
                if (e.KeyCode == Keys.Right)
                {
                    if (Global.lab[Global.x1, Global.y1 + 1] != 0)
                    {
                        Global.y1 += 2;
                        pictureBox1.Location = new System.Drawing.Point(10 + (Global.y1 - 1) * step, 10 + (Global.x1 - 1) * step);
                    }
                }              
            }
            */
        }
        protected void stepup(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, 5);
            Pen p2 = new Pen(BackColor, 5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * 20 + 10, (x - 1) * 20 + 10);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textBox1.Visible = true;
                textBox1.Text = "exit: true";
                Global.exit = false;
            }
            if (Global.exit)
            {
                if (Global.lab[x, y + 1] != 0 && Global.lab2[x, y + 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y + 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y + 1] = 0;
                    stepright(x, y + 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y + 2) * Global.step, x * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x, y - 1] != 0 && Global.lab2[x, y - 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y - 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y - 1] = 0;
                    stepleft(x, y - 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y - 2) * Global.step, x * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x - 1, y] != 0 && Global.lab2[x - 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step);
                    Global.lab2[x - 1, y] = 0;
                    stepup(x - 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step); }
                }
            }
        }
        protected void stepright(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, 5);
            Pen p2 = new Pen(BackColor, 5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * 20 + 10, (x - 1) * 20 + 10);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textBox1.Visible = true;
                textBox1.Text = "exit: true"; Global.exit = false;
            }
            if (Global.exit)
            {
                if (Global.lab[x + 1, y] != 0 && Global.lab2[x + 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step);
                    Global.lab2[x + 1, y] = 0;
                    stepdown(x + 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x, y + 1] != 0 && Global.lab2[x, y + 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y + 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y + 1] = 0;
                    stepright(x, y + 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y + 2) * Global.step, x * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x - 1, y] != 0 && Global.lab2[x - 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step);
                    Global.lab2[x - 1, y] = 0;
                    stepup(x - 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step); }
                }
            }
        }
        protected void stepleft(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, 5);
            Pen p2 = new Pen(BackColor, 5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * 20 + 10, (x - 1) * 20 + 10);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textBox1.Visible = true;
                textBox1.Text = "exit: true"; Global.exit = false;
            }
            if (Global.exit)
            {
                if (Global.lab[x + 1, y] != 0 && Global.lab2[x + 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step);
                    Global.lab2[x + 1, y] = 0;
                    stepdown(x + 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x, y - 1] != 0 && Global.lab2[x, y - 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y - 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y - 1] = 0;
                    stepleft(x, y - 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y - 2) * Global.step, x * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x - 1, y] != 0 && Global.lab2[x - 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step);
                    Global.lab2[x - 1, y] = 0;
                    stepup(x - 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step); }
                }
            }
        }
        protected void stepdown(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, 5);
            Pen p2 = new Pen(BackColor, 5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * 20 + 10, (x - 1) * 20 + 10);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textBox1.Visible = true;
                textBox1.Text = "exit: true";
                Global.exit = false;
            }
            if (Global.exit)
            {
                if (Global.lab[x + 1, y] != 0 && Global.lab2[x + 1, y] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step);
                    Global.lab2[x + 1, y] = 0;
                    stepdown(x + 2, y);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x, y + 1] != 0 && Global.lab2[x, y + 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y + 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y + 1] = 0;
                    stepright(x, y + 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y + 2) * Global.step, x * Global.step); }
                }
            }
            if (Global.exit)
            {
                if (Global.lab[x, y - 1] != 0 && Global.lab2[x, y - 1] != 0)
                {
                    g.DrawLine(p, y * Global.step, x * Global.step, (y - 2) * Global.step, (x) * Global.step);
                    Global.lab2[x, y - 1] = 0;
                    stepleft(x, y - 2);
                    if (Global.exit) { g.DrawLine(p2, y * Global.step, x * Global.step, (y - 2) * Global.step, x * Global.step); }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e) //go to ep
        {
            Global.exit = true;
            stepdown(Global.x1, Global.y1);
            if (Global.exit)
            {
                textBox1.Visible = true;
                textBox1.Text = "exit: false";
            }
            for (int i = 0; i < Global.n; i++)
            {
                for (int j = 0; j < Global.n; j++)
                {
                    Global.lab2[i, j] = 1;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e) //generation
        {
            int size = Global.size;//nechetniy
            int n = size * 2 + 1;
            int[,] lab = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == 0 || i == n - 1)
                    {
                        lab[i, j] = 0;
                    }
                    if (j == 0 || j == n - 1)
                    {
                        lab[i, j] = 0;
                    }
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        lab[i, j] = 2;
                    }
                    if ((i > 0 && i % 2 == 0 && i < n - 1 && j > 0 && j % 2 == 1 && j < n - 1) ||
                        (i > 0 && i % 2 == 1 && i < n - 1 && j > 0 && j % 2 == 0 && j < n - 1))
                    {
                        lab[i, j] = rnd.Next(0, Global.density);
                    }
                }
                lab[n - 1, n / 2] = 1;
                lab[n - 1, n / 2-1] = 1;
            }
            int[,] lab2 = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    lab2[i, j] = 1;
                }
            }
            Global.lab2 = lab2;
            Global.lab = lab;
            Global.n = n;
            Global.x = 5;
            Global.y = 5;
            Global.x1 = 1;
            Global.y1 = 1;
            Graphics g = this.CreateGraphics();
            g.Clear(BackColor);
            pictureBox1.Visible = true;
            pictureBox1.Location = new System.Drawing.Point(10, 10);
            Pen p = new Pen(Color.Red, 3);
            int step = Global.step;
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step - step, i * step + step);
                    }
                    if (Global.lab[i, j + 1] == 0)
                    {
                        g.DrawLine(p, j * step + step, i * step - step, j * step + step, i * step + step);
                    }
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step + step, i * step - step);
                    }
                    if (Global.lab[i + 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step + step, j * step + step, i * step + step);
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e) //cleartrack
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Red, 3);
            int step = Global.step;
            g.Clear(BackColor);
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step - step, i * step + step);
                    }
                    if (Global.lab[i, j + 1] == 0)
                    {
                        g.DrawLine(p, j * step + step, i * step - step, j * step + step, i * step + step);
                    }
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step - step, j * step + step, i * step - step);
                    }
                    if (Global.lab[i + 1, j] == 0)
                    {
                        g.DrawLine(p, j * step - step, i * step + step, j * step + step, i * step + step);
                    }
                }
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e) //speed
        {
            Global.speed = 100 / trackBar1.Value;
            textBox2.Text = Convert.ToString(trackBar1.Value);
        }
        private void trackBar2_Scroll(object sender, EventArgs e) //size
        {
            Global.size = trackBar2.Value;
            textBox3.Text = Convert.ToString(trackBar2.Value);
        }
        private void trackBar3_Scroll(object sender, EventArgs e) //plotnost
        {
            Global.density = trackBar3.Value;
            textBox4.Text = Convert.ToString(100/trackBar3.Value)+"%";
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        public class Global
        {
            private static int[,] _lab;
            public static int[,] lab
            { get { return _lab; } set { _lab = value; } }
            private static int[,] _lab2;
            public static int[,] lab2
            { get { return _lab2; } set { _lab2 = value; } }
            private static int _n;
            public static int n
            { get { return _n; } set { _n = value; } }
            private static int _x;
            public static int x
            { get { return _x; } set { _x = value; } }
            private static int _y;
            public static int y
            { get { return _y; } set { _y = value; } }
            private static int _x1;
            public static int x1
            { get { return _x1; } set { _x1 = value; } }
            private static int _y1;
            public static int y1
            { get { return _y1; } set { _y1 = value; } }
            private static bool _exit;
            public static bool exit
            { get { return _exit; } set { _exit = value; } }
            private static int _speed;
            public static int speed
            { get { return _speed; } set { _speed = value; } }
            private static int _size;
            public static int size
            { get { return _size; } set { _size = value; } }
            private static int _step;
            public static int step
            { get { return _step; } set { _step = value; } }
            private static int _density;
            public static int density
            { get { return _density; } set { _density = value; } }
        }
        public static Random rnd = new Random();
    }
}
