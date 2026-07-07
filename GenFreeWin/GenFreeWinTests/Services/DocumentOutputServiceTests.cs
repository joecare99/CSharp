using Gen_FreeWin.Services;
using Gen_FreeWin.Services.Models;
using GenFree.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Text;

namespace Gen_FreeWin.Tests.Services
{
    [TestClass]
    public class DocumentOutputServiceTests
    {
        [TestMethod]
        public void TrimEnd_WithTrailingWhitespace_RemovesTrailingWhitespace()
        {
            var service = new DocumentOutputService();
            var document = new TestDocument("Alpha   ");

            var result = service.TrimEnd(document);

            Assert.IsTrue(result);
            Assert.AreEqual("Alpha", document.Text);
        }

        [TestMethod]
        public void Retweg3_WithTrailingWhitespaceAndLineBreaks_RemovesTrailingTerminators()
        {
            var service = new DocumentOutputService();
            var document = new TestDocument("Alpha  \r\n\r\n");

            service.Retweg3(document);

            Assert.AreEqual("Alpha", document.Text);
        }

        [TestMethod]
        public void RenderHeading_WithDefinition_RendersLeadingNewlineAndHeadingText()
        {
            var service = new DocumentOutputService();
            var document = new TestDocument("Alpha  ");
            var heading = new NamenSuchHeadingDefinition
            {
                Text = "Überschrift: ",
                LeadingNewlineCount = 2,
                TrimDocumentEndFirst = true
            };

            service.RenderHeading(document, heading);

            Assert.AreEqual("Alpha\n\nÜberschrift: ", document.Text);
            Assert.IsNotNull(document.LastFont);
            Assert.AreEqual(FontStyle.Regular, document.LastFont.Style);
        }

        [TestMethod]
        public void RenderHeidatEventPrefix_WithIndentedDefinition_RendersPrefixAndIndent()
        {
            var service = new DocumentOutputService();
            var document = new TestDocument("Start");
            var eventDefinition = new NamenSuchHeidatEventDefinition
            {
                PrefixText = "Heirat",
                LeadingNewline = true,
                EnsureIndent = true,
                IndentValue = 20
            };

            service.RenderHeidatEventPrefix(document, eventDefinition);

            Assert.AreEqual("Start \nHeirat ", document.Text);
            Assert.AreEqual(20, document.Indent);
        }

        [TestMethod]
        public void RenderHeidatEventPrefix_WithEmptyPrefix_DoesNothing()
        {
            var service = new DocumentOutputService();
            var document = new TestDocument("Start");
            var eventDefinition = new NamenSuchHeidatEventDefinition
            {
                PrefixText = ""
            };

            service.RenderHeidatEventPrefix(document, eventDefinition);

            Assert.AreEqual("Start", document.Text);
        }

        private sealed class TestDocument : IDocument
        {
            private readonly StringBuilder _buffer;
            private int _indent;

            public TestDocument(string initialText = "")
            {
                _buffer = new StringBuilder(initialText);
            }

            public string Text => _buffer.ToString();

            public Font LastFont { get; private set; }

            public int Indent => _indent;

            public bool IsEmpty => _buffer.Length == 0;

            public void AppendImage(Image image)
            {
                throw new NotSupportedException();
            }

            public void AppendText(string text)
            {
                _buffer.Append(text);
            }

            public bool AppendTextIfNd(string sText = "\n", int iCnt = 1)
            {
                if (string.IsNullOrEmpty(sText) || iCnt <= 0)
                {
                    return false;
                }

                for (var i = 0; i < iCnt; i++)
                {
                    _buffer.Append(sText);
                }

                return true;
            }

            public void ClearDocument()
            {
                _buffer.Clear();
            }

            public int GetIndent()
            {
                return _indent;
            }

            public void SetAlignment<T>(T eTextAlign) where T : Enum
            {
            }

            public void SetFont(Font font)
            {
                LastFont = font;
            }

            public void SetIndent(int iIndent)
            {
                _indent = iIndent;
            }

            public void SetHangingIndent(int iHIndent)
            {
            }

            public bool TrimEnd()
            {
                var originalLength = _buffer.Length;
                while (_buffer.Length > 0 && char.IsWhiteSpace(_buffer[_buffer.Length - 1]))
                {
                    _buffer.Length--;
                }

                return _buffer.Length != originalLength;
            }

            public bool TrimEnd(string sText)
            {
                if (string.IsNullOrEmpty(sText) || _buffer.Length < sText.Length)
                {
                    return false;
                }

                var currentText = _buffer.ToString();
                if (!currentText.EndsWith(sText, StringComparison.Ordinal))
                {
                    return false;
                }

                _buffer.Length -= sText.Length;
                return true;
            }

            public void ReplaceLast(string v1, string v2)
            {
                throw new NotSupportedException();
            }
        }
    }
}
