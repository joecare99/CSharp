using System.Threading;

namespace Leonardo.ST.Cxaml;

/// <summary>Hosts Leonardo's native modal prompts outside declarative CXAML markup.</summary>
internal sealed class LeonardoInteractionAdapter
{
    public string RequestInput(string prompt)
        => RunInSta(() =>
        {
            using Form dialog = new()
            {
                Text = "Leonardo",
                StartPosition = FormStartPosition.CenterParent,
                Width = 420,
                Height = 150,
            };
            Label label = new() { Left = 12, Top = 12, Width = 380, Text = prompt };
            TextBox input = new() { Left = 12, Top = 38, Width = 380 };
            Button confirm = new() { Text = "OK", Left = 236, Top = 72, DialogResult = DialogResult.OK };
            Button cancel = new() { Text = "Cancel", Left = 317, Top = 72, DialogResult = DialogResult.Cancel };
            dialog.AcceptButton = confirm;
            dialog.CancelButton = cancel;
            dialog.Controls.AddRange([label, input, confirm, cancel]);
            return dialog.ShowDialog() == DialogResult.OK ? input.Text : string.Empty;
        });

    public void ShowMessage(string message)
        => RunInSta(() =>
        {
            MessageBox.Show(message, "Leonardo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        });

    private static T RunInSta<T>(Func<T> action)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            return action();
        }

        T? result = default;
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                result = action();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null)
        {
            throw new InvalidOperationException("The Leonardo dialog could not be displayed.", failure);
        }

        return result is not null
            ? result
            : throw new InvalidOperationException("The Leonardo dialog did not return a result.");
    }
}
