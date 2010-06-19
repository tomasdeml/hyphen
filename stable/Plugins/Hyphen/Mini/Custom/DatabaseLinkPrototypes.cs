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
using System.Runtime.InteropServices;
using Virtuoso.Hyphen;

namespace Virtuoso.Hyphen.Mini.Custom
{
    //int (*getCapability) ( int flag );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GetCapabilityPrototype(int flags);

    //int (*getFriendlyName) ( char * buf, size_t cch, int shortName );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GetFriendlyNamePrototype(IntPtr buf, int size, int shortName);
    
    //int (*makeDatabase) ( char * profile, int * error );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int MakeDatabasePrototype([MarshalAs(UnmanagedType.LPStr)] string profile, ref int error);

    //int (*grokHeader) ( char * profile, int * error );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GrokHeaderPrototype([MarshalAs(UnmanagedType.LPStr)] string profile, ref int error);

    //int (*Init) ( char * profile, void * link );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int InitPrototype([MarshalAs(UnmanagedType.LPStr)] string profile, IntPtr link);

    //int (*Unload) ( int wasLoaded );
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int UnloadPrototype(int wasLoaded);
}

/* typedef struct {
	int cbSize;	
	
	returns what the driver can do given the flag
	
	int (*getCapability) ( int flag );

	/
		buf: pointer to a string buffer
		cch: length of buffer
		shortName: if true, the driver should return a short but descriptive name, e.g. "3.xx profile"
		Affect: The database plugin must return a "friendly name" into buf and not exceed cch bytes,
			e.g. "Database driver for 3.xx profiles"
		Returns: 0 on success, non zero on failure
	
	int (*getFriendlyName) ( char * buf, size_t cch, int shortName );

	
		profile: pointer to a string which contains full path + name
		Affect: The database plugin should create the profile, the filepath will not exist at
			the time of this call, profile will be C:\..\<name>.dat
		Note: Do not prompt the user in anyway about this operation.
		Note: Do not initialise internal data structures at this point!
		Returns: 0 on success, non zero on failure - error contains extended error information, see EMKPRF_*
	
	int (*makeDatabase) ( char * profile, int * error );

	
		profile: [in] a null terminated string to file path of selected profile
		error: [in/out] pointer to an int to set with error if any
		Affect: Ask the database plugin if it supports the given profile, if it does it will
			return 0, if it doesnt return 1, with the error set in error -- EGROKPRF_* can be valid error
			condition, most common error would be [EGROKPRF_UNKHEADER] 
		Note: Just because 1 is returned, doesnt mean the profile is not supported, the profile might be damaged
			etc.
		Returns: 0 on success, non zero on failure
	
	int (*grokHeader) ( char * profile, int * error );

	
	Affect: Tell the database to create all services/hooks that a 3.xx legecy database might support into link,
		which is a PLUGINLINK structure
	Returns: 0 on success, nonzero on failure
	
	int (*Init) ( char * profile, void * link );

	
	Affect: The database plugin should shutdown, unloading things from the core and freeing internal structures
	Returns: 0 on success, nonzero on failure
	Note: Unload() might be called even if Init() was never called, wasLoaded is set to 1 if Init() was ever called.
	
	int (*Unload) ( int wasLoaded );

} DATABASELINK; */

