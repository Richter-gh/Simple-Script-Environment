namespace ScriptCore
{
    public interface IExecutable
    {
        string Author { get; }
        string Name { get; }
        string Version { get; }
        bool IsRunnable { get; }
        void Action();
        void OnLoad();       
        void Run();
    }
}