using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Point = System.Windows.Point;

namespace GrafikaProj
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
    {
        private readonly ImageCustomizator _imageCustomizator;
        private ChartWindow chartWindow = new ChartWindow();

        private bool IsSelecting = false;
        private Point? mouseStart;
        private Point startPoint = new Point(0,0), endPoint = new Point(590,590);
        private Thickness startMargin;

        public MainWindow()
		{

            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            _imageCustomizator = new ImageCustomizator(chartWindow);
			InitializeComponent();

        }

        private void LoaderClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {Title = "Załaduj obrazek", Filter = "Pliki obrazów (*.bmp)|*.bmp"};
            if (dialog.ShowDialog() != true) return;
            var loadedImage = new BitmapImage(new Uri(dialog.FileName));
            _imageCustomizator.SetSource(loadedImage);
            OriginImageViewer.Source = _imageCustomizator.GetCustomizedSource();
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();

            startMargin = CustomizedImageViewer.Margin;
            Console.WriteLine(CustomizedImageViewer.Stretch);
            UpdateLayout();
            Console.WriteLine(CustomizedImageViewer.ActualWidth);
            Console.WriteLine(CustomizedImageViewer.ActualHeight);
            CustomizedImageViewerBg.MouseLeftButtonDown += MouseDown;
            CustomizedImageViewerBg.MouseLeftButtonUp += MouseUp;
            CustomizedImageViewerBg.MouseMove += MouseMove;
            CustomizedImageViewerBg.MouseRightButtonUp += ResetPosition;

        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetBrightness((int) e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }


        private void Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetContrast((int)e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        private void Gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetGamma(e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
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

        private void MouseDown(object sender, MouseEventArgs e)
        {
            IsSelecting = true;
            // Save the start point.
            mouseStart = e.GetPosition(CustomizedImageViewerBg);
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            IsSelecting = false;
        }
        private void ResetPosition(object sender, MouseEventArgs e)
        {
            selectedSpace.Margin = startMargin;
            startPoint = new Point(0,0);
            endPoint = new Point(590, 590);
        }

        private void SaveClick(object sender, System.EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap|*.bmp";
            saveFileDialog.Title = "Zapisz obraz";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource) (CustomizedImageViewer.Source)));
                encoder.Save(fs);
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsSelecting) return;

            var mouseEnd = e.GetPosition(CustomizedImageViewerBg);
            var startX = mouseStart.Value.X;
            var startY = mouseStart.Value.Y;
            var endX = mouseEnd.X;
            var endY = mouseEnd.Y;
            if (startX > endX)
            {
                var tmp = startX;
                startX = endX;
                endX = tmp;
            }
            if (startY > endY)
            {
                var tmp = startY;
                startY = endY;
                endY = tmp;
            }
            startPoint = new Point(startX, startY);
            endPoint = new Point(endX, endY);
            var margin = selectedSpace.Margin;
            margin.Top = CustomizedImageViewerBg.Margin.Top + startY;
            margin.Left = CustomizedImageViewerBg.Margin.Left + startX;
            margin.Bottom = 243 + (590 - endY);
            margin.Right = 600 - endX;
            selectedSpace.Margin = margin;
        }
    }
}
