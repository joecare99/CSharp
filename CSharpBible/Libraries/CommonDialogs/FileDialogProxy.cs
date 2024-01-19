using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonDialogs.Interfaces;

namespace CommonDialogs
{
    public class FileDialogProxy<T> : IFileDialog where T : FileDialog
    {
        private T _fileDialog;
        public FileDialogProxy(T fileDialog)
        {
            _fileDialog = fileDialog;
        }

        public T This => _fileDialog;

        public string FileName
        {
            get => _fileDialog.FileName;
            set => _fileDialog.FileName = value;
        }

        public bool? ShowDialog()
        {

            return _fileDialog.ShowDialog() == DialogResult.OK;
        }

        public bool? ShowDialog(object owner)
        {
            return _fileDialog.ShowDialog(owner as IWin32Window) == DialogResult.OK;
        }

    }
}