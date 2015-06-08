namespace WebCompiler
{
    internal interface ICompiler
    {
        CompilerResult Compile(Config config);
    }
}