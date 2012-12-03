using System.Web.Mvc;

namespace TryThis.Controllers
{
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using Roslyn.Scripting;
    using Roslyn.Scripting.CSharp;
    using System;
    using System.Linq;

    public class HomeController : Controller
    {
        private Session session = new ScriptEngine().CreateSession();

        public ActionResult Index(object result)
        {
            return this.View("Index", result);
        }

        public JsonResult Compile(string code)
        {
            session.AddReference("System");
            session.AddReference("System.Core");
            try
            {
                var executionResult = session.Execute(code);
                return Json(new { result = executionResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}
