using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace labirint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            KeyPreview = true;
            Global.size = 5;
            Global.step = 20;
            Global.density = 3;
            Global.WidthBrouser = 70;
            Global.LengthBrouser = 450;
            Global.StartPositionX = 455;
            Global.StartPositionY = 0;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush WallUp = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, 0), new Point(Global.LengthBrouser / 2, Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallLeft = new LinearGradientBrush(new Point(0, Global.LengthBrouser / 2), new Point(Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallDown = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, Global.LengthBrouser), new Point(Global.LengthBrouser / 2, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallRight = new LinearGradientBrush(new Point(Global.LengthBrouser, Global.LengthBrouser / 2), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallForward = new LinearGradientBrush(new Point(Global.WidthBrouser, Global.WidthBrouser), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            if (Global.lab[Global.x1, Global.y1 - 1] == 0)
            {
                DrawHandLeft(e.Graphics, WallLeft, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandLeft(e.Graphics, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1, Global.y1 + 1] == 0)
            {
                DrawHandRight(e.Graphics, WallRight, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandRight(e.Graphics, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1 - 1, Global.y1] == 0)
            {
                DrawHandUp(e.Graphics, WallUp, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandUp(e.Graphics, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1 + 1, Global.y1] == 0)
            {
                DrawHandDown(e.Graphics, WallDown, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandDown(e.Graphics, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            DrawHandForward(e.Graphics, WallForward, Global.LengthBrouser, Global.WidthBrouser, 1);
            DrawHandLine(e.Graphics, new Pen(Color.Black), Global.LengthBrouser, Global.WidthBrouser, 1);
            int step = Global.step;
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Red, Global.step / 5);
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step + step * 11 / 10);
                    } //leftborder
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step - step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step - step);
                    } //upborder
                    if (i == Global.n - 2 || j == Global.n - 2)
                    {
                        if (Global.lab[i, j + 1] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step + step * 11 / 10);
                        } //rightborder
                        if (Global.lab[i + 1, j] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step + step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step + step);
                        } //downborder
                    }
                }
            }
            for (int i = 1; i < Global.n - 1; i += 2)
            {
                for (int j = 1; j < Global.n - 1; j += 2)
                {
                    if (Global.sand[i, j] == 0)
                        sand(i, j);
                }
            }            
            if (!Global.exit)
            {
                Pen p2 = new Pen(Color.Black, Global.step / 5);
                int x = Global.x1;
                int y = Global.y1;
                road t = Global.way;
                x = Global.x1; y = Global.y1;
                    while (t != null)
                    {
                        if (t.cur == 1) { g.DrawLine(p2, y * Global.step, x * Global.step, (y + 2) * Global.step, x * Global.step); y += 2; }
                        if (t.cur == 2) { g.DrawLine(p2, y * Global.step, x * Global.step, (y - 2) * Global.step, x * Global.step); y -= 2; }
                        if (t.cur == 3) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x + 2) * Global.step); x += 2; }
                        if (t.cur == 4) { g.DrawLine(p2, y * Global.step, x * Global.step, y * Global.step, (x - 2) * Global.step); x -= 2; }
                        t = t.next;
                    }
            }            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        protected void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Global.exit)
            {
                int step = Global.step;
                if (e.KeyCode == Keys.A && Global.molot > 0)
                {
                    Global.ataka = true;
                    Global.molot--;
                    textmolot.Text = Convert.ToString(Global.molot);
                }
                Graphics g = this.CreateGraphics();
                Pen p = new Pen(BackColor, Global.step / 5);
                if (e.KeyCode == Keys.B && Global.bomba > 0)
                {
                    if (Global.x1 != Global.size * 2 - 1)
                    {
                        Global.lab[Global.x1 + 1, Global.y1] = 1;
                    }
                    if (Global.x1 != 1)
                    {
                        Global.lab[Global.x1 - 1, Global.y1] = 1;
                    }
                    if (Global.y1 != 1)
                    {
                        Global.lab[Global.x1, Global.y1 - 1] = 1;
                    }
                    if (Global.y1 != Global.size * 2 - 1)
                    {
                        Global.lab[Global.x1, Global.y1 + 1] = 1;
                    }
                    clear();
                    Global.bomba--;
                    textbomb.Text = Convert.ToString(Global.bomba);
                    Global.ataka = false;
                }
                if (Global.ataka && e.KeyCode != Keys.A)
                {
                    if (e.KeyCode == Keys.Down && Global.x1 != Global.size * 2 - 1)
                    {
                        Global.lab[Global.x1 + 1, Global.y1] = 1;
                    }
                    if (e.KeyCode == Keys.Up && Global.x1 != 1)
                    {
                        Global.lab[Global.x1 - 1, Global.y1] = 1;
                    }
                    if (e.KeyCode == Keys.Left && Global.y1 != 1)
                    {
                        Global.lab[Global.x1, Global.y1 - 1] = 1;
                    }
                    if (e.KeyCode == Keys.Right && Global.y1 != Global.size * 2 - 1)
                    {
                        Global.lab[Global.x1, Global.y1 + 1] = 1;
                    }
                    clear();
                    Global.ataka = false;
                }
                if (!Global.ataka)
                {
                    if (e.KeyCode == Keys.Down)
                    {
                        if (Global.lab[Global.x1 + 1, Global.y1] != 0)
                        {
                            Global.x1 += 2;
                            pictureBox1.Location = new System.Drawing.Point(Global.StartPositionX + Global.step / 2 + (Global.y1 - 1) * step, Global.StartPositionY + Global.step / 2 + (Global.x1 - 1) * step);
                        }
                    }
                    if (e.KeyCode == Keys.Up)
                    {
                        if (Global.lab[Global.x1 - 1, Global.y1] != 0)
                        {
                            Global.x1 -= 2;
                            pictureBox1.Location = new System.Drawing.Point(Global.StartPositionX + Global.step / 2 + (Global.y1 - 1) * step, Global.StartPositionY + Global.step / 2 + (Global.x1 - 1) * step);
                            }
                    }
                    if (e.KeyCode == Keys.Left)
                    {
                        if (Global.lab[Global.x1, Global.y1 - 1] != 0)
                        {
                            Global.y1 -= 2;
                            pictureBox1.Location = new System.Drawing.Point(Global.StartPositionX + Global.step / 2 + (Global.y1 - 1) * step, Global.StartPositionY + Global.step / 2 + (Global.x1 - 1) * step);
                        }
                    }
                    if (e.KeyCode == Keys.Right)
                    {
                        if (Global.lab[Global.x1, Global.y1 + 1] != 0)
                        {
                            Global.y1 += 2;
                            pictureBox1.Location = new System.Drawing.Point(Global.StartPositionX + Global.step / 2 + (Global.y1 - 1) * step, Global.StartPositionY + Global.step / 2 + (Global.x1 - 1) * step);
                        }
                    }
                }
            }
            if ((Global.x1 == Global.n && Global.y1 == Global.n / 2) || (Global.x1 == Global.n && Global.y1 == Global.n / 2 - 1))
            {
                Global.exit = false;
                textresult.Text = "exit: true";
            }
            else
            {
                if (Global.sand[Global.y1, Global.x1] == 0)
                {
                    Global.sand[Global.y1, Global.x1] = 1;
                    Global.sgore += 100;
                    textSgore.Text = Convert.ToString(Global.sgore);
                }
            }
            clear();
        }
        /*
        protected void stepup(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Black, Global.step / 5);
            Pen p2 = new Pen(BackColor, Global.step / 5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step/2, (x - 1) * Global.step + Global.step/2);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textresult.Text = "exit: true";
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
            Pen p = new Pen(Color.Black, Global.step/5);
            Pen p2 = new Pen(BackColor, Global.step/5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step/2, (x - 1) * Global.step + Global.step/2);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textresult.Text = "exit: true"; Global.exit = false;
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
            Pen p = new Pen(Color.Black, Global.step/5);
            Pen p2 = new Pen(BackColor, Global.step/5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step/2, (x - 1) * Global.step + Global.step/2);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textresult.Text = "exit: true"; Global.exit = false;
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
            Pen p = new Pen(Color.Black, Global.step/5);
            Pen p2 = new Pen(BackColor, Global.step/5);
            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step/2, (x - 1) * Global.step + Global.step/2);
            System.Threading.Thread.Sleep(Global.speed);
            if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2-1))
            {
                textresult.Text = "exit: true";
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
         */ //old algoritm exit
        protected void clear()
        {
            int step = Global.step;
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Red, Global.step / 5);
            g.Clear(BackColor);
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step + step * 11 / 10);
                    } //leftborder
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step - step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step - step);
                    } //upborder
                    if (i == Global.n - 2 || j == Global.n - 2)
                    {
                        if (Global.lab[i, j + 1] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step + step * 11 / 10);
                        } //rightborder
                        if (Global.lab[i + 1, j] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step + step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step + step);
                        } //downborder
                    }
                }
            }
            for (int i = 1; i < Global.n - 1; i += 2)
            {
                for (int j = 1; j < Global.n - 1; j += 2)
                {
                    if (Global.sand[i, j] == 0)
                        sand(i, j);
                }
            }
            LinearGradientBrush WallUp = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, 0), new Point(Global.LengthBrouser / 2, Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallLeft = new LinearGradientBrush(new Point(0, Global.LengthBrouser / 2), new Point(Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallDown = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, Global.LengthBrouser), new Point(Global.LengthBrouser / 2, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallRight = new LinearGradientBrush(new Point(Global.LengthBrouser, Global.LengthBrouser / 2), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallForward = new LinearGradientBrush(new Point(Global.WidthBrouser, Global.WidthBrouser), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            if (Global.lab[Global.x1, Global.y1 - 1] == 0)
            {
                DrawHandLeft(g, WallLeft, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandLeft(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1, Global.y1 + 1] == 0)
            {
                DrawHandRight(g, WallRight, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandRight(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1 - 1, Global.y1] == 0)
            {
                DrawHandUp(g, WallUp, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandUp(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1 + 1, Global.y1] == 0)
            {
                DrawHandDown(g, WallDown, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandDown(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            DrawHandForward(g, WallForward, Global.LengthBrouser, Global.WidthBrouser, 1);
            DrawHandLine(g, new Pen(Color.Black), Global.LengthBrouser, Global.WidthBrouser, 1);
        }
        protected void sand(int x, int y)
        {
            Graphics g = this.CreateGraphics();
            SolidBrush blue = new SolidBrush(Color.DarkViolet);
            Point[] points = new Point[3];
            points[0].X = Global.StartPositionX + x * Global.step - Global.step / 2;
            points[0].Y = Global.StartPositionY + y * Global.step + Global.step / 4;
            points[1].X = Global.StartPositionX + x * Global.step;
            points[1].Y = Global.StartPositionY + y * Global.step - Global.step / 4;
            points[2].X = Global.StartPositionX + x * Global.step + Global.step / 2;
            points[2].Y = Global.StartPositionY + y * Global.step + Global.step / 4;
            g.FillPolygon(blue, points);
        }
        /*
        private void button1_Click(object sender, EventArgs e) //go to ep
        {
            if (Global.size < 80 && Global.size > 10)
            {
                Global.exit = true;
                stepdown(Global.x1, Global.y1);
                if (Global.exit)
                {
                    textresult.Text = "exit: false";
                    pictureBox1.Location = new System.Drawing.Point(Global.y1 * Global.step - Global.step / 2, Global.x1 * Global.step - Global.step / 2);
                }
                for (int i = 0; i < Global.n; i++)
                {
                    for (int j = 0; j < Global.n; j++)
                    {
                        Global.lab2[i, j] = 1;
                    }
                }
            }
            else
            {
                if (Global.size > 10)
                    textresult.Text = "big size";
                else
                {
                    textresult.Text = "small size";
                }
            }
        }
         */
        private void button1_Click_1(object sender, EventArgs e)//goto new road
        {
            for (int i = 0; i < Global.n; i++)
            {
                for (int j = 0; j < Global.n; j++)
                {
                    Global.lab2[i, j] = 1;
                }
            }
            int x = Global.x1;
            int y = Global.y1;
            int back;
            road way = new road();
            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step / 2, (x - 1) * Global.step + Global.step / 2);
            Global.exit = true;
            while (Global.exit)
            {
                if ((x == Global.n && y == Global.n / 2) || (x == Global.n && y == Global.n / 2 - 1))
                {
                    textresult.Text = "exit: true";
                    Global.exit = false;
                    Global.way = way.head;
                }
                else
                {
                    System.Threading.Thread.Sleep(Global.speed);
                    if (Global.lab[x + 1, y] != 0 && Global.lab2[x + 1, y] != 0) //down
                    {
                        Global.lab2[x + 1, y] = 0;
                        way.add(3); x += 2;
                        pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step / 2, (x - 1) * Global.step + Global.step / 2);
                    }
                    else
                        if (Global.lab[x, y + 1] != 0 && Global.lab2[x, y + 1] != 0) //right
                        {
                            Global.lab2[x, y + 1] = 0;
                            way.add(1); y += 2;
                            pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step / 2, (x - 1) * Global.step + Global.step / 2);
                        }
                        else
                            if (Global.lab[x, y - 1] != 0 && Global.lab2[x, y - 1] != 0) //left
                            {
                                Global.lab2[x, y - 1] = 0;
                                way.add(2); y -= 2;
                                pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step / 2, (x - 1) * Global.step + Global.step / 2);
                            }
                            else
                                if (Global.lab[x - 1, y] != 0 && Global.lab2[x - 1, y] != 0) //up
                                {
                                    Global.lab2[x - 1, y] = 0;
                                    way.add(4); x -= 2;
                                    pictureBox1.Location = new System.Drawing.Point((y - 1) * Global.step + Global.step / 2, (x - 1) * Global.step + Global.step / 2);
                                }//1-r 2-l 3-d 4-u
                                else
                                {
                                    if (way.size() != 0)
                                    {
                                        back = way.deq();
                                        if (back == 1) { y -= 2; }
                                        if (back == 2) { y += 2; }
                                        if (back == 3) { x -= 2; }
                                        if (back == 4) { x += 2; }
                                    }
                                    else
                                    {
                                        textresult.Text = "exit: false";
                                        Global.exit = false;
                                        Global.way = null;
                                    }
                                }
                }
            }
        } 
        private void button2_Click(object sender, EventArgs e) //generation
        {
            Global.step = 100 / Global.size;
            Global.ataka = false;
            pictureBox1.Height = Global.step;
            pictureBox1.Width = Global.step;
             textresult.Text = "exit:";
            Global.exit = true;
            Global.molot = 3;
            Global.bomba = 1;
            Global.sgore = 0;
            textbomb.Text = "1";
            textmolot.Text = "3";
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
                //if (size > 1)
                //lab[1, 2] = 1;lab[2, 1] = 1;
            }
            int[,] lab2 = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    lab2[i, j] = 1;
                }
            }
            Global.sand = lab2;
            Global.lab2 = lab2;
            Global.lab = lab;
            Global.n = n;
            Global.x = 5;
            Global.y = 5;
            Global.x1 = 1;
            Global.y1 = 1;
            Graphics g = this.CreateGraphics();
            //g.Clear(BackColor);
            pictureBox1.Visible = true;
            pictureBox1.Location = new System.Drawing.Point(Global.StartPositionX + Global.step / 2, Global.StartPositionY + Global.step / 2);
            Pen p = new Pen(Color.Red, Global.step/5);
            int step = Global.step;
            for (int i = 1; i < Global.n - 1; i = i + 2)
            {
                for (int j = 1; j < Global.n - 1; j = j + 2)
                {
                    if (Global.lab[i, j - 1] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step - step, Global.StartPositionY + i * step + step * 11 / 10);
                    } //leftborder
                    if (Global.lab[i - 1, j] == 0)
                    {
                        g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step - step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step - step);
                    } //upborder
                    if (i == n - 2 || j == n - 2)
                    {
                        if (Global.lab[i, j + 1] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step - step * 11 / 10, Global.StartPositionX + j * step + step, Global.StartPositionY + i * step + step * 11 / 10);
                        } //rightborder
                        if (Global.lab[i + 1, j] == 0)
                        {
                            g.DrawLine(p, Global.StartPositionX + j * step - step * 11 / 10, Global.StartPositionY + i * step + step, Global.StartPositionX + j * step + step * 11 / 10, Global.StartPositionY + i * step + step);
                        } //downborder
                    }
                }
            }
            for (int i = 1; i < Global.n; i++)
            {
                Global.sand[rnd.Next(1, Global.n / 2 + 1) * 2 - 1, rnd.Next(1, Global.n / 2 + 1) * 2 - 1] = 0;
            }
            for (int i = 1; i < Global.n-1; i += 2)
            {
                for (int j = 1; j < Global.n-1; j += 2)
                {
                    if(Global.sand[i,j]==0)
                    sand(i, j);
                }
            }
            clear();
            LinearGradientBrush WallUp = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, 0), new Point(Global.LengthBrouser / 2, Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallLeft = new LinearGradientBrush(new Point(0, Global.LengthBrouser / 2), new Point(Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallDown = new LinearGradientBrush(new Point(Global.LengthBrouser / 2, Global.LengthBrouser), new Point(Global.LengthBrouser / 2, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            LinearGradientBrush WallRight = new LinearGradientBrush(new Point(Global.LengthBrouser, Global.LengthBrouser / 2), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser / 2), Color.Gray, Color.LightGray);
            LinearGradientBrush WallForward = new LinearGradientBrush(new Point(Global.WidthBrouser, Global.WidthBrouser), new Point(Global.LengthBrouser - Global.WidthBrouser, Global.LengthBrouser - Global.WidthBrouser), Color.Gray, Color.LightGray);
            if (Global.lab[Global.x1, Global.y1 - 1] == 0)
            {
                DrawHandLeft(g, WallLeft, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandLeft(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }            
            if (Global.lab[Global.x1, Global.y1 + 1] == 0)
            {
                DrawHandRight(g, WallRight, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandRight(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }            
            if (Global.lab[Global.x1 - 1, Global.y1] == 0)
            {
                DrawHandUp(g, WallUp, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandUp(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            if (Global.lab[Global.x1 + 1, Global.y1] == 0)
            {
                DrawHandDown(g, WallDown, Global.LengthBrouser, Global.WidthBrouser, 1);
            }
            else
            {
                DrawHandDown(g, new SolidBrush(Color.White), Global.LengthBrouser, Global.WidthBrouser, 1);
            }            
            DrawHandForward(g, WallForward, Global.LengthBrouser, Global.WidthBrouser, 1);
            DrawHandLine(g, new Pen(Color.Black), Global.LengthBrouser, Global.WidthBrouser, 1);
        }
        private void button3_Click(object sender, EventArgs e) //cleartrack
        {
            clear();
        }
        private void trackBar1_Scroll(object sender, EventArgs e) //speed
        {
            Global.speed = 100 / trackspeed.Value;
            textspeed.Text = Convert.ToString(trackspeed.Value);
        }
        private void trackBar2_Scroll(object sender, EventArgs e) //size
        {
            Global.size = tracksize.Value;
            textsize.Text = Convert.ToString(tracksize.Value);
        }
        private void trackBar3_Scroll(object sender, EventArgs e) //plotnost
        {
            Global.density = trackdensity.Value;
            textdensity.Text = Convert.ToString(100/trackdensity.Value)+"%";
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void DrawHandUp(Graphics g, Brush brush, int length, int width, int x)
        {
            Point[] points = new Point[4];
            points[0].X = 0; points[0].Y = 0;
            points[1].X = length; points[1].Y = 0;
            points[2].X = length - width; points[2].Y = width;
            points[3].X = 0 + width; points[3].Y = width;
            g.FillPolygon(brush, points);
        }
        private void DrawHandLeft(Graphics g, Brush brush, int length, int width, int x)
        {
            Point[] points = new Point[4];
            points[0].X = 0; points[0].Y = 0;
            points[1].X = width; points[1].Y = width;
            points[2].X = width; points[2].Y = length - width;
            points[3].X = 0; points[3].Y = length;
            g.FillPolygon(brush, points);
        }
        private void DrawHandDown(Graphics g, Brush brush, int length, int width, int x)
        {
            Point[] points = new Point[4];
            points[0].X = 0; points[0].Y = length;
            points[1].X = width; points[1].Y = length - width;
            points[2].X = length - width; points[2].Y = length - width;
            points[3].X = length; points[3].Y = length;
            g.FillPolygon(brush, points);
        }
        private void DrawHandRight(Graphics g, Brush brush, int length, int width, int x)
        {
            Point[] points = new Point[4];
            points[0].X = length; points[0].Y = 0;
            points[1].X = length - width; points[1].Y = width;
            points[2].X = length - width; points[2].Y = length - width;
            points[3].X = length; points[3].Y = length;
            g.FillPolygon(brush, points);
        }
        private void DrawHandForward(Graphics g, Brush brush, int length, int width, int x)
        {
            Point[] points = new Point[4];
            points[0].X = width; points[0].Y = width;
            points[1].X = length - width; points[1].Y = width;
            points[2].X = length - width; points[2].Y = length - width;
            points[3].X = width; points[3].Y = length - width;
            g.FillPolygon(brush, points);
        }
        private void DrawHandLine(Graphics g, Pen pen, int length, int width, int x)
        {
            g.DrawLine(pen, 0, 0, width, width);
            g.DrawLine(pen, 0, 0, length, 0);
            g.DrawLine(pen, 0, 0, 0, length);
            g.DrawLine(pen, width, width, width, length - width);
            g.DrawLine(pen, width, width, length - width, width);
            g.DrawLine(pen, length, 0, length - width, width);
            g.DrawLine(pen, length, 0, length, length);
            g.DrawLine(pen, 0, length, width, length - width);
            g.DrawLine(pen, 0, length, length, length);
            g.DrawLine(pen, width, length - width, length - width, length - width);
            g.DrawLine(pen, length - width, width, length - width, length - width);
            g.DrawLine(pen, length - width, length - width, length, length);


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
            private static int _molot;
            public static int molot
            { get { return _molot; } set { _molot = value; } }
            private static int _bomba;
            public static int bomba
            { get { return _bomba; } set { _bomba = value; } }
            private static bool _ataka;
            public static bool ataka
            { get { return _ataka; } set { _ataka = value; } }
            private static int[,] _sand;
            public static int[,] sand
            { get { return _sand; } set { _sand = value; } }
            private static int _sgore;
            public static int sgore
            { get { return _sgore; } set { _sgore = value; } }
            private static road _way;
            public static road way
            { get { return _way; } set { _way = value; } }
            private static int _LengthBrouser;
            public static int LengthBrouser
            { get { return _LengthBrouser; } set { _LengthBrouser = value; } }
            private static int _WidthBrouser;
            public static int WidthBrouser
            { get { return _WidthBrouser; } set { _WidthBrouser = value; } }
            private static int _StartPositionX;
            public static int StartPositionX
            { get { return _StartPositionX; } set { _StartPositionX = value; } }
            private static int _StartPositionY;
            public static int StartPositionY
            { get { return _StartPositionY; } set { _StartPositionY = value; } }
        }
        public class road
        {
            public road next; //ссылка на следующий элемент   
            public road prev; //ссылка на предыдущий элемент
            public int cur; //запись значения в список
            public road head = null; //ссылка на начало списка
            public road end = null; //ссылка на конец списка
            public road s = null; //ссылка на первый элемент
            public road()
            { //конструктор списка
                end = null;
            }
            public void add(int e)
            { //добавление элемента в список
                road l = new road();
                if (head == null)
                { //если список пуст
                    l.cur = e;
                    end = l;
                    l.next = null;
                    head = l;
                    s = l;
                }
                else
                { //если в списке есть хотя бы один элемент
                    s = end;
                    end = l;
                    s.next = l;
                    l.cur = e;
                    l.prev = s;
                    l.next = null;
                    s = l;
                }
            }
            public int getlast()
            {//ф-я возвращает последний элемент
                return end.cur;
            }
            public int size()
            { //ф-я возвращает размер списка
                int i = 0;
                road t = head;
                while (t != null)
                {
                    i++;
                    t = t.next;
                }
                return i;
            }
            public void print()
            { //ф-я выводит на экран весь список
                road t = head;
                while (t != null)
                {
                    Console.Write(t.cur);
                    t = t.next;
                }
            }
            public int deq()
            { //ф-я возвращает последний элемент при этом удаляя его из списка  
                int q;
                q = end.cur;
                if (size() > 1)
                {
                    end = end.prev;
                    end.next = null;
                }
                else
                {
                    head = null;
                    end = null;
                }
                return q;
            }
        }
        public static Random rnd = new Random();
    }
}
