using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab2
{
    public partial class Form1 : Form
    {
        List<Points3D> p3 = new List<Points3D>();
        List<Points3D> Scaled3D = new List<Points3D>();
        List<Points3D> Moved3D = new List<Points3D>();
        Point[] points = { new Point(0, 0) };
        Bitmap myBitmap;
        Graphics g;
        Pen myPen = new Pen(Color.Black, 1);
        int[,] connect = new int[29, 2];
        bool animation = false;
        int angle_rotate = 0, scale = 0, transfer = 0, spiral = 0;
        int angle_x, angle_y, angle_z;
        double scale_x, scale_z, scale_y;
        int move_x, move_y, move_z;

        public Form1()
        {
            InitializeComponent();
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(myBitmap);
            g.TranslateTransform(330, 400);
        }

        private void button1_Click(object sender, EventArgs e) //загрузить
        {
            g.Clear(pictureBox1.BackColor);
            p3.Clear();
            try
            {
                using (StreamReader reader = new StreamReader("Elem.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] str = line.Split(new char[] { ' ' });
                        p3.Add(new Points3D(Convert.ToDouble(str[0]), Convert.ToDouble(str[1]), Convert.ToDouble(str[2])));
                    }
                }
                using (StreamReader reader = new StreamReader("Connectors.txt"))
                {
                    string line;
                    int l = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] str = line.Split(new char[] { ' ' });
                        connect[l, 0] = Convert.ToInt32(str[0]);
                        connect[l, 1] = Convert.ToInt32(str[1]);
                        Console.WriteLine(connect[l, 0]);
                        l++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem with files. " + ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            To2D(p3);
            Draw();
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        public void To2D(List<Points3D> point2)
        {
            points = new Point[point2.Count];
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = Convert.ToInt32(p3[i].X/(1 - (p3[i].Z/250)));
                points[i].Y = Convert.ToInt32(p3[i].Y/(1 - (p3[i].Z/250)));
            }
        }

        public void Draw()
        {
            Pen ax = new Pen(Color.Indigo, 2);
            g.Clear(pictureBox1.BackColor);
            g.DrawLine(ax, 0, 0, 350, 0);
            g.DrawLine(ax, 0, 0, 0, -350);
            g.DrawLine(ax, 0, 0, -200, 200);

            for (int i = 0; i < connect.GetLength(0); i++)
            {
                g.DrawLine(myPen, points[connect[i, 0] - 1].X, points[connect[i, 0] - 1].Y, points[connect[i, 1] - 1].X, points[connect[i, 1] - 1].Y);
            }
            pictureBox1.Image = myBitmap;
        }

        private void button6_Click(object sender, EventArgs e) //движение по спирали 
        {
            spiral = 0;
            if (animation)
            {
                MessageBox.Show("Wait a second, please ;)", "Not yet!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                animation = true;
                timerSpiral.Enabled = true;
            }
        }

        private void timerSpiral_Tick(object sender, EventArgs e)
        {
            if (spiral == 100)
            {
                animation = false;
                timerSpiral.Enabled = false;
            }
            for (int i = 0; i < p3.Count; i++)
            {
                p3[i].X += Convert.ToDouble((10 / (Math.PI * 2)) * spiral * Math.Cos(spiral));
                p3[i].Y += Convert.ToDouble((10 / (Math.PI * 2)) * spiral * Math.Sin(spiral));
                p3[i].Z--;
            }
            To2D(p3);

            if (OutOfPictureBox())
            {
                animation = false;
                timerSpiral.Enabled = false;
                MessageBox.Show("Out of the PictureBox!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                Draw();
                spiral++;
            }
        }

        public bool OutOfPictureBox()
        {
            for (int i = 0; i < p3.Count; i++)
            {
                if (points[i].X < pictureBox1.Location.X - 330 || points[i].Y < pictureBox1.Location.Y - 400
                    || points[i].X > pictureBox1.Location.X - 330 + pictureBox1.Width
                    || points[i].Y > pictureBox1.Location.Y - 400 + pictureBox1.Height)
                    return true;
            }
            return false;

        }

        private void button4_Click(object sender, EventArgs e) //перенос
        {
            if (animation)
            {
                MessageBox.Show("Wait a second, please ;)", "Not yet!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                move_x = Convert.ToInt32(textBox9.Text);
                move_y = Convert.ToInt32(textBox8.Text);
                move_z = Convert.ToInt32(textBox7.Text);
                transfer = 0;
                animation = true;
                timerMove.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Enter only positive values!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timerMove_Tick(object sender, EventArgs e)
        {
            if (transfer == 100)
            {
                animation = false;
                timerMove.Enabled = false;
            }
            double step_x = move_x / 100.0;
            double step_y = move_y / 100.0;
            double step_z = -move_z / 100.0;

            for (int i = 0; i < p3.Count; i++)
            {
                p3[i].X += step_x;
                p3[i].Y += step_y;
                p3[i].Z += step_z;
            }
            To2D(p3);

            if (OutOfPictureBox())
            {
                animation = false;
                timerMove.Enabled = false;
                MessageBox.Show("Out of the PictureBox!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                Draw();
                transfer++;
            }
        }

        private void button3_Click(object sender, EventArgs e) //масштаб
        {
            Scaled3D.Clear();
            scale = 0;
            if (animation)
            {
                MessageBox.Show("Wait a second, please ;)", "Not yet!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                scale_x = Convert.ToDouble(textBox6.Text);
                scale_y = Convert.ToDouble(textBox5.Text);
                scale_z = Convert.ToDouble(textBox4.Text);

                if (scale_x <= 0 || scale_y <= 0 || scale_z <= 0)
                {
                    MessageBox.Show("Enter only positive values!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                for (int i = 0; i < p3.Count; i++)
                {
                    Scaled3D.Add(new Points3D(Convert.ToDouble((p3[i].X * scale_x - p3[i].X) / 100), Convert.ToDouble((p3[i].Y * scale_y - p3[i].Y) / 100), Convert.ToDouble((p3[i].Z * scale_z - p3[i].Z) / 100)));
                }

                animation = true;
                timerScale.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Enter only positive values!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void timerScale_Tick(object sender, EventArgs e)
        {
            if (scale == 100)
            {
                animation = false;
                timerScale.Enabled = false;
            }
            for (int i = 0; i < p3.Count; i++)
            {
                p3[i].X += Scaled3D[i].X;
                p3[i].Y += Scaled3D[i].Y;
                p3[i].Z += Scaled3D[i].Z;
            }

            To2D(p3);
            if (OutOfPictureBox())
            {
                animation = false;
                timerScale.Enabled = false;
                MessageBox.Show("Out of the PictureBox!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Draw();
                scale++;
            }
        }

        private void button5_Click(object sender, EventArgs e) //отражение
        {
            for (int i = 0; i < p3.Count; i++)
            {
                if (radioButton1.Checked)
                    p3[i].X = -p3[i].X;
                if (radioButton3.Checked)
                    p3[i].Y = -p3[i].Y;
                if (radioButton2.Checked)
                    p3[i].Z = -p3[i].Z;
            }

            To2D(p3);
            if (OutOfPictureBox())
            {
                MessageBox.Show("Out of the PictureBox!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Draw();
        }

        private void button2_Click(object sender, EventArgs e) //поворот
        {
            angle_rotate = 0;
            if (animation)
            {
                MessageBox.Show("Wait a second, please ;)", "Not yet!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                angle_x = Convert.ToInt32(textBox1.Text);
                angle_y = Convert.ToInt32(textBox2.Text);
                angle_z = Convert.ToInt32(textBox3.Text);
                if (angle_x < 0 || angle_y < 0 || angle_z < 0)
                    throw new FormatException();

                animation = true;
                timerRotate.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Enter only positive angles!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timerRotate_Tick(object sender, EventArgs e)
        {
            if (angle_rotate > angle_x && angle_rotate > angle_y && angle_rotate > angle_z)
            {
                animation = false;
                timerRotate.Enabled = false;
            }
            double x=0, y=0, z=0;
            if (angle_rotate < angle_x)
            {
                for (int i = 0; i < p3.Count; i++)
                {
                    y = p3[i].Y * Math.Cos(Math.PI / 180) - p3[i].Z * Math.Sin(Math.PI / 180);
                    z = p3[i].Y * Math.Sin(Math.PI / 180) + p3[i].Z * Math.Cos(Math.PI / 180);
                    p3[i].Z = z;
                    p3[i].Y = y;
                }
            }

            if (angle_rotate < angle_y)
            {
                for (int i = 0; i < p3.Count; i++)
                {
                    x = p3[i].X * Math.Cos(Math.PI / 180) - p3[i].Z * Math.Sin(Math.PI / 180);
                    z = p3[i].X * Math.Sin(Math.PI / 180) + p3[i].Z * Math.Cos(Math.PI / 180);
                    p3[i].X = x;
                    p3[i].Z = z;
                }
            }

            if (angle_rotate < angle_z)
            {
                for (int i = 0; i < p3.Count; i++)
                {
                    x = p3[i].X * Math.Cos(Math.PI / 180) - p3[i].Y * Math.Sin(Math.PI / 180);
                    y = p3[i].X * Math.Sin(Math.PI / 180) + p3[i].Y * Math.Cos(Math.PI / 180);
                    p3[i].Y = y;
                    p3[i].X = x;
                }
            }

            To2D(p3);
            if (OutOfPictureBox())
            {
                animation = false;
                timerRotate.Enabled = false;
                MessageBox.Show("Out of the PictureBox!","Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Draw();
                angle_rotate++;
            }
        }
    }
}
