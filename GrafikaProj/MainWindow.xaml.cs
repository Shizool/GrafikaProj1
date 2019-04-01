using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace GrafikaProj
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        ImageCustomizator imageCustomizator;
        public MainWindow()
		{
            imageCustomizator = new ImageCustomizator();
			InitializeComponent();
		}

        private void LoaderClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Załaduj obrazek";
            dialog.Filter = "Pliki obrazów (*.bmp)|*.bmp";
            if (dialog.ShowDialog() == true)
            {
                BitmapImage loadedImage = new BitmapImage(new Uri(dialog.FileName));
                this.imageCustomizator.SetSource(loadedImage);
                OriginImageViewer.Source = this.imageCustomizator.GetCustomizedSource();
                CustomizedImageViewer.Source = this.imageCustomizator.GetCustomizedSource();

            }
        }
    }
}
