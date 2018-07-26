using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.SPOT;

namespace SmartSecSystem
{
    public static class DataHandler
    {
        private static Hashtable userData;
        private static Hashtable areaData;

        static DataHandler()
        {
            userData = new Hashtable();
            areaData = new Hashtable();
        }

        public static string[] GetUserDetails(string value)
        {
            string[] userInfo = null;

            if (userData.Keys.Count > 0)
            {
                var allKeys = userData.Keys;

                foreach(var key in allKeys)
                {
                    string[] tempInfo = (string[])userData[key];

                    if (tempInfo[1] == value) // match password
                    {
                        userInfo = tempInfo;
                    }
                }
            }

            return userInfo;
        }

        public static string[] GetAreaDetails(string areaID)
        {
            string[] areaInfo = null;

            if (areaData.Keys.Count > 0)
            {
                areaInfo = (string[])areaData[areaID];
            }

            return areaInfo;
        }

        public static void SetUserData(string key, string value)
        {
            userData[key] = value;
        }

        public static void ProccessUserData(FileStream dataStream, DataType paramDataType)
        {
            byte[] data = new byte[dataStream.Length];
            int read_count = dataStream.Read(data, 0, data.Length);
            var all_data = new string(Encoding.UTF8.GetChars(data), 0, read_count);
            dataStream.Close();

            string[] text = all_data.Split(new char[] { '\r', '\n' });

            foreach (string value in text)
            {
                if (value != "")
                {
                    string[] temp = value.Split(',');

                    string[] Details = new string[] { temp[1], temp[2], temp[3]};  
                    
                    // ID, Username, Password, Accessible Room/Area
                    //   OR
                    // ID, Room, Restricted time limit: start-time, end-time

                    if(paramDataType == DataType.user)
                    {
                        userData.Add(temp[0], Details);
                    }

                    if (paramDataType == DataType.area)
                    {
                        areaData.Add(temp[0], Details);
                    }
                }
            }
        }

        public enum DataType
        {
            user = 1,
            area = 2
        }

        /* One more data file for area information like, rooms & different places
         * Assuming, there is a motion detector & camera in front of the door or at some random area */
    }
}
