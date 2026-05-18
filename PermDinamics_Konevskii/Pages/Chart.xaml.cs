using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PermDinamics_Konevskii.Pages
{
    /// <summary>
    /// Логика взаимодействия для Chart.xaml
    /// </summary>
    public partial class Chart : Page
    {
        public MainWindow mainWindow;
        public double actualHeightCanvas = 0;
        public double maxValue = 0;
        private double averageValue = 0;
        private Line averageLine;
        private readonly Random random = new Random();

        public DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public Chart(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            UpdateCanvasHeight();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Tick += CreateNewValue;
            dispatcherTimer.Start();
            CreateChart();
            ColorChart();
        }

        public void CreateNewValue(object sender, EventArgs e)
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            double newValue = value * (random.NextDouble() + 0.5d);
            mainWindow.pointsInfo.Add(new Classes.PointInfo(newValue));
            ControlCreateChart();
        }

        public void CreateChart()
        {
            canvas.Children.Clear();
            maxValue = mainWindow.pointsInfo.Max(point => point.value);

            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                Line line = new Line();
                line.X1 = i * 20;
                line.X2 = (i + 1) * 20;
                if (i == 0)
                    line.Y1 = actualHeightCanvas;
                else
                    line.Y1 = GetPointY(mainWindow.pointsInfo[i - 1].value);
                line.Y2 = GetPointY(mainWindow.pointsInfo[i].value);
                line.StrokeThickness = 2;
                mainWindow.pointsInfo[i].line = line;
                canvas.Children.Add(line);
            }
        }

        public void CreatePoint()
        {
            Line line = new Line();
            line.X1 = (mainWindow.pointsInfo.Count - 1) * 20;
            line.X2 = mainWindow.pointsInfo.Count * 20;
            line.Y1 = GetPointY(mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 2)].value);
            line.Y2 = GetPointY(mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].value);
            line.StrokeThickness = 2;
            mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].line = line;
            canvas.Children.Add(line);
        }

        public void ControlCreateChart()
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            if (value <= maxValue)
            {
                CreatePoint();
            }
            else
            {
                CreateChart();
            }
            ColorChart();
        }

        public void ColorChart()
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            averageValue = mainWindow.pointsInfo.Average(point => point.value);
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                if (value < averageValue)
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Red;
                else
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Green;
            }

            canvas.Width = mainWindow.pointsInfo.Count * 20 + 300;
            DrawAverageLine();
            scroll.ScrollToHorizontalOffset(canvas.Width);
            current_value.Content = "Тек. знач:" + Math.Round(value, 2);
            average_value.Content = "Сред. знач:" + Math.Round(averageValue, 2);
        }

        private void Page_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (mainWindow == null || mainWindow.pointsInfo.Count == 0)
                return;

            UpdateCanvasHeight();
            CreateChart();
            ColorChart();
        }

        private double GetPointY(double value)
        {
            if (maxValue <= 0)
                return actualHeightCanvas;

            return actualHeightCanvas - ((value / maxValue) * actualHeightCanvas);
        }

        private void DrawAverageLine()
        {
            if (averageLine != null)
            {
                canvas.Children.Remove(averageLine);
                averageLine = null;
            }

            if (mainWindow == null || mainWindow.pointsInfo.Count == 0 || show_average.IsChecked != true)
                return;

            double canvasWidth = canvas.Width;
            if (double.IsNaN(canvasWidth) || double.IsInfinity(canvasWidth) || canvasWidth <= 0)
                canvasWidth = mainWindow.pointsInfo.Count * 20 + 300;

            double averageY = GetPointY(averageValue);
            if (double.IsNaN(averageY) || double.IsInfinity(averageY))
                return;

            averageLine = new Line
            {
                X1 = 0,
                X2 = canvasWidth,
                Y1 = averageY,
                Y2 = averageY,
                Stroke = Brushes.DodgerBlue,
                StrokeDashArray = new DoubleCollection { 6, 4 },
                StrokeThickness = 2
            };
            canvas.Children.Add(averageLine);
        }

        private void UpdateCanvasHeight()
        {
            if (mainWindow == null)
                return;

            double height = mainWindow.ActualHeight > 0 ? mainWindow.ActualHeight : mainWindow.Height;
            actualHeightCanvas = Math.Max(0, height - 50d);
            canvas.Height = actualHeightCanvas;
        }

        private void AverageVisibilityChanged(object sender, RoutedEventArgs e)
        {
            DrawAverageLine();
        }
    }
}
