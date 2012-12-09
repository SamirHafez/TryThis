using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TryThis.Core
{
    public class Compiler
    {
        private static readonly ScriptEngine ScriptEngine = new ScriptEngine();
        public Session Session { get; private set; }

        public Compiler()
        {
            Session = ScriptEngine.CreateSession();
            Session.AddReference("System");
            Session.AddReference("System.Core");
        }

        public T Compile<T>(string code)
        {
            return Session.Execute<T>(code);
        }
    }
}