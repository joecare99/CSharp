using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WinAhnenNew.Messages
{
    /// <summary>
    /// Signals that the current genealogy content was replaced or regenerated.
    /// </summary>
    public sealed class GenealogyChangedMessage : ValueChangedMessage<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenealogyChangedMessage"/> class.
        /// </summary>
        /// <param name="iPersonCount">The number of generated persons.</param>
        public GenealogyChangedMessage(int iPersonCount)
            : base(iPersonCount)
        {
        }
    }
}
