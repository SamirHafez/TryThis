using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace TryThis.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundleCollection)
        {
            bundleCollection.Add(new Bundle("~/scripts/libs", new JsMinify()).Include("~/scripts/jquery-1.8.3.js")
                                                                             .Include("~/scripts/modernizr-2.0.6-development-only.js")
                                                                             .Include("~/scripts/bootstrap.js")
                                                                             .Include("~/scripts/codemirror-2.35/lib/codemirror.js")
                                                                             .Include("~/scripts/codemirror-2.35/mode/clike/clike.js"));

            bundleCollection.Add(new Bundle("~/css/libs", new CssMinify()).Include("~/content/metro-bootstrap.css")
                                                                          .Include("~/scripts/codemirror-2.35/lib/codemirror.css")
                                                                          .Include("~/scripts/codemirror-2.35/theme/neat.css")
                                                                          .Include("~/content/Base.css"));
        }
    }
}