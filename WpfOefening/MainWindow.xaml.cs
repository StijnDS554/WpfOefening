using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfOefening.components;

namespace WpfOefening
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color selectedColor;
        private readonly balStyle style;
        private Ellipse draggedBal;
        private Point startDragPoint;
        private int fontSize = 30;
        private FontFamily FontFamily;
        public MainWindow()
        {
            InitializeComponent();
            combobox.ItemsSource = typeof(Colors).GetProperties();
            combobox.SelectedIndex = 0;
            Achtergrond2.ImageSource = new BitmapImage(new Uri(@"components/Images/vuilnisbak.png", UriKind.Relative));
            selectedColor = (Color)(combobox.SelectedItem as PropertyInfo).GetValue(null, null);
            bal.Fill = new SolidColorBrush(selectedColor);
            this.ResizeMode = ResizeMode.CanMinimize;
            Lettertype.SelectedIndex = 0;
            KaartText.FontFamily = Lettertype.SelectedItem as FontFamily;
            KaartText.FontSize = fontSize;
            fontSizeIndicator.FontSize = fontSize;
            fontSizeIndicator.Text = fontSize.ToString();
        }

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            canvas.AllowDrop = true;
            canvas.Children.Clear();
            if (Kerstkaart.IsChecked)
            { Achtergrond.ImageSource = new BitmapImage(new Uri(@"components/Images/kerstkaart.jpg", UriKind.Relative)); }
            else
            {
            Achtergrond.ImageSource = new BitmapImage(new Uri(@"components/Images/geboortekaart.jpg", UriKind.Relative));
            Geboortekaart.IsChecked = true;
            }
            Achtergrond2.ImageSource = new BitmapImage(new Uri(@"components/Images/vuilnisbak.png", UriKind.Relative));
            statusLinks.Content = "Nieuw";
            
            Opslaan.IsEnabled = true;
            Printen.IsEnabled = true;
            kaarten.IsEnabled = true;
        }
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void PrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Kerstkaart_Click(object sender, RoutedEventArgs e)
        {
            Achtergrond.ImageSource = new BitmapImage(new Uri(@"components/Images/kerstkaart.jpg", UriKind.Relative));
            Kerstkaart.IsChecked = true;
            Geboortekaart.IsChecked = false;
        }

        private void Geboortekaart_Click(object sender, RoutedEventArgs e)
        {
            Achtergrond.ImageSource = new BitmapImage(new Uri(@"components/Images/geboortekaart.jpg", UriKind.Relative));
            Kerstkaart.IsChecked = false;
            Geboortekaart.IsChecked = true;
        }
        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedColor = (Color)(combobox.SelectedItem as PropertyInfo).GetValue(null, null);
            bal.Fill = new SolidColorBrush(selectedColor);
        }

        private void canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Equals(bal))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            canvas.Children.Remove(draggedBal);
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = draggedBal.Fill;
            ellipse.Stroke = draggedBal.Stroke;
            ellipse.StrokeThickness = draggedBal.StrokeThickness;
            ellipse.Height = draggedBal.Height;
            ellipse.Width = draggedBal.Width;
            Point dropPoint = e.GetPosition(canvas);
            Canvas.SetLeft(ellipse, dropPoint.X);
            Canvas.SetTop(ellipse, dropPoint.Y);
            ellipse.MouseMove += new MouseEventHandler(bal_MouseMove);
            canvas.Children.Add(ellipse);

        }

        private void bal_MouseMove(object sender, MouseEventArgs e)
        {
            draggedBal = (Ellipse)sender;
            if ((e.LeftButton == MouseButtonState.Pressed) && (draggedBal.Fill != Brushes.White))
            {
                DataObject sleepKleur = new DataObject(typeof(Brush), draggedBal.Fill);
                DragDrop.DoDragDrop(draggedBal, sleepKleur, DragDropEffects.Move);
            }
            if ((e.LeftButton == MouseButtonState.Pressed) && (draggedBal.Parent == canvas))
                canvas.Children.Remove(draggedBal);
        }


        private void Vuilbak_Drop(object sender, DragEventArgs e)
        {
            canvas.Children.Remove(draggedBal);
        }

        private void plus_Click(object sender, RoutedEventArgs e)
        {
            if (fontSize < 40)
            {
                fontSize += 1;
                fontSizeIndicator.FontSize = fontSize;
                KaartText.FontSize = fontSize;
                fontSizeIndicator.Text = fontSize.ToString();
            }
        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {
            if (fontSize > 10)
            {
                fontSize += -1;
                fontSizeIndicator.FontSize = fontSize;
                KaartText.FontSize = fontSize;
                fontSizeIndicator.Text = fontSize.ToString();
            }
        }

        private void Lettertype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KaartText.FontFamily = Lettertype.SelectedItem as FontFamily;
        }
    }
}
