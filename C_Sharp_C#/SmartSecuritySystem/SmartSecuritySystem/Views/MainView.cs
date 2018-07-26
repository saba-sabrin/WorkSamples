using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Input;
using System.IO;
using System.Text;
using Microsoft.SPOT.IO;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using SmartSecSystem;

namespace SmartSecSystem.Views
{
    public class MainView : SuperView
    {
        #region Common UI components

        private static string[] userDetails;
        private static string[] areaDetails;
        private static string password = string.Empty;
        private string areaID = "1";
        public static bool IsLoggedIn = false;

        public GT.Timer tmrObjectDetect;

        private Canvas loginMenu;
        private Canvas userMenu;
        private GT.Timer tmrUpdateTime;
        Canvas numberCanvas;
        Image num0;
        Image num1;
        Image num2;
        Image num3;
        Image num4;
        Image num5;
        Image num6;
        Image num7;
        Image num8;
        Image num9;

        Image btnAccept;
        Image btnReload;

        Text txtTime;
        Text txtUserName;

        private static Bitmap num_0 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_zero), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_one), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_2 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_two), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_3 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_three), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_4 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_four), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_5 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_five), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_6 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_six), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_7 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_seven), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_8 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_eight), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap num_9 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.number_nine), Bitmap.BitmapImageType.Jpeg);

        private static Bitmap btnAcc = new Bitmap(Resources.GetBytes(Resources.BinaryResources.accept), Bitmap.BitmapImageType.Jpeg);
        private static Bitmap btnRel = new Bitmap(Resources.GetBytes(Resources.BinaryResources.reload), Bitmap.BitmapImageType.Jpeg);

        private static Bitmap bmpRight = new Bitmap(Resources.GetBytes(Resources.BinaryResources.right), Bitmap.BitmapImageType.Jpeg);

        #endregion

        // Main entry point for this View
        public MainView()
        {
            SetTitle("Smart Security System", Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Black, GT.Color.LightGray, GT.Color.Green);

            txtTime = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.NinaB),
                Width = 310,
                ForeColor = GT.Color.Black,
                TextAlignment = TextAlignment.Right,
                TextContent = DateTime.Now.ToString("HH:mm:ss")
            };
            this.AddElement(txtTime, 20, 0);

            // Create Login Menu
            loginMenu = new Canvas() { Height = 240, Width = 320 };

            // Set menu items
            Bitmap bm = new Bitmap(Resources.GetBytes(Resources.BinaryResources.login), Bitmap.BitmapImageType.Jpeg);
            this.CreateMenuItem(loginMenu, bm, new TouchEventHandler(Login_TouchDown), 22, "", 0);

            // Add menu elements
            this.AddElement(loginMenu, 50, 0);

            // Create Login number panel
            this.CreateLoginPanel();

            // Get area details
            areaDetails = DataHandler.GetAreaDetails(areaID);

            // Configure timer for handling Idle Screen timeout
            tmrUpdateTime = new GT.Timer(TimeSpan.FromTicks(60 * TimeSpan.TicksPerSecond));  // 60 seconds
            tmrUpdateTime.Tick += tmrUpdateTime_Tick;
            tmrUpdateTime.Start();

            // Configure time for Distance sensor
            tmrObjectDetect = new GT.Timer(TimeSpan.FromTicks(5 * TimeSpan.TicksPerSecond));
            tmrObjectDetect.Tick += tmrObjectDetect_Tick;

            // Initialize Distance Sensor
            //CheckOccupancyTime(txtTime.TextContent);
            CheckOccupancyTime("16:00");
        }

        #region Functions

        private void CreateUserPanel()
        {
            // Create User Menu
            userMenu = new Canvas() { Height = 240, Width = 320, Visibility = Visibility.Collapsed };

            // Set back option
            Bitmap bm2 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.back), Bitmap.BitmapImageType.Jpeg);
            this.CreateMenuItem(userMenu, bm2, new TouchEventHandler(Back_TouchDown), 22, "", 0);

            txtUserName = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.NinaB),
                Width = 310,
                ForeColor = GT.Color.DarkGray,
                TextAlignment = TextAlignment.Center,
                TextContent = "Welcome " + userDetails[0] + " !!"
            };
            userMenu.Children.Add(txtUserName, 100, 0);

            // Add menu to elements
            this.AddElement(userMenu, 50, 0);
        }

        private void CreateLoginPanel()
        {
            numberCanvas = new Canvas() { Height = 240, Width = 320, Visibility = Microsoft.SPOT.Presentation.Visibility.Visible };

            StackPanel panel1 = new StackPanel() { Orientation = Orientation.Horizontal };
            StackPanel panel2 = new StackPanel() { Orientation = Orientation.Horizontal };
            StackPanel panel3 = new StackPanel() { Orientation = Orientation.Horizontal };

            num0 = new Image(num_0);
            num0.SetMargin(5, 2, 5, 2);
            num0.TouchDown += new TouchEventHandler(Number_TouchDown);

            num1 = new Image(num_1);
            num1.SetMargin(5, 2, 5, 2);
            num1.TouchDown += new TouchEventHandler(Number_TouchDown);

            num2 = new Image(num_2);
            num2.SetMargin(5, 2, 5, 2);
            num2.TouchDown += new TouchEventHandler(Number_TouchDown);

            num3 = new Image(num_3);
            num3.SetMargin(5, 2, 5, 2);
            num3.TouchDown += new TouchEventHandler(Number_TouchDown);

            num4 = new Image(num_4);
            num4.SetMargin(5, 2, 5, 2);
            num4.TouchDown += new TouchEventHandler(Number_TouchDown);

            num5 = new Image(num_5);
            num5.SetMargin(5, 2, 5, 2);
            num5.TouchDown += new TouchEventHandler(Number_TouchDown);

            num6 = new Image(num_6);
            num6.SetMargin(5, 2, 5, 2);
            num6.TouchDown += new TouchEventHandler(Number_TouchDown);

            num7 = new Image(num_7);
            num7.SetMargin(5, 2, 5, 2);
            num7.TouchDown += new TouchEventHandler(Number_TouchDown);

            num8 = new Image(num_8);
            num8.SetMargin(5, 2, 5, 2);
            num8.TouchDown += new TouchEventHandler(Number_TouchDown);

            num9 = new Image(num_9);
            num9.SetMargin(5, 2, 5, 2);
            num9.TouchDown += new TouchEventHandler(Number_TouchDown);

            btnAccept = new Image(btnAcc);
            btnAccept.SetMargin(5, 5, 5, 10);
            btnAccept.TouchDown += new TouchEventHandler(Accept_TouchDown);

            btnReload = new Image(btnRel);
            btnReload.SetMargin(5, 5, 5, 10);
            btnReload.TouchDown += new TouchEventHandler(Reload_TouchDown);

            panel1.Children.Add(num0);
            panel1.Children.Add(num1);
            panel1.Children.Add(num2);
            panel1.Children.Add(num3);

            panel2.Children.Add(num4);
            panel2.Children.Add(num5);
            panel2.Children.Add(num6);
            panel2.Children.Add(num7);

            panel3.Children.Add(num8);
            panel3.Children.Add(num9);
            panel3.Children.Add(btnAccept);
            panel3.Children.Add(btnReload);

            numberCanvas.Children.Add(panel1, 22, 5);
            numberCanvas.Children.Add(panel2, 82, 5);
            numberCanvas.Children.Add(panel3, 140, 5);

            numberCanvas.Visibility = Visibility.Collapsed;
            this.AddElement(numberCanvas, 23, 0);
        }

        private static bool UserAuthentication()
        {
            IsLoggedIn = false;

            userDetails = DataHandler.GetUserDetails(password);

            if(userDetails != null)
            {
                IsLoggedIn = true;
            }

            return IsLoggedIn;
        }

        private void CreateMenuItem(Canvas menu, Bitmap bitmap, TouchEventHandler handler,
            int iconMarginLeft, string labelContent, int labelMarginLeft)
        {
            var image = new Image(bitmap);
            image.TouchDown += handler;
            menu.Children.Add(image, 0, iconMarginLeft);

            var label = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.NinaB),
                Width = 120,
                TextAlignment = TextAlignment.Center,
                TextWrap = true,
                TextContent = labelContent
            };
            label.TouchDown += handler;
            menu.Children.Add(label, 54, labelMarginLeft);
        }

        public void UpdateTime()
        {
            txtTime.TextContent = DateTime.Now.ToString("HH:mm:ss");
        }

        public void PlayAlarm()
        {
            Tunes.MusicNote[] notes = new Tunes.MusicNote[5];

            Tunes.Melody melody = new Tunes.Melody();

            int start = 120;
            for (int i = 0; i < notes.Length; i++)
            {
                Tunes.Tone tone = new Tunes.Tone(start);
                notes[i] = new Tunes.MusicNote(tone, 100);
                start = start + 10;
            }

            melody.Add(notes);
            Program.alarm.Play(melody);
        }

        #endregion

        #region "Event Handlers"

        enum TimeType
        {
            Morning = 1,   
            Day_Night = 2, 
            MidNight = 3   
        }

        TimeType CheckTimeType(string[] timeValues)
        {
            TimeType type = new TimeType();

            int temp = Convert.ToInt32(timeValues[0]);

            if ((Convert.ToInt32(timeValues[0]) >= 6) && (Convert.ToInt32(timeValues[0]) < 12)) // 06:00 to 11:59
            {
                type = TimeType.Morning;
            }
            else if ((Convert.ToInt32(timeValues[0]) >= 12) && (Convert.ToInt32(timeValues[0]) <= 23))  // 12:00 to 23:59
            {
                type = TimeType.Day_Night;
            }
            else  // 00:00 to 05:59
            {
                type = TimeType.MidNight;
            }

            return type;
        }

        void CheckOccupancyTime(string currentTime)
        {
            string[] currentTimeValues;
            string[] timeStart;
            string[] timeEnd;

            // Sample Time format : (HH:mm:ss)
            // Sample Range : 20:00, 06:00
            //currentTime = "20:01";
            currentTimeValues = currentTime.Split(':');
            TimeType typeCurr = CheckTimeType(currentTimeValues);

            timeStart = areaDetails[1].Split(':');
            TimeType typeTimeSt = CheckTimeType(timeStart);

            timeEnd = areaDetails[2].Split(':');
            TimeType typeTimeEnd = CheckTimeType(timeEnd);
            
            if(typeCurr == TimeType.Morning)
            {
                if(typeTimeSt == TimeType.Morning)
                {
                    if (Convert.ToInt32(currentTimeValues[0]) >= Convert.ToInt32(timeStart[0]))  // Check time
                    {
                        if (!tmrObjectDetect.IsRunning)
                        {
                            // Start timer for Motion Detection
                            tmrObjectDetect.Start();
                        }
                    }
                }
                else
                {
                    if (tmrObjectDetect.IsRunning)
                    {
                        if (Convert.ToInt32(currentTimeValues[0]) <= Convert.ToInt32(timeStart[0]))  // Check time
                        {
                            tmrObjectDetect.Restart();
                        }
                        else
                        {
                            tmrObjectDetect.Stop();
                        }
                    }
                }
            }
            else if (typeCurr == TimeType.Day_Night)
            {
                if (typeTimeSt == TimeType.Day_Night)
                {
                    if (Convert.ToInt32(currentTimeValues[0]) >= Convert.ToInt32(timeStart[0]))  // Check time
                    {
                        if (!tmrObjectDetect.IsRunning)
                        {
                            // Start timer for Motion Detection
                            tmrObjectDetect.Start();
                        }
                    }
                }
                else
                {
                    if (tmrObjectDetect.IsRunning)
                    {
                        if (Convert.ToInt32(currentTimeValues[0]) <= Convert.ToInt32(timeStart[0]))  // Check time
                        {
                            tmrObjectDetect.Restart();
                        }
                        else
                        {
                            tmrObjectDetect.Stop();
                        }
                    }
                }
            }
            else
            {
                if (typeTimeSt == TimeType.MidNight)
                {
                    if (Convert.ToInt32(currentTimeValues[0]) >= Convert.ToInt32(timeStart[0]))  // Check time
                    {
                        if (!tmrObjectDetect.IsRunning)
                        {
                            // Start timer for Motion Detection
                            tmrObjectDetect.Start();
                        }
                    }
                }
                else
                {
                    if (tmrObjectDetect.IsRunning)
                    {
                        if (Convert.ToInt32(currentTimeValues[0]) <= Convert.ToInt32(timeStart[0]))  // Check time
                        {
                            tmrObjectDetect.Restart();
                        }
                        else
                        {
                            tmrObjectDetect.Stop();
                        }
                    }
                }
            }
        }

        // Timer for detecting object through distance sensor
        void tmrObjectDetect_Tick(GT.Timer timer)
        {
            // Indicating values of motion through led light
            Program.ledLight.TurnAllLedsOff();

            // Getting the distance values from the sensors
            double distanceValue1 = Program.distSensor1.GetDistance(10);
            double distanceValue2 = Program.distSensor2.GetDistance(10);

            if ((distanceValue1 > 13.0 && distanceValue1 < 21.0) || (distanceValue2 > 13.0 && distanceValue2 < 21.0))
            {
                Program.ledLight.TurnAllLedsOn();
                this.PlayAlarm();
            }

            Debug.Print("object 1 distance : " + distanceValue1.ToString() + ", object 2 distance : " + distanceValue2.ToString());
        }

        // Timer for updating time
        void tmrUpdateTime_Tick(GT.Timer timer)
        {
            // Check if current time is not empty
            if (Program.currentTime != "")
            {
                txtTime.TextContent = Program.currentTime;
                Debug.Print(Program.currentTime);
                //CheckOccupancyTime(Program.currentTime);
            }

            CheckOccupancyTime("16:05");
        }

        private void Login_TouchDown(object sender, TouchEventArgs e)
        {
            tmrObjectDetect.Stop();
            tmrUpdateTime.Stop();
            loginMenu.Visibility = Visibility.Hidden;
            numberCanvas.Visibility = Visibility.Visible;
            Debug.Print("Please Log In...");
        }

        private void Back_TouchDown(object sender, TouchEventArgs e)
        {
            Program.ledLight.TurnAllLedsOff();
            userMenu.Visibility = Visibility.Hidden;
            numberCanvas.Visibility = Visibility.Hidden;
            IsLoggedIn = false;
            password = string.Empty;

            MainView main = new MainView();
            loginMenu.Visibility = Visibility.Visible;
            Debug.Print("Logged out!");
        }

        // Number pressed should be checked & more accuracy !!!
        void Number_TouchDown(object sender, TouchEventArgs e)
        {
            Program.ledLight.TurnAllLedsOff();

            Image image = sender as Image;

            if (image == num0)
            {
                password = password + "0";
                Program.ledLight.TurnLedOn(0);
                Debug.Print("Number 0");
                Debug.Print(password);
            }
            else if (image == num1)
            {
                password = password + "1";
                Program.ledLight.TurnLedOn(1);
                Debug.Print("Number 1");
                Debug.Print(password);
            }
            else if (image == num2)
            {
                password = password + "2";
                Program.ledLight.TurnLedOn(2);

                Debug.Print("Number 2");
                Debug.Print(password);
            }
            else if (image == num3)
            {
                password = password + "3";
                Program.ledLight.TurnLedOn(3);

                Debug.Print("Number 3");
                Debug.Print(password);
            }
            else if (image == num4)
            {
                password = password + "4";
                Debug.Print("Number 4");
                Debug.Print(password);
            }
            else if (image == num5)
            {
                password = password + "5";
                Debug.Print("Number 5");
                Debug.Print(password);
            }
            else if (image == num6)
            {
                password = password + "6";
                Debug.Print("Number 6");
                Debug.Print(password);
            }
            else if (image == num7)
            {
                password = password + "7";
                Debug.Print("Number 7");
                Debug.Print(password);
            }
            else if (image == num8)
            {
                password = password + "8";
                Debug.Print("Number 8");
                Debug.Print(password);
            }
            else if (image == num9)
            {
                password = password + "9";
                Debug.Print("Number 9");
                Debug.Print(password);
            }
            else
            {
                Debug.Print("No Match !!");
            }
        }

        // Event for checking user authentication
        void Accept_TouchDown(object sender, TouchEventArgs e)
        {
            bool IsLoggedIn = UserAuthentication();

            if (IsLoggedIn)
            {
                this.PlayAlarm();
            }
                
            numberCanvas.Visibility = Visibility.Hidden;
            loginMenu.Visibility = Visibility.Hidden;

            // Creation of User view panel
            this.CreateUserPanel();
            userMenu.Visibility = Visibility.Visible;
        }

        void Reload_TouchDown(object sender, TouchEventArgs e)
        {
            password = string.Empty;
            Program.ledLight.TurnAllLedsOff();
            Debug.Print("Password cleared!");
        }

        #endregion
    }
}
