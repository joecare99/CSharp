using GenFree.Interfaces.VB;
using System;
using System.Windows.Forms;

namespace GenFree;

public class CProjectData : IProjectData
{
    public void ClearProjectError()
    {
        // This method is intended to clear any project error.
        // In a real implementation, you might reset an error state or log.

    }

    public Exception CreateProjectError(int v)
    {
        // This method is intended to create a project error based on an error code.
        // In a real implementation, you might throw an exception or return an error object.
        return new Exception($"Project error with code: {v}");
    }

    public void EndApp()
    {
        // This method is intended to end the application.
        // In a real implementation, you might call Application.Exit() or Environment.Exit(0).
        Application.Exit();
    }

    public void SetProjectError(Exception ex, int erl = 0)
    {
        // This method is intended to set a project error.
        // In a real implementation, you might log the error or display it to the user.
        if (ex != null)
        {
            MessageBox.Show($"Project Error: {ex.Message}\nError Code: {erl}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            MessageBox.Show("An unknown project error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}