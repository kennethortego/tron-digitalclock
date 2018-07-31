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
using System.Diagnostics;
using System.Configuration;

namespace TronDigitalClock
{
    public partial class TronDigitalClockForm : Form
    {
        public static bool isarcde = false;

        private const int PIXEL_SIZE = 2;
        private const int PIXEL_SIZE_BOLD = 3;

        private const int FONT_ROWS = 7;
        private const int FONT_COLS = 5;
        private const int DIGIT_PADDING = 1;
        private const int DIGIT_SPACING = 1;

        private const int BUTTON_ROWS = 100;
        private const int BUTTON_COLS = 200;
        private const int BUTTON_SPACING_X = 100;
        private const int BUTTON_SPACING_Y = 150;
        private const int BUTTON_TEXT_OFFSET_X = 26;
        private const int BUTTON_TEXT_OFFSET_Y = 190;

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
        SolidBrush brushTerminal;
        SolidBrush brushBlack;

        Graphics formGraphics = null;

        private bool isExitMenu = false;
        private bool isBold = false;
        private int activeButton = 1;

        #region User32 Functions

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        #endregion

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
            //TopMost = isarcde ? false : true;
            TopMost = true;

            brushTerminal = new SolidBrush(Color.FromArgb(0x9D, 0xF6, 0xD8));
            brushBlack = new SolidBrush(Color.Black);
            
            drawDigitTimer.Interval = 1;
            drawDigitTimer.Tick += new EventHandler(drawDigitTimer_Tick);
            drawDigitTimer.Start();

            label1.Text = "";
            label1.Visible = false;
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

            brush = brushTerminal;
            formGraphics = e.Graphics;

            if (isExitMenu)
            {
                drawExitMenu();
                return;
            }

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

            //startingPt = new Point(((Screen.PrimaryScreen.Bounds.Width - (txtStr.Length * (((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING))) / 2), ((Screen.PrimaryScreen.Bounds.Height + (FONT_ROWS * (PIXEL_SIZE + DIGIT_SPACING))) / 2));
            startingPt = getStartingPoint(txtStr);

            drawText(txtStr);
        }

