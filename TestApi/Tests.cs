using NUnit.Framework;
using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace TestApi
{
    public class Tests
    {
        string TimeURLXml = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp";
        string TimeURLJSON = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp&responsetype=json";
        [SetUp]
        public void Setup()
        {

        }
        /// <summary>
        /// Тестирование времени 
        /// </summary>
        [Test]
        public void TestTimeXML()
        {
            DateTime ComporisonTime;
            string xmlStr;
            using (var wc = new WebClient())
            {
                wc.Credentials = new NetworkCredential("root", "");
                ComporisonTime = DateTime.Now;
                xmlStr = wc.DownloadString(TimeURLXml);
                xmlStr = xmlStr.Remove(0,xmlStr.IndexOf("<?xml"));
            }
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            DateTime ServerTime = DateTime.ParseExact(xmlDoc.DocumentElement.InnerText.Trim(),
                                                      "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            double time = (ComporisonTime - ServerTime).TotalSeconds;
            bool IsTimeDifferIn15 = time <= 15;
            Assert.IsTrue(IsTimeDifferIn15, $"Разница во времени между этим компьютером и сервером больше на {time} секунд");
        }
        [Test]
        public void TestTimeJSON()
        {
            DateTime ComporisonTime;
            string jsonStr;
            using (var wc = new WebClient())
            {
                ComporisonTime = DateTime.Now;
                jsonStr = wc.DownloadString(TimeURLJSON);
                jsonStr = jsonStr.Remove(0, jsonStr.IndexOf("\r\n\r\n\""));
 
            }
            string Time = JsonConvert.DeserializeObject<string>(jsonStr);
 
            DateTime ServerTime = DateTime.ParseExact(Time,
                                                   "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            double time = (ComporisonTime - ServerTime).TotalSeconds;
            bool IsTimeDifferIn15 = time <= 15;
            Assert.IsTrue(IsTimeDifferIn15, $"Разница во времени между этим компьютером и сервером больше на {time} секунд");
        }
        [Test]
        public void TestConfigurationChanelsXML()
        {
            Assert.IsTrue(true);
        }
    }
}