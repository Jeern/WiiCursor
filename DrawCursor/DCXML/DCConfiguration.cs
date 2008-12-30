using System;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;
using CursorCommon;

namespace DrawCursor.XML
{
    /// <summary>
    /// This class holds Configuration data for exactly one XML file that conforms to the DCConfiguration.xsd schema
    /// </summary>
    internal class DCConfiguration : IXmlInitialized
    {
        internal string Name { get; set; }
        internal bool Default { get; set; }
        internal int MarkerSize { get; set; }
        internal Color MarkerColor { get; set; }

        #region IXmlInitialized Members

        /// <summary>
        /// Initializes the Object by reading from the XML file using LinqToXML
        /// </summary>
        /// <param name="document"></param>
        public void Initialize(XDocument document)
        {
            Name = (from element in document.Descendants()
                                 where element.Name.LocalName == "Name"
                                 select element.Value).First();

            var defaultElements = from element in document.Descendants()
                       where element.Name.LocalName == "Default"
                       select element.Value;
            if (defaultElements.Count() > 0)
            {
                Default = Convert.ToBoolean(defaultElements.First(), CultureInfo.InvariantCulture);
            }
            else
            {
                Default = false;
            }

            string markerSize = (from element in document.Descendants()
                         where element.Name.LocalName == "MarkerSize"
                         select element.Value).First();
            MarkerSize = Convert.ToInt32(markerSize, CultureInfo.InvariantCulture);

            string markerColor = (from element in document.Descendants()
                                 where element.Name.LocalName == "MarkerColor"
                                select element.Value).First();
            MarkerColor = (Color)ColorConverter.ConvertFromString(markerColor);
        }

        #endregion
    }
}
