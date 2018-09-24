using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSharpLearning.XML_Reading
{
    class XMLReaderSample
    {
        public static void main(String[] args)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("https://www.w3schools.com/xml/tempconvert.asmx?WSDL");

            foreach(XmlNode nodes in doc.ChildNodes)
            {
                if(nodes.InnerText.ToString() == "wsdl:types")
                {

                }
            }
            
        }
    }
}
