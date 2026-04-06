using System;
using BaseGenClasses.Model;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using GenSecure.Contracts;

namespace WinAhnenNew.Services
{
    /// <summary>
    /// Creates `BaseGenClasses` model instances for `GenSecure` genealogy loading.
    /// </summary>
    public sealed class BaseGenGenealogyModelFactory : IGenealogyModelFactory
    {
        private readonly IMessenger _messenger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGenGenealogyModelFactory"/> class.
        /// </summary>
        /// <param name="messenger">The shared application messenger.</param>
        public BaseGenGenealogyModelFactory(IMessenger messenger)
        {
            _messenger = messenger;
        }

        /// <inheritdoc />
        public IGenealogy CreateGenealogy(Guid gUid)
            => new Genealogy(_messenger) { UId = gUid };

        /// <inheritdoc />
        public IGenPerson CreatePerson(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenPerson { UId = gUid, ID = iId };

        /// <inheritdoc />
        public IGenFamily CreateFamily(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenFamily { UId = gUid, ID = iId };

        /// <inheritdoc />
        public IGenPlace CreatePlace(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenPlace(Array.Empty<string>()) { UId = gUid, ID = iId };

        /// <inheritdoc />
        public IGenFact CreateFact(IGenEntity genOwner, Guid gUid, int iId, DateTime? dtLastChange, EFactType eFactType)
            => new GenFact(genOwner, eFactType) { UId = gUid, ID = iId };

        /// <inheritdoc />
        public IGenConnects CreateConnection(IGenEntity? genConnectedEntity, Guid gUid, EGenConnectionType eConnectionType)
            => new GenConnect { UId = gUid, eGenConnectionType = eConnectionType, Entity = genConnectedEntity };

        /// <inheritdoc />
        public IGenDate CreateDate(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenDate { UId = gUid, ID = iId };
    }
}
