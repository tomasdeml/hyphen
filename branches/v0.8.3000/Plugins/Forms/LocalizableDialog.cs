/***********************************************************************\
 * Virtuoso.Miranda.Plugins (Hyphen)                                   *
 * Provides a managed wrapper for API of IM client Miranda.            *
 * Copyright (C) 2006-2009 virtuoso                                    *
 *                    deml.tomas@seznam.cz                             *
 *                                                                     *
 * This library is free software; you can redistribute it and/or       *
 * modify it under the terms of the GNU Lesser General Public          *
 * License as published by the Free Software Foundation; either        *
 * version 2.1 of the License, or (at your option) any later version.  *
 *                                                                     *
 * This library is distributed in the hope that it will be useful,     *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of      *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU   *
 * Lesser General Public License for more details.                     *
\***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Native;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;
using System.ComponentModel;

namespace Virtuoso.Miranda.Plugins.Forms
{
    [Flags]
    public enum FormTranslationFlags : int
    {
        None = 0,
        TranslateNonReadOnlyEditControls = 1,     //translate all edit controls. By default non-read-only edit controls are not translated
        NoTitleTranslation = 2           //do not translate the title of the dialog
    }

    public class LocalizableDialog : SingletonDialog
    {
        #region Fields

        private const string MS_LANGPACK_TRANSLATEDIALOG = "LangPack/TranslateDialog";

        private FormTranslationFlags translateFlags;
        private readonly Collections.ControlCollection nonLocalizableControls;

        [Browsable(false)]
        public Collections.ControlCollection NonLocalizableControls
        {
            get { return nonLocalizableControls; }
        } 

        public FormTranslationFlags TranslateFlags
        {
            get { return translateFlags; }
            set { translateFlags = value; }
        } 

        #endregion

        #region .ctors

        protected LocalizableDialog() : this(null, FormTranslationFlags.None) { }

        protected LocalizableDialog(FormTranslationFlags flags) : this(null, flags) { }

        protected LocalizableDialog(string dialogName, FormTranslationFlags flags) : base(dialogName)
        {
            translateFlags = flags;
            nonLocalizableControls = new Collections.ControlCollection();
        }

        #endregion

        #region UI handlers

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                if ((translateFlags & FormTranslationFlags.NoTitleTranslation) != FormTranslationFlags.NoTitleTranslation)
                    Text = LanguagePack.TranslateString(Text);

                foreach (Control control in Controls)
                {
                    TextBoxBase editCtrl = control as TextBoxBase;

                    if (!nonLocalizableControls.Contains(control))
                    {
                        if (editCtrl != null && !editCtrl.ReadOnly && (translateFlags & FormTranslationFlags.TranslateNonReadOnlyEditControls) != FormTranslationFlags.TranslateNonReadOnlyEditControls)
                            continue;

                        control.Text = LanguagePack.TranslateString(control.Text);
                    }
                }
            }

            base.OnLoad(e);
        }

        #endregion
    }
}
