using ListBinding.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ListBinding.Model
{
    public class Persons : IPersons, IEnumerable<Person>
    {
        /// <summary>
        /// The persons
        /// </summary>
        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        public Persons()
        {

            if (string.IsNullOrEmpty(Settings.Default.Data))
                persons = new ObservableCollection<Person>();
            else
                try
                {
                    // Construct an instance of the XmlSerializer with the type
                    // of object that is being deserialized.
                    var mySerializer = new XmlSerializer(typeof(ObservableCollection<Person>));
                    // To read the file, create a FileStream.
                    using var myFileStream = new MemoryStream();
                    byte[] b;
                    myFileStream.Write(b = Encoding.UTF8.GetPreamble(), 0, b.Length);
                    myFileStream.Write(b = Encoding.UTF8.GetBytes(Settings.Default.Data), 0, b.Length);
                    myFileStream.Position = 0;
                    // Call the Deserialize method and cast to the object type.
                    persons = (ObservableCollection<Person>)mySerializer.Deserialize(myFileStream);
                }
                catch { persons = new ObservableCollection<Person>(); }
        }
        class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        ~Persons()
        {
            // Insert code to set properties and fields of the object.  
            XmlSerializer mySerializer = new(typeof(ObservableCollection<Person>));
            // To write to a file, create a StreamWriter object.  
            StringWriter myWriter = new Utf8StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(myWriter);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 2;
            mySerializer.Serialize(xmlTextWriter, persons);
            myWriter.Close();
            Settings.Default.Data = myWriter.ToString();
            Settings.Default.Save();
        }
        ObservableCollection<Person> IPersons.Persons => persons;

        public IEnumerator<Person> GetEnumerator()
            => persons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => persons.GetEnumerator();
    }
}
