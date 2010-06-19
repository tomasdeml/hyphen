Hyphen plugin for Miranda - allows you to write managed plugins in Microsoft.NET languages.
You and an user must have the .NET FX 2.0 RTM installed in order to use and load this plugin.

Disclaimer
================================================================================================================================================================================================================================================================================================
Still beta, comes with no warranty. 
Some features are not fully implemented yet, so expect exceptions and bugs. Some APIs may change in the future, no binary compatibility guaranteed.


API
================================================================================================================================================================================================================================================================================================
Documentation is not done yet (because of the amount of changes Hyphen is going through) so you have to help yourself with VS (Express) IntelliSense, Object explorer, ildasm or Reflector.

In a nutshell, you have to derive your plugins from the Virtuoso.Miranda.Plugins.MirandaPlugin class. Then, decorate your methods matching the Virtuoso.Miranda.Plugins.Callback delegate signature with the Virtuoso.Miranda.Plugins.ServiceFunction / EventHook attributes.
To declare menu items, decorate your plugin class with the Virtuoso.Miranda.Plugins.MenuItemDeclaration attributes.

Hyphen.dll (Hyphen.dll and Virtuoso.Miranda.Plugins.dll were merged)
Partially written in IL assembler and C#; contains 3 unmanaged Miranda API exports. Represents a proxy between the unmanaged Miranda API and the .NET world. Instantiates the PluginManager class which does all the stuff.
This is the assembly you have to compile your plugins againts (just add the reference in the VS, but DON'T allow it to copy the assembly locally!).
This assembly contains the PluginManager class that loads/unloads plugins and provides elementary functionality to your managed plugins. 
As far as the API uses non-CLS compliant features (unsigned data types, for example), you probably will not be able to interface with the API from VB.NET.

To load your plugin, mark its assembly with the Virtuoso.Miranda.Plugins.ExposingPluginAttribute attribute and put it into the <MirandaFolder>\plugins\managed folder.


Requirements
================================================================================================================================================================================================================================================================================================
To compile the sample plugin, you will need MS Visual C# Express or thoroughbred Visual Studio 2005 installed.
.NET Framework 2.0 RTM is required to compile and LOAD managed plugins as well.


Licence
================================================================================================================================================================================================================================================================================================
I will release the source under the LGPL licence as soon as the beta stage is left.

Any questions or bugs found? You can reach me at ICQ 177-147-220 (GMT+1h) or deml.tomas@seznam.cz.
Enjoy!

