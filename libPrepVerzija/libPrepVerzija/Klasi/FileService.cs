using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libPrepVerzija.Interfaces;
namespace libPrepVerzija.Klasi
{
    
    public class FileService
    {
        private List<string> _zabranetiFalovi = new List<string>();
        private IKurir _status; 
        public FileService(IKurir status)
        {
            _status = status;
            _zabranetiFalovi = new List<string>();
            _zabranetiFalovi.Add("Konfig.xml");
            _zabranetiFalovi.Add("WrKonfig.xml");
            _zabranetiFalovi.Add("APT5Konfig.xml");
            _status.addLog("File", "Prevzemanje na file:");
        }
        public FileService(IKurir status,List<string> zabranetiFajlovi) : this(status)
        {
            _zabranetiFalovi = zabranetiFajlovi;
            SrediZabranetiFajlovi();
        }
        public List<FileInfo> ZemiFajlovi(string pateka, TipFajlovi tip)
        {
            List<FileInfo> lstFajlovi = new List<FileInfo>();
            SrediPateka(ref pateka);
            if (Directory.Exists(pateka))
            {
                DirectoryInfo dirPom = new DirectoryInfo(pateka);
                lstFajlovi.AddRange(dirPom.GetFiles(VratiNastavka(tip), SearchOption.TopDirectoryOnly));
                ProveriZabranetiFajlovi(ref lstFajlovi);
                _status.addInfo(TipServisi.FileService, "Zemam fajlovi(" + tip.ToString().ToUpper() + ")=> " + pateka);
            }
            else
            {
                _status.addGreska(TipServisi.FileService, "Ne postoi direktorium! => " + pateka); 
            }
            if (lstFajlovi.Count == 0)
            {
                _status.addInfo(TipServisi.FileService, "Nema fajlovi za kopiranje! => Fajlovi(" + tip.ToString().ToUpper() + ") => " + pateka);
            }
            else
            {
                _status.addInfo(TipServisi.FileService, lstFajlovi.Count.ToString() +  " fajlovi(" + tip.ToString().ToUpper() + ") za kopiranje!");
                _status.addLog("File", tip.ToString().ToUpper() + ":" + lstFajlovi.Count.ToString() + " fajlovi.");
            }
            return lstFajlovi;
        }
        public List<FileInfo> ZemiFajlovi(string pateka, TipFajlovi tip, DateTime kojDatum)
        {
            List<FileInfo> lstFajlovi = new List<FileInfo>();

            var lstFajl = ZemiFajlovi(pateka,tip);

            foreach (FileInfo fiPom in lstFajl)
            {
                if (fiPom.LastWriteTime.Date >= kojDatum || fiPom.CreationTime.Date>=kojDatum)
                {
                    lstFajlovi.Add(fiPom);
                }
            }
            if (lstFajlovi.Count == 0)
            {
                _status.addInfo(TipServisi.FileService, "Nema fajlovi za kopiranje za IZBRANIOT DATUM("+ kojDatum.ToString("dd-MM-yyyy") +")! => Fajlovi(" + tip.ToString().ToUpper() + ") => " + pateka);
            }
            else
            {
                _status.addInfo(TipServisi.FileService, lstFajlovi.Count.ToString() + " fajlovi(" + tip.ToString().ToUpper() + ") za IZBRANIOT DATUM(" + kojDatum.ToString("dd-MM-yyyy") + ")!");
                _status.addLog("File", tip.ToString().ToUpper() + ":" + lstFajlovi.Count.ToString() + " fajlovi za IZBRANIOT DATUM(" + kojDatum.ToString("dd-MM-yyyy") + ")!");
            }
            
            return lstFajlovi;

        }
        public bool ZemiFajloviIkopiraj(string patekaSorce,string patekaDest, TipFajlovi tip)
        {
            var koiFajl = ZemiFajlovi(patekaSorce, tip);
            return KopirajFajlovi(patekaDest, koiFajl);
        }
        public bool ZemiFajloviIkopiraj(string patekaSorce, string patekaDest, TipFajlovi tip, DateTime kojDatum)
        {
            var koiFajl = ZemiFajlovi(patekaSorce, tip, kojDatum);

            return KopirajFajlovi(patekaDest, koiFajl);
        }
        public bool ZemiEdenFajlIKopiraj(string patekaSorce, string patekaDest)
        {
            FileInfo fi = ZemiEdenFajl(patekaSorce);

            return KopirajFajlovi(patekaDest, fi);
        }
        public FileInfo ZemiEdenFajl(string pateka)
        {
            FileInfo fi = null;

            if(File.Exists(pateka))
            {
                fi = new FileInfo(pateka);
                _status.addLog("File", "Dodaden e fajlot - " + fi.Name + ". Datum(file):" + fi.LastWriteTime.ToString());
            }
            else
            {
                _status.addGreska(TipServisi.FileService, "File-ot ne postoi => " + pateka);
            }
            return fi;
        }
        private bool KopirajFajlovi(string patekaDest, List<FileInfo> lstFajlovi)
        {
            if (lstFajlovi.Count==0)
            {
                return true;
            }
            bool uspeh = false; 
            try 
	        {
                SrediPateka(ref patekaDest);
                ProveriPateka(patekaDest);
		        foreach(FileInfo fiPom in lstFajlovi)
                {
                    fiPom.CopyTo(patekaDest + fiPom.Name,true);
                }
                uspeh = true;
                _status.addInfo(TipServisi.FileService, "Kopirani fajlovi => " + patekaDest);
	        }
	        catch (Exception ex)
	        {
		        uspeh=false;
                _status.addGreska(TipServisi.FileService, "Greska(KopirajFajlovi):" + ex.Message);
	        }
            return uspeh;
        }
        private bool KopirajFajlovi(string patekaDest, FileInfo fi)
        {
            bool uspeh = false;
            if(fi==null)
            {
                return uspeh;
            }
            try
            {
                
                SrediPateka(ref patekaDest);
                ProveriPateka(patekaDest);               
                fi.CopyTo(patekaDest + fi.Name,true);
                uspeh = true;
            }
            catch (Exception ex)
            {
                uspeh = false;
                _status.addGreska(TipServisi.FileService, "Greska(KopirajFajlovi):" + ex.Message);
            }
            return uspeh;
        }
        private string VratiNastavka(TipFajlovi kakovTip)
        {
            string nastavka = "";
            switch (kakovTip)
            {
                case TipFajlovi.CristalRpt:
                    nastavka = "*.rpt";
                    break;
                case TipFajlovi.XML:
                    nastavka = "*.xml";
                    break;
                case TipFajlovi.SQL:
                    nastavka = "*.sql";
                    break;
                case TipFajlovi.EXE:
                    nastavka = "*.exe";
                    break;
                case TipFajlovi.IKONI:
                    nastavka = "*.ico";
                    break;
                case TipFajlovi.TEXT:
                    nastavka = "*.txt";
                    break;
                case TipFajlovi.DLL:
                    nastavka = "*.dll";
                    break;
                case TipFajlovi.PDF:
                    nastavka = "*.pdf";
                    break;
                case TipFajlovi.DOC:
                    nastavka = "*.docx";
                    break;
                case TipFajlovi.DOCX:
                    nastavka = "*.docx";
                    break;
                case TipFajlovi.XLS:
                    nastavka = "*.xls";
                    break;
                case TipFajlovi.XLSX:
                    nastavka = "*.xlsx";
                    break;
                default:
                    nastavka = "*.*";
                    break;
            }
            return nastavka;
        }
        private void SrediPateka(ref string pateka)
        {
            if (pateka.EndsWith("\\") == false)
            {
                pateka = pateka + "\\";
            }
        }
        private void ProveriPateka(string pateka)
        {
            if(Directory.Exists(pateka) == false)
            {
                Directory.CreateDirectory(pateka);
            }
        }
        private void ProveriZabranetiFajlovi(ref List<FileInfo> listaFajl)
        {       
            if (_zabranetiFalovi.Count>0)
            {
                List<FileInfo> kopija = new List<FileInfo>();
                foreach (FileInfo fiPom in listaFajl)
                {
                    kopija.Add(fiPom);
                }

                foreach (FileInfo fiPom in kopija)
                {
                    if (_zabranetiFalovi.Contains(fiPom.Name.ToLower()))
                    {
                        listaFajl.Remove(fiPom);
                        _status.addInfo(TipServisi.FileService, "Info: Zabranet fajl => " + fiPom.Name.ToLower());
                    }
                }
            }
        }
        private void SrediZabranetiFajlovi()
        {
            for (int i = 0; i < _zabranetiFalovi.Count  ; i++)
            {
                _zabranetiFalovi[i] = _zabranetiFalovi[i].ToLower();
            }
        }
        public byte[] VratiFileVoByte(string pateka)
        {
            byte[] fajl = null;
            if (File.Exists(pateka))
            {
                fajl = File.ReadAllBytes(pateka);
            }
            else
            {
                _status.addGreska(TipServisi.FileService, "File-ot ne postoi => " + pateka);
            }

            return fajl;
        }
    }
}
