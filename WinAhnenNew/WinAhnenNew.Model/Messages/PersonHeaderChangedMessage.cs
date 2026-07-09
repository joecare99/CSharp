using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WinAhnenNew.Messages
{
    /// <summary>
    /// Signals that header-relevant data of the selected person has changed.
    /// </summary>
    public sealed class PersonHeaderChangedMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonHeaderChangedMessage"/> class.
        /// </summary>
        public PersonHeaderChangedMessage()
            : base(true)
        {
        }
    }
}
