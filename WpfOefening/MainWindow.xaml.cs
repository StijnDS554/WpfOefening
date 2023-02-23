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

namespace WpfOefening
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

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            Achtergrond.ImageSource = new BitmapImage(new Uri(@"components/Images/geboortekaart.jpg", UriKind.Relative));
            statusLinks.Content = "Nieuw";
   
        }
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
       
    }
}
