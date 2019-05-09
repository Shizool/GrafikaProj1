using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using LiveCharts;
using LineSeries = LiveCharts.Wpf.LineSeries;

namespace GrafikaProj
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public bool ClosingByMainWindow = false;
        public ChartWindow()
        {
            InitializeComponent();
            Chart.DataTooltip = null;
            Chart.Hoverable = false;
            Chart.ToolTip = null;
            Chart.AxisX[0].Labels = Enumerable.Range(0, 256).Select(element => element.ToString()).ToList();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ClosingByMainWindow == false)
            {
                e.Cancel = true;
                Hide();
            }
        }

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