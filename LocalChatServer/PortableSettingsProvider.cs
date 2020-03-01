using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LocalChatServer
{
    class PortableSettingsProvider : SettingsProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        public override string ApplicationName
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
            set {}
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty property in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(property)
                {
                    IsDirty = false
                };
                values.Add(value);
            }
            if (!File.Exists(GetSavingPath()))
            {
                return values;
            }
            using (XmlTextReader tr = new XmlTextReader(GetSavingPath()))
            {
                try
                {
                    tr.ReadStartElement("root");
                    foreach (SettingsPropertyValue value in values)
                    {
                        if (IsUserScoped(value.Property))
                        {
                            try
                            {
                                tr.ReadStartElement(value.Name);
                                value.SerializedValue = tr.ReadContentAsObject();
                                tr.ReadEndElement();
                            }
                            catch (XmlException e) { Debug.Print(e.Message); }
                        }
                    }
                    tr.ReadEndElement();
                }
                catch (XmlException e) { Debug.Print(e.Message); }
            }
            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            using (XmlTextWriter tw = new XmlTextWriter(GetSavingPath(),Encoding.Unicode))
            {
                tw.WriteStartDocument();
                tw.WriteStartElement("root");
                foreach (SettingsPropertyValue value in collection)
                {
                    tw.WriteStartElement(value.Name);
                    tw.WriteValue(value.SerializedValue);
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
            }
        }

        private string GetSavingPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "settings.config";
        }

        private bool IsUserScoped(SettingsProperty property)
        {
            return property.Attributes.ContainsKey(typeof(UserScopedSettingAttribute));
        }
    }
}
