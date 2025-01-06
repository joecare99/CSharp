using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.View.Extension;
using MVVM.ViewModel;
using System;
using System.Linq;
using VBUnObfusicator.Interfaces.Code;
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
        private string _result2 = string.Empty;

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
            try
            {
                // Check if code is empty
                if (string.IsNullOrEmpty(Code))
                    Result = PlsEnterCode; //"Please enter code";                
                else
                {
                    Result = DoExecute(Code, Reorder, RemoveLbl, DoWhile);
                    Result2 = string.Format(Resource.Result2, Code.Length, Code.Count((c)=>c==Environment.NewLine[0]),
                        Result.Length, Result.Count((c) => c == Environment.NewLine[0]));
                }
            }
            catch (Exception ex)
            {
                Result2 = ex.Message;
                Result = "";
            }
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
