using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

namespace De.Markellus.HybridInjector.Misc
{
    internal class PortableSettingsProvider : SettingsProvider
    {
        private readonly string _savingPath;

        public sealed override string ApplicationName
        {
            get => "De.Markellus.HybridInjector";
            set { }
        }

        public PortableSettingsProvider()
        {
            _savingPath = ApplicationName + ".xml";
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty property in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(property) {IsDirty = false};
                values.Add(value);
            }

            if (!File.Exists(_savingPath))
            {
                return values;
            }

            using (XmlTextReader tr = new XmlTextReader(_savingPath))
            {
                try
                {
                    tr.ReadStartElement("de.markellus.hybridinjector");
                    foreach (SettingsPropertyValue value in values)
                    {
                        try
                        {
                            tr.ReadStartElement(value.Name);
                            value.SerializedValue = tr.ReadContentAsObject();
                            tr.ReadEndElement();
                        }
                        catch (XmlException)
                        {
                            /* ugly */
                        }
                    }

                    tr.ReadEndElement();
                }
                catch (XmlException)
                {
                    /* ugly */
                }
            }
            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            using (XmlTextWriter tw = new XmlTextWriter(_savingPath, Encoding.Unicode))
            {
                tw.WriteStartDocument();
                tw.WriteStartElement("de.markellus.hybridinjector");
                foreach (SettingsPropertyValue propertyValue in collection)
                {
                    tw.WriteStartElement(propertyValue.Name);
                    tw.WriteValue(propertyValue.SerializedValue);
                    tw.WriteEndElement();
                }

                tw.WriteEndElement();
                tw.WriteEndDocument();
            }
        }
    }
}
