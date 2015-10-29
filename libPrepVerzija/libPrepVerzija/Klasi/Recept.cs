using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libPrepVerzija.Interfaces;
namespace libPrepVerzija.Klasi
{
    public class Recept : IRecept   
    {
        List<TipFajlovi> _fajlovi = new List<TipFajlovi>();
        List<TipProceduri> _proc = new List<TipProceduri>();
        DateTime _datum;
        bool _filterDatum;
        Aplikacii _kojaApp;
        
        public Recept()
        {
            FilterPoDatum = false;
        }
        public Recept(DateTime datumVer)
        {
            FilterPoDatum = true;
            DatumVerzija = datumVer;
        }
        public void addSqlObj(TipProceduri tipProc)
        {
            _proc.Add(tipProc);       
        }

        public void addFileObj(TipFajlovi tipFile)
        {
            _fajlovi.Add(tipFile);
        }

        public List<TipProceduri> getSqlObj()
        {
            return _proc;
        }

        public List<TipFajlovi> getFileObj()
        {
            return _fajlovi;
        }

        public DateTime DatumVerzija
        {
            get
            {
                return _datum;
            }
            set
            {
                _datum = value;
            }
        }
        public  bool FilterPoDatum
        {
            get
            {
                return _filterDatum;
            }
            set
            {
                _filterDatum = value;
            }
        }
        public Aplikacii Aplikacija
        {
            get
            {
                return _kojaApp;
            }
            set
            {
                _kojaApp = value;
            }
        }

    }
}
