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

        /// <summary>
        /// Called once when script is compiled successfully and added tot he list
        /// </summary>
        public virtual void OnLoad() { }
        /// <summary>
        /// Not yet implemented
        /// </summary>
        public virtual void OnDisable() { }
        /// <summary>
        /// Is called by ScriptManager in an endless loop if IsRunnable is true
        /// </summary>
        public abstract void Run();

        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Name: {1}, Author: {0}, Version: {2}", Author, Name, Version);
        }

        /// <summary>
        /// This is called when the action button is pressed
        /// </summary>
        public abstract void Action();

        #endregion
    }
}