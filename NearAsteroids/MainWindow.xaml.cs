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

namespace NearAsteroids
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
        private void SearchByDateLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchByDate searchByDate = new SearchByDate();
            searchByDate.ShowDialog();

        }

        private void SearchByNASAID_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchByNASAID searchByNASAID = new SearchByNASAID();
            searchByNASAID.ShowDialog();
        }

        private void BrowseLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BrowseAsteroids browseAsteroids = new BrowseAsteroids();
            browseAsteroids.ShowDialog();
        }
    }
}
