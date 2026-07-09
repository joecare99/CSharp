namespace WinAhnenNew.Services
{
    /// <summary>
    /// Provides a platform-specific way to terminate the current application.
    /// </summary>
    public interface IApplicationShutdownService
    {
        /// <summary>
        /// Requests the current application to shut down.
        /// </summary>
        void Shutdown();
    }
}