        private Point getStartingPoint(string txtStr)
        {
            int x = ((Screen.PrimaryScreen.Bounds.Width - (txtStr.Length * (((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING))) / 2);
            int y = ((Screen.PrimaryScreen.Bounds.Height + (FONT_ROWS * (PIXEL_SIZE + DIGIT_SPACING))) / 2);

            return new Point(x, y);
        }

        private Point getStartingPointExitMenu(string txtStr)
        {
            int contentHeight = (FONT_ROWS * PIXEL_SIZE) + BUTTON_SPACING_Y + BUTTON_ROWS;

            int x = ((Screen.PrimaryScreen.Bounds.Width - (txtStr.Length * (((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING))) / 2);
            int y = ((Screen.PrimaryScreen.Bounds.Height - contentHeight) / 2);
            
            return new Point(x, y);
        }


        private void drawExitMenu()
        {
            string txtTitle = " PICK ANY ONE YOU WANT.";
            //string sometxt = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string txtButton1 = " ARCADE ";
            string txtButton2 = "SHUTDOWN";

            startingPt = getStartingPointExitMenu(txtTitle);


            drawText(txtTitle);

            int buttonsWidth = (BUTTON_COLS * 2) + BUTTON_SPACING_X;
            int startX = (Screen.PrimaryScreen.Bounds.Width - buttonsWidth) / 2;


            bool btn1Outline = activeButton == 1 ? false : true;
            bool btn2Outline = activeButton == 2 ? false : true;

            drawButton(startX, startingPt.Y + BUTTON_SPACING_Y, btn1Outline);
            drawButton(startX + BUTTON_SPACING_X + BUTTON_COLS, startingPt.Y + BUTTON_SPACING_Y, btn2Outline);

            isBold = true;
            brush = btn1Outline ? brushTerminal : brushBlack;
            startingPt = new Point(startX + BUTTON_TEXT_OFFSET_X, startingPt.Y + BUTTON_TEXT_OFFSET_Y);

            drawText(txtButton1);

            brush = btn2Outline ? brushTerminal : brushBlack;
            startingPt = new Point(startX + BUTTON_COLS + BUTTON_SPACING_X + BUTTON_TEXT_OFFSET_X, startingPt.Y);
            drawText(txtButton2);

            isBold = false;
        }

        private void drawText(string txtStr)
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
                    case 'A':
                        drawA();
                        break;
                    case 'B':
                        drawB();
                        break;
                    case 'C':
                        drawC();
                        break;
                    case 'D':
                        drawD();
                        break;
                    case 'E':
                        drawE();
                        break;
                    case 'F':
                        drawF();
                        break;
                    case 'G':
                        drawG();
                        break;
                    case 'H':
                        drawH();
                        break;
                    case 'I':
                        drawI();
                        break;
                    case 'J':
                        drawJ();
                        break;
                    case 'K':
                        drawK();
                        break;
                    case 'L':
                        drawL();
                        break;
                    case 'M':
                        drawM();
                        break;
                    case 'N':
                        drawN();
                        break;
                    case 'O':
                        drawO();
                        break;
                    case 'P':
                        drawP();
                        break;
                    case 'Q':
                        drawQ();
                        break;
                    case 'R':
                        drawR();
                        break;
                    case 'S':
                        drawS();
                        break;
                    case 'T':
                        drawT();
                        break;
                    case 'U':
                        drawU();
                        break;
                    case 'V':
                        drawV();
                        break;
                    case 'W':
                        drawW();
                        break;
                    case 'X':
                        drawX();
                        break;
                    case 'Y':
                        drawY();
                        break;
                    case 'Z':
                        drawZ();
                        break;
                    case ' ':
                        drawSpace();
                        break;
                    case '.':
                        drawPeriod();
                        break;
                    case ':':
                        drawColon();
                        break;
                }
            }
        }

        private void drawButton(int startX, int startY, bool isOutline = false)
        {
            Point origin = getStartingPoint("");
            origin.X = startX;
            origin.Y = startY;

            Point startpt = new Point(origin.X, origin.Y);

            Size pixelsize = new Size(1, 1);

            for (int i = 0; i < BUTTON_ROWS; i++)
            {
                for (int j = 0; j < BUTTON_COLS; j++)
                {
                    if (!isOutline || i == 0 || i == (BUTTON_ROWS - 1) || j == 0 || j == (BUTTON_COLS - 1))
                    {
                        formGraphics.FillRectangle(brush, new Rectangle(origin, pixelsize));
                    }

                    // move right one pixel
                    origin.X++;
                }

                // go back to beginning of line
                origin.X = startpt.X;

                // move down one pixel
                origin.Y++;
            }

            // set up x position (magic number) for the next digit
            startingPt.X = startpt.X;
        }

        #region draw characters

        void drawSpace()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawPeriod()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 0,0,0,0,0},
                { 1,1,0,0,0},
                { 1,1,0,0,0}
            };

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
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

            drawCharOnScreen(mappings);
        }

        void drawA()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,1,0,0},
                { 0,1,0,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawB()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawC()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawD()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawE()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawF()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0}

            };

            drawCharOnScreen(mappings);
        }
        void drawG()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,1,1,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawH()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawI()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawJ()
        {
            int[,] mappings = new int[,]
            {
                { 0,0,1,1,1},
                { 0,0,0,1,0},
                { 0,0,0,1,0},
                { 0,0,0,1,0},
                { 0,0,0,1,0},
                { 1,0,0,1,0},
                { 0,1,1,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawK()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,1,0},
                { 1,0,1,0,0},
                { 1,1,0,0,0},
                { 1,0,1,0,0},
                { 1,0,0,1,0},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawL()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawM()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,1,0,1,1},
                { 1,0,1,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawN()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,0,0,1},
                { 1,0,1,0,1},
                { 1,0,0,1,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawO()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawP()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 1,0,0,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawQ()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,1,0,1},
                { 1,0,0,1,0},
                { 0,1,1,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawR()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,1,1,1,0},
                { 1,0,1,0,0},
                { 1,0,0,1,0},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawS()
        {
            int[,] mappings = new int[,]
            {
                { 0,1,1,1,1},
                { 1,0,0,0,0},
                { 1,0,0,0,0},
                { 0,1,1,1,0},
                { 0,0,0,0,1},
                { 0,0,0,0,1},
                { 1,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawT()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawU()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,1,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawV()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,0,1,0},
                { 0,0,1,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawW()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,1,0,1},
                { 1,0,1,0,1},
                { 1,0,1,0,1},
                { 0,1,0,1,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawX()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,0,1,0},
                { 0,0,1,0,0},
                { 0,1,0,1,0},
                { 1,0,0,0,1},
                { 1,0,0,0,1}
            };

            drawCharOnScreen(mappings);
        }
        void drawY()
        {
            int[,] mappings = new int[,]
            {
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 1,0,0,0,1},
                { 0,1,0,1,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0},
                { 0,0,1,0,0}
            };

            drawCharOnScreen(mappings);
        }
        void drawZ()
        {
            int[,] mappings = new int[,]
            {
                { 1,1,1,1,1},
                { 0,0,0,0,1},
                { 0,0,0,1,0},
                { 0,0,1,0,0},
                { 0,1,0,0,0},
                { 1,0,0,0,0},
                { 1,1,1,1,1}
            };

            drawCharOnScreen(mappings);
        }

        #endregion

        void drawCharOnScreen(int[,] mappings)
        {
            if (isBold)
            {
                drawCharOnScreenBold(mappings);
                return;
            }

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
        void drawCharOnScreenBold(int[,] mappings)
        {
            Point origin = startingPt;
            Size pixelsize = new Size(PIXEL_SIZE_BOLD, PIXEL_SIZE_BOLD);
            

            for (int i = 0; i < FONT_ROWS; i++)
            {
                for (int j = 0; j < FONT_COLS; j++)
                {

                    if (mappings[i, j] == 1)
                    {
                        formGraphics.FillRectangle(brush, new Rectangle(origin, pixelsize));
                    }

                    // move right one pixel
                    origin.X += PIXEL_SIZE_BOLD;
                }

                // go back to beginning of line
                origin.X = startingPt.X;

                // move down one pixel
                origin.Y += PIXEL_SIZE_BOLD;
            }

            // set up x position (magic number) for the next digit
            startingPt.X += ((FONT_COLS + 1) * pixelsize.Width) + DIGIT_PADDING;

        }


        private void TronDigitalClockForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isarcde)
                return;

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
            if (isarcde)
                return;

            if (!previewMode)
                Application.Exit();
        }
        private void TronDigitalClockForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isarcde)
            {
                this.label1.Text = ((int)e.KeyChar).ToString();

                if (isExitMenu)
                {
                    if ((int)e.KeyChar == 27) // esc key                
                    {
                        isExitMenu = false;
                        activeButton = 1;
                        drawDigitTimer.Start();
                    }
                    else if ((int)e.KeyChar == 49) // 1 key
                    {
                        string program = string.Empty;
                        string param = string.Empty;
                        
                        if (activeButton == 1)
                        {
                            program = Convert.ToString(ConfigurationManager.AppSettings["ExitProgram"]);
                        }
                        else if (activeButton == 2)
                        {
                            program = Convert.ToString(ConfigurationManager.AppSettings["ShutdownProgram"]);
                            param = Convert.ToString(ConfigurationManager.AppSettings["ShutdownProgramParams"]);
                        }

                        if (!string.IsNullOrWhiteSpace(program))
                        {
                            ProcessStartInfo psi = new ProcessStartInfo(program, param);
                            psi.CreateNoWindow = true;
                            psi.UseShellExecute = false;

                            Process p = Process.Start(psi);
                            p.Close();
                        }

                        Application.Exit();

                    }
                }
                else
                {
                    if ((int)e.KeyChar == 27) // esc key                
                    {
                        drawDigitTimer.Stop();

                        isExitMenu = true;
                        Invalidate();
                        //return;
                    }
                }


                return;
            }


            if (!previewMode)
                Application.Exit();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (isarcde)
            {
                if (isExitMenu)
                {
                    if (keyData == Keys.Left)
                    {
                        activeButton = 1;
                        Invalidate();
                    }
                    else if (keyData == Keys.Right)
                    {
                        activeButton = 2;
                        Invalidate();
                    }
                }
            }
            else
            {
                if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
                {
                    if (!previewMode)
                        Application.Exit();

                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
