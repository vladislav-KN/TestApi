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
        //����������� ������ �� ��������� 
        private string mTimeURLXml = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp";
        private string mTimeURLJSON = "http://demo.macroscop.com:8080/command?type=gettime&amp&login=root&amp;password=&amp&responsetype=json";
        private string mChannelsURLXml = "http://demo.macroscop.com:8080/configex?login=root&amp&password=";
        /// <summary>
        /// ������� ��� ��������� � ����������
        /// �� ���� ������� ������ �� �������� � ������� � ��������� �� ������� ��� ��������� ����������
        /// �� ����� ��������� ������ ��� ��������� ����������
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
                Assert.Fail("���������� ��������� ��������.\n" 
                            + ex.Message);
            }
            return DownloadString;
        }

        /// <summary>
        /// ������������ ������� �� ������� ����� ��������� � ��������� � ������� XML 
        /// </summary>
        [Test]
        public void �ommandGettime_ReciveXML_15SecLocalTimeDiff()
        {
            //������������� �����
            DateTime CurentTime = DateTime.UtcNow;
            //�������� xml ��������
            string XmlStr = GetDownloadString(mTimeURLXml, "<?xml");

            Assert.IsTrue(!string.IsNullOrEmpty(XmlStr), "������ � ������� ������");

            //����������� ���������� ������ � XML ��������
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                XmlDoc.LoadXml(XmlStr);
            }
            catch(Exception ex)
            {
                Assert.Fail("���������� ��������� XML ����\n" + ex.Message);
            }

            DateTime ServerTime;
            //��������� ����������� ��������� ����� � ��������� ������
            if (!DateTime.TryParseExact(
                XmlDoc.DocumentElement.InnerText.Trim(), 
                "dd.MM.yyyy HH:mm:ss", 
                CultureInfo.CurrentCulture, 
                DateTimeStyles.None, 
                out ServerTime))
            {
                Assert.Fail("���������� �������������� ���� ��� ������ dd.MM.yyyy HH: mm:ss");
            }


            double time = (CurentTime - ServerTime).TotalSeconds;
            Assert.IsTrue(time <= 15, $"������� �� ������� ����� ���� ����������� � �������� ������ �� {time} ������");
        }
        /// <summary>
        /// ������������ ������� �� ������� ����� ��������� � ��������� � ������� JSON
        /// </summary>
        [Test]
        public void �ommandGettime_ReciveJSON_15SecLocalTimeDiff()
        {
            //������������� �����
            DateTime CurentTime = DateTime.UtcNow;
            //�������� json ��������
            string JsonStr = GetDownloadString(mTimeURLJSON, "\r\n\r\n\"");

            Assert.IsTrue(!string.IsNullOrEmpty(JsonStr), "������ � ������� ������"); 

            string Time = string.Empty;
            //��������� �������������� ������ �� JSON
            try
            {
                Time = JsonConvert.DeserializeObject<string>(JsonStr);
            }
            catch (Exception ex)
            {
                Assert.Fail("���������� ��������� XML ����\n" + ex.Message);

            }

            DateTime ServerTime;
            //��������� ����������� ��������� ����� � ��������� ������
            if (!DateTime.TryParseExact(
                Time, 
                "dd.MM.yyyy HH:mm:ss", 
                CultureInfo.CurrentCulture,
                DateTimeStyles.None, 
                out ServerTime))
            {
                Assert.Fail("���������� �������������� ���� ��� ������ dd.MM.yyyy HH: mm:ss");
            }

            double time = (CurentTime - ServerTime).TotalSeconds;
            Assert.IsTrue(time <= 15, $"������� �� ������� ����� ���� ����������� � �������� ������ �� {time} ������");
        }
        /// <summary>
        /// ������������ ������� �� ������� ����� ��������� � ��������� � ������� XML
        /// </summary>
        [Test]
        public void �onfigex_ReciveXML_ChanelsMoreThan6()
        {
            //��������� ��������
            string XmlStr = GetDownloadString(mChannelsURLXml, "<?xml");
            Assert.IsTrue(!string.IsNullOrEmpty(XmlStr), "������ � ������� ������");

            //����������� ���������� ������ � XML ��������
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                XmlDoc.LoadXml(XmlStr);
            }
            catch (Exception ex)
            {
                Assert.Fail("���������� ��������� XML ����\n" + ex.Message);
            }
            //��������� ������������� �������
            if (XmlDoc.SelectSingleNode("/Configuration/Channels/ChannelInfo") == null)
                Assert.Fail($"� ���������� ������������ ��� �������");

            //�������� ��� ����� � ������ ChannelInfo
            XmlNodeList xmlNodeList = XmlDoc.SelectNodes("/Configuration/Channels/ChannelInfo");
            
            Assert.IsTrue(xmlNodeList.Count>=6, $"� ���������� ������������ {xmlNodeList.Count} �������");
        }
    }
}