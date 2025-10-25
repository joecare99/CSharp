using AA19_FilterLists.Properties;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AA19_FilterLists.Model;

public class Persons : IPersons, IEnumerable<Person>
{
    private ObservableCollection<Person> persons = new();

    public Persons()
    {
        try
        {
            if (string.IsNullOrEmpty(Settings.Default.Data))
                persons = new ObservableCollection<Person>();
            else
            {
                var mySerializer = new XmlSerializer(typeof(ObservableCollection<Person>));
                using var myFileStream = new MemoryStream();
                byte[] b;
                myFileStream.Write(b = Encoding.UTF8.GetPreamble(), 0, b.Length);
                myFileStream.Write(b = Encoding.UTF8.GetBytes(Settings.Default.Data), 0, b.Length);
                myFileStream.Position = 0;
                persons = (mySerializer.Deserialize(myFileStream) as ObservableCollection<Person>) ?? new ObservableCollection<Person>();
            }
        }
        catch { persons = new ObservableCollection<Person>(); }
    }
    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    ~Persons()
    {
        XmlSerializer mySerializer = new(typeof(ObservableCollection<Person>));
        StringWriter myWriter = new Utf8StringWriter();
        XmlTextWriter xmlTextWriter = new XmlTextWriter(myWriter) { Formatting = Formatting.Indented, Indentation = 2 };
        mySerializer.Serialize(xmlTextWriter, persons);
        myWriter.Close();
        Settings.Default.Data = myWriter.ToString();
        // Settings.Default.Save(); // not needed for ApplicationSettings, kept managed externally
    }
    ObservableCollection<Person> IPersons.Persons => persons;

    public IEnumerator<Person> GetEnumerator() => persons.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => persons.GetEnumerator();
}
