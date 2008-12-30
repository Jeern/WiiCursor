using System.Xml.Linq;

namespace CursorCommon
{
    /// <summary>
    /// The Initialize interface is used to initialize an object from an XML file. 
    /// </summary>
    public interface IXmlInitialized
    {
        void Initialize(XDocument document);
    }
}
