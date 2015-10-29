using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libPrepVerzija.Interfaces;
using System.IO;

namespace libPrepVerzija.Klasi
{
    public class Kurir : IKurir
    {
       
        Dictionary<TipServisi, List<string>> _greski = new Dictionary<TipServisi, List<string>>();
        Dictionary<TipServisi, List<string>> _info = new Dictionary<TipServisi, List<string>>();
        Dictionary<string, List<string>> _log = new Dictionary<string, List<string>>();
        List<IObserver<string>> klienti = new List<IObserver<string>>();
        public bool DaliEVoRed
        {
            get
            {
                return _greski.Count==0;
            }
        }
        public void addGreska(TipServisi kluc, string poraka)
        {
            poraka = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "# " + poraka;
            if (_greski.ContainsKey(kluc))
            {
                _greski[kluc].Add(poraka);
            }
            else
            {
                _greski.Add(kluc, new List<string>());
                _greski[kluc].Add(poraka);
            }
           
        }
        public void addInfo(TipServisi kluc, string poraka)
        {
            poraka = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "# " + poraka;
            if (_info.ContainsKey(kluc))
            {
                _info[kluc].Add(poraka);
            }
            else
            {
                _info.Add(kluc, new List<string>());                
                _info[kluc].Add(poraka);
            }
           
        }
        public void addLog(string kategorija, string poraka)
        {       
            if (_log.ContainsKey(kategorija))
            {
                _log[kategorija].Add(poraka);
            }
            else
            {
                _log.Add(kategorija, new List<string>());
                _log[kategorija].Add("#" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"));
                _log[kategorija].Add(poraka);
            }
        }
        
        public string VratiInfo()
        {
            return "INFO:\n" + VratiPoraki(TipPoraki.Info);
        }
        public string VratiGreski()
        {

            return "ERROR:\n" +  VratiPoraki(TipPoraki.Greski);
        }
        private string VratiPoraki(TipPoraki tip)
        {
            Dictionary<TipServisi, List<string>> pom = new Dictionary<TipServisi,List<string>>();
            StringBuilder sb = new StringBuilder();
            if(tip== TipPoraki.Info)
            {
                pom = _info;
            }
            else if(tip==TipPoraki.Greski )
            {
                pom = _greski;
            }

            foreach (TipServisi ser in pom.Keys)
            {
                sb.AppendLine(ser.ToString() + "=>");
                sb.AppendLine(VratiString(pom[ser]));
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public string VratiLog()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string ser in _log.Keys)
            {
                sb.AppendLine("==================" + ser.ToString() + "==============>");
                sb.AppendLine(VratiString(_log[ser]));
                sb.AppendLine("-----------------------------------------------------");
                sb.AppendLine();
            }
            return sb.ToString();
        }
        private string VratiString(List<string> lista)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string pom in lista)
            {
                sb.AppendLine(pom);
            }
            return sb.ToString();
        }       
        public void ResetPoraki(TipServisi kluc)
        {
            if (_greski.ContainsKey(kluc))
            {
                _greski.Remove(kluc);
            }

            if (_info.ContainsKey(kluc))
            {
                _info.Remove(kluc);
            }
        }      
    }
}
