using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace ScriptCore
{
    public class ExecutableScript
    {
        internal IExecutable Script;
        public string FileName;
        public string ScriptName;
        public bool Run;
    }

    public class ScriptManager
    {
        #region Properties

        private List<ExecutableScript> _scripts;

        private string _compilationError;
        public List<ExecutableScript> Scripts { get { return _scripts; }}
        
        /// <summary>
        /// Is populaterd when SM is initialized with IDict
        /// </summary>
        public string ErrorMessage;

        #endregion

        #region Constructors

        public ScriptManager(IDictionary<string, bool> scripts)
        {
            _scripts = new List<ExecutableScript>();
            string messages = "";
            foreach (var elem in scripts)
            {
                string temp;
                if (!Add(elem.Key, elem.Value, out temp))
                {
                    messages += temp + "\n";
                }
            }
            if (messages.Length > 0)
            {
                ErrorMessage = messages;
            }
        }

        public ScriptManager()
        {
            _scripts = new List<ExecutableScript>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the script in file to the collection if it compiles.
        /// </summary>
        /// <param name="file">Path to file win the script.</param>
        /// <param name="run">True if script should be executed.</param>
        /// <param name="message">Contains error message or an empty string</param>
        /// <returns>True if the script is successfully added to colelction.</returns>
        public bool Add(string file, bool run, out string message)
        {
            IExecutable compiled;
            try
            {
                compiled = (from type in Compile(file).GetTypes()
                            where type.GetInterfaces().Contains(typeof(IExecutable))
                            select (IExecutable)Activator.CreateInstance(type)).SingleOrDefault();
            }
            catch (NullReferenceException e)
            {
                compiled = null;
            }
            if (compiled != null)
            {

                if (!_scripts.Any(x => x.Script.Name.Equals(compiled.Name)))
                {
                    _scripts.Add(
                        new ExecutableScript
                        {
                            Script = compiled,
                            FileName = file,
                            Run = run,
                            ScriptName = compiled.Name
                        });
                    message = "";
                    return true;
                }
                else
                {

                    message = string.Format("{0} is already added", new FileInfo(file).Name);
                    return false;
                }
            }
            else
            {
                message = string.Format("{0} did not compile,errors: \n{1}", new FileInfo(file).Name, _compilationError);
                return false;
            }
        }

        /// <summary>
        /// Removes the script from collection
        /// </summary>
        /// <param name="file">File containing the script</param>
        /// <returns>True if the script was removed</returns>
        public bool Remove(string file)
        {
            var removable = _scripts.SingleOrDefault(x => x.FileName == file);
            if (removable != null)
            {
                _scripts.Remove(removable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executes all scripts from the collection in parallel mode.
        /// </summary>
        public void Execute()
        {
            Task.Factory.StartNew( ()=>
                Parallel.ForEach<ExecutableScript>(_scripts, script =>
                {
                    if (script.Run)
                        script.Script.Execute();
                }));
            
        }

        /// <summary>
        /// Compiles the assembly from file
        /// </summary>
        /// <param name="fileName">Path to file with the script</param>
        /// <returns>Compiled assembly or null if compilation failed</returns>
        private Assembly Compile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            CodeDomProvider provider;
            bool fromFile = true;
            FileInfo fileInfo;
            var additionalCompOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };
            try
            {
                fileInfo = new FileInfo(fileName);
                if (fileInfo.Extension.ToLower(CultureInfo.InvariantCulture) == ".cs")
                    provider = CodeDomProvider.CreateProvider("CSharp", additionalCompOptions);
                else if (fileInfo.Extension == "")
                {
                    provider = CodeDomProvider.CreateProvider("CSharp", additionalCompOptions);
                    fromFile = false;
                }
                else
                    return null;
            }
            catch (ArgumentException e)
            {
                provider = CodeDomProvider.CreateProvider("CSharp", additionalCompOptions);
                fromFile = false;
            }
            if (provider != null)
            {
                var compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };
                var tempStrings = File.ReadAllLines(fileName);
                tempStrings = tempStrings.Where(x => x.StartsWith("using")).Select(x => x).ToArray();
                for (int i = 0; i < tempStrings.Length; i++)
                {
                    tempStrings[i] = Regex.Match(tempStrings[i], "using (.*);").Groups[1].Value;

                   // compilerParameters.ReferencedAssemblies.Add(tempStrings[i]+".dll");
                }
                compilerParameters.ReferencedAssemblies.Add("ScriptCore.dll");
                var asm = AppDomain.CurrentDomain.GetAssemblies().Where(x=>!x.IsDynamic).Select(x => x.Location).ToArray();
                compilerParameters.ReferencedAssemblies.AddRange(asm);
                
                CompilerResults compilerResults;
                try
                {
                    if (fromFile)
                        compilerResults = provider.CompileAssemblyFromFile(
                        compilerParameters, fileName);
                    else
                        compilerResults = provider.CompileAssemblyFromSource(
                            compilerParameters, fileName);
                    if (compilerResults.Errors.Count != 0)
                    {
                        _compilationError = "";
                        foreach (CompilerError item in compilerResults.Errors)
                        {
                            _compilationError += item.ErrorText + '\n';
                        }
                        return null;
                    }
                    return compilerResults.CompiledAssembly;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }
        #endregion
    }
}