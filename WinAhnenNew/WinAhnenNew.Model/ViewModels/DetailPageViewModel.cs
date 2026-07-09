using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the detail tab page.
    /// </summary>
    public sealed partial class DetailPageViewModel : PersonHeaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public DetailPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
            : base(personSelectionService, messenger)
        {
        }
    }
}
