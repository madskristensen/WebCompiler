using System.Reflection;
using System.Runtime.InteropServices;
using WebCompilerVsix;

[assembly: AssemblyTitle(Constants.VSIX_NAME)]
[assembly: AssemblyDescription("Compiles LESS, Sass and CoffeeScript files directly within Visual Studio. This plugin is based on Mads Kristensen WebCompiler (https://github.com/madskristensen/WebCompiler) but uses dart-sass instead of node-sass.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(Constants.VSIX_NAME)]
[assembly: AssemblyCopyright("Alex Garcia")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("en-US")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(WebCompilerPackage.Version)]
[assembly: AssemblyFileVersion(WebCompilerPackage.Version)]
