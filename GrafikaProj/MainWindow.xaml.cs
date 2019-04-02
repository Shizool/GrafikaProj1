using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        double defaultBorderTop;
        double defaultBorderBottom;
        double defaultBorderLeft;
        double defaultBorderRight;
        int maxWidth = 600;
        int maxHeight = 600;

        public MainWindow()
		{
            imageCustomizator = new ImageCustomizator();
			InitializeComponent();
            defaultBorderTop = ImageBorder.Margin.Top;
            defaultBorderBottom = ImageBorder.Margin.Bottom;
            defaultBorderLeft = ImageBorder.Margin.Left;
            defaultBorderRight = ImageBorder.Margin.Right;

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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.imageCustomizator.SetBrightness((int) e.NewValue);
            this.imageCustomizator.ApplyFilters();
            CustomizedImageViewer.Source = this.imageCustomizator.GetCustomizedSource();
        }

        private void BottomRangeSliderValueChanged(object sender, RoutedEventArgs e)
        {
            if (ImageBorder != null)
            {
                string[] parameters = sender.ToString().Split('-');
                double min = 0;
                double max = 0;
                double.TryParse(parameters.First(), out min);
                double.TryParse(parameters.Last(), out max);
                double left = defaultBorderLeft + (maxWidth * min);
                double right = defaultBorderRight + (maxWidth * (1 - max));
                Thickness margin = ImageBorder.Margin;
                margin.Left = left;
                margin.Right = right;
                ImageBorder.Margin = margin;
            }
        }

        private void LeftRangeSliderValueChanged(object sender, RoutedEventArgs e)
        {
            if (ImageBorder != null)
            {
                string[] parameters = sender.ToString().Split('-');
                double min = 0;
                double max = 0;
                double.TryParse(parameters.First(), out min);
                double.TryParse(parameters.Last(), out max);
                double top = defaultBorderTop + (maxHeight * (1 - max));
                double bottom = defaultBorderBottom + (maxHeight * min);
                Thickness margin = ImageBorder.Margin;
                margin.Top = top;
                margin.Bottom = bottom;
                ImageBorder.Margin = margin;
            }
        }
    }
}
