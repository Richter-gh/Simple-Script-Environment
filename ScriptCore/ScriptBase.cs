namespace ScriptCore
{
    public abstract class ScriptBase : IExecutable
    {
        #region Properties

        public virtual string Author { get { return "Me"; } }
        public abstract string Name { get; }
        public virtual string Version { get { return "0"; } }

        public abstract bool IsRunnable { get; }

        #endregion       

        #region Methods    
        public virtual void OnLoad() { }
        public virtual void OnDisable() { }
        /// <summary>
        /// Mandatory function for scripts.
        /// Is called by ScriptManager if script is set to run
        /// </summary>
        public abstract void Run();

        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Name: {1}, Author: {0}, Version: {2}", Author, Name, Version);
        }

        public abstract void Action();

        #endregion
    }
}