using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GrafikaProj
{
    /// <summary>
    /// Klasa obsługująca przetwarzanie obrazu
    /// </summary>
    class ImageCustomizator
    {
        /// <summary>
        /// Obraz źródłowy
        /// </summary>
        private BitmapSource _sourceBitmap;
        /// <summary>
        /// Przetworzony obraz
        /// </summary>
        private BitmapSource _customizedBitmap;
        /// <summary>
        /// wartość jasność
        /// </summary>
        private int _brightness = 0;
        /// <summary>
        /// wartość kontrastu
        /// </summary>
        private double _contrast = 1.0;
        /// <summary>
        /// wartość gammy
        /// </summary>
        private double _gamma;
        /// <summary>
        /// Okno z histogramem
        /// </summary>
        private ChartWindow _chartWindow;
        /// <summary>
        /// Mapa wyliczonych wartości kontrastu dla każdego odcienia
        /// </summary>
        private int[] contrastMap = new int[256];
        /// <summary>
        /// Mapa wyliczonych wartości gammy dla każdego odcienia
        /// </summary>
        private int[] gammaMap = new int[256];
        /// <summary>
        /// Mapa wyliczonych wartości jasności dla każdego odcienia
        /// </summary>
        private int[] brightnessMap = new int[256];

        public ImageCustomizator(ChartWindow chartWindow)
        {
            _chartWindow = chartWindow;
        }

        /// <summary>
        /// Ustawianie źródłowego obrazu.
        /// </summary>
        /// <param name="source">Źródłowy obraz w postaci BitmapSource</param>
        public void SetSource(BitmapSource source)
        {
            BitmapSource grayImage = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            _sourceBitmap = new WriteableBitmap(grayImage);
            _customizedBitmap = grayImage;
        }
        /// <summary>
        /// Pobieranie przetworzonego obrazu
        /// </summary>
        /// <returns>Przetworzony obraz w postaci BitmapSource</returns>
        public BitmapSource GetCustomizedSource()
        {
            return _customizedBitmap;
        }
        /// <summary>
        /// Ustawianie jasności
        /// </summary>
        /// <param name="value">wartość jasności</param>
        public void SetBrightness(int value)
        {
            _brightness = value;
        }

        /// <summary>
        /// Ustawianie kontrakstu
        /// </summary>
        /// <param name="value">wartość kontrastu</param>
        public void SetContrast(int value)
        {
            _contrast = (259.0 * (value + 255.0)) / (255.0 * (259.0 - value));
        }

        /// <summary>
        /// ustawianie gammy
        /// </summary>
        /// <param name="value">wartość gammy</param>
        public void SetGamma(double value)
        {
            _gamma = value;
        }

        /// <summary>
        /// Zastosowanie filtrów jasności, kontrastu i gammy w prostokącie definiowanym przez startPoint i endPoint
        /// </summary>
        /// <param name="startPoint">Punkt startowy zaznaczonego obszaru</param>
        /// <param name="endPoint">Punkt końcowy zaznaczonego obszaru</param>
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
        /// <summary>
        /// Metoda służy do obcinania podanej wartości do zakresu <0, 255>
        /// </summary>
        /// <param name="value">wyliczona wartość</param>
        /// <returns>obciętą wartość</returns>
        private static int Truncate(int value)
        {
            if(value < 0) return 0;
            if(value > 255) return 255;

            return value;
        }

        /// <summary>
        /// Przygotowanie map wartości dla wszystkich możliwych odcieni szarości
        /// </summary>
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
