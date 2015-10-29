using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using libPrepVerzija.Klasi;
using System.Globalization;
using System.Net.Mime;

namespace PripVerIBmvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Prikazi = "hidden";
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            ViewBag.Message = "Spremam verzija!.";
            ViewBag.Prikazi = "hidden";
            string verFile = "";
            byte[] verZip = null;
            AppMum mojApp;
            Kurir kr = new Kurir();
            Recept recApp = srediForma(form);
            mojApp = new AppMum(recApp.Aplikacija, recApp, kr, Server.MapPath("~") + @"\Konfig.xml");
            if (kr.DaliEVoRed)
            {
                mojApp.NapraviVerzija(ref verZip, ref verFile);
                if (kr.DaliEVoRed)
                {
                    return File(verZip, MediaTypeNames.Application.Zip, verFile);
                }
            }
            ViewBag.Prikazi = "";
            ViewBag.Poraka = kr.VratiGreski();
            return View();
        }

        private Recept srediForma(FormCollection form)
        {
            Recept _tmp = new Recept();
            DateTime _verDatum = DateTime.ParseExact(form["tbDatum"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            _tmp.DatumVerzija = _verDatum;
            _tmp.FilterPoDatum = true;
            foreach(string kluc in form.Keys)
            {
                srediRecept(kluc, form[kluc],ref  _tmp);
            }
            return _tmp;
        }
        private void srediRecept(string kluc,string vrednost,ref Recept recApp)
        {
            if (vrednost == null || vrednost=="false")
                return;
            switch(kluc)
            {
                case "rbCFMA":
                    {
                        recApp.Aplikacija = Aplikacii.WTRG;
                        break;
                    }
                case "rbCFMAKS":
                    {
                        recApp.Aplikacija = Aplikacii.WTRGKS;
                        break;
                    }
                case "rbAPT5MK":
                    {
                        recApp.Aplikacija = Aplikacii.APT5MK;
                        break;
                    }
                case "cbExe":
                    {
                        recApp.addFileObj(TipFajlovi.EXE);
                        break;
                    }
                case "cbXml":
                    {
                        recApp.addFileObj(TipFajlovi.XML);
                        break;
                    }
                case "cbRpt":
                    {
                        recApp.addFileObj(TipFajlovi.CristalRpt);
                        break;
                    }
                case "cbDll":
                    {
                        recApp.addFileObj(TipFajlovi.DLL);
                        break;
                    }
                case "cbIco":
                    {
                        recApp.addFileObj(TipFajlovi.IKONI);
                        break;
                    }
                case "cbSqlUpg":
                    {
                        recApp.addFileObj(TipFajlovi.SQL);
                        break;
                    }
                case "cbSqlSp":
                    {
                        recApp.addSqlObj(TipProceduri.SP);
                        break;
                    }
                case "cbSqlRK":
                    {
                        recApp.addSqlObj(TipProceduri.RK);
                        break;
                    }
                case "cbSqlFix":
                    {
                        recApp.addSqlObj(TipProceduri.Fix);
                        break;
                    }
                case "cbSqlFn":
                    {
                        recApp.addSqlObj(TipProceduri.Fn);
                        break;
                    }
                case "cbSqlTrig":
                    {
                        recApp.addSqlObj(TipProceduri.Trigeri);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}