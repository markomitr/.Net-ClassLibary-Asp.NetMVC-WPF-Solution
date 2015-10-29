using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using libPrepVerzija.Interfaces;
using libPrepVerzija.Klasi;

namespace IBprepVerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string patekaKonf = "";     
            string verFile = "";
            byte[] verZip = null;
            AppMum mojApp;
            Kurir kr = new Kurir();
            Recept mojRecept = new Recept(DateTime.Now.AddMonths(-4));
            //patekaKonf=@"C:\VerIBprep\Konfig.xml";
            patekaKonf ="Konfig.xml";
            //mojRecept.addSqlObj(TipProceduri.Fn);
            //mojRecept.addSqlObj(TipProceduri.Fix);
            mojRecept.addFileObj(TipFajlovi.XML);
            mojRecept.addFileObj(TipFajlovi.CristalRpt);
            mojApp = new AppMum(Aplikacii.WTRG, mojRecept, kr, patekaKonf);
            if (kr.DaliEVoRed)
            {
                mojApp.NapraviVerzija(ref verZip, ref verFile);
            }


            txtInfo.TextWrapping = TextWrapping.Wrap;
            txtErr.TextWrapping = TextWrapping.Wrap;
            txtInfo.IsReadOnly = true;
            txtErr.IsReadOnly = true;
            txtInfo.Text = kr.VratiInfo();
            txtErr.Text = kr.VratiGreski();
            
        }
    }
}
