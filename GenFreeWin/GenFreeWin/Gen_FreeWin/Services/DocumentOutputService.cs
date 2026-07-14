using GenFreeWin.Services.Interfaces;
using GenFreeWin.Services.Models;
using GenFree.Interfaces;
using System.Drawing;

namespace GenFreeWin.Services
{
    /// <summary>
    /// Implements document output helper logic formerly hosted by the ViewModel.
    /// </summary>
    public sealed class DocumentOutputService : IDocumentOutputService
    {
        /// <inheritdoc />
        public void Retweg3(IDocument document)
        {
            while (!document.IsEmpty)
            {
                if (TrimEnd(document))
                {
                    continue;
                }

                if (document.TrimEnd("\n"))
                {
                    continue;
                }

                if (document.TrimEnd("\r"))
                {
                    continue;
                }

                break;
            }
        }

        /// <inheritdoc />
        public bool TrimEnd(IDocument document)
        {
            return document.TrimEnd();
        }

        /// <inheritdoc />
        public void RenderHeading(IDocument document, NamenSuchHeadingDefinition headingDefinition)
        {
            if (document == null || headingDefinition == null || string.IsNullOrEmpty(headingDefinition.Text))
            {
                return;
            }

            if (headingDefinition.TrimDocumentEndFirst)
            {
                Retweg3(document);
            }

            if (headingDefinition.LeadingNewlineCount > 0)
            {
                document.AppendTextIfNd("\n", headingDefinition.LeadingNewlineCount);
            }

            document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
            document.AppendText(headingDefinition.Text);
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
        }

        /// <inheritdoc />
        public void RenderHeidatEventPrefix(IDocument document, NamenSuchHeidatEventDefinition eventDefinition)
        {
            if (document == null || eventDefinition == null || string.IsNullOrWhiteSpace(eventDefinition.PrefixText))
            {
                return;
            }

            document.AppendTextIfNd(" ");
            if (eventDefinition.LeadingNewline)
            {
                document.AppendText("\n");
            }

            if (eventDefinition.EnsureIndent && document.GetIndent() == 0)
            {
                document.SetIndent(eventDefinition.IndentValue);
            }

            document.AppendText(eventDefinition.PrefixText + " ");
        }
    }
}
