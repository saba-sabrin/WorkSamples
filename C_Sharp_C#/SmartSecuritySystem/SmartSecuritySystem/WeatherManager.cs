using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Globalization;
using GT = Gadgeteer;
using Gadgeteer.Modules;
using Gadgeteer.Networking;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace SmartSecSystem
{
    public class WeatherManager
    {
        public static XmlTextReader reader = null;

        // <summary>
        /// The function that returns the current conditions for the specified location.
        /// </summary>
        /// <param name="location">City or ZIP code</param>
        /// <returns></returns>
        public static WeatherCondition ProcessWeatherConditions()
        {
            WeatherCondition conditions = new WeatherCondition();

            //reader = new XmlTextReader("http://www.google.com/ig/api?weather={0}" + location);
            //doc.Load(reader);

            /*XmlDocument xmlConditions = new XmlDocument();
            //xmlConditions.Load(string.Format("http://www.google.com/ig/api?weather={0}", location));

            if (xmlConditions.SelectSingleNode("xml_api_reply/weather/problem_cause") != null)
            {
                conditions = null;
            }
            else
            {
                conditions.City = xmlConditions.SelectSingleNode("/xml_api_reply/weather/forecast_information/city").Attributes["data"].InnerText;
                conditions.Condition = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/condition").Attributes["data"].InnerText;
                conditions.TempC = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_c").Attributes["data"].InnerText;
                conditions.TempF = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_f").Attributes["data"].InnerText;
                conditions.Humidity = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/humidity").Attributes["data"].InnerText;
                conditions.Wind = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/wind_condition").Attributes["data"].InnerText;
            }*/

            return conditions;
        }

        public static void GetWeatherData(string location)
        {
            GETContent getContent = new GETContent();
            // xml.weather.yahoo.com/  ,  https://weather.yahooapis.com/forecastrss?w=2502265

            HttpRequest getRequest = HttpHelper.CreateHttpGetRequest("https://xml.weather.yahoo.com/forecastrss?w=2502265", getContent);

            //HttpRequest getRequest = HttpHelper.CreateHttpGetRequest("https://www.google.com/ig/api?weather=london" + location, getContent);
            getRequest.ResponseReceived += getRequest_ResponseReceived;
            getRequest.SendRequest();
        }

        static void getRequest_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            /*if (response.StatusCode != "200")
            {
                Debug.Print("Error " + response.StatusCode + ": Failed to load weather data " + response.URL);
                return;
            }*/

            reader = new XmlTextReader(response.Stream);
        }
    }

    public class WeatherCondition
    {
        public string City { get; set; }
        public string Condition { get; set; }
        public string TempF { get; set; }
        public string TempC { get; set; }
        public string Humidity { get; set; }
        public string Wind { get; set; }
        public string Day { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
    }
}
