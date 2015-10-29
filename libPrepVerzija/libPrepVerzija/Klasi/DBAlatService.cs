using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using libPrepVerzija.Interfaces;

namespace libPrepVerzija.Klasi
{
    class DBAlatService
    {
        Baza db;
        SqlConnection cn = new SqlConnection();
        string _pateka = "";
        string _imefajl = "";
        IKurir _status;
        int _defSQlTimeout = 15 * 60000;
        private string Pateka
        {
            get
            {
                return _pateka + _imefajl;
            }
        }
        public DBAlatService(IKurir status,string cnString, string pateka, string imefajl)
        {
            cn = new SqlConnection(cnString);
            _pateka = pateka;
            _imefajl = imefajl;
            _status = status;
        }
        public DBAlatService(IKurir status, Dictionary<string, string> paramKonf)
        {
            _status = status;
            db = new Baza(paramKonf);
            SrediKonfig(paramKonf);
        }
        public void NapraviSkript(TipProceduri tip)
        {
            NapraviProc(tip);
        }
        public bool ZemiTrigeri()
        {
            bool uspeh = true;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = _defSQlTimeout;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            string Drop,ImeTrig;

            try
            {
                if (UnvFunc.OtvoriKonekcija(ref cn) == false)
                {
                    _status.addGreska(TipServisi.DBAlatService, "Ne mozam da vospostavam konekcija so bazata! =>" + cn.ConnectionString);
                    return false ;
                }
                _status.addInfo(TipServisi.DBAlatService, "Zemam lista od Trigeri.");
                cmd.Connection = cn;
                cmd.CommandText = "Select name From Sysobjects Where XTYPE='TR' And Name Not Like 'pltr%' And Name Not Like 'retr%'";
                cmd.CommandType = CommandType.Text;
                da.SelectCommand = cmd;
                da.Fill(ds, "Trigeri");

            }
            catch (Exception ex)
            {
                _status.addGreska(TipServisi.DBAlatService, "Greska:(ZemiTrigeri)" + ex.Message);
                uspeh = false;
            }

            if(uspeh)
            {
                _status.addInfo(TipServisi.DBAlatService, "Zemam telo(kod) na sekoj Triger i zapisuvam vo fajl. =>" + Pateka);
               foreach (DataRow red in ds.Tables["Trigeri"].Rows)
               {

                   ImeTrig = Convert.ToString(red["name"]);

                   TextWriter output = File.AppendText(Pateka);  

                  Drop = "IF EXISTS (SELECT name FROM sysobjects WHERE name = '#####' AND type = 'TR') DROP TRIGGER #####";
                  Drop = Drop.Replace("#####", ImeTrig);             
                  output.WriteLine(Drop);
                  output.WriteLine("\nGo");
                 
                    try
                    {
                        
                        cmd.Connection = cn;
                        cmd.CommandText = "sp_helptext";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@objname", ImeTrig);
                        da.SelectCommand = cmd;
                        da.Fill(ds, "TeloTrig");

                    }
                    catch (Exception ex)
                    {
                       uspeh =false;
                        _status.addGreska(TipServisi.DBAlatService,"Greska(ZemiTrigeri):" + ex.Message);
                    }

                    if (uspeh)
                    {
                        foreach (DataRow redTrig in ds.Tables["TeloTrig"].Rows)
                        {
                            string line = "";
                            line = Convert.ToString(redTrig[0]);
                            if (!(line.Trim().StartsWith("/**") || line.Trim() == ""))
                            {
                                output.Write(line);
                            }
                        }

                        output.WriteLine("\nGo");
                        output.Close();
                        da.Dispose();
                        ds.Tables["TeloTrig"].Clear();

                    }
                }
               _status.addInfo(TipServisi.DBAlatService, "Zavrsiv so zapisuvanje na Trigeri vo fajl! =>" + Pateka);
               _status.addLog("SQL", "Dodadeni se Trigeri.");
            }
            return uspeh;
        }
        private bool ZemiProeceduri(ref  DataRow []red, TipProceduri tipProc)
        {
            bool uspeh = true;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = _defSQlTimeout;
            SqlDataAdapter da = new SqlDataAdapter();
           DataSet  dsTek = new DataSet();   
                try
                {
                    if (UnvFunc.OtvoriKonekcija(ref cn) == false)
                    {
                        _status.addGreska(TipServisi.DBAlatService, "Ne mozam da vospostavam konekcija so bazata! =>" + cn.ConnectionString);
                        return false;
                    }
                    _status.addInfo(TipServisi.DBAlatService, "Zemam lista objekti od bazata - TIP:" + tipProc.ToString().ToUpper());
                    cmd.Connection = cn;
                    cmd.CommandText = "sp_stored_procedures";
                    cmd.CommandType = CommandType.StoredProcedure;

                    da.SelectCommand = cmd;
                    da.Fill(dsTek, "Proceduri");

                }
                catch (Exception ex)
                {
                    uspeh = false;
                    _status.addGreska(TipServisi.DBAlatService, "Greska(ZemiProceduri):" + ex.Message);
                }
                if (uspeh)
                {
                    red = dsTek.Tables["Proceduri"].Select(this.PripremiSlect(tipProc));
                }
                _status.addInfo(TipServisi.DBAlatService, "Vkupno "  +  red.Length.ToString() +  " objekti!");
           return  uspeh ;
        }
        private bool PripremiTextSqlObj(string ImeProc, ref string strObjSql,TipProceduri tip)
        {
            bool uspeh = true;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = _defSQlTimeout;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();
            string Drop1, Drop2;                

            Drop1 = "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[#####]'))";

            if(ImeProc.StartsWith("fix_") || ImeProc.StartsWith("fn_"))
            {
                Drop2 = "drop function [dbo].[#####]";              
            }
            else
            {
                Drop2 = "drop procedure [dbo].[#####]";
            }


            if (ImeProc.StartsWith("fix_"))
            {
                Drop1 = "if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[#####]'))";
                Drop2 = "begin declare @SSQL as varchar(8000) set @SSQL = '";
            }

            Drop1 = Drop1.Replace("#####", ImeProc);
            Drop2 = Drop2.Replace("#####", ImeProc);

            sb.AppendLine(Drop1);
            sb.AppendLine(Drop2);

            if (ImeProc.StartsWith("fix_"))
            {
                sb.AppendLine("\nGo");
            }

            try
            {
                if (UnvFunc.OtvoriKonekcija(ref cn) == false)
                {
                    return false;
                }       

                cmd.Connection = cn;
                cmd.CommandText = "sp_helptext";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@objname", ImeProc);
                da.SelectCommand = cmd;
                da.Fill(ds, "TeloProc");

            }
            catch (Exception ex)
            {
                uspeh = false;
                _status.addGreska(TipServisi.DBAlatService, "Greska(PripremiTextSqlObj):" + ex.Message);
            }

            if (uspeh)
            {

                foreach (DataRow redProc in ds.Tables["TeloProc"].Rows)
                {
                    string line = "";
                    line = Convert.ToString(redProc[0]);
                    if (!line.Trim().StartsWith("/**"))
                    {
                        if (ImeProc.StartsWith("fix_"))
                        {
                            line = line.Replace("'", "''");
                        }
                        if (line.Trim() != "")
                            sb.Append(line);
                    }
                }

                if (ImeProc.StartsWith("fix_"))
                {
                    sb.AppendLine("' exec(@SSQL)");
                    sb.AppendLine("end");
                }
                sb.AppendLine("\nGo");
                da.Dispose();
                ds.Tables["TeloProc"].Clear();

                strObjSql = sb.ToString();
            }
            return uspeh;
        }
        private string PripremiSlect(TipProceduri tip)
        {
            string select = "PROCEDURE_NAME LIKE ";
            string uslov="";
            switch (tip)
            {
                case TipProceduri.SP:
                    {
                        uslov = select + "'sp_%'";
                        _status.addLog("SQL", "Dodadeni se SP proceduri.");
                        break;
                    }
                case TipProceduri.RK:
                    {
                        uslov = select + "'rk_%'";
                        _status.addLog("SQL", "Dodadeni se RK proceduri.");
                        break;
                    }
                case TipProceduri.Fix:
                    {
                        uslov = select + "'fix_%'";
                        _status.addLog("SQL", "Dodadeni se FIX funkcii.");
                        break;
                    }
                case TipProceduri.Fn:
                    {
                        uslov = select + "'fn_%'";
                        _status.addLog("SQL", "Dodadeni se FN funkcii.");
                        break;
                    }
                case TipProceduri.Wtrg:
                    {
                        uslov = select + "'sp_%' OR " + select + " 'fn_%' OR " + select + " 'fix_%'"  ;
                        break;
                    }
                case TipProceduri.WApt:
                    {
                        uslov = select + "'sp_%' OR " + select + "'rk_%' OR " + select + " 'fn_%' OR " + select + " 'fix_%'";
                        break;
                    }
                case TipProceduri.Site:
                    {
                        uslov = select +  "'%'";
                        _status.addLog("SQL", "Dodadeni se site objekti od sql.");
                        break;
                    }
            }
          
            return uslov;
        }
        private bool NapraviProc(TipProceduri tip)
        {
            bool uspeh = true;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = _defSQlTimeout;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
               DataRow[] redDS = null;
               uspeh = ZemiProeceduri(ref redDS,tip);
               if (uspeh)
                {
                    _status.addInfo(TipServisi.DBAlatService, "Zemam telo na objektite od baza.");
                    TextWriter output = File.AppendText(Pateka);
                    foreach (DataRow red in redDS)
                    {
                        string ImeProc = "";
                        ImeProc = (Convert.ToString(red["PROCEDURE_NAME"]).Substring(0, Convert.ToString(red["PROCEDURE_NAME"]).Length - 2)).ToLower();
                        string strProc = "";
                        if (PripremiTextSqlObj(ImeProc, ref strProc, tip))
                        {
                            output.WriteLine(strProc);
                        }
                        else
                        {
                            break;
                        }

                    }
                    output.Close();
                    _status.addInfo(TipServisi.DBAlatService, "Zavrsiv so zapisuvanje objekti od TIP:" + tip.ToString().ToUpper());
                }
                UnvFunc.ZatvoriKonekcija(ref cn);
            }
            catch (Exception ex)
            {
                uspeh = false;
                _status.addGreska(TipServisi.DBAlatService, "Greska(NapraviProc):" + ex.Message);
            }        
            return uspeh;
        }
        private void SrediKonfig(Dictionary<string, string> paramKonf)
        {
            _imefajl = UnvFunc.RecnikZemiPole(paramKonf, "APP_IME") + ".sql";
            _pateka = @UnvFunc.RecnikZemiPole(paramKonf, "EXPORT_PATEKA_APP_TMP");
            _pateka = UnvFunc.SrediPateka(_pateka) + "SQL" + "\\";
            cn = new SqlConnection(db.cnString);
            _status.addLog("Baza", "Server:" + db.server);
            _status.addLog("Baza", "Baza:" + db.baza);
            _status.addLog("Baza", "User:" + db.user);
        }
    }
}
