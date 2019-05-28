using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using LiveCharts;
using LineSeries = LiveCharts.Wpf.LineSeries;

namespace GrafikaProj
{
    /// <summary>
    /// Okno wyświetlające histogram
    /// </summary>
    public partial class ChartWindow : Window
    {
        public bool ClosingByMainWindow = false;
        /// <summary>
        /// Konstruktor inicjujący domyślne wartości wykresu
        /// </summary>
        public ChartWindow()
        {
            InitializeComponent();
            Chart.DataTooltip = null;
            Chart.Hoverable = false;
            Chart.ToolTip = null;
            Chart.AxisX[0].Labels = Enumerable.Range(0, 256).Select(element => element.ToString()).ToList();
        }

        /// <summary>
        /// Metoda służy do zamykania okna
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (ClosingByMainWindow == false)
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// Metoda służy do zastosowania nowych danych do wykresu odcieni szarości
        /// </summary>
        /// <param name="grayColorCount">Tablica zawierająca liczbe wystąpień poszczególnych odcieni</param>
        public void applyDataToChart(int[] grayColorCount)
        {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    var seriesCollection = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Values = new ChartValues<int>(grayColorCount),
                            LineSmoothness = 0,
                            PointGeometry = null
                        }
                    };
                    Chart.AxisY[0].MinValue = 0;
                    Chart.AxisY[0].Unit = 1000;
                    Chart.Series = seriesCollection;
                });
        }
    }
}