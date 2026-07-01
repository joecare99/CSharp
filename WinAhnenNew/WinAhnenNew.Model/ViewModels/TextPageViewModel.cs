using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the text tab page.
    /// </summary>
    public sealed partial class TextPageViewModel : PersonHeaderViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public TextPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
            : base(personSelectionService, messenger)
        {
        }
    }
}
