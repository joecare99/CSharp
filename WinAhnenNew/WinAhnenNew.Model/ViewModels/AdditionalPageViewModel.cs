using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the additional tab page.
    /// </summary>
    public sealed partial class AdditionalPageViewModel : PersonHeaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public AdditionalPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
            : base(personSelectionService, messenger)
        {
        }
    }
}
