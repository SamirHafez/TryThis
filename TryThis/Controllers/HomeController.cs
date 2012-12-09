using System.Web.Mvc;

namespace TryThis.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TryThis.Core;

    public class HomeController : Controller
    {
        private Compiler _compiler;
        private IO _io;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _compiler = new Compiler();
            _io = new IO(Server.MapPath("~/SavedCode"));
        }

        public ActionResult Index(string id)
        {
            if (id != null)
            {
                string code, result;
                if (_io.Get(id, out code, out result))
                {
                    ViewBag.Code = code;
                    ViewBag.Result = result;
                }
            }

            return this.View("Index");
        }

        //This timeout solution is HIGHLY deprecated.
        //TODO Find a way to make the ASP.NET timeout mechanism work!
        public JsonResult Compile(string code)
        {
            object result = null;
            Exception ex = null;
            var executionThread = new Thread(() =>
            {
                try
                {
                    result = _compiler.Compile<object>(code);
                }
                catch (Exception e)
                {
                    ex = e;
                }
            });
            executionThread.Start();

            if (!executionThread.Join(5000))
            {
                executionThread.Abort();
                return Json(new { error = "Request timeout. This can occur when the code contains infinite loops. Please review it, and try again." }, JsonRequestBehavior.AllowGet);
            }

            if (ex != null)
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(string code, string result)
        {
            var id = _io.Save(code, result);

            return Json(new { url = id }, JsonRequestBehavior.AllowGet);
        }
    }
}
