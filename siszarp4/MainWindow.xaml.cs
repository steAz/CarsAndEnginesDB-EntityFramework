using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace siszarp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Car> carsList;
        MyDbContext ctx;
        int myCount = 0;
        public void sortClicked(object sender, System.EventArgs e)
        {
            var column = myGrid.Columns[2]; //engine
            ListSortDirection sortDirection = ListSortDirection.Ascending;
            if (myGrid.Columns[2].SortDirection == sortDirection)
            {
                sortDirection = ListSortDirection.Descending;
            }
            myGrid.Items.SortDescriptions.Clear();
            myGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));
            myGrid.Items.Refresh();
        }

        public MainWindow()
        {
            InitializeComponent();
            ctx = new MyDbContext();
            carsList = ctx.Cars.Include("Engine").ToList();

            myGrid.Columns.Add(new DataGridTextColumn { Header = "Id", Binding = new Binding("Id") });
            myGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("Model") });
            myGrid.Columns.Add(new DataGridTextColumn { Header = "Engine", Binding = new Binding("Engine") });
            myGrid.Columns.Add(new DataGridTextColumn { Header = "Year", Binding = new Binding("Year") });
            myGrid.Columns.Add(new DataGridTextColumn { Header = "EngineType", Binding = new Binding("EngineType") });
            foreach (Car car in carsList)
            {
                myGrid.Items.Add(new Row { Id = car.Id, Model = car.Model, Engine = car.Engine.ToString(), Year = car.Year, EngineType = car.Engine.Model == "TDI" ? "diesel" : "petrol" });
            }
            myGrid.Sorting += new DataGridSortingEventHandler(sortClicked);
            myCount = myGrid.Items.Count;
        }

        public class Row
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string Engine { get; set; }
            public int Year { get; set; }
            public string EngineType { get; set; }
        }

        private void findClicked(object sender, RoutedEventArgs e)
        {
            string colName = comboInfo.Text; // Year / Model
            string toFind = textInfo.Text;

            ItemCollection rows = new DataGrid().Items;
            rows.Clear();
            myGrid.Items.Clear();
            foreach (Car car in carsList)
            {
                myGrid.Items.Add(new Row { Id = car.Id, Model = car.Model, Engine = car.Engine.ToString(), Year = car.Year, EngineType = car.Engine.Model == "TDI" ? "diesel" : "petrol" });
            }
            if (toFind == "" || colName == "none")
            {
                myGrid.Items.Refresh();
                return;
            }
            if (colName == "Model")
            {
                foreach (Row row in myGrid.Items)
                {
                    if (row.Model.Contains(toFind))
                    {
                        rows.Add(new Row { Id = row.Id, Model = row.Model, Engine = row.Engine.ToString(), Year = row.Year, EngineType = row.Model == "TDI" ? "diesel" : "petrol" });
                    }
                }
            }
            else
            {
                int x = 0;
                if (Int32.TryParse(toFind, out x))
                {
                    foreach (Row row in myGrid.Items)
                    {
                        if ((x == row.Year))
                        {
                            rows.Add(new Row { Id = row.Id, Model = row.Model, Engine = row.Engine.ToString(), Year = row.Year, EngineType = row.Model == "TDI" ? "diesel" : "petrol" });
                        }
                    }
                }
            }
            myGrid.Items.Clear();
            foreach (Row row in rows)
            {
                myGrid.Items.Add(row);
            }
            myGrid.Items.Refresh();
        }

        public void myRefresh()
        {
            carsList = ctx.Cars.Include("Engine").ToList();
            myGrid.Items.Clear();
            foreach (Car car in carsList)
            {
                myGrid.Items.Add(new Row { Id = car.Id, Model = car.Engine.Model, Engine = car.Engine.ToString(), Year = car.Year, EngineType = car.Engine.Model == "TDI" ? "diesel" : "petrol" });
            }
            myGrid.Items.Refresh();
        }

        private void addCarClicked(object sender, RoutedEventArgs e)
        {
            Window1 wnd1 = new Window1(ctx, null, myCount);
            wnd1.Closed += ChildWindowClosed;
            wnd1.Show();
        }

        private void rowDoubleClicked(object sender, RoutedEventArgs e)
        {
            int rowIndex = myGrid.SelectedIndex;
            Row newId = (Row)myGrid.Items.GetItemAt(rowIndex);
            Car oldCar = ctx.Cars.Where(x => x.Id == newId.Id).FirstOrDefault();
            Window1 wnd1 = new Window1(ctx, oldCar, myCount);
            wnd1.modelText.Text = oldCar.Model;
            wnd1.displacementText.Text = oldCar.Engine.Displacement.ToString();
            wnd1.hpText.Text = oldCar.Engine.HorsePower.ToString();
            wnd1.yearText.Text = oldCar.Year.ToString();
            wnd1.engineModelText.Text = oldCar.Engine.Model;
            wnd1.Closed += ChildWindowClosed;
            wnd1.Show();
        }
        private void ChildWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= ChildWindowClosed;
            myRefresh();
        }
        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            e.Handled = true;
            findClicked(sender, e);
        }
    }
}
