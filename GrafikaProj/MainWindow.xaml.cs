using System;
using System.ComponentModel;
using System.Runtime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Point = System.Windows.Point;

namespace GrafikaProj
{
	/// <summary>
	/// Główne okno programu
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
        /// <summary>
        /// Obsługa ładowania obrazka
        /// </summary>
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
        /// <summary>
        /// Obsługa zdarzenie zmiany wartości slider'a jasności
        /// </summary>
        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetBrightness((int) e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        /// <summary>
        /// Obsługa zdarzenie zmiany wartości slider'a kontrastu
        /// </summary>
        private void Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetContrast((int)e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        /// <summary>
        /// Obsługa zdarzenie zmiany wartości slider'a Gammy
        /// </summary>
        private void Gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _imageCustomizator.SetGamma(e.NewValue);
            _imageCustomizator.ApplyFilters(startPoint, endPoint);
            CustomizedImageViewer.Source = _imageCustomizator.GetCustomizedSource();
        }

        /// <summary>
        /// Obsługa przycisku histogramu
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            chartWindow.Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            chartWindow.ClosingByMainWindow = true;
            chartWindow.Close();
        }

        /// <summary>
        /// Obsługa zdarzenie kliknięcia myszą dla zaznaczania obszaru
        /// </summary>
        private new void MouseDown(object sender, MouseEventArgs e)
        {
            IsSelecting = true;
            mouseStart = e.GetPosition(CustomizedImageViewerBg);
        }

        /// <summary>
        /// Obsługa zdarzenie odkliknięcia myszy dla zaznaczania obszaru
        /// </summary>
        private new void MouseUp(object sender, MouseEventArgs e)
        {
            IsSelecting = false;
        }

        /// <summary>
        /// Obsługa zdarzenie kliknięcia prawego przycisku myszy.
        /// Powoduje zresetowanie pozycji zaznaczonego obszaru
        /// </summary>
        private void ResetPosition(object sender, MouseEventArgs e)
        {
            selectedSpace.Margin = startMargin;
            startPoint = new Point(0,0);
            endPoint = new Point(590, 590);
        }

        /// <summary>
        /// Obsługa zapisu obrazka do pliku
        /// Obsługiwane formaty PNG i BMP
        /// </summary>
        private void SaveClick(object sender, System.EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            if (CustomizedImageViewer.Source == null) return;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pliki obrazów (*.bmp)|*.bmp| Pliki obrazów (*.png)|*.png";
            saveFileDialog.Title = "Zapisz obraz";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {

                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                switch (saveFileDialog.FilterIndex)
                {
                    case 0: 
                        var bmpEncoder = new BmpBitmapEncoder();
                        bmpEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)(CustomizedImageViewer.Source)));
                        bmpEncoder.Save(fs);
                        break;
                    case 1:
                    default:
                        var pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)(CustomizedImageViewer.Source)));
                        pngEncoder.Save(fs);
                        break;
                }
            }
        }
        /// <summary>
        /// Poruszanie myszy po obrazie.
        /// Za każdym poruszeniem pobiera aktualną pozycję myszy i ustawia odpowiednio obszar
        /// </summary>
        private new void MouseMove(object sender, MouseEventArgs e)
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
