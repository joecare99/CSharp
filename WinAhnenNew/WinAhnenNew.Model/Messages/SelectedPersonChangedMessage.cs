using CommunityToolkit.Mvvm.Messaging.Messages;
using GenInterfaces.Interfaces.Genealogic;

namespace WinAhnenNew.Messages
{
    /// <summary>
    /// Signals that the current person selected for editing has changed.
    /// </summary>
    public sealed class SelectedPersonChangedMessage : ValueChangedMessage<IGenPerson?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedPersonChangedMessage"/> class.
        /// </summary>
        /// <param name="genSelectedPerson">The selected person, or <see langword="null"/> if no person is selected.</param>
        public SelectedPersonChangedMessage(IGenPerson? genSelectedPerson)
            : base(genSelectedPerson)
        {
        }
    }
}
