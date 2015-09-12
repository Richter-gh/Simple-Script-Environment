using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace ScriptCore
{
    public class ExecutableScript
    {
        public IExecutable Script;
        public string FileName;
        public bool Run;
    }
    public class ScriptManager
    {
        private List<ExecutableScript> _scripts;

        public ScriptManager(IDictionary<string,bool> scripts)
        {
            _scripts = new List<ExecutableScript>();
            foreach (var elem in scripts)
            {
                var compiled = (from type in Compile(elem.Key).GetTypes()
                                where type.GetInterfaces().Contains(typeof (IExecutable))
                                select (IExecutable) Activator.CreateInstance(type)).SingleOrDefault();
                if (compiled != null)
                    _scripts.Add(
                        new ExecutableScript
                        {
                            Script = compiled,
                            FileName = elem.Key,
                            Run = elem.Value
                        });
            }
        }

        public bool Add(string file, bool run)
        {
            var compiled = (from type in Compile(file).GetTypes()
                            where type.GetInterfaces().Contains(typeof(IExecutable))
                            select (IExecutable)Activator.CreateInstance(type)).SingleOrDefault();
            if (compiled != null)
            {
                _scripts.Add(
                      new ExecutableScript
                      {
                          Script = compiled,
                          FileName = file,
                          Run = run
                      });
                return true;
            }
            return false;
        }
        public bool Remove(string file)
        {
            var removable = _scripts.FirstOrDefault(x => x.FileName == file);
            if (removable != null)
            {
                _scripts.Remove(removable);
                return true;
            }
            return false;
        }
        public void Execute()
        {
            lock (_scripts)
            {
                Parallel.ForEach(_scripts, script =>
                {
                    if (script.Run)
                        script.Script.Execute();
                });
            }
        }

        private Assembly Compile(string fileName)
        {
            CodeDomProvider provider;
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Extension.ToLower(CultureInfo.InvariantCulture) == ".cs")
                provider = CodeDomProvider.CreateProvider("CSharp");
            else
                throw new Exception("Unsupported extension of script");
            if (provider != null)
            {
                var compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };
                var compilerResults = provider.CompileAssemblyFromFile(
                    compilerParameters, fileName);
                if (compilerResults.Errors.Count != 0)
                    throw new Exception(string.Format("{0} did not compile", fileName));
                return compilerResults.CompiledAssembly;
            }
            return null;
        }
    }
}
