//--------------------------------------------------------------------------
// 
//  Copyright (c) Chili Software.  All rights reserved. 
// 
//  File: CueBannerTextBox.cs
//
//  Description: Text box that allows to display a cue banner.
// 
//--------------------------------------------------------------------------

// Idea from: http://www.delphipraxis.net/topic13132_editgetcuebannertext+win+xp.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
	/// <summary>
	/// This class represents a text box that is enhanced to display a little cue banner, if 
	/// no text has been entered. This could be used to inform the user what should be entered
	/// in the text box.
	/// </summary>
	public class CueBannerTextBox : TextBox
	{
		#region NativeMethods

		private const uint ECM_FIRST = 0x1500; 
		private const uint EM_SETCUEBANNER = ECM_FIRST + 1;
		private const uint WM_SETFONT = 0x30;

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, string lParam);

		#endregion

		private string _bannerText;
		private Font _bannerFont;

		/// <summary>
		/// The banner text associated with the control.
		/// </summary>
		[Description("The banner text associated with the control.")]
		[Category("Appearance")]
		public string BannerText
		{
			get { return _bannerText; }
			set
			{
				_bannerText = value;

				// If supported set the value as banner text.
				if (IsSupported)
					SendMessage(this.Handle, EM_SETCUEBANNER, IntPtr.Zero, value);
			}
		}

		/// <summary>
		/// The banner font used to display the banner text in the control.
		/// </summary>
		[Description("The banner font used to display the banner text in the control.")]
		[Category("Appearance")]
		public Font BannerFont
		{
			get 
			{
				if (_bannerFont == null && this.Parent != null)
					return this.Parent.Font;

				return _bannerFont; 
			}
			set { _bannerFont = value; }
		}

		#region Overridden Members

		/// <summary>
		/// Invoked each time the focus is lost.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			// Notify the text box to change the font to the banner font.
			if (this.Text.Length == 0 && IsSupported)
			{
				SendMessage(this.Handle, WM_SETFONT, _bannerFont.ToHfont(), null);
			}
		}

		/// <summary>
		/// Invoked each time the focus is gotten.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			// Notify the text box to change the font back.
			if (IsSupported)
				SendMessage(this.Handle, WM_SETFONT, base.Font.ToHfont(), null);
		}

		#endregion

		/// <summary>
		/// Returns whether the OS supports banner texts. It is fine if the application
		/// runs on XP or higher.
		/// </summary>
		private bool IsSupported
		{
			get 
			{ 
				Version v = Environment.OSVersion.Version;
				return ((v.Major == 5 && v.Minor == 1) || v.Major > 5); 
			}
		}
	}
}
