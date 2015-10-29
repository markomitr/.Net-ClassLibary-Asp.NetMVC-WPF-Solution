using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace libPrepVerzija.Klasi
{
    public static class UnvFunc
    {

        public static string RecnikZemiPole(Dictionary<string, string> recnik, string kluc)
        {
            string vrednost = "";
            if(recnik !=null && recnik.ContainsKey(kluc))
            {
                vrednost = @recnik[kluc].ToString();
            }

            return vrednost;
        }
        public static bool OtvoriKonekcija(string cnString, ref SqlConnection sqlCn)
        {
            bool uspeh = false;
            if (sqlCn == null)
                sqlCn = new SqlConnection(cnString);

            try
            {
                if (sqlCn != null)
                {
                    
                    if (sqlCn.State !=  ConnectionState.Open)
                    {
                        sqlCn.ConnectionString = cnString;
                        sqlCn.Open();
                        uspeh = true;
                    }
                    else
                    {
                        uspeh = true;
                    }
                }
            }
            catch (Exception ex)
            {
                uspeh = false;
            }
            return uspeh;

        
        }
        public static bool OtvoriKonekcija(ref SqlConnection sqlCn)
        {
            bool uspeh = false;

            try
            {
                if (sqlCn != null)
                {
                    if (sqlCn.State != ConnectionState.Open)
                    {
                        sqlCn.Open();
                        uspeh = true;
                    }
                    else
                    {
                        uspeh = true;
                    }
                }
            }
            catch (Exception ex)
            {
                uspeh = false;
            }
            return uspeh;


        }
        public static bool ZatvoriKonekcija(ref SqlConnection sqlCn)
        {
            try
            {
                if (sqlCn != null)
                {
                    if (sqlCn.State != ConnectionState.Closed)
                    {
                        sqlCn.Close();
                    }
                }
            }
            catch (Exception ex)
            { }
            return true;
        }
        public static string SpremiCnString(Dictionary<string, string> paramKonf)
        {
            string cnString = "";
            string server, baza, user, pass;
            string serverSql, bazaSql, userSql, passSql,passTipSql;
            string serverApp, bazaApp, userApp, passApp,passTipApp;

            serverSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_SERVER");
            bazaSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_BAZA");
            userSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_USER");
            passSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_PASSWORD");
            passTipSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_PASSWORD_TYPE");

            passSql = SrediPassword(passTipSql, passSql);

            serverApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_SERVER");
            bazaApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_BAZA");
            userApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_USER");
            passApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_PASSWORD");
            passTipApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_PASSWORD_TYPE");

            passApp =SrediPassword(passTipApp, passApp);

            server = serverApp.Trim() != "" ? serverApp : serverSql;
            baza = bazaApp.Trim() != "" ? bazaApp : bazaSql;
            user = userApp.Trim() != "" ? userApp : userSql;
            pass = passApp.Trim() != "" ? passApp : passSql;

            cnString = "Server=" + server  +';';
            cnString += "Database=" + baza  +';';
            cnString += "User Id=" + user  +';';
            cnString += "Password=" + pass + ';';

            return cnString;
            
        }
        public static string SrediPassword(string tip, string password)
        {
            string sredenPass = "";
            switch (tip.ToUpper())
            {
                case "TEXT":
                    {

                        sredenPass = password;
                        break;
                    }
                case "01":
                    {
                        var data = UnvFunc.GetBytesFromBinaryString(password);
                        var text = Encoding.ASCII.GetString(data);
                        sredenPass = text;
                        break;
                    }
                default:
                    {
                        sredenPass = password;
                        break;
                    }

            }
            return sredenPass;

        }
        public static  Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }
        public  static string SrediPateka(string pateka)
        {
            if (pateka.EndsWith("\\") == false)
            {
                pateka = pateka + "\\";
            }
            return pateka;
        }
        public static string GenerImeToken(string ime)
        {
            string uique = "";
            Random rdm = new Random();
            rdm.Next();
            uique= ime + vratiDatumIVremeVoString() + "_" +rdm.Next().ToString();
            return uique;
        }
        private static string  vratiDatumIVremeVoString()
        {
            return "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss");
        }
    }
}
