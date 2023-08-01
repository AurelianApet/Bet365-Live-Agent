using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bet365LiveAgent
{
    class Config
    {        
        private const string TAG_WEBSOCK_SERVER_PORT = "WebSockServerPort";

        public int WebSockServerPort { get; set; }

        private static Config _instance = null;
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Config();
                return _instance;
            }            
        }

        public Config()
        {
            WebSockServerPort = 9000;
        }        

        public void LoadConfig()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Global.ConfigFileName);

                XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("config");
                XmlNode xmlRoot = xmlNodeList.Item(0);

                XmlNode xmlNode = xmlRoot.FirstChild;
                while(xmlNode != null)
                {
                    switch(xmlNode.Name)
                    {
                        case TAG_WEBSOCK_SERVER_PORT:
                            WebSockServerPort = Convert.ToUInt16(xmlNode.InnerText);
                            break;
                        default:
                            break;
                    }
                    xmlNode = xmlNode.NextSibling;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in LoadConfig() : {ex}");
            }
        }

        public void SaveConfig()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<config/>");

                XmlElement xmlRoot = xmlDocument.DocumentElement;

                XmlDocumentFragment xmlDocFragment = xmlDocument.CreateDocumentFragment();
                xmlDocFragment.InnerXml = $"<{TAG_WEBSOCK_SERVER_PORT}>{WebSockServerPort}</{TAG_WEBSOCK_SERVER_PORT}>";

                xmlRoot.AppendChild(xmlDocFragment);

                xmlDocument.Save(Global.ConfigFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in SaveConfig() : {ex}");
            }
        }
    }
}
