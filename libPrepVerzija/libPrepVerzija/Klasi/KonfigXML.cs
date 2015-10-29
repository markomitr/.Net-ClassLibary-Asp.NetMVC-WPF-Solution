using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using libPrepVerzija.Interfaces;

namespace libPrepVerzija.Klasi
{
    public class KonfigXML
    {
        IKurir _status;
        string KonfigIme = "Konfig.xml";
        string _patekaKonf = "";
        public KonfigXML(string patekaKonfig,IKurir status)
        {
            _status = status;
            _patekaKonf = patekaKonfig;
        }
        public bool CitajXML(ref Dictionary<String,String> param,Aplikacii kojApp)
        {
            bool uspeh = true;
            XmlDocument xml = new XmlDocument();
            string PatekaFull = _patekaKonf; //UnvFunc.SrediPateka(_patekaKonf) 
            try
            {
                if (!File.Exists(PatekaFull))
                {
                    _status.addGreska(TipServisi.KonfigService, "Ne postoi konfigot - " + PatekaFull);
                    return false;
                }
                xml.Load(PatekaFull);
                SrediSQL(xml, ref param);
                SrediAplikacija(xml, ref param, kojApp);
                SrediExport(xml, ref param);
            }
            catch (Exception ex)
            {
                _status.addGreska(TipServisi.KonfigService, "Greska(CitajXML):" + ex.Message);
                uspeh = false;
            }
            return uspeh;
           
        }
        private bool SrediSQL(XmlDocument xmlDoc,ref Dictionary<String, String> param)
        {
            if (param == null)
                param = new Dictionary<string, string>();

            XmlNode xmlNod = xmlDoc.SelectSingleNode("/Verzija/SQL");

            param.Add("SQL_SERVER", @xmlNod.SelectSingleNode("Server").InnerText);
            param.Add("SQL_BAZA", @xmlNod.SelectSingleNode("Baza").InnerText);
            param.Add("SQL_USER", @xmlNod.SelectSingleNode("User").InnerText);
            param.Add("SQL_PASSWORD", @xmlNod.SelectSingleNode("Password").InnerText);
            param.Add("SQL_PASSWORD_TYPE", @xmlNod.SelectSingleNode("Password").Attributes["Type"].Value );
            return true;

        }
        private bool SrediExport(XmlDocument xmlDoc, ref Dictionary<String, String> param)
        {
            if (param == null)
                param = new Dictionary<string, string>();

            XmlNode xmlNod = xmlDoc.SelectSingleNode("/Verzija/Export/Pateka");
            string ExpApp = UnvFunc.SrediPateka(@xmlNod.InnerText);
            string ExpTmp="";
            param.Add("EXPORT_PATEKA", ExpApp);
            ExpApp = ExpApp + UnvFunc.RecnikZemiPole(param, "APP_IME") + "_Ver";
            param.Add("EXPORT_PATEKA_APP", ExpApp);
            ExpTmp = UnvFunc.SrediPateka(ExpApp) + UnvFunc.GenerImeToken(UnvFunc.RecnikZemiPole(param, "APP_IME"));
            param.Add("EXPORT_PATEKA_APP_TMP", ExpTmp);
            return true;
        }
        private bool SrediAplikacija(XmlDocument xmlDoc, ref Dictionary<String, String> param,Aplikacii kojApp)
        {
            if (param == null)
                param = new Dictionary<string, string>();

            XmlNode xmlNod = xmlDoc.SelectSingleNode("/Verzija/Aplikacii/Program[@Ime=\""+ VratiKojaApp(kojApp) +"\"]");
            if (xmlNod != null && xmlNod.HasChildNodes)
            {
                param.Add("APP_IME", @xmlNod.Attributes["Ime"].Value);
                param.Add("APP_PATEKASLN", @xmlNod.SelectSingleNode("PatekaSLN").InnerText);
                param.Add("APP_PATEKAVSEXE", @xmlNod.SelectSingleNode("PatekaVSexe").InnerText);
                param.Add("APP_PATEKAXML", @xmlNod.SelectSingleNode("PatekaXML").InnerText);
                param.Add("APP_PATEKARPT", @xmlNod.SelectSingleNode("PatekaRPT").InnerText);
                param.Add("APP_PATEKASQLUPGRADE", @xmlNod.SelectSingleNode("PatekaSQLupgrade").InnerText);
                param.Add("APP_PATEKAEXE", @xmlNod.SelectSingleNode("PatekaEXE").InnerText);
                param.Add("APP_SERVER", @xmlNod.SelectSingleNode("Server").InnerText);
                param.Add("APP_BAZA", @xmlNod.SelectSingleNode("Baza").InnerText);
                param.Add("APP_USER", @xmlNod.SelectSingleNode("User").InnerText);
                param.Add("APP_PASSWORD", @xmlNod.SelectSingleNode("Password").InnerText);
                param.Add("APP_PASSWORD_TYPE", @xmlNod.SelectSingleNode("Password").Attributes["Type"].Value);
            }
            else
            {
                _status.addGreska(TipServisi.KonfigService, "Nema zapis za izbranata aplikacija vo konfigot => " + VratiKojaApp(kojApp));
            }

            return true;
        }
        private string VratiKojaApp(Aplikacii kojaApp)
        {
            string app = "";
            switch (kojaApp)
            {
                case Aplikacii.WTRG:
                    {
                        app="WTRG";
                        break;
                    }
                case Aplikacii.WTRGKS:
                    {
                        app="WTRGKS";
                        break;
                    }
                 case Aplikacii.APT5MK:
                    {
                        app="APT5MK";
                        break;
                    }
                default:
                    {
                        app="X";
                        break;
                    }
            }
            return app;
        }

    }
}
