using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TronDigitalClock
{
    public partial class TronDigitalClockForm : Form
    {
        private const int PIXEL_SIZE = 2;
        private const int FONT_ROWS = 7;
        private const int FONT_COLS = 5;
        private const int DIGIT_PADDING = 1;
        private const int DIGIT_SPACING = 1;

        private int millisecondCounter;
        private int millisecondsD1Counter;
        private int millisecondsD2Counter;

        private int millisecondsD1;
        private int millisecondsD2;

        private int seconds;
        private int minutes;
        private int hours;
        private int days;

        private bool hasSeconds;
        private bool hasMinutes;
        private bool hasHours;
        private bool hasDays;

        private Point mouseLocation;
        private Random rand = new Random();
        private bool previewMode = false;

        Point startingPt;
        Size pixelSize;

        SolidBrush brush;
        Graphics formGraphics = null;


        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        public TronDigitalClockForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            pixelSize = new Size(PIXEL_SIZE, PIXEL_SIZE);

            DoubleBuffered = true;
        }

        public TronDigitalClockForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);


            previewMode = true;

        }

        private void TronDigitalClockForm_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            TopMost = true;

            drawDigitTimer.Interval = 1;
            drawDigitTimer.Tick += new EventHandler(drawDigitTimer_Tick);
            drawDigitTimer.Start();

        }

        private void drawDigitTimer_Tick(object sender, System.EventArgs e)
        {
            calculateDigits();
            Invalidate();
        }

        void calculateDigits()
        {

            // D1 and D2 are for each of the milliseconds places
            millisecondsD1Counter++;
            millisecondsD2Counter++;
            millisecondCounter++;


            // increment D1 by 1 every frame
            millisecondsD1++;

            if (millisecondsD1 == 10)
            {
                millisecondsD1 = 0;
            }

            // increment D2 by 1 every 5 frames, to simulate fast ticking of milliseconds
            if (millisecondsD2Counter == 5)
            {
                millisecondsD2Counter = 0;
                millisecondsD2++;

                if (millisecondsD2 == 10)
                {
                    millisecondsD2 = 0;
                }
            }

            // increment seconds every 60 frames
            if (millisecondCounter == 60)
            {
                millisecondCounter = 0;

                seconds++;
                hasSeconds = true;


                // now that the seconds are good, the rest of the calculations line up
                if (seconds >= 60)
                {
                    seconds = 0;

                    minutes++;
                    hasMinutes = true;

                    if (minutes >= 60)
                    {
                        minutes = 0;

                        hours++;
                        hasHours = true;

                        if (hours >= 60)
                        {
                            hours = 0;

                            days++;
                            hasDays = true;
                        }
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            brush = new SolidBrush(Color.FromArgb(0x9D, 0xF6, 0xD8));
            formGraphics = e.Graphics;
            
            updateDigitUI();
        }

        private void updateDigitUI()
        {

            // set milliseconds
            string txtStr = string.Format("{0}{1}", millisecondsD2, millisecondsD1);

            if (hasSeconds)
            {
                // set seconds
                txtStr = string.Format("{0:00}:{1}", seconds, txtStr);

                if (hasMinutes)
                {
                    // set minutes
                    txtStr = string.Format("{0:00}:{1}", minutes, txtStr);

                    if (hasHours)
                    {
                        // set hours
                        txtStr = string.Format("{0:00}:{1}", hours, txtStr);

                        if (hasDays)
                        {
                            // set Days
                            txtStr = string.Format("{0:00}:{1}", days, txtStr);
                        }

                    }
                }
            }

            startingPt = new Point(((Screen.PrimaryScreen.Bounds.Width - (txtStr.Length * (((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING))) / 2), ((Screen.PrimaryScreen.Bounds.Height+ (FONT_ROWS * (PIXEL_SIZE + DIGIT_SPACING))) / 2));
            
            drawDigits(txtStr);
        }

        private void drawDigits(string txtStr)
        {

            for (int i = 0; i < txtStr.Length; i++)
            {

                char c = txtStr[i];

                switch (c)
                {
                    case '0':
                        draw0();
                        break;
                    case '1':
                        draw1();
                        break;
                    case '2':
                        draw2();
                        break;
                    case '3':
                        draw3();
                        break;
                    case '4':
                        draw4();
                        break;
                    case '5':
                        draw5();
                        break;
                    case '6':
                        draw6();
                        break;
                    case '7':
                        draw7();
                        break;
                    case '8':
                        draw8();
                        break;
                    case '9':
                        draw9();
                        break;
                    case ':':
                        drawColon();
                        break;
                }

            }

        }

        void drawColon()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,0,0,0},
                { 0,1,1,0,0},
                { 0,1,1,0,0},
                { 0,0,0,0,0},
                { 0,1,1,0,0},
                { 0,1,1,0,0},
                { 0,0,0,0,0}
            };

            drawNumber(mappings);
        }

        void draw0()
        {
            int[,] mappings = new int[,]
             {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,1,1},
                { 1,0,1,0,1},
                { 1,1,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
             };

            drawNumber(mappings);
        }

        void draw1()
        {

            int[,] mappings = new int[,]
            {
                { 0,0,1,0,0},
                { 0,1,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,1,1,1,0}
            };

            drawNumber(mappings);
        }

        void draw2()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 0,0,0,0,1},
                { 0,0,0,1,0},
                { 0,0,1,0,0},
                { 0,1,0,0,0},
                { 1,1,1,1,1}
            };

            drawNumber(mappings);
        }

        void draw3()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 0,0,0,1,0},
                { 0,0,1,0,0},
                { 0,0,0,1,0},
                { 0,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawNumber(mappings);
        }

        void draw4()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,0,1,0},
                { 0,0,1,1,0},
                { 0,1,0,1,0},
                { 1,0,0,1,0},
                { 1,1,1,1,1},
                { 0,0,0,1,0},
                { 0,0,0,1,0}
            };

            drawNumber(mappings);
        }

        void draw5()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 1,0,0,0,0},
                { 1,1,1,1,0},
                { 0,0,0,0,1},
                { 0,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawNumber(mappings);
        }

        void draw6()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,1,1,0},
                { 0,1,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawNumber(mappings);
        }

        void draw7()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 0,0,0,0,1},
                { 0,0,0,1,0},
                { 0,0,1,0,0},
                { 0,1,0,0,0},
                { 0,1,0,0,0},
                { 0,1,0,0,0}
            };

            drawNumber(mappings);
        }

        void draw8()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawNumber(mappings);
        }

        void draw9()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,1},
                { 0,0,0,0,1},
                { 0,0,0,1,0},
                { 0,1,1,0,0}
            };

            drawNumber(mappings);
        }

        void drawNumber(int[,] mappings)
        {

            Point origin = startingPt;

            for (int i = 0; i < FONT_ROWS; i++)
            {
                for (int j = 0; j < FONT_COLS; j++)
                {

                    if (mappings[i, j] == 1)
                    {
                        formGraphics.FillRectangle(brush, new Rectangle(origin, pixelSize));
                    }

                    // move right one pixel
                    origin.X += (PIXEL_SIZE + DIGIT_SPACING);
                }

                // go back to beginning of line
                origin.X = startingPt.X;

                // move down one pixel
                origin.Y += (PIXEL_SIZE + DIGIT_SPACING);
            }

            // set up x position (magic number) for the next digit
            startingPt.X += ((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING;

        }


        private void TronDigitalClockForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                // Terminate if mouse is moved a significant distance
                if (Math.Abs(mouseLocation.X - e.X) > 5 || Math.Abs(mouseLocation.Y - e.Y) > 5)
                {
                    if (!previewMode)
                        Application.Exit();
                }
            }

            // Update current mouse location
            mouseLocation = e.Location;

        }

        private void TronDigitalClockForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void TronDigitalClockForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }
        
    }
}
