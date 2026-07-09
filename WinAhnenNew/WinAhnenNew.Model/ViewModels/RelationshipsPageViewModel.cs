using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the relationships tab page.
    /// </summary>
    public sealed partial class RelationshipsPageViewModel : PersonHeaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipsPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public RelationshipsPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
            : base(personSelectionService, messenger)
        {
        }
    }
}
