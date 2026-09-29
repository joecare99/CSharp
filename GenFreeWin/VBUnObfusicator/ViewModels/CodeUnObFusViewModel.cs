using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using VBUnObfusicator.Properties;
#if NET8_0_OR_GREATER
using TranspilerLib.CSharp.StatEqualCheck;
using TranspilerLib.CSharp.VBLegacyReplace;
using TranspilerLib.CSharp.VBLegacyReplace.Models;
#endif

namespace VBUnObfusicator.ViewModels
{
    public partial class CodeUnObFusViewModel : BaseViewModelCT
    {
#if NET8_0_OR_GREATER
        private readonly Func<ICodeBlock, ICodeBlock, System.Collections.Generic.IEnumerable<ICodeBlock>?, System.Collections.Generic.IEnumerable<ICodeBlock>?, StatEqualResult> _comparer;
        private readonly LegacyReplacementEngine _defaultLegacyEngine;
#endif
        private readonly ICSCode _codeEngine;

        public CodeUnObFusViewModel()
#if NET8_0_OR_GREATER
            : this(CodeEng, StatEqualCheck.Compare, LegacyReplacementEngine.LoadDefaultRules())
#else
            : this(CodeEng)
#endif
        {
        }

#if NET8_0_OR_GREATER
        public CodeUnObFusViewModel(ICSCode codeEngine,
            Func<ICodeBlock, ICodeBlock, System.Collections.Generic.IEnumerable<ICodeBlock>?, System.Collections.Generic.IEnumerable<ICodeBlock>?, StatEqualResult> comparer,
            LegacyReplacementEngine defaultLegacyEngine)
        {
            _codeEngine = codeEngine ?? throw new ArgumentNullException(nameof(codeEngine));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _defaultLegacyEngine = defaultLegacyEngine ?? throw new ArgumentNullException(nameof(defaultLegacyEngine));
        }
#else
        public CodeUnObFusViewModel(ICSCode codeEngine)
        {
            _codeEngine = codeEngine ?? throw new ArgumentNullException(nameof(codeEngine));
        }
#endif

        [ObservableProperty]
        private string _code = string.Empty;

        [ObservableProperty]
        private string _result = string.Empty;

        [ObservableProperty]
        private string _comparisonOutput = string.Empty;

        [ObservableProperty]
        private string _result2 = string.Empty;

        [ObservableProperty]
        private bool _reorder = true;

        [ObservableProperty]
        private bool _removeLbl = true;

        [ObservableProperty]
        private bool _doWhile = true;

        [ObservableProperty]
        private bool _checkEquivalence;

        [ObservableProperty]
        private bool _replaceVbLegacy;

        [ObservableProperty]
        private string _legacyRulesPath = string.Empty;

        [ObservableProperty]
        private string _analysisSummary = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private CodeAnalysisFinding? _selectedFinding;

        [ObservableProperty]
        private int _selectedOutputTabIndex;

        public ObservableCollection<CodeAnalysisFinding> Findings { get; } = new();
        public string CheckEquivalenceLabel => Resource.CheckEquivalenceLabel;
        public string ReplaceVbLegacyLabel => Resource.ReplaceVbLegacyLabel;
        public string LegacyRulesLabel => Resource.LegacyRulesLabel;
        public string FindingsLabel => Resource.FindingsLabel;
        public string OptimizedOutputLabel => Resource.OptimizedOutputLabel;
        public string FinalOutputLabel => Resource.FinalOutputLabel;

