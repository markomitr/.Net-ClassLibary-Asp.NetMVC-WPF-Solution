using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libPrepVerzija.Klasi
{
    class Baza
    {
        public string cnString = "";
        public string server, baza, user, pass;
        string serverSql, bazaSql, userSql, passSql, passTipSql;
        string serverApp, bazaApp, userApp, passApp, passTipApp;

        public Baza(Dictionary<string, string> paramKonf)
        {
            this.SpremiCnString(paramKonf);
        }
        private string SpremiCnString(Dictionary<string, string> paramKonf)
        {         
            serverSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_SERVER");
            bazaSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_BAZA");
            userSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_USER");
            passSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_PASSWORD");
            passTipSql = UnvFunc.RecnikZemiPole(paramKonf, "SQL_PASSWORD_TYPE");

            passSql = UnvFunc.SrediPassword(passTipSql, passSql);

            serverApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_SERVER");
            bazaApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_BAZA");
            userApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_USER");
            passApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_PASSWORD");
            passTipApp = UnvFunc.RecnikZemiPole(paramKonf, "APP_PASSWORD_TYPE");

            passApp = UnvFunc.SrediPassword(passTipApp, passApp);

            server = serverApp.Trim() != "" ? serverApp : serverSql;
            baza = bazaApp.Trim() != "" ? bazaApp : bazaSql;
            user = userApp.Trim() != "" ? userApp : userSql;
            pass = passApp.Trim() != "" ? passApp : passSql;

            cnString = "Server=" + server + ';';
            cnString += "Database=" + baza + ';';
            cnString += "User Id=" + user + ';';
            cnString += "Password=" + pass + ';';

            return cnString;

        }
       
    }
}
