namespace WebCompiler
{
    class LessDependencyResolver : SassDependencyResolver
    {
        public override string[] SearchPatterns
        {
            get { return new string[] { "*.less" }; }
        }

        public override string FileExtension
        {
            get
            {
                return ".less";
            }
        }

    }
}
