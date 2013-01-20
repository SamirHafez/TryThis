using System.Web.Mvc;

namespace TryThis.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Routing;
    using TryThis.Core;

    public class HomeController : Controller
    {
        public Compiler Compiler { get; protected set; }
        public IO IOManager { get; protected set; }
        public const int CompilingTime = 5000;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Compiler = new Compiler();
            IOManager = new IO(Server.MapPath("~/SavedCode"));
        }

        public ActionResult Index(string id)
        {
            if (id != null)
            {
                string code, result;
                if (IOManager.Get(id, out code, out result))
                {
                    ViewBag.Code = code;
                    ViewBag.Result = result;
                }
            }

            return this.View("Index");
        }

        //This timeout solution is HIGHLY deprecated.
        //TODO Find a way to make the ASP.NET timeout mechanism work!
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Compile(string code)
        {
            object result = null;
            Exception exception = null;
            var executionThread = new Thread(() =>
            {
                try
                {
                    result = Compiler.Compile<object>(code);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            });
            executionThread.Start();

            if (!executionThread.Join(CompilingTime))
            {
                executionThread.Abort();
                return Json(new { error = "Request timeout. This can occur when the code contains infinite loops. Please review it, and try again." }, JsonRequestBehavior.AllowGet);
            }

            if (exception != null)
                return Json(new { error = exception.Message }, JsonRequestBehavior.AllowGet);

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(string code, string result)
        {
            var id = IOManager.Save(code, result);

            return Json(new { url = id }, JsonRequestBehavior.AllowGet);
        }
    }
}
