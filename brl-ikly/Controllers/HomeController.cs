using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using brl_ikly.Models;

namespace brl_ikly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Shorten(Url inputUrl)
        {
            string outputUrl = "";
            int outputVisitCount = 0;

            UrlDBContext db = new UrlDBContext();
            Url existingUrl = db.Urls.Where(o => o.UrlLongName.ToLower().Equals(inputUrl.UrlLongName.ToLower())).FirstOrDefault();

            if (existingUrl == null)
            {
                //means not yet existing

                //generate unique number
                int rid = db.Urls.Count() + 1;
                int unq = (((rid % 10) * 1000) * ShortURL.Base) + (rid *ShortURL.Base);

                outputUrl = ShortURL.Encode(unq);

                Url newUrl = new Models.Url();
                newUrl.UrlId = rid;
                newUrl.UrlLongName = inputUrl.UrlLongName;
                newUrl.UrlShortName = outputUrl;
                newUrl.UrlVisitCount = 0;

                db.Urls.Add(newUrl);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    //TODO: Log Error
                }
            }
            else
            {
                outputUrl = existingUrl.UrlShortName;
                outputVisitCount = existingUrl.UrlVisitCount;                
            }

            inputUrl.UrlShortName = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + outputUrl;
            inputUrl.UrlVisitCount = outputVisitCount;

            return View("Index", inputUrl);
        }

        public ActionResult GotoOriginalUrl(string shortUrl)
        {
            UrlDBContext db = new UrlDBContext();
            Url existingUrl = db.Urls.Where(o => o.UrlShortName.Equals(shortUrl)).FirstOrDefault();

            if (existingUrl != null)
            {
                //means short link exists

                string fixedLongName = existingUrl.UrlLongName;

                if (!(fixedLongName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || fixedLongName.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                {
                    fixedLongName = "http://" + fixedLongName;
                }

                existingUrl.UrlVisitCount += 1;
                db.SaveChanges();

                return Redirect(fixedLongName);
            }
            else
            {

            }

            //TODO: Error page if not valid shorturl
            return View();
        }
    }
}