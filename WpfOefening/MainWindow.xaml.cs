using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
        private Color selectedColor;
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
            Printen.IsEnabled = false;
            kaarten.IsEnabled = true;
        }
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                canvas.Children.Clear();
                canvas.AllowDrop = true;
                Opslaan.IsEnabled = true;
                Printen.IsEnabled = false;
                kaarten.IsEnabled = true;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Kaart";
                dlg.DefaultExt = ".kaart";
                dlg.Filter = "Kaartjes |*.kaart";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        string loadedAchtergrond = bestand.ReadLine();
                        var achtergrondinfo = loadedAchtergrond.Split(' ');
                        statusLinks.Content = dlg.SafeFileName.Split(".")[0] + " " + dlg.FileName;
                        Achtergrond.ImageSource = new BitmapImage(new Uri(@achtergrondinfo[0], UriKind.Relative));
                        if (achtergrondinfo[1] == "Geboortekaart")
                        {
                            Geboortekaart.IsChecked = true;
                        }
                        else
                        {
                            Kerstkaart.IsChecked = true;
                        }
                        int aantalBallen = Convert.ToInt32((string)bestand.ReadLine());
                        for (int i = 0; i < aantalBallen; i++)
                        {
                            string bal = bestand.ReadLine();
                            var balSettings = bal.Split(' ');
                            string color = balSettings[0];
                            double x = Convert.ToDouble(balSettings[1]);
                            double y = Convert.ToDouble(balSettings[2]);
                            Ellipse ellipse = new Ellipse();
                            ellipse.Style = this.bal.Style;

                            ellipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
                            Canvas.SetLeft(ellipse, x);
                            Canvas.SetTop(ellipse, y);
                            ellipse.MouseMove += bal_MouseMove;
                            canvas.Children.Add(ellipse);
                        }
                        KaartText.Text = bestand.ReadLine();
                        var fonts = Fonts.SystemFontFamilies;
                        var currentfont = bestand.ReadLine();
                        foreach (var font in fonts)
                        {
                            if (font.Source == currentfont)
                            {
                                KaartText.FontFamily = font;
                                Lettertype.SelectedItem = font;
                            }
                        }
                        if (!bestand.EndOfStream)
                        KaartText.FontSize = Convert.ToDouble(bestand.ReadLine());
                        fontSizeIndicator.Text = KaartText.FontSize.ToString();
                        fontSizeIndicator.FontSize = KaartText.FontSize;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("openen mislukt : " + ex.Message);
            }
        }
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "Kaart";
                dlg.DefaultExt = ".kaart";
                dlg.Filter = "Textbox documents |*.kaart";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        if (Kerstkaart.IsChecked)
                        {
                            bestand.WriteLine(Achtergrond.ImageSource.ToString() + " " + Kerstkaart.Name);
                        }
                        else
                        {
                            bestand.WriteLine(Achtergrond.ImageSource.ToString() + " " + Geboortekaart.Name);
                        }
                        bestand.WriteLine(canvas.Children.Count);
                        if (canvas.Children.Count > 0)
                        {
                            foreach (Ellipse item in canvas.Children)
                            {
                                double x = Canvas.GetLeft(item);
                                double y = Canvas.GetTop(item);
                                bestand.WriteLine(item.Fill + " " + x + " " + y);
                            }
                        }
                        bestand.WriteLine(KaartText.Text);
                        bestand.WriteLine(KaartText.FontFamily.Source);
                        bestand.WriteLine(fontSizeIndicator.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("opslaan mislukt : " + ex.Message);
            }
        }
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var result = MessageBox.Show("wilt u afsluiten?", "Afsluiten", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
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
            fontSize = (int)KaartText.FontSize;
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
            fontSize = (int)KaartText.FontSize;
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
