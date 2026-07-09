using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the siblings tab page.
    /// </summary>
    public sealed partial class SiblingsPageViewModel : PersonHeaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiblingsPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public SiblingsPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
            : base(personSelectionService, messenger)
        {
        }
    }
}
