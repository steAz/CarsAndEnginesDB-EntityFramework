using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace siszarp4
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MyDbContext ctx;
        Car oldCar;
        int newId;
        public Window1(MyDbContext ctx, Car oldCar, int newId)
        {
            InitializeComponent();
            this.ctx = ctx;
            this.oldCar = oldCar;
            this.newId = newId;
        }
        private void saveClicked(object sender, RoutedEventArgs e)
        {
            double disp;
            double.TryParse(displacementText.Text, out disp);
            double hp;
            double.TryParse(hpText.Text, out hp);
            int year;
            Int32.TryParse(yearText.Text, out year);
            Engine engine = new Engine
            {
                Displacement = disp,
                HorsePower = hp,
                Model = engineModelText.Text
            };
            Car car = new Car
            {
                Id = newId,
                Model = modelText.Text,
                Engine = engine,
                Year = year
            };
            if (oldCar == null)
            {
                //dodanie nowego
                ctx.Cars.Add(car);
            }
            else
            {
                //edycja
                car.Id = oldCar.Id;
                ctx.Cars.Remove(oldCar);
                ctx.Cars.Add(car);
            }
            ctx.SaveChanges();
            this.Close();
        }
    }
}
