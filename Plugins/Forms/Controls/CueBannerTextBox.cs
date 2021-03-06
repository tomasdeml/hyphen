﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    /// <summary>
    /// Draws a textbox with a prompt inside of it, similar to the "Quick Search" box 
    /// in Outlook 2007, IE7 or the Firefox 2.0 search box. The prompt will disappear when
    /// the focus is placed in the textbox, and will not display again if the Text property
    /// contains any value. If the Text property is empty, then the prompt will display
    /// again when the textbox loses the focus.
    /// </summary>
    public class CueBannerTextBox : System.Windows.Forms.TextBox
    {
        // Windows message constants
        const int WM_SETFOCUS = 7;
        const int WM_KILLFOCUS = 8;
        const int WM_ERASEBKGND = 14;
        const int WM_PAINT = 15;

        // private internal variables
        private bool _focusSelect = true;
        private bool _drawPrompt = true;
        private string _bannerText = String.Empty;
        private Color _bannerColor = SystemColors.GrayText;
        private Font _bannerFont = null;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <remarks>Uncomment the SetStyle line to activate the OnPaint logic in place of the WndProc logic</remarks>
        public CueBannerTextBox()
        {
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.BannerFont = this.Font;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Appearance")]
        [Description("The prompt text to display when there is nothing in the Text property.")]
        public string BannerText
        {
            get { return _bannerText; }
            set { if (value == null) value = String.Empty; _bannerText = value.Trim(); this.Invalidate(); }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Appearance")]
        [Description("The ForeColor to use when displaying the PromptText.")]
        public Color BannerForeColor
        {
            get { return _bannerColor; }
            set { _bannerColor = value; this.Invalidate(); }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Appearance")]
        [Description("The Font to use when displaying the PromptText.")]
        public Font BannerFont
        {
            get { return _bannerFont; }
            set { _bannerFont = value; this.Invalidate(); }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Behavior")]
        [Description("Automatically select the text when control receives the focus.")]
        public bool FocusSelect
        {
            get { return _focusSelect; }
            set { _focusSelect = value; }
        }

        /// <summary>
        /// When the textbox receives an OnEnter event, select all the text if any text is present
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            if (this.Text.Length > 0 && _focusSelect)
                this.SelectAll();

            base.OnEnter(e);
        }

        /// <summary>
        /// Redraw the control when the text alignment changes
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Redraw the control with the prompt
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>This event will only fire if ControlStyles.UserPaint is set to true in the constructor</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Only draw the prompt in the OnPaint event and when the Text property is empty
            if (_drawPrompt && this.Text.Length == 0)
                DrawTextPrompt(e.Graphics);
        }

        /// <summary>
        /// Overrides the default WndProc for the control
        /// </summary>
        /// <param name="m">The Windows message structure</param>
        /// <remarks>
        /// This technique is necessary because the OnPaint event seems to be doing some
        /// extra processing that I haven't been able to figure out.
        /// </remarks>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_SETFOCUS:
                    _drawPrompt = false;
                    break;

                case WM_KILLFOCUS:
                    _drawPrompt = true;
                    break;
            }

            base.WndProc(ref m);

            // Only draw the prompt on the WM_PAINT event and when the Text property is empty
            if (m.Msg == WM_PAINT && _drawPrompt && this.Text.Length == 0 && !this.GetStyle(ControlStyles.UserPaint))
                DrawTextPrompt();
        }

        /// <summary>
        /// Overload to automatically create the Graphics region before drawing the text prompt
        /// </summary>
        /// <remarks>The Graphics region is disposed after drawing the prompt.</remarks>
        protected virtual void DrawTextPrompt()
        {
            using (Graphics g = this.CreateGraphics())
            {
                DrawTextPrompt(g);
            }
        }

        /// <summary>
        /// Draws the PromptText in the TextBox.ClientRectangle using the PromptFont and PromptForeColor
        /// </summary>
        /// <param name="g">The Graphics region to draw the prompt on</param>
        protected virtual void DrawTextPrompt(Graphics g)
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top | TextFormatFlags.EndEllipsis;
            Rectangle rect = this.ClientRectangle;

            // Offset the rectangle based on the HorizontalAlignment, 
            // otherwise the display looks a little strange
            switch (this.TextAlign)
            {
                case HorizontalAlignment.Center:
                    flags = flags | TextFormatFlags.HorizontalCenter;
                    rect.Offset(0, 1);
                    break;
                case HorizontalAlignment.Left:
                    flags = flags | TextFormatFlags.Left;
                    rect.Offset(1, 1);
                    break;
                case HorizontalAlignment.Right:
                    flags = flags | TextFormatFlags.Right;
                    rect.Offset(0, 1);
                    break;
            }

            // Draw the prompt text using TextRenderer
            TextRenderer.DrawText(g, _bannerText, _bannerFont, rect, _bannerColor, this.BackColor, flags);
        }
    }
}
