using NUnit.Framework;
using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace TestApi
{
    public class DemoMacroscopTests
    {
        //Константные ссылки на документы 
        private string mTimeURLXml = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp";
        private string mTimeURLJSON = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp&responsetype=json";
        private string mChannelsURLXml = "http://demo.macroscop.com:8080/configex?login=root&amp&password=";
        /// <summary>
        /// Функция для получения с документом
        /// На вход подаётся ссылка на документ и символы в документе до которых идёт серверная информация
        /// На выход передаётся строка без серверной информации
        /// </summary>
        string GetDownloadString(string URL, string DeleteUntilSymbol)
        {
            string DownloadString = "";
            try
            {
                using (var wc = new WebClient())
                {
                    DownloadString = wc.DownloadString(URL);
                    DownloadString = DownloadString.Remove(0, DownloadString.IndexOf(DeleteUntilSymbol));
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Невозможно загрузить документ.\n" 
                            + ex.Message);
            }
            return DownloadString;
        }

        /// <summary>
        /// Тестирование разницы во времени между локальным и серверным в формате XML 
        /// </summary>
        [Test]
        public void СommandGettime_ReciveXML_15SecLocalTimeDiff()
        {
            //Устанавливаем время
            DateTime CurentTime = DateTime.UtcNow;
            //Получаем xml документ
            string XmlStr = GetDownloadString(mTimeURLXml, "<?xml");

            Assert.IsTrue(!string.IsNullOrEmpty(XmlStr), "Строка с данными пустая");

            //Преобразуем полученную строку в XML документ
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                XmlDoc.LoadXml(XmlStr);
            }
            catch(Exception ex)
            {
                Assert.Fail("Невозможно загрузить XML файл\n" + ex.Message);
            }

            DateTime ServerTime;
            //Проверяем возможность перевести время в локальный формат
            if (!DateTime.TryParseExact(
                XmlDoc.DocumentElement.InnerText.Trim(), 
                "dd.MM.yyyy HH:mm:ss", 
                CultureInfo.CurrentCulture, 
                DateTimeStyles.None, 
                out ServerTime))
            {
                Assert.Fail("Невозможно конвертировать дату под формат dd.MM.yyyy HH: mm:ss");
            }


            double time = (CurentTime - ServerTime).TotalSeconds;
            Assert.IsTrue(time <= 15, $"Разница во времени между этим компьютером и сервером больше на {time} секунд");
        }
        /// <summary>
        /// Тестирование разницы во времени между локальным и серверным в формате JSON
        /// </summary>
        [Test]
        public void СommandGettime_ReciveJSON_15SecLocalTimeDiff()
        {
            //Устанавливаем время
            DateTime CurentTime = DateTime.UtcNow;
            //Получаем json документ
            string JsonStr = GetDownloadString(mTimeURLJSON, "\r\n\r\n\"");

            Assert.IsTrue(!string.IsNullOrEmpty(JsonStr), "Строка с данными пустая"); 

            string Time = string.Empty;
            //Выполняем десериализацию строки из JSON
            try
            {
                Time = JsonConvert.DeserializeObject<string>(JsonStr);
            }
            catch (Exception ex)
            {
                Assert.Fail("Невозможно загрузить XML файл\n" + ex.Message);

            }

            DateTime ServerTime;
            //Проверяем возможность перевести время в локальный формат
            if (!DateTime.TryParseExact(
                Time, 
                "dd.MM.yyyy HH:mm:ss", 
                CultureInfo.CurrentCulture,
                DateTimeStyles.None, 
                out ServerTime))
            {
                Assert.Fail("Невозможно конвертировать дату под формат dd.MM.yyyy HH: mm:ss");
            }

            double time = (CurentTime - ServerTime).TotalSeconds;
            Assert.IsTrue(time <= 15, $"Разница во времени между этим компьютером и сервером больше на {time} секунд");
        }
        /// <summary>
        /// Тестирование разницы во времени между локальным и серверным в формате XML
        /// </summary>
        [Test]
        public void Сonfigex_ReciveXML_ChanelsMoreThan6()
        {
            //загружаем документ
            string XmlStr = GetDownloadString(mChannelsURLXml, "<?xml");
            Assert.IsTrue(!string.IsNullOrEmpty(XmlStr), "Строка с данными пустая");

            //Преобразуем полученную строку в XML документ
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                XmlDoc.LoadXml(XmlStr);
            }
            catch (Exception ex)
            {
                Assert.Fail("Невозможно загрузить XML файл\n" + ex.Message);
            }
            //Проверяем существование каналов
            if (XmlDoc.SelectSingleNode("/Configuration/Channels/ChannelInfo") == null)
                Assert.Fail($"В полученной конфигурации нет каналов");

            //Выбираем все ветви с именем ChannelInfo
            XmlNodeList xmlNodeList = XmlDoc.SelectNodes("/Configuration/Channels/ChannelInfo");
            
            Assert.IsTrue(xmlNodeList.Count>=6, $"В полученной конфигурации {xmlNodeList.Count} каналов");
        }
    }
}