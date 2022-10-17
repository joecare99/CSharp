using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDialogs
{
    public class TaskDialog : Component
    {
        private Microsoft.WindowsAPICodePack.Dialogs.TaskDialog _td = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialog();

        //
        // Zusammenfassung:
        //     Creates a basic TaskDialog window
        public TaskDialog()
        {

        }

        //
        // Zusammenfassung:
        //     TaskDialog Finalizer
        ~TaskDialog()
        {
            _td.Dispose();
        }

        //
        // Zusammenfassung:
        //     Indicates whether this feature is supported on the current platform.
        public static bool IsPlatformSupported { get => Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.IsPlatformSupported; }
        /*
        // Zusammenfassung:
        //     Gets or sets the progress bar on the taskdialog. ProgressBar a visual representation
        //     of the progress of a long running operation.
        public TaskDialogProgressBar ProgressBar { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the startup location.
        public TaskDialogStartupLocation StartupLocation { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the expansion mode for this dialog.
        public TaskDialogExpandedDetailsLocation ExpansionMode { get; set; }
        /*/
        // Zusammenfassung:
        //     Gets or sets a value that indicates if the footer checkbox is checked.
        public bool? FooterCheckBoxChecked { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that determines if hyperlinks are enabled.
        public bool HyperlinksEnabled { get; set; }
        /*
        // Zusammenfassung:
        //     Gets a value that contains the TaskDialog controls.
        public DialogControlCollection<TaskDialogControl> Controls { get; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the standard buttons.
        public TaskDialogStandardButtons StandardButtons { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the footer icon.
        public TaskDialogStandardIcon FooterIcon { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the TaskDialog main icon.
        public TaskDialogStandardIcon Icon { get; set; }
        /*/
        // Zusammenfassung:
        //     Gets or sets a value that determines if Cancelable is set.
        public bool Cancelable { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the collapsed control text.
        public string DetailsCollapsedLabel { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the expanded control text.
        public string DetailsExpandedLabel { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the expanded text in the details section.
        public string DetailsExpandedText { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that determines if the details section is expanded.
        public bool DetailsExpanded { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the footer text.
        public string FooterText { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the caption text.
        public string Caption { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the instruction text.
        public string InstructionText { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the message text.
        public string Text { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the owner window's handle.
        public IntPtr OwnerWindowHandle { get; set; }
        //
        // Zusammenfassung:
        //     Gets or sets a value that contains the footer check box text.
        public string FooterCheckBoxText { get; set; }


        //
        // Zusammenfassung:
        //     Occurs when a user clicks on Help.
        public event EventHandler HelpInvoked { add => _td.HelpInvoked += value; remove => _td.HelpInvoked -= value; }
        //
        // Zusammenfassung:
        //     Occurs when the TaskDialog is closing.
        public event EventHandler<Microsoft.WindowsAPICodePack.Dialogs.TaskDialogClosingEventArgs> Closing;
        //
        // Zusammenfassung:
        //     Occurs when a user clicks a hyperlink.
        public event EventHandler<Microsoft.WindowsAPICodePack.Dialogs.TaskDialogHyperlinkClickedEventArgs> HyperlinkClick;
        //
        // Zusammenfassung:
        //     Occurs when a progress bar changes.
        public event EventHandler<Microsoft.WindowsAPICodePack.Dialogs.TaskDialogTickEventArgs> Tick;
        //
        // Zusammenfassung:
        //     Occurs when the TaskDialog is opened.
        public event EventHandler Opened { add=> _td.Opened+=value; remove => _td.Opened -= value; }

        //
        // Zusammenfassung:
        //     Creates and shows a task dialog with the specified supporting text, main instruction,
        //     and dialog caption.
        //
        // Parameter:
        //   text:
        //     The supporting text to display.
        //
        //   instructionText:
        //     The main instruction text to display.
        //
        //   caption:
        //     The caption for the dialog.
        //
        // Rückgabewerte:
        //     The dialog result.
        public static bool Show(string text, string instructionText, string caption)
            => Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.Show(text,instructionText,caption) == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok;
        //
        // Zusammenfassung:
        //     Creates and shows a task dialog with the specified supporting text and main instruction.
        //
        // Parameter:
        //   text:
        //     The supporting text to display.
        //
        //   instructionText:
        //     The main instruction text to display.
        //
        // Rückgabewerte:
        //     The dialog result.
        public static bool Show(string text, string instructionText)
            => Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.Show(text,instructionText)==Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok;
        //
        // Zusammenfassung:
        //     Creates and shows a task dialog with the specified message text.
        //
        // Parameter:
        //   text:
        //     The text to display.
        //
        // Rückgabewerte:
        //     The dialog result.
        public static bool Show(string text)
            => Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.Show(text)==Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok;
        //
        // Zusammenfassung:
        //     Close TaskDialog with a given TaskDialogResult
        //
        // Parameter:
        //   closingResult:
        //     TaskDialogResult to return from the TaskDialog.Show() method
        //
        // Ausnahmen:
        //   T:System.InvalidOperationException:
        //     if TaskDialog is not showing.
        public void Close(bool result)=>_td.Close(result ? Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok:
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Cancel);
        //
        // Zusammenfassung:
        //     Close TaskDialog
        //
        // Ausnahmen:
        //   T:System.InvalidOperationException:
        //     if TaskDialog is not showing.
        public void Close()=>_td.Close();
        //
        // Zusammenfassung:
        //     Dispose TaskDialog Resources
#if NET50_OR_GREATER
        public override void Dispose()=>_td.Dispose();
#else
        public new void Dispose()=>_td.Dispose();
#endif
        //
        // Zusammenfassung:
        //     Dispose TaskDialog Resources
        //
        // Parameter:
        //   disposing:
        //     If true, indicates that this is being called via Dispose rather than via the
        //     finalizer.
#if NET50_OR_GREATER
        public override void Dispose(bool disposing) => _td.Dispose(disposing);
#else
        public new void Dispose(bool disposing) => _td.Dispose(disposing);
#endif
        //
        // Zusammenfassung:
        //     Creates and shows a task dialog.
        //
        // Rückgabewerte:
        //     The dialog result.
        public bool? Show()=> _td.Show()==Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok;

    }
}
