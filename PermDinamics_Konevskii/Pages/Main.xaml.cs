using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PermDinamics_Konevskii.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public MainWindow mainWindow;
        public Main(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void OpenPageChart(object sender, RoutedEventArgs e)
        {
            double value;
            if (!double.TryParse(tb_value.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out value) &&
                !double.TryParse(tb_value.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                MessageBox.Show("Введите числовое значение курса.", "Некорректное значение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (value <= 0)
            {
                MessageBox.Show("Начальное значение курса должно быть больше нуля.", "Некорректное значение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            mainWindow.pointsInfo.Clear();
            mainWindow.pointsInfo.Add(new Classes.PointInfo(value));
            mainWindow.OpenPages(MainWindow.pages.chart);
        }
    }
}
