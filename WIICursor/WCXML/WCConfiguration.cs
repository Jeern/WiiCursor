using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using CursorCommon;

namespace WiiCursor.WCXML
{
    /// <summary>
    /// This class holds Configuration data for exactly one XML file that conforms to the WCConfiguration.xsd schema
    /// </summary>
    internal class WCConfiguration : IXmlInitialized
    {
        internal string Name { get; set; }
        internal bool Default { get; set; }
        internal int ConnectionWaitTime { get; set; }
        internal int ConnectionRetries { get; set; }
        internal WCKeys Keys { get; set; }

        #region IXmlInitialized Members

        /// <summary>
        /// Initializes the Object by reading from the XML file
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

            string connectionWaitTime = (from element in document.Descendants()
                                         where element.Name.LocalName == "ConnectionWaitTime"
                                      select element.Value).First();
            ConnectionWaitTime = Convert.ToInt32(connectionWaitTime, CultureInfo.InvariantCulture);

            string connectionRetries = (from element in document.Descendants()
                                        where element.Name.LocalName == "ConnectionRetries"
                                         select element.Value).First();
            ConnectionRetries = Convert.ToInt32(connectionRetries, CultureInfo.InvariantCulture);

            Keys = new WCKeys();
            Keys.Initialize(document);


        }

        #endregion
    }
}
