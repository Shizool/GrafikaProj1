using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace GrafikaProj
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
    {
        private readonly ImageCustomizator _imageCustomizator;
        private readonly double _defaultBorderTop;
        private readonly double _defaultBorderBottom;
        private readonly double _defaultBorderLeft;
        private readonly double _defaultBorderRight;
        private new const int MaxWidth = 600;
        private new const int MaxHeight = 600;
        private ChartWindow chartWindow = new ChartWindow();

        public MainWindow()
		{
            _imageCustomizator = new ImageCustomizator(chartWindow);
			InitializeComponent();
//            _defaultBorderTop = ImageBorder.Margin.Top;
//            _defaultBorderBottom = ImageBorder.Margin.Bottom;
//            _defaultBorderLeft = ImageBorder.Margin.Left;
//            _defaultBorderRight = ImageBorder.Margin.Right;

        }

        private void LoaderClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {Title = "Załaduj obrazek", Filter = "Pliki obrazów (*.bmp)|*.bmp"};
            if (dialog.ShowDialog() != true) return;
            var loadedImage = new BitmapImage(new Uri(dialog.FileName));
            _imageCustomizator.SetSource(loadedImage);
            OriginImageViewer.Source = _imageCustomizator.GetCustomizedSource();
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetBrightness((int) e.NewValue);
            _imageCustomizator.ApplyFilters();
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

//        private void BottomRangeSliderValueChanged(object sender, RoutedEventArgs e)
//        {
//            if (ImageBorder == null) return;
//            
//            var parameters = sender.ToString().Split('-');
//            double.TryParse(parameters.First(), out var min);
//            double.TryParse(parameters.Last(), out var max);
//            var left = _defaultBorderLeft + MaxWidth * min;
//            var right = _defaultBorderRight + MaxWidth * (1 - max);
//            var margin = ImageBorder.Margin;
//            margin.Left = left;
//            margin.Right = right;
//            ImageBorder.Margin = margin;
//        }
//
//        private void LeftRangeSliderValueChanged(object sender, RoutedEventArgs e)
//        {
//            if (ImageBorder == null) return;
//            
//            var parameters = sender.ToString().Split('-');
//            double.TryParse(parameters.First(), out var min);
//            double.TryParse(parameters.Last(), out var max);
//            var top = _defaultBorderTop + MaxHeight * (1 - max);
//            var bottom = _defaultBorderBottom + MaxHeight * min;
//            var margin = ImageBorder.Margin;
//            margin.Top = top;
//            margin.Bottom = bottom;
//            ImageBorder.Margin = margin;
//        }

        private void Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetContrast((int)e.NewValue);
            _imageCustomizator.ApplyFilters();
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        private void Gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetGamma(e.NewValue);
            _imageCustomizator.ApplyFilters();
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            chartWindow.Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            chartWindow.ClosingByMainWindow = true;
            chartWindow.Close();
        }
    }
}
