using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToursApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
            Manager.MainFrame = MainFrame;

            //ImportTours();
        }

        private void ImportTours()
        {
            var fileData = File.ReadAllLines(@"C:\Users\oleg-\OneDrive\Рабочий стол\Разработка ПМ\УП.01.01\import до\Туры2.txt");
            var images = Directory.GetFiles(@"C:\Users\oleg-\OneDrive\Рабочий стол\Разработка ПМ\УП.01.01\import до\Туры фото");

            foreach (var line in fileData)
            {
                var data  = line.Split('\t');

                var tempTour = new Tour
                {
                    Name = data[0].Replace("\"", ""),
                    TicketCount = int.Parse(data[2]),
                    Price = Decimal.Parse(data[3]),
                    isActual = (data[4] == "0") ? false : true,
                };

                foreach (var tourType in data[5].Replace("\"", "").Split(new String[] { ", "}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var currentType = ToursBaseEntities2.GetContext().Types.ToList().FirstOrDefault(p => p.Name == tourType);
                    if (currentType != null)
                        tempTour.Types.Add(currentType);
                }

                try
                {
                    tempTour.ImagePreview = File.ReadAllBytes(images.FirstOrDefault(p => p.Contains(tempTour.Name)));
                }
                catch (Exception ex) 
                {
                        MessageBox.Show(ex.Message);
                }

                ToursBaseEntities2.GetContext().Tours.Add(tempTour);
                ToursBaseEntities2.GetContext().SaveChanges();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                btnBack.Visibility = Visibility.Visible;
            }
            else
            {
                btnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}
