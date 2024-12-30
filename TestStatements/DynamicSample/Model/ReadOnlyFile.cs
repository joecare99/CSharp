using System;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;

/// <summary>
/// The Model namespace.
/// </summary>
namespace DynamicSample.Model
{
    /// <summary>
    /// Enum StringSearchOption
    /// </summary>
    public enum StringSearchOption
    {
        /// <summary>
        /// The starts with
        /// </summary>
        StartsWith,
        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        Contains,
        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith
    }
    /// <summary>
    /// Class ReadOnlyFile.
    /// Implements the <see cref="DynamicObject" />
    /// </summary>
    /// <seealso cref="DynamicObject" />
    class ReadOnlyFile : DynamicObject
    {
        // Store the path to the file and the initial line count value.
        private string p_filePath;

        // Public constructor. Verify that file exists and store the path in 
        // the private variable.
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFile"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="Exception">File path does not exist.</exception>
        public ReadOnlyFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File path does not exist.");
            }

            p_filePath = filePath;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="StringSearchOption">The string search option.</param>
        /// <param name="trimSpaces">if set to <c>true</c> [trim spaces].</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetPropertyValue(string propertyName,
                                     StringSearchOption StringSearchOption = StringSearchOption.StartsWith,
                                     bool trimSpaces = true)
        {
            StreamReader sr = null;
            List<string> results = new List<string>();
            string line = "";
            string testLine = "";

            try
            {
                sr = new StreamReader(p_filePath);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    // Perform a case-insensitive search by using the specified search options.
                    testLine = line.ToUpper();
                    if (trimSpaces) { testLine = testLine.Trim(); }

                    switch (StringSearchOption)
                    {
                        case StringSearchOption.StartsWith:
                            if (testLine.StartsWith(propertyName.ToUpper())) { results.Add(line); }
                            break;
                        case StringSearchOption.Contains:
                            if (testLine.Contains(propertyName.ToUpper())) { results.Add(line); }
                            break;
                        case StringSearchOption.EndsWith:
                            if (testLine.EndsWith(propertyName.ToUpper())) { results.Add(line); }
                            break;
                    }
                }
            }
            catch
            {
                // Trap any exception that occurs in reading the file and return null.
                results = null;
            }
            finally
            {
                if (sr != null) { sr.Close(); }
            }

            return results;
        }

        // Implement the TryGetMember method of the DynamicObject class for dynamic member calls.
        /// <summary>
        /// Stellt die Implementierung für Vorgänge, die Memberwerte abrufen.
        /// Abgeleitete Klassen die <see cref="T:System.Dynamic.DynamicObject" /> Klasse kann überschreiben diese Methode, um dynamisches Verhalten für Vorgänge wie das Abrufen eines Werts für eine Eigenschaft anzugeben.
        /// </summary>
        /// <param name="binder">Enthält Informationen über das Objekt, das den dynamischen Vorgang aufgerufen hat.
        /// Die binder.Name -Eigenschaft gibt den Namen des Elements für den dynamische Vorgang ausgeführt wird.
        /// Z. B. für die Console.WriteLine(sampleObject.SampleProperty) -Anweisung, in denen sampleObject ist eine Instanz der abgeleiteten Klasse von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse, binder.Name "SampleProperty" zurückgegeben.
        /// Die binder.IgnoreCase -Eigenschaft gibt an, ob der Membername Groß-/Kleinschreibung beachtet wird.</param>
        /// <param name="result">Das Ergebnis des Get-Vorgangs.
        /// Z. B. wenn die Methode für eine Eigenschaft aufgerufen wird, können Sie zuweisen den Eigenschaftswert <paramref name="result" />.</param>
        /// <returns><see langword="true" />, wenn der Vorgang erfolgreich ist, andernfalls <see langword="false" />.
        /// Wenn diese Methode gibt <see langword="false" />, die Laufzeitbinder der Sprache bestimmt das Verhalten.
        /// (In den meisten Fällen wird eine Laufzeitausnahme ausgelöst.)</returns>
        public override bool TryGetMember(GetMemberBinder binder,
                                          out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }

        // Implement the TryInvokeMember method of the DynamicObject class for 
        // dynamic member calls that have arguments.
#if NET5_0_OR_GREATER
        public override bool TryInvokeMember(InvokeMemberBinder binder,  object?[]? args, out object result)
#else
        /// <summary>
        /// Stellt die Implementierung für Vorgänge, die einen Member aufzurufen.
        /// Abgeleitete Klassen die <see cref="T:System.Dynamic.DynamicObject" /> Klasse kann überschreiben diese Methode, um dynamisches Verhalten für Vorgänge wie das Aufrufen einer Methode anzugeben.
        /// </summary>
        /// <param name="binder">Enthält Informationen zu den dynamischen Vorgang.
        /// Die binder.Name -Eigenschaft gibt den Namen des Elements für den dynamische Vorgang ausgeführt wird.
        /// Beispielsweise für die Anweisung sampleObject.SampleMethod(100), wobei sampleObject ist eine Instanz der abgeleiteten Klasse von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse binder.Name gibt "z. B. SampleMethod" zurück.
        /// Die binder.IgnoreCase -Eigenschaft gibt an, ob der Membername Groß-/Kleinschreibung beachtet wird.</param>
        /// <param name="args">Die Argumente, die während des Aufrufvorgangs an dem Objektmember übergeben werden.
        /// Beispielsweise für die Anweisung sampleObject.SampleMethod(100), wobei sampleObject stammt von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse, <paramref name="args[0]" /> gleich 100 ist.</param>
        /// <param name="result">Das Ergebnis des Memberaufrufs.</param>
        /// <returns><see langword="true" />, wenn der Vorgang erfolgreich ist, andernfalls <see langword="false" />.
        /// Wenn diese Methode gibt <see langword="false" />, die Laufzeitbinder der Sprache bestimmt das Verhalten.
        /// (In den meisten Fällen wird eine sprachspezifische Laufzeitausnahme ausgelöst.)</returns>
        /// <exception cref="ArgumentException">StringSearchOption argument must be a StringSearchOption enum value.</exception>
        /// <exception cref="ArgumentException">trimSpaces argument must be a Boolean value.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder,object[] args, out object result)
#endif
        {
            StringSearchOption StringSearchOption = StringSearchOption.StartsWith;
            bool trimSpaces = true;

            try
            {
                if (args.Length > 0) { StringSearchOption = (StringSearchOption)args[0]; }
            }
            catch
            {
                throw new ArgumentException("StringSearchOption argument must be a StringSearchOption enum value.");
            }

            try
            {
                if (args.Length > 1) { trimSpaces = (bool)args[1]; }
            }
            catch
            {
                throw new ArgumentException("trimSpaces argument must be a Boolean value.");
            }

            result = GetPropertyValue(binder.Name, StringSearchOption, trimSpaces);

            return result == null ? false : true;
        }

    }
}
