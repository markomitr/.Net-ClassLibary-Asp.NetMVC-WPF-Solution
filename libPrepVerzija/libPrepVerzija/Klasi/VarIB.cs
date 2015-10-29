using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libPrepVerzija.Klasi
{
    public class VarIB
    {}

    public enum TipFajlovi
    {
        CristalRpt,
        XML,
        SQL,
        EXE,
        IKONI,
        TEXT,
        DLL,
        PDF,
        DOC,
        DOCX,
        XLS,
        XLSX,
        SITE
    }
    public enum TipProceduri
    {
        SP,
        RK,
        Fix,
        Fn,
        Trigeri,
        Wtrg,
        WApt,
        Site
    }
    public enum Aplikacii
    {
        WTRG,
        WTRGKS,
        APT5MK
    }
    public enum TipServisi
    {
        FileService,
        DBAlatService,
        KonfigService,
        AppService
    }
    public enum TipPoraki
    {
        Greski,
        Info,
    }

}
