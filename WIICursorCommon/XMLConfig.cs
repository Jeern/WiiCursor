using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using System;

namespace CursorCommon
{ 
    /// <summary>
    /// This class contains functionality for reading and validating all XML files in a given directory.
    /// </summary>
    /// <typeparam name="C">This is the type of the object to be initialized with data from each XML file</typeparam>
    public class XMLConfig<C> where C : IXmlInitialized, new()
    {
        /// <summary>
        /// The XSD file to validate against
        /// </summary>
        private string m_XsdFile;

        public XMLConfig(string xsdFile)
        {
            m_XsdFile = xsdFile;
        }
            
        /// <summary>
        /// Reads All XML files and returns a List of objects containing the data. Also validates each file.
        /// Uses Read() which reads and validates one file.
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public List<C> ReadAll(string xmlPath)
        {
            try
            {
                string[] files = Directory.GetFiles(xmlPath, "*.xml");
                List<C> configurated = new List<C>();
                foreach (string file in files)
                {
                    configurated.Add(Read(file));
                }
                return configurated;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Problem with reading XmlFiles from Path: {0}", xmlPath), ex);
            }
        }

        /// <summary>
        /// Reads and validates an XML file.
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        private C Read(string xmlFile)
        {
            try
            {
                XmlSchema schema;
                using (var fs = new FileStream(m_XsdFile, FileMode.Open))
                {
                    schema = XmlSchema.Read(fs, (o, e) =>
                    {
                        throw new InvalidDataException(string.Format("The Schema: {0} is Invalid. Fails with this message: {1}", m_XsdFile, e.Message));
                    });
                }
                var schemas = new XmlSchemaSet();
                schemas.Add(schema);
                var doc = XDocument.Load(xmlFile);
                doc.Validate(schemas, (o, e) =>
                {
                    throw new InvalidDataException(string.Format("The file: {0} does not validate against {1}. Fails with this message: {2}", xmlFile, m_XsdFile, e.Message));
                });
                var configurated = new C();
                configurated.Initialize(doc);
                return configurated;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Problem with reading XmlFile: {0}", xmlFile), ex);
            }
        }
    }
}
