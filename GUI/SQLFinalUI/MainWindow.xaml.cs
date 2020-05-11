using MySql.Data.MySqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLFinalUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string MasterConnection = "Server=mi3-ss13.a2hosting.com;Port=3306;Database=fractur3_hotel;UID=fractur3_FraWor;password=FraWor;";
        private string lastCustomer = "", lastOrder = "";
        public MainWindow()
        {
            InitializeComponent();
            populateCustomers();
        }

        private void populateCustomers()
        {
            using (MySqlConnection connection = new MySqlConnection(MasterConnection))
            {
                string query = "select * from customers;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        var result = command.ExecuteReader();
                        List_Customers.Items.Clear();
                        List_OrderDates.Items.Clear();

                        int count = 0;
                        while (result.Read())
                        {
                            List_Customers.Items.Add(string.Format("{0}", result.GetString(1)));
                            count++;
                        }
                        result.Close();
                        if (List_Customers.Items.Count == 0)
                        {
                            List_Customers.Items.Add("No customers found");
                        }
                        Timestamp_Customers.Text = "Last Fetched: " + DateTime.Now.ToString("mm/dd/yyyy") + "  at " + DateTime.Now.ToString("hh:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        List_Customers.Items.Add("ERROR encountered with Database Connection, whoops.");
                        Console.WriteLine("\n\n" + ex.Message + "\n\n");
                    }
                    connection.Close();
                }
            }
        }
        private void populateOrderDates(string CustomerNum)
        {
            using (MySqlConnection connection = new MySqlConnection(MasterConnection))
            {
                string query = "select orders.* from orders join customers on orders.customerNumber = customers.customerNumber where customerName = '"+ CustomerNum + "';";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        var result = command.ExecuteReader();
                        List_OrderDates.Items.Clear();

                        int count = 0;
                        while (result.Read())
                        {
                            List_OrderDates.Items.Add(string.Format("{0} {1}", result.GetString(0), result.GetString(1).Split()[0]));
                            count++;
                        }
                        result.Close();
                        if (List_OrderDates.Items.Count == 0)
                        {
                            List_OrderDates.Items.Add("No orders found");
                        }
                        lastCustomer = CustomerNum;
                        Timestamp_Orders.Text = "Last Fetched: " + DateTime.Now.ToString("mm/dd/yyyy") + "  at " + DateTime.Now.ToString("hh:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        List_OrderDates.Items.Add("ERROR encountered with Database Connection, whoops.");
                        Console.WriteLine("\n\n" + ex.Message + "\n\n");
                    }
                    connection.Close();
                }
            }
        }
        private void populateOrderData(string OrderNum)
        {
            using (MySqlConnection connection = new MySqlConnection(MasterConnection))
            {
                string query = "select * from orderdetails where orderNumber = '" + OrderNum + "';";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        var result = command.ExecuteReader();
                        List_OrderData.Items.Clear();

                        int count = 0;
                        while (result.Read())
                        {
                            List_OrderData.Items.Add(string.Format("{0} {1} {2} {3} {4}", result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3), result.GetString(4)));
                            count++;
                        }
                        result.Close();
                        if (List_OrderData.Items.Count == 0)
                        {
                            List_OrderData.Items.Add("No order data found");
                        }
                        lastOrder = OrderNum;
                        Timestamp_Data.Text = "Last Fetched: " + DateTime.Now.ToString("mm/dd/yyyy") + "  at " + DateTime.Now.ToString("hh:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        List_OrderData.Items.Add("ERROR encountered with Database Connection, whoops.");
                        Console.WriteLine("\n\n" + ex.Message + "\n\n");
                    }
                    connection.Close();
                }
            }
        }
        private void List_Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(List_Customers.SelectedItem != null)
            {
                populateOrderDates(List_Customers.SelectedItem.ToString());
                List_OrderData.Items.Clear();
                Timestamp_Data.Text = "";
            }
        }

        private void List_OrderDates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(List_OrderDates.SelectedItem != null)
            {
                populateOrderData(List_OrderDates.SelectedItem.ToString().Split()[0]);
            }
        }

        private void Btn_refresh_customers_Click(object sender, RoutedEventArgs e)
        {
            populateCustomers();
            List_OrderDates.Items.Clear();
            List_OrderData.Items.Clear();
            lastOrder = "";
            lastCustomer = "";
            Timestamp_Data.Text = "";
            Timestamp_Orders.Text = "";
        }

        private void Btn_refresh_orders_Click(object sender, RoutedEventArgs e)
        {
            if(lastCustomer != "")
            {
                populateOrderDates(lastCustomer);
                List_OrderData.Items.Clear();
                lastOrder = "";
                Timestamp_Data.Text = "";
            }
        }

        private void Btn_refresh_orderdata_Click(object sender, RoutedEventArgs e)
        {
            if(lastOrder != "")
            {
                populateOrderData(lastOrder);
            }
        }
    }
}