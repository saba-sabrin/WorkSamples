using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;

namespace CatRcsWeb
{
    public static class UtilityFunc
    {
        public static string GetItemData(string filePath)
        {
            // Reading item pool data from Items file
            FileStream dataStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[dataStream.Length];
            int read_count = dataStream.Read(data, 0, data.Length);
            string result_data = new string(Encoding.UTF8.GetChars(data), 0, read_count);
            dataStream.Close();

            return result_data;
        }
    }
}