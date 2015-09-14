namespace ScriptCore
{
    public interface IExecutable
    {
        string Author { get; }
        string Name { get; }
        string Version { get; }

        void Execute();
    }
}