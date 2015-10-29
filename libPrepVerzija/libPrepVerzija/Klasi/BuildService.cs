using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libPrepVerzija.Klasi
{
    public class BuildService
    {
       public  string _errorLog;
       string _patekVSdevExe;
       string _solutionFile;
       string _solutionConfig;

       public BuildService(string patekaVSdev, string solutionFile)
       {
           this._patekVSdevExe = patekaVSdev;
           this._solutionFile = solutionFile;
           this._solutionConfig = "Release";
       }
       public BuildService(Dictionary<string, string> paramKonf)
       {
           SrediKonfig(paramKonf);
       }
       public bool Build()
       {
           bool uspeh = false;
           // get temp logfile path
           string logFileName = System.IO.Path.GetTempFileName();
           // populate process environment
           System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
           psi.FileName = _patekVSdevExe;
           psi.ErrorDialog = true;
           psi.Arguments = "\"" + _solutionFile + "\"" + @" /rebuild " + _solutionConfig + " /out " + logFileName;
           // start process
           System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
           // instruct process to wait for exit
           p.WaitForExit();
           // get return code
           int exitCode = p.ExitCode;
           // free process resources
           p.Close();
           // if there was a build error, display build log to console			
           if (exitCode != 0)
           {
               uspeh = false;
               System.IO.TextReader reader = System.IO.File.OpenText(logFileName);
               _errorLog = reader.ReadToEnd();
               reader.Close();
               //
           }
           else
           {
               uspeh = true;
           }
           // delete temp logfile
           System.IO.File.Delete(logFileName);
           // return process exit code
           return uspeh;
       }
       private void SrediKonfig(Dictionary<string, string> paramKonf)
       {
           _patekVSdevExe = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKAVSEXE");
           _solutionFile = @UnvFunc.RecnikZemiPole(paramKonf, "APP_PATEKASLN");
           _solutionConfig = "Release";
       }
    }
}
