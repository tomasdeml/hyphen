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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Security.Permissions;
using System.Security;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, Flags = SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.Execution | SecurityPermissionFlag.UnmanagedCode)]

// HyphenVersion information for an assembly consists of the following four values:
//
//      Major HyphenVersion
//      Minor HyphenVersion 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

// ************************************************ //
// !! CHANGE THE CONFIG FILE + .MINI VERSION TOO !!!
// ************************************************ //
// x.x.YYYY.MMDD
[assembly: AssemblyVersion("0.8.3000." /* !!! CONFIG !!! */ + "00909" )] 
[assembly: AssemblyFileVersion("0.8.3000.00909")]

[assembly: InternalsVisibleTo("Virtuoso.Hyphen.Mini, PublicKey=00240000048000009400000006020000002400005253413100040000010001005d9bd3582d0c01dcd54854ac2f36c94f7bef235b2e2b5479248efddd65431bceef6c92d759d7f23f3692704cd18f0c5b7ee3436a0f7b9b2eaf8fbf205b85150d171a0fbb7658fb50c1531f6eee3ec70239ae38ac383dd742a754691c965cc23cd716618b8c89b25ca04402ea4a579a66bdf50335e4b6d2b0c72bd183328487b5")]
[assembly: InternalsVisibleTo("Virtuoso.Miranda.Plugins.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100190e0cd0962bfe7835b22be43ce49acd109d5d0c0512534f74aaf01fcdb7712fca7b81b5048a51a43750fa8de5c168628c2e4f90acf43559bc328024265df53d5b21a61720c3be75e9a3b15046a4b0892f60a215e1cb8db467d84d2626100e7390a929f35b53c4f853d2523cfe87d484246ddf446c1849c6b5e430b12cc0b6aa")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Virtuoso.Hyphen")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("virtuoso")]
[assembly: AssemblyProduct("Virtuoso.Hyphen")]
[assembly: AssemblyCopyright("© 2006-2010, virtuoso")]
[assembly: AssemblyTrademark("virtuoso")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("00c91fc1-dc8e-473d-be9e-3c72289abdf2")]

[assembly: CLSCompliant(true)]
