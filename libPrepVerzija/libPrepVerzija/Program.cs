using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libPrepVerzija.Klasi;
using System.IO;
using System.IO.Compression;
namespace libPrepVerzija
{
    class Program
    {
        static void Main(string[] args)
        {
            string verFile = "";
            byte[] verZip = null;
            AppMum mojApp;
            Kurir kr = new Kurir();
            //AppMum mojApp = new AppMum(Aplikacii.WTRGKS,kr);
            //mojApp.NapraviVerzija(ref verZip,ref verFile);
            //File.WriteAllBytes(verFile, verZip);
            //Console.ReadLine();

            //mojApp = new AppMum(Aplikacii.WTRG, kr);
            //mojApp.NapraviVerzija(ref verZip, ref verFile);
            //File.WriteAllBytes(verFile, verZip);
            //Console.ReadLine();

            Recept mojRecept = new Recept(DateTime.Now.AddMonths(-4));

            //mojRecept.addSqlObj(TipProceduri.Fn);
            //mojRecept.addSqlObj(TipProceduri.Fix);
            mojRecept.addFileObj(TipFajlovi.XML);
            mojRecept.addFileObj(TipFajlovi.CristalRpt);

            mojApp = new AppMum(Aplikacii.WTRG, mojRecept, kr,"");
            mojApp.NapraviVerzija(ref verZip, ref verFile);
            File.WriteAllBytes(verFile, verZip);
            

            Console.ReadLine();
        }
    }
}
