HYPHEN SRC README
===================================================
© 2006 - 2009, virtuoso
deml.tomas@seznam.cz

v0.8.2009.0201

PREREQUISITES
===================================================
- Microsoft Visual Studio 2008 (no Express editions)
  Full MS VS 2k8 is required because the build
  solution features multilanguage projects.
  (NMAKE, C#)
- Microsoft .NET 2.0 SDK

SOLUTION
===================================================
- In \Virtuoso.Hyphen\Virtuoso.Hyphen.sln
- HYPHEN project should be set as START PROJECT

- PLUGINS project
  This is a C# project with Miranda .NET API.
  It is build with VS and then disassembled using IlDasm.exe
  into the Plugins.IL file.
- HYPHEN project
  This is an NMAKE project containing an MSIL stub
  which acts as a bootstrapper for MS.NET.
  It merges the Plugins.il with the LoaderStub.il
  into a single Hyphen.dll assembly using IlAsm.exe.
- HYPHEN.MINI project
  This is an NMAKE project containg custom MSIL stub
  for database and protocol plugins which cannot
  be loaded with Hyphen directly.

HOW TO BUILD
===================================================
- 1) Include 
     "%WINDIR%\Microsoft.NET\Framework\v2.0.50727" and
     "%PROGRAMFILES%\Microsoft Visual Studio 8\SDK\v2.0\Bin"
     in the PATH variable.
     These paths contain IlAsm.exe and IlDasm.exe respectively
     which are needed during the build process.

- 2) Open the solution.

- 3) Switch to the release configuration.

- 4) Open the Hyphen project properties and change
     the Output directory to point to
     %YOUR_MIRANDA_FOLDER_GOES_HERE\Plugins directory.
     
- 5) Press F6 to build the solution. You should see
     IlAsm output in the output window and IlDasm window
     with progress bar.
     
- 6) Once it's done, you should find Hyphen.dll in your
     Miranda\Plugins directory.
 
PROBLEMS
===================================================  
- If you are getting "Failed to define document writer"
  errors during the build, make sure the 
  Hyphen\Bin\(CONFIG_NAME_HERE) folder exists.
  CONFIG_NAME_HERE stands for Debug or Release.
- Feel free to !MAIL! me in case of problems.

     
SOURCES
===================================================     
Source codes are mostly undocumented (unfortunately).

