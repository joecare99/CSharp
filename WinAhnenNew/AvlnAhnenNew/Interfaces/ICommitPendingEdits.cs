namespace AvlnAhnenNew.Interfaces
{
    /// <summary>
    /// Defines a view that can flush pending edit bindings before navigation changes.
    /// </summary>
    public interface ICommitPendingEdits
    {
        /// <summary>
        /// Commits pending edits to the bound view model.
        /// </summary>
        void CommitPendingEdits();
    }
}
