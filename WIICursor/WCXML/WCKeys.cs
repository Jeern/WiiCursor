
using CursorCommon;
using System.Linq;
using System.Xml.Linq;
namespace WiiCursor.WCXML
{
    /// <summary> 
    /// The Keys of the wii controller and what they are supposed to do (each returns a string with the Action).
    /// </summary>
    public class WCKeys : IXmlInitialized
    {
        public string A { get; set; }
        public string B { get; set; }
        public string Plus { get; set; }
        public string Minus { get; set; }
        public string Home { get; set; }
        public string One { get; set; }
        public string Two { get; set; }
        public string Up { get; set; }
        public string Down { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }

        #region IXmlInitialized Members

        public void Initialize(XDocument document)
        {
            A = (from element in document.Descendants()
                 where element.Name.LocalName == "A"
                    select element.Value).First();
            B  = (from element in document.Descendants()
                 where element.Name.LocalName == "B"
                 select element.Value).First();
            Plus = (from element in document.Descendants()
                 where element.Name.LocalName == "Plus"
                 select element.Value).First();
            Minus  = (from element in document.Descendants()
                 where element.Name.LocalName == "Minus"
                 select element.Value).First();
            Home  = (from element in document.Descendants()
                 where element.Name.LocalName == "Home"
                 select element.Value).First();
            One  = (from element in document.Descendants()
                 where element.Name.LocalName == "One"
                 select element.Value).First();
            Two  = (from element in document.Descendants()
                 where element.Name.LocalName == "Two"
                 select element.Value).First();
            Up = (from element in document.Descendants()
                 where element.Name.LocalName == "Up"
                 select element.Value).First();
            Down  = (from element in document.Descendants()
                 where element.Name.LocalName == "Down"
                 select element.Value).First();
            Left  = (from element in document.Descendants()
                 where element.Name.LocalName == "Left"
                 select element.Value).First();
            Right  = (from element in document.Descendants()
                      where element.Name.LocalName == "Right"
                 select element.Value).First();

        }

        #endregion
    }
}
