using System;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GrafikaProj
{
    class ImageCustomizator
    {
        private BitmapSource _sourceBitmap;
        private BitmapSource _customizedBitmap;
        private int _brightness = 0;
        private double _contrast = 1.0;
        private double _gamma;
        private ChartWindow _chartWindow;

        public ImageCustomizator(ChartWindow chartWindow)
        {
            _chartWindow = chartWindow;
        }

        public void SetSource(BitmapSource source)
        {
            BitmapSource grayImage = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            _sourceBitmap = grayImage;
            _customizedBitmap = grayImage;
        }

        public BitmapSource GetCustomizedSource()
        {
            return _customizedBitmap;
        }

        public void SetBrightness(int value)
        {
            _brightness = value;
        }

        public void SetContrast(int value)
        {
            _contrast = (259.0 * (value + 255.0)) / (255.0 * (259.0 - value));
        }
        public void SetGamma(double value)
        {
            _gamma = value;
        }

        public async void ApplyFilters()
        {
            if (_sourceBitmap == null) return;
            
            var pixelsArray = new byte[_sourceBitmap.PixelHeight * _sourceBitmap.PixelWidth];
            _sourceBitmap.CopyPixels(pixelsArray, _sourceBitmap.PixelWidth, 0);
            int[] grayColorCount = new int[256];
            for (var x = 0; x < pixelsArray.Length; x++)
            {
                pixelsArray[x] = Truncate(pixelsArray[x] + _brightness);
                pixelsArray[x] = Truncate((int)(_contrast * (pixelsArray[x] - 128) + 128));
                pixelsArray[x] = Truncate((int)(255.0 * System.Math.Pow(pixelsArray[x] / 255.0, _gamma)));
                grayColorCount[pixelsArray[x]] += 1;
            }
            var temp = BitmapSource.Create(_sourceBitmap.PixelWidth, _sourceBitmap.PixelHeight, _sourceBitmap.DpiX, _sourceBitmap.DpiY, PixelFormats.Gray8, null, pixelsArray, _sourceBitmap.PixelWidth);
            _customizedBitmap = temp;
            _chartWindow.applyDataToChart(grayColorCount);
        }
        
        private static byte Truncate(int value)
        {
            if(value < 0) return 0;
            if(value > 255) return 255;

            return (byte) value;
        }

    }
}
