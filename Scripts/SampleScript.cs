using ScriptCore;

namespace Scripts
{
    internal class SampleScript : IExecutable
    {
        public string Author
        {
            get
            {
                return "SomeAuthor";
            }
        }

        public string Name
        {
            get
            {
                return "SampleScript";
            }
        }

        public string Version
        {
            get
            {
                return "0.1";
            }
        }

        public void Execute()
        {
            //Do something
        }
    }
}