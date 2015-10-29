using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using libPrepVerzija.Interfaces;
namespace libPrepVerzija.Klasi
{
   public class AppMum
    {
        string _patekaExport;

        public string PatekaExport
        {
            get { return _patekaExport; }
            set { _patekaExport = value; }
        }
        string _patekaXML;

        public string PatekaXML
        {
            get { return _patekaXML; }
            set { _patekaXML = value; }
        }
        string _patekaRPT;

        public string PatekaRPT
        {
            get { return _patekaRPT; }
            set { _patekaRPT = value; }
        }
        string _patekaEXE;

        public string PatekaEXE
        {
            get { return _patekaEXE; }
            set { _patekaEXE = value; }
        }
        string _patekaApp;

        public string PatekaApp
        {
            get { return _patekaApp; }
            set { _patekaApp = value; }
        }
        string _patekaSQL;

        public string PatekaSQL
        {
            get { return _patekaSQL; }
            set { _patekaSQL = value; }
        }
        string _imeApp;

        public string ImeApp
        {
            get { return _imeApp; }
            set { _imeApp = value; }
        }
        string _patekaAppTmp;

        public string PatekaAppTmp
        {
            get { return _patekaAppTmp; }
            set { _patekaAppTmp = value; }
        }

        string _patekaKonfig = "";
        Aplikacii _KojaApp;
        IKurir  kr;
        KonfigXML konf;
        BuildService bs;
        DBAlatService dbA;
        FileService fs;
        Dictionary<string, string> paramKonf;
        IRecept _recept;
        public AppMum(Aplikacii kojaApp, IKurir msg, string patekaKonfig)
        {
            _patekaKonfig = patekaKonfig;
            kr = msg;
            paramKonf = new Dictionary<string, string>();
            konf = new KonfigXML(patekaKonfig,kr);
            konf.CitajXML(ref paramKonf,kojaApp);
            dbA = new DBAlatService(kr, paramKonf);
            fs = new FileService(kr);
            this._KojaApp = kojaApp;
            _recept = new Recept();
            NapraviDefaultRecept();
        }
        public AppMum(Aplikacii kojaApp, IRecept recept, IKurir msg, string patekaKonfig)
            : this(kojaApp, msg,patekaKonfig)
        {
            _recept = recept;
        }
        private void srediKonfig()
        {
            ImeApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_IME");
            PatekaExport = @UnvFunc.RecnikZemiPole(paramKonf, "EXPORT_PATEKA");
            PatekaApp = @UnvFunc.RecnikZemiPole(paramKonf, "EXPORT_PATEKA_APP");
            PatekaAppTmp = @UnvFunc.RecnikZemiPole(paramKonf, "EXPORT_PATEKA_APP_TMP");
            PatekaAppTmp = UnvFunc.SrediPateka(PatekaAppTmp);
            PatekaXML = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKAXML");
            PatekaRPT = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKARPT");
            PatekaEXE = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKAEXE");
            PatekaSQL = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKASQLUPGRADE");         
        }
        private void SrediFolderi()
        {
            string FolderApp = UnvFunc.SrediPateka(PatekaApp);
            string FolderAppSql = PatekaAppTmp + ImeApp + "\\";
            string FolderAppExe = PatekaAppTmp + "SQL" + "\\";

            try
            {
                if (Directory.Exists(PatekaExport) == false)
                {
                    Directory.CreateDirectory(PatekaExport);
                }

                if (Directory.Exists(FolderApp) == false)
                {
                    Directory.CreateDirectory(FolderApp);
                }
                if (Directory.Exists(PatekaAppTmp) == false)
                {
                    Directory.CreateDirectory(PatekaAppTmp);
                }

                if (Directory.Exists(FolderAppExe) == false)
                {
                    Directory.CreateDirectory(FolderAppExe);
                }

                if (Directory.Exists(FolderAppSql) == false)
                {
                    Directory.CreateDirectory(FolderAppSql);
                }
            }
            catch (Exception ex)
            {
                kr.addGreska(TipServisi.AppService, "Greska:" + ex.Message);
            }

        }
        private void NapraviDefaultRecept()
        {
            _recept = new Recept();
            switch (_KojaApp)
            {
                case Aplikacii.WTRG:
                case Aplikacii.WTRGKS:
                    {
                        _recept.addSqlObj(TipProceduri.Fix);
                        _recept.addSqlObj(TipProceduri.Trigeri);
                        _recept.addFileObj(TipFajlovi.CristalRpt);
                        _recept.addFileObj(TipFajlovi.XML);
                        _recept.addFileObj(TipFajlovi.SQL);
                        _recept.addFileObj(TipFajlovi.EXE);
                        break;
                    }
                case Aplikacii.APT5MK:
                    {
                        _recept.addSqlObj(TipProceduri.WApt);
                        _recept.addSqlObj(TipProceduri.Trigeri);
                        _recept.addFileObj(TipFajlovi.CristalRpt);
                        _recept.addFileObj(TipFajlovi.XML);
                        _recept.addFileObj(TipFajlovi.SQL);
                        _recept.addFileObj(TipFajlovi.EXE);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        private bool NapraviVerzija(ref string zipVerFilePateka)
        {
            kr.addLog("Aplikacija", "Verzija se sprema za: " + _KojaApp.ToString());
            kr.addLog("Aplikacija", "Od datum: " + _recept.DatumVerzija.ToString("dd-MM-yyyy"));

            List<TipProceduri> lsProc = _recept.getSqlObj();
            List<TipFajlovi> lsFile = _recept.getFileObj();
            if (kr.DaliEVoRed)
            {
                srediKonfig();
            }
            if (kr.DaliEVoRed)
            {
                SrediFolderi();
            }
            foreach(TipProceduri tipP in lsProc)
            {
                if (kr.DaliEVoRed)
                    SrediSqlOdRecept(tipP);
                else
                    break;
            }

            foreach (TipFajlovi tipf in lsFile)
            {
                if (kr.DaliEVoRed)
                    SrediFileOdRecept(tipf);
                else
                    break;
            }
            if (kr.DaliEVoRed)
            {
                ZapisiVoLog();
                DirectoryInfo di = new DirectoryInfo(PatekaAppTmp);
                string zipVerFile = di.Parent.FullName + "\\" + di.Name + ".zip";
                ZipFile.CreateFromDirectory(PatekaAppTmp, di.Parent.FullName + "\\" + di.Name + ".zip");
                zipVerFilePateka = zipVerFile;
                kr.addInfo(TipServisi.AppService, "Spremen ZIP file => " + zipVerFile);

            }
            return kr.DaliEVoRed;
        }
        public bool NapraviVerzija(ref byte[] zipVerFile, ref string filename)
        {
            string pateka = "";
            bool uspeh = NapraviVerzija(ref pateka);
            if (uspeh == false)
                return uspeh;
            FileInfo fi = new FileInfo(pateka);
            filename = fi.Name;
            if (uspeh)
                zipVerFile = fs.VratiFileVoByte(pateka);
            return uspeh;
        }
        private void SrediFileOdRecept(TipFajlovi tipF)
        {
            switch(tipF)
            {
                case TipFajlovi.CristalRpt:
                    if (_recept.FilterPoDatum)
                        fs.ZemiFajloviIkopiraj(PatekaRPT, PatekaAppTmp + ImeApp + "\\", tipF,_recept.DatumVerzija);
                    else
                        fs.ZemiFajloviIkopiraj(PatekaRPT, PatekaAppTmp + ImeApp + "\\", tipF);
                    break;
                case TipFajlovi.XML:
                    if (_recept.FilterPoDatum)
                        fs.ZemiFajloviIkopiraj(PatekaXML, PatekaAppTmp + ImeApp + "\\", tipF,_recept.DatumVerzija);
                    else
                        fs.ZemiFajloviIkopiraj(PatekaXML, PatekaAppTmp + ImeApp + "\\", tipF);
                    break;
                case TipFajlovi.SQL:
                     fs.ZemiEdenFajlIKopiraj(PatekaSQL, PatekaAppTmp + "SQL" + "\\");
                    break;
                case TipFajlovi.EXE:
                    fs.ZemiEdenFajlIKopiraj(PatekaEXE, PatekaAppTmp + ImeApp + "\\");
                    break;
                case TipFajlovi.IKONI:
                    fs.ZemiFajloviIkopiraj(PatekaXML, PatekaAppTmp + ImeApp + "\\", tipF);
                    break;
                case TipFajlovi.DLL:
                    fs.ZemiFajloviIkopiraj(PatekaXML, PatekaAppTmp + ImeApp + "\\", tipF);
                    break;
                default:
                    break;
            }
        }
        private void SrediSqlOdRecept(TipProceduri tipP)
        {
            if(tipP == TipProceduri.Trigeri)
                dbA.ZemiTrigeri();
            else
                dbA.NapraviSkript(tipP);
        }
       private void ZapisiVoLog()
        {
            TextWriter output = File.AppendText(PatekaAppTmp + "Log.txt");
            output.Write(kr.VratiLog());
            output.Close();
        }
    }
}
