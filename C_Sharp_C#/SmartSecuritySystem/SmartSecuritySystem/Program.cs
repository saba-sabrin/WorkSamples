/* Copyright 2016 Saba Sabrin, saba.sabrin@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections;
using System.Net;
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
using Microsoft.SPOT.Net.NetworkInformation;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Networking;

namespace SmartSecSystem
{
    public partial class Program
    {
        #region "Common variables"

        private string user_file_path = "user_data.csv";
        private string area_file_path = "area_data.csv";
        private Window window;
        private GT.Timer tmrIdleTimer;
        private TimeManager timeMgr;

        // Static Global variables
        public static string currentTime = "";
        public static LEDStrip ledLight;
        public static Tunes alarm;
        public static DistanceUS3 distSensor1;
        public static DistanceUS3 distSensor2;

        #endregion

        #region "Functions"

        // Main entry function
        // This method executes when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Welcome to Smart Security System!");

            // Initialize Display, network, time, data
            this.LoadStartupView();
            this.StartNetwork();
            this.LoadData();

            // Global output modules for led light, sound and motion
            ledLight = ledStrip;
            alarm = tunes;
            distSensor1 = distanceUS3;
            distSensor2 = distanceUS32;

            //this.LoadLoginView();  // temporary, should be deleted.
        }

        private void LoadStartupView()
        {
            window = displayTE35.WPFWindow;
            window.Background = new SolidColorBrush(GT.Color.White);
            window.Child = ViewManager.StartupView;
        }

        private void LoadLoginView()
        {
            // Start main view
            window.Child = ViewManager.MainView;
            ViewManager.MainView.TouchDown += MainView_TouchDown;
            ViewManager.IdleTimeView.TouchDown += IdleTimeView_TouchDown;

            // Configure timer for handling Idle Screen timeout (1.5 minute timeout set)
            tmrIdleTimer = new GT.Timer(TimeSpan.FromTicks(120 * TimeSpan.TicksPerSecond));
            tmrIdleTimer.Tick += tmrIdleTimer_Tick;
            tmrIdleTimer.Start();
        }
        
        // Function for starting network
        private void StartNetwork()
        {
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            ethernetJ11D.DebugPrintEnabled = true;
            ethernetJ11D.NetworkInterface.Open();

            if(ethernetJ11D.NetworkInterface.IsDhcpEnabled)  // DHCP enabled network configuration
            {
                Debug.Print("DHCP enabled!");
                ethernetJ11D.NetworkInterface.EnableDhcp();
                ethernetJ11D.UseDHCP();
            }
            else if (ethernetJ11D.NetworkInterface.IsDynamicDnsEnabled)  
            {
                Debug.Print("Dynamic DNS enabled!");
                ethernetJ11D.NetworkInterface.EnableDynamicDns();
            }
            else
            {
                // Static configuration of IP address
                ethernetJ11D.NetworkInterface.EnableStaticIP("130.83.155.213", "255.255.255.0", "130.83.155.254");
                string[] dnsAddressses = new string[] { "130.83.22.63", "130.83.56.60" };
                ethernetJ11D.NetworkInterface.EnableStaticDns(dnsAddressses);
            }
        }

        // Function for starting time service
        private void StartClock()
        {
            // Time synchronization with time server using NTP protocol
            ViewManager.StartupView.SetMessage("Time synchronization going on...");
            timeMgr = TimeManager.TimeMgr;
            bool syncStatus = timeMgr.TimeSynchronize(120); // GMT + 2 (2 hours ahead of the GMT time)

            if (syncStatus && timeMgr.IsTimeSync) // To check initial time is synchronized properly & updated
            {
                currentTime = timeMgr.UpdatedTime;
            }
            else
            {
                currentTime = DateTime.Now.ToString("HH:mm:ss");
                Debug.Print(timeMgr.ExceptionMsg);
            }

            this.LoadLoginView();
        }

        void tmrIdleTimer_Tick(GT.Timer timer)
        {
            tmrIdleTimer.Stop();
            window.Background = new SolidColorBrush(GT.Color.Black);
            currentTime = timeMgr.UpdatedTime;
            window.Child = ViewManager.IdleTimeView;
        }

        // Function for loading user and area data from SD card
        private void LoadData()
        {
            if (sdCard.IsCardMounted)
            {
                try
                {
                    ViewManager.StartupView.SetMessage("Reading Data...");

                    // Reads user data file.
                    using (FileStream dataStream = sdCard.StorageDevice.OpenRead(user_file_path))
                    {
                        DataHandler.ProccessUserData(dataStream, DataHandler.DataType.user);
                    }

                    // Reads area data file.
                    using (FileStream dataStream = sdCard.StorageDevice.OpenRead(area_file_path))
                    {
                        DataHandler.ProccessUserData(dataStream, DataHandler.DataType.area);
                    }
                }
                catch (IOException ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        #endregion

        #region "Event Handlers"

        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            if (!sender.IsNetworkConnected && !sender.IsNetworkUp)
            {
                Debug.Print("Network is not available!");
            }
        }

        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            if(sender.IsNetworkConnected && sender.IsNetworkUp)
            {
                Debug.Print("Connected to network.");
                Debug.Print(ethernetJ11D.NetworkSettings.IPAddress);
                Thread.Sleep(30000);
                this.StartClock();
                this.LoadLoginView();

                // Reads weather data  (Does not work at the moment)
                //WeatherManager.GetWeatherData("london+uk");
                //WeatherManager.ProcessWeatherConditions();
            }
            else
            {
                Debug.Print("Network is not available!");
            }
        }

        void IdleTimeView_TouchDown(object sender, TouchEventArgs e)
        {
            window.Background = new SolidColorBrush(GT.Color.White);
            currentTime = timeMgr.UpdatedTime;
            window.Child = ViewManager.MainView;
            
            tmrIdleTimer.Start();
        }

        void MainView_TouchDown(object sender, TouchEventArgs e)
        {
            tmrIdleTimer.Stop();
            tmrIdleTimer.Start();
        }

        void window_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            int x;
            int y;
            // Use the out parameters to get the position of touch.
            e.GetPosition(window, 0, out x, out y);

            displayTE35.SimpleGraphics.DisplayEllipse(GT.Color.Red, 5, GT.Color.Yellow, x, y, 50, 30, 0);
        }

        #endregion
    }
}
