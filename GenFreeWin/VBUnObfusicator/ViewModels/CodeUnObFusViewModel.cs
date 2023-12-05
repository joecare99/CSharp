using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.View.Extension;
using MVVM.ViewModel;
using VBUnObfusicator.Models;
using VBUnObfusicator.Properties;

namespace VBUnObfusicator.ViewModels
{
    public partial class CodeUnObFusViewModel : BaseViewModelCT
    {

        [ObservableProperty]
        private string _code = string.Empty;

        [ObservableProperty]
        private string _result = string.Empty;

        [ObservableProperty]
        private bool _reorder = true;

        [ObservableProperty]
        private bool _removeLbl = true;

        [ObservableProperty]
        private bool _doWhile = true;

        /// <summary>Executes the code-parsing and optimization.</summary>
        [RelayCommand()]
#pragma warning disable IDE1006 // Benennungsstile
        private void Execute()
#pragma warning restore IDE1006 // Benennungsstile
        {
            // Check if code is empty
            if (string.IsNullOrEmpty(Code))
                Result = PlsEnterCode; //"Please enter code";                
            else
                Result = DoExecute(Code, Reorder, RemoveLbl, DoWhile);
        }

        private static string DoExecute(string code, bool reorder, bool removeLbl, bool doWhile)
        {
            // Get the code engine
            ICSCode codeEng = CodeEng;

            // Give code to engine
            codeEng.OriginalCode = code;

            // Parse the code
            var cStruct = codeEng.Parse();

            // Reorder labels if wanted
            if (reorder)
                codeEng.ReorderLabels(cStruct);

            
            codeEng.DoWhile = doWhile;
            // Remove single source labels if wanted
            if (removeLbl)
                codeEng.RemoveSingleSourceLabels1(cStruct);

            // Convert the code back to "readable" code
            return codeEng.ToCode(cStruct);
        }

        private static ICSCode CodeEng => IoC.GetRequiredService<ICSCode>();

        private static string PlsEnterCode => Resource.PlsEnterCode;
    }
}
