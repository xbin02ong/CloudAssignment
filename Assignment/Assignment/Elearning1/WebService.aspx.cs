using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net;
using System.Configuration;
using System.IO;

namespace Assignment.Elearning1
{
    public partial class WebService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XmlDocument wsResponseXmlDoc = new XmlDocument();

            //Declaring Web Service App Key
            string WebServiceKey = ConfigurationSettings.AppSettings["WebServiceKey"];

            //http://api.worldweatheronline.com/free/v1/weather.ashx?q=china&format=xml&num_of_days=5&key=*********************
            //id=jipx(spacetime0)
            UriBuilder url = new UriBuilder();
            url.Scheme = "http";// Same as "http://"

            url.Host = "api.worldweatheronline.co";
            url.Path = "free/v2/weather.ashx";
            url.Query = "q=china&format=xml&num_of_days=5&key="+WebServiceKey;

            // Check for cache URL
            String URIcache = File.ReadAllText("C:/Users/Xinbin/Desktop/CloudAssignment/URIcache.txt");

            if (String.Compare(URIcache, url.ToString()) == 0)
            {
                //if it matches the URL, load and Deisplay XML file
                wsResponseXmlDoc = new XmlDocument();
                wsResponseXmlDoc.Load("C:/Users/Xinbin/Desktop/CloudAssignment/WebServiceCache.xml");

                 
                String xmlString = wsResponseXmlDoc.InnerXml;
                Response.ContentType = "text/xml";
                Response.Write(xmlString);


            }
            else
            {


                //Make a HTTP request to the global weather web service
                wsResponseXmlDoc = MakeRequest(url.ToString());

                
               
                int NumberofError = 0;
                // Recall 3 times if unsuccessful
              
            for (NumberofError = 0; NumberofError < 3; NumberofError++) {
                wsResponseXmlDoc = MakeRequest(url.ToString());
                if (wsResponseXmlDoc != null) { break;}
            }
                // When Still error display error msg 
                if (wsResponseXmlDoc == null)
                {
                  
                    wsResponseXmlDoc = new XmlDocument();
                    wsResponseXmlDoc.Load("C:/Users/Xinbin/Desktop/CloudAssignment/WebService.xml");

                  
                    String xmlString = wsResponseXmlDoc.InnerXml;
                    Response.ContentType = "text/xml";
                    Response.Write(xmlString);

                }
                else
                {
                    
                    String xmlString = wsResponseXmlDoc.InnerXml;
                    Response.ContentType = "text/xml";
                    Response.Write(xmlString);


 
                    String filePath = "C:/Users/Xinbin/Desktop/CloudAssignment";
                    File.WriteAllText(filePath + @"/URIcache.txt", url.ToString());

 
                    wsResponseXmlDoc.Save("C:/Users/Xinbin/Desktop/CloudAssignment/WebServiceCache.xml");


                }

            }

        }

        public static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                // Set timeout to 5 seconds
                request.Timeout = 5 * 1000;
                request.KeepAlive = false;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}