        /// <summary>Executes the code-parsing and optimization.</summary>
        [RelayCommand()]
#pragma warning disable IDE1006 // Benennungsstile
        private void Execute()
#pragma warning restore IDE1006 // Benennungsstile
        {
            try
            {
                ErrorMessage = string.Empty;
                AnalysisSummary = string.Empty;
                SelectedFinding = null;
                Findings.Clear();
                // Check if code is empty
                if (string.IsNullOrEmpty(Code))
                {
                    Result = PlsEnterCode; //"Please enter code";
                    ComparisonOutput = Result;
                    SelectedOutputTabIndex = 0;
                }
                else
                {
                    var optimized = DoExecute(Code, Reorder, RemoveLbl, DoWhile);
                    ComparisonOutput = optimized;
                    if (CheckEquivalence)
                        CheckCodeEquivalence(Code, optimized);

                    Result = ReplaceVbLegacy ? ReplaceLegacy(optimized) : optimized;
                    SelectedOutputTabIndex = ReplaceVbLegacy ? 1 : 0;
                    Result2 = string.Format(Resource.Result2, Code.Length, Code.Count((c) => c == Environment.NewLine[0]),
                        Result.Length, Result.Count((c) => c == Environment.NewLine[0]));
                }
            }
            catch (Exception ex)
            {
                Result2 = ex.Message;
                ErrorMessage = ex.Message;
                ComparisonOutput = string.Empty;
                Result = "";
                SelectedOutputTabIndex = 0;
            }
        }

#if NET8_0_OR_GREATER
        private void CheckCodeEquivalence(string original, string optimized)
        {
            var originalBlock = new CodeBlock { Name = "Original", Type = CodeBlockType.MainBlock, Code = original };
            var candidateBlock = new CodeBlock { Name = "Candidate", Type = CodeBlockType.MainBlock, Code = optimized };
            var comparison = _comparer(originalBlock, candidateBlock, null, null);

            AnalysisSummary = string.Format(Resource.EquivalenceSummary, comparison.Status);
            foreach (var finding in comparison.Findings)
            {
                Findings.Add(new CodeAnalysisFinding(
                    finding.Severity.ToString(),
                    finding.Code,
                    finding.Message,
                    finding.LeftSpan?.Start,
                    finding.LeftSpan?.Length,
                    finding.RightSpan?.Start,
                    finding.RightSpan?.Length));
            }
        }

        private string ReplaceLegacy(string source)
        {
            var replacementEngine = string.IsNullOrWhiteSpace(LegacyRulesPath)
                ? _defaultLegacyEngine
                : LegacyReplacementEngine.LoadFile(LegacyRulesPath);
            var replacement = replacementEngine.Apply(source);

            var issues = replacement.Diagnostics
                .Where(diagnostic => diagnostic.Severity != RuleDiagnosticSeverity.Information)
                .ToArray();
            if (issues.Length > 0)
                ErrorMessage = string.Join(Environment.NewLine, issues.Select(diagnostic => $"Legacy replacement [{diagnostic.Severity}] {diagnostic.Kind}: {diagnostic.Message}"));

            return AddRequiredUsings(replacement.Source, replacement.RequiredUsings);
        }

        private static string AddRequiredUsings(string source, System.Collections.Generic.IReadOnlyList<string> requiredUsings)
        {
            var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var missingUsings = requiredUsings
                .Where(requiredUsing => !lines.Any(line => string.Equals(line.Trim(), $"using {requiredUsing};", StringComparison.Ordinal)))
                .Select(requiredUsing => $"using {requiredUsing};")
                .ToArray();
            if (missingUsings.Length == 0)
                return source;

            var documentationCount = lines.TakeWhile(line => line.TrimStart().StartsWith("///", StringComparison.Ordinal)).Count();
            var documentation = string.Join(Environment.NewLine, lines.Take(documentationCount));
            var code = string.Join(Environment.NewLine, lines.Skip(documentationCount)).TrimStart('\r', '\n');
            var directives = string.Join(Environment.NewLine, missingUsings);
            return string.IsNullOrEmpty(documentation)
                ? $"{directives}{Environment.NewLine}{code}"
                : $"{documentation}{Environment.NewLine}{directives}{Environment.NewLine}{code}";
        }
#else
        private void CheckCodeEquivalence(string original, string optimized)
            => throw new NotSupportedException("Equivalence checking requires .NET 8 or later.");

        private string ReplaceLegacy(string source)
            => throw new NotSupportedException("Visual Basic legacy replacements require .NET 8 or later.");
#endif

        private string DoExecute(string code, bool reorder, bool removeLbl, bool doWhile)
        {
            ICSCode codeEng = _codeEngine;

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

