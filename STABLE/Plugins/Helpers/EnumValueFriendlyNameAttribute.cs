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
using System.Reflection;
using System.Collections.Specialized;

namespace Virtuoso.Miranda.Plugins.Helpers
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EnumValueFriendlyNameAttribute : Attribute
    {
        #region Fields

        private string friendlyName;

        #endregion

        #region .ctors

        public EnumValueFriendlyNameAttribute(string name)
        {
            this.friendlyName = name;
        }

        #endregion

        #region Properties
        
        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }

        #endregion

        #region Methods

        public static Dictionary<TEnum, string> GetFriendlyNames<TEnum>() where TEnum : struct
        {
            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
                throw new ArgumentException("TEnum is not an enumeration.", "TEnum");

            Type thisType = typeof(EnumValueFriendlyNameAttribute);
            Dictionary<TEnum, string> results = new Dictionary<TEnum, string>(1);

            foreach (FieldInfo field in enumType.GetFields())
            {
                if ((field.Attributes & FieldAttributes.Literal) != FieldAttributes.Literal)
                    continue;

                EnumValueFriendlyNameAttribute[] names = (EnumValueFriendlyNameAttribute[])field.GetCustomAttributes(thisType, false);

                if (names != null && names.Length > 0)
                    results.Add((TEnum)field.GetRawConstantValue(), names[0].FriendlyName);
                else
                    results.Add((TEnum)field.GetRawConstantValue(), field.Name);
            }

            return results;
        }

        #endregion
    }
}
