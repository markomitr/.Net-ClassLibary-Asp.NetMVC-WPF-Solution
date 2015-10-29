using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libPrepVerzija.Klasi;

namespace libPrepVerzija.Interfaces
{
   public interface IRecept
    {
        void addSqlObj(TipProceduri tipProc);
        void addFileObj(TipFajlovi tipFile);
        List<TipProceduri> getSqlObj();
        List<TipFajlovi> getFileObj();
        DateTime DatumVerzija
        { get; set;  }
        bool FilterPoDatum
        { get; set; }
        Aplikacii Aplikacija
        { get; set; }
    }
}
