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
using Microsoft.SPOT.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System.Net;

namespace SmartSecSystem
{
    public class TimeManager
    {
        public bool IsTimeSync { get; set; }

        public string UpdatedTime { get; set; }

        public string ExceptionMsg { get; set; }

        private static TimeManager _timeManager;
        public static TimeManager TimeMgr
        {
            get
            {
                if (_timeManager == null)
                    _timeManager = new TimeManager();

                return _timeManager;
            }
        }

        private TimeManager()
        {
            TimeService.SystemTimeChanged += TimeService_SystemTimeChanged;
            TimeService.TimeSyncFailed += TimeService_TimeSyncFailed;
        }

        public bool TimeSynchronize(int timeZoneOffset)
        {
            try
            {
                // Configure TimeService settings.
                TimeServiceSettings settings = new TimeServiceSettings();
                settings.AutoDayLightSavings = true;
                settings.ForceSyncAtWakeUp = true;
                settings.RefreshTime = 1;    // in seconds.

                // Getting time from server, setting primary server IP address

                IPAddress[] address = Dns.GetHostEntry("time.windows.com").AddressList;
                settings.PrimaryServer = address[0].GetAddressBytes();

                // Getting time from server for setting alternate server IP address
                address = Dns.GetHostEntry("time-a.nist.gov").AddressList;
                if (address != null && address.Length > 0)
                    settings.AlternateServer = address[0].GetAddressBytes();

                TimeService.Settings = settings;

                // Add the time zone offset to the retrieved UTC Time, GMT + 2
                TimeService.SetTimeZoneOffset(timeZoneOffset);

                // Gets the Internet time.
                TimeService.Stop();
                TimeService.Start();

                Debug.Print("Time synchronization successful!");
                Debug.Print("Today is : " + DateTime.Now.ToString());

                return true;
            }
            catch(System.Net.Sockets.SocketException ex)
            {
                this.ExceptionMsg = ex.Message;
                return false;
            }
        }

        void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
            Debug.Print("Time synchronization failed!" + e.ErrorCode.ToString());
        }

        void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
            // creating instance of manager
            TimeManager mgr = TimeManager.TimeMgr;

            // Updating current time
            mgr.UpdatedTime = DateTime.Now.ToString("HH:mm:ss");
            Program.currentTime = DateTime.Now.ToString("HH:mm:ss");

            if (!mgr.IsTimeSync)
                mgr.IsTimeSync = true;
        }
    }
}
