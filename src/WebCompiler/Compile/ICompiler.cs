namespace WebCompiler
{
    public interface ICompiler
    {
        CompilerResult Compile(Config config);
    }
}