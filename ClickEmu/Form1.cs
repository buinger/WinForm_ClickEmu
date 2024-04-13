using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickEmu
{


    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private Thread clickThread;
        private bool keepClicking = false;

        public Form1()
        {
            this.TopMost = true;
            InitializeComponent();
            // 注册全局热键 Ctrl + Alt + S
            RegisterHotKey(this.Handle, 1, 0x0002 | 0x0001, (uint)Keys.S);

            RegisterHotKey(this.Handle, 2, 0x0002 | 0x0001, (uint)Keys.M);

            RegisterHotKey(this.Handle, 3, 0x0002 | 0x0001, (uint)Keys.A);

            this.FormClosing += MainForm_FormClosing;
        }




        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case 1:
                        StopClicking();
                        break;
                    case 2:
                        ShowMousePos();
                        break;
                    case 3:
                        
                        StartClicking();
                        break;
                }
                // 按下了全局热键
                
            }

        }


        void ShowMousePos()
        {
            var position = Cursor.Position;
            label1.Text = position.X.ToString();
            label2.Text = position.Y.ToString();
            //MessageBox.Show($"X: {position.X}, Y: {position.Y}", "Mouse Position");
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 注销热键
            UnregisterHotKey(this.Handle, 0);
            // 注销全局热键
            UnregisterHotKey(this.Handle, 0);
        }
        private void StartClicking()
        {
            button1.Enabled = false;
            keepClicking = true;
            clickThread = new Thread(() =>
            {
                try
                {
                    int x, y;
                    bool xCheck = int.TryParse(label1.Text, out x);
                    bool yCheck = int.TryParse(label2.Text, out y);
                    int time;
                    bool timeCheck = int.TryParse(textBox1.Text, out time);
                    if ((xCheck && yCheck&&timeCheck) == false)
                    {
                        keepClicking = false;
                        button1.Invoke(new MethodInvoker(delegate
                        {
                            button1.Enabled = true;
                        }));
                    }

                  
                    int interval = time*1000;
                    Console.WriteLine( time);
                    Cursor.Position = new System.Drawing.Point(x, y);
                    while (keepClicking)
                    {
                        if (Cursor.Position.X != x || Cursor.Position.Y != y)
                        {
                            Console.WriteLine(Cursor.Position.X);
                            button1.Invoke(new MethodInvoker(delegate
                            {
                                button1.Enabled = true;
                            }));
                            keepClicking = false;
                            break;
                        }
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        Thread.Sleep(interval);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            });
            clickThread.Start();
        }

        private void StopClicking()
        {
            keepClicking = false;
            clickThread?.Join();
            button1.Enabled = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            StartClicking();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowMousePos();
        }
    }
}
