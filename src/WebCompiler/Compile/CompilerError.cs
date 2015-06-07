namespace WebCompiler
{
    public class CompilerError
    {
        public string FileName { get; set; }

        public string Message { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }
    }
}
