namespace WebCompilerVsix
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class GuidList
    {
        public const string guidBundlerPackageString = "6e1b31b9-e1c1-4697-9b0a-e638eece7765";
        public const string guidBundlerCmdSetString = "92a030a3-2493-40f9-b24b-34fdfffafb7d";
        public static Guid guidBundlerPackage = new Guid(guidBundlerPackageString);
        public static Guid guidBundlerCmdSet = new Guid(guidBundlerCmdSetString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageCommands
    {
        public const int MyMenuGroup = 0x1020;
        public const int CreateConfigFile = 0x0100;
        public const int RecompileConfigFile = 0x0200;
    }
}
