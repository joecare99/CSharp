using BaseLib.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.IO;
using WinAhnenCls.Model.GenBase;

namespace WinAhnenCls.Model.HejInd
{
    public class CPerson : IDisposable, IGenPerson
    {
        private bool _disposedValue;
        private WeakReference<IGenealogy>? _WLowner;
        private IGenPerson Indi { get; set; } = null!;

        public CPerson()
        {
            _disposedValue = false;
        }

        // Basic-Properies
        public string Name { get => $"{Indi.Surname}, {Indi.GivenName}"; set => throw new NotImplementedException(); }
        public string GivenName { get => Indi.GivenName; set => Indi.GivenName = value; }
        public string Surname { get => Indi.Surname; set => Indi.Surname = value; }
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Sex { get => Indi.Sex; set => Indi.Sex = value; }
        public string IndRefID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Relationship-Properties
        public IGenPerson Father { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPerson Mother { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ChildCount => throw new NotImplementedException();

        public IIndexedList<IGenPerson> Children { get; }  

        public IGenFamily ParentFamily { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int FamilyCount => throw new NotImplementedException();

        public IIndexedList<IGenFamily> Families { get; }

        public int SpouseCount => throw new NotImplementedException();

        public IIndexedList<IGenPerson> Spouses { get; }

        // Vital-Properties

        public string Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        DateTime? IGenObject.LastChange => throw new NotImplementedException();

        public IGenDate BirthDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace BirthPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenFact Birth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenDate BaptDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace BaptPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenFact Baptism { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenDate DeathDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace DeathPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenFact Death { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenDate BurialDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace BurialPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenFact Burial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Religion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Occupation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace OccuPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenPlace Residence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<IGenFact> Facts { get; init; }
        public IList<IGenConnects> Connects { get ; init ; }

        public IGenFact Start => throw new NotImplementedException();

        public IGenFact End => throw new NotImplementedException();

        public IList<IGenSources> Sources { get ; init ; }
        public Guid UId { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public EGenType eGenType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        IList<IGenSources> IGenEntity.Sources { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenMedia> Media { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public IGenealogy Owner => (_WLowner?.TryGetTarget(out var t)??false)?t:null;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<IGenPerson> ReadFromStream(StreamReader sr)
        {
            throw new NotImplementedException();
        }
    }
}
