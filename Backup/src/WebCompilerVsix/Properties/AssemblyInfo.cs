using System.Reflection;
using System.Runtime.InteropServices;
using WebCompilerVsix;

[assembly: AssemblyTitle(Constants.VSIX_NAME)]
[assembly: AssemblyDescription("Compiles LESS, Sass and CoffeeScript files directly within Visual Studio")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mads Kristensen")]
[assembly: AssemblyProduct(Constants.VSIX_NAME)]
[assembly: AssemblyCopyright("Mads Kristensen")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("en-US")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(WebCompilerPackage.Version)]
[assembly: AssemblyFileVersion(WebCompilerPackage.Version)]
