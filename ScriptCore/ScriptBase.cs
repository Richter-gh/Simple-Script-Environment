namespace ScriptCore
{
    public abstract class ScriptBase : IExecutable
    {
        public ScriptBase()
        {
            OnLoad();
        }

        public virtual string Author { get { return "Me"; } }
        public virtual string Name { get { return "Temp"; } }
        public virtual string Version { get { return "0.0.1"; } }

        public virtual void OnLoad(){}

        public abstract void Execute();

        public override string ToString()
        {
            return string.Format("Name: {1}, Author: {0}, Version: {2}", Author, Name, Version);
        }
    }

}