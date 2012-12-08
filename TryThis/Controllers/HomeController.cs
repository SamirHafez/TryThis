using System.Web.Mvc;

namespace TryThis.Controllers
{
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using Roslyn.Scripting;
    using Roslyn.Scripting.CSharp;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private Session session = new ScriptEngine().CreateSession();

        public ActionResult Index(object result)
        {
            return this.View("Index", result);
        }

        public async Task<JsonResult> Compile(string code)
        {
            Task<JsonResult> task = Task.Factory.StartNew(
                () =>
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
                });

            task.Wait(5000);

            if (task.IsCompleted)
                return await task;

            return Json(new { error = "Request timeout. This can occur when the code contains infinite loops. Please review it, and try again." }, JsonRequestBehavior.AllowGet);
        }
    }
}
