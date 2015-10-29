using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libPrepVerzija.Klasi;

namespace libPrepVerzija.Interfaces
{
    public interface IKurir
    {
        bool DaliEVoRed
        { get;}
        void addGreska(TipServisi kluc, String poraka);
        void addInfo(TipServisi kluc, String poraka);
        void addLog(String kategorija, String poraka);
        void ResetPoraki(TipServisi kluc);
        string VratiGreski();
        string VratiInfo();
        string VratiLog();
    }
}
