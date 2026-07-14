using GenFreeWin.Services.Interfaces;
using GenFreeWin.Services.Models;
using GenFree.Data;

namespace GenFreeWin.Services
{
    /// <summary>
    /// Creates NamenSuch document structure elements independently from concrete document output APIs.
    /// </summary>
    public sealed class NamenSuchDocumentComposer : INamenSuchDocumentComposer
    {
        /// <inheritdoc />
        public NamenSuchHeadingDefinition ComposeBerufeHeading(
            EEventArt eventArt,
            int eventCount,
            string occupationText,
            string titleText,
            string text70,
            string text444,
            string text445)
        {
            switch (eventArt)
            {
                case EEventArt.eA_300: // Occupation of the Person
                    return new NamenSuchHeadingDefinition
                    {
                        LeadingNewlineCount = 1,
                        Text = eventCount == 1
                            ? occupationText + ": "
                            : titleText + " "
                    };

                case EEventArt.eA_301: //(Akademic) Title of the Person
                    return eventCount >= 1
                        ? new NamenSuchHeadingDefinition
                        {
                            LeadingNewlineCount = 1,
                            Text = text70 + " "
                        }
                        : null;

                case EEventArt.eA_302: // Residence of the Person
                    return new NamenSuchHeadingDefinition
                    {
                        LeadingNewlineCount = 1,
                        Text = eventCount == 1
                            ? text444 + " "
                            : text445 + " "
                    };

                case EEventArt.eA_602: // Residence of the Family
                    return new NamenSuchHeadingDefinition
                    {
                        LeadingNewlineCount = 2,
                        Text = eventCount == 1
                            ? "Wohnung der Familie: "
                            : "Wohnungen der Familie: ",
                        TrimDocumentEndFirst = true,
                        ResetContextUbgT1 = true
                    };

                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public NamenSuchHeidatEventDefinition ComposeHeidatEvent(
            EEventArt eventArt,
            bool emitIndentedOutput,
            string eventText)
        {
            return new NamenSuchHeidatEventDefinition
            {
                PrefixText = eventText ?? string.Empty,
                LeadingNewline = emitIndentedOutput,
                EnsureIndent = emitIndentedOutput,
                IndentValue = 20,
                IsDivorceEvent = eventArt == EEventArt.eA_504,
                WitnessLabel = eventArt == EEventArt.eA_Marriage || eventArt == EEventArt.eA_MarrReligious
                    ? "Trauzeugen"
                    : "Zeugen"
            };
        }
    }
}
