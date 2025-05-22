
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CalliAPI.Utilities
{
    /// <summary>
    /// Manages keyword highlighting and tooltip display in a RichTextBox.
    /// </summary>
    public class KeywordTooltipManager
    {
        private readonly RichTextBox _richTextBox;
        private readonly ToolTip _toolTip;
        private readonly Dictionary<string, string> _keywords;
        private readonly Dictionary<(int start, int length), string> _keywordRanges = new();

        public KeywordTooltipManager(RichTextBox richTextBox, ToolTip toolTip, Dictionary<string, string> keywords)
        {
            _richTextBox = richTextBox;
            _toolTip = toolTip;
            _keywords = keywords;

            _richTextBox.MouseMove += RichTextBox_MouseMove;
        }

        /// <summary>
        /// Call this after setting the RichTextBox text to highlight keywords.
        /// </summary>
        public void HighlightKeywords()
        {
            _keywordRanges.Clear();

            foreach (var kvp in _keywords)
            {
                string keyword = kvp.Key;
                string pattern = $@"\b{Regex.Escape(keyword)}\b";
                foreach (Match match in Regex.Matches(_richTextBox.Text, pattern, RegexOptions.IgnoreCase))
                {
                    _richTextBox.Select(match.Index, match.Length);
                    _richTextBox.SelectionColor = Color.Blue;
                    _richTextBox.SelectionFont = new Font(_richTextBox.Font, FontStyle.Underline);
                    _keywordRanges[(match.Index, match.Length)] = kvp.Value;
                }
            }

            _richTextBox.Select(0, 0); // Reset selection
        }

        private void RichTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            int charIndex = _richTextBox.GetCharIndexFromPosition(e.Location);

            foreach (var range in _keywordRanges)
            {
                int start = range.Key.start;
                int length = range.Key.length;

                if (charIndex >= start && charIndex < start + length)
                {
                    _toolTip.SetToolTip(_richTextBox, range.Value);
                    return;
                }
            }

            _toolTip.SetToolTip(_richTextBox, string.Empty); // Clear tooltip if not on keyword
        }
    }
}
