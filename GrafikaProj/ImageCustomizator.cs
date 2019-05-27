using System;
using System.Diagnostics;
using System.Runtime;
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
        private int[] contrastMap = new int[256];
        private int[] gammaMap = new int[256];
        private int[] brightnessMap = new int[256];

        public ImageCustomizator(ChartWindow chartWindow)
        {
            _chartWindow = chartWindow;
        }

        public void SetSource(BitmapSource source)
        {
            BitmapSource grayImage = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            _sourceBitmap = new WriteableBitmap(grayImage);
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

        public void ApplyFilters(Point startPoint, Point endPoint)
        {
            if (_sourceBitmap == null) return;
            prepareMaps();

            var area = new Int32Rect(
                (int) (_sourceBitmap.PixelWidth * (startPoint.X / 590)),
                (int) (_sourceBitmap.PixelHeight * (startPoint.Y / 590)), 
                (int)((_sourceBitmap.PixelWidth * (endPoint.X / 590)) - (_sourceBitmap.PixelWidth * (startPoint.X / 590))),
                (int)((_sourceBitmap.PixelHeight * (endPoint.Y / 590)) - (_sourceBitmap.PixelHeight * (startPoint.Y / 590)))
                );

            var pixelsArrayCopy = new byte[(int)(((_sourceBitmap.PixelWidth * (endPoint.X / 590)) - (_sourceBitmap.PixelWidth * (startPoint.X / 590))) * ((_sourceBitmap.PixelHeight * (endPoint.Y / 590)) - (_sourceBitmap.PixelHeight * (startPoint.Y / 590))))];
            _sourceBitmap.CopyPixels(area, pixelsArrayCopy, (int)((_sourceBitmap.PixelWidth * (endPoint.X / 590)) - (_sourceBitmap.PixelWidth * (startPoint.X / 590))), 0);

            int[] grayColorCount = new int[256];
            var arrayLength = pixelsArrayCopy.Length;

            for (var x = 0; x < arrayLength; x++)
            {
                var tmpPixel = (int) pixelsArrayCopy[x];
                tmpPixel = brightnessMap[tmpPixel];
                tmpPixel = contrastMap[tmpPixel];
                pixelsArrayCopy[x] = (byte) gammaMap[tmpPixel];
                grayColorCount[tmpPixel] += 1;
            }
            
            var wrBitmap = (WriteableBitmap)(_sourceBitmap.Clone());
            wrBitmap.WritePixels(area, pixelsArrayCopy,
                (int) ((_sourceBitmap.PixelWidth * (endPoint.X / 590)) -
                       (_sourceBitmap.PixelWidth * (startPoint.X / 590))), 0);
            _customizedBitmap = wrBitmap.Clone();
            _chartWindow.applyDataToChart(grayColorCount);
        }
        
        private static int Truncate(int value)
        {
            if(value < 0) return 0;
            if(value > 255) return 255;

            return value;
        }

        private void prepareMaps()
        {
            for (var x = 0; x < 256; x++)
            {
                brightnessMap[x] = Truncate(x + _brightness);
                contrastMap[x] = Truncate((int)(_contrast * (x - 128) + 128));
                gammaMap[x] = Truncate((int)(255.0 * System.Math.Pow(x / 255.0, _gamma)));
            }
        }

    }
}
