using ScriptCore;

namespace Scripts
{
    internal class SampleScript : ScriptBase,IExecutable
    {
        public override string Author
        {
            get
            {
                return "Me";
            }
        }

        public override bool IsRunnable
        {
            get
            {
                return true;
            }
        }

        public override string Name
        {
            get
            {
                return "SampleScript";
            }
        }

        public override string Version
        {
            get
            {
                return "0.1";
            }
        }

        public override void Action()
        {
            
        }
        
        public override void Run()
        {
            
        }
    }
}