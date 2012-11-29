using System.Web.Mvc;

namespace TryThis.Controllers
{
    using Roslyn.Compilers;
    using Roslyn.Scripting;
    using Roslyn.Scripting.CSharp;

    public class HomeController : Controller
    {
        private Session session = new ScriptEngine().CreateSession();

        public ActionResult Index(object result)
        {
            return this.View("Index", result);
        }

        public JsonResult Compile(string code)
        {
            session.AddReference("System.Core");
            session.AddReference("System.Linq");

            try
            {
                var executionResult = session.Execute(code);
                return Json(new {result = executionResult}, JsonRequestBehavior.AllowGet);
            }
            catch(CompilationErrorException e)
            {
                return Json(new {error = e.Message}, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var engine = new ScriptEngine();
            var session = engine.CreateSession();
            session.AddReference("System.Core");
            session.AddReference("System.Linq");

            var execution = session.Execute(code);

            return this.View("Index", new { result = execution});
        }
    }
}
