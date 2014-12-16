using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using DBAccess;


namespace ModelMapping
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Map mapping = new Map();
        string connectionString = ""; //Your oracle connection string

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutputLocation.Text) || string.IsNullOrEmpty(txtModelName.Text) || string.IsNullOrEmpty(txtProjectName.Text))
            {
                MessageBox.Show("Faltan campos por completar");
            }
            else
            {
                if (txtModelName.Text.Contains(".cs"))
                {
                    txtModelName.Text = txtModelName.Text.Split('.')[0];
                }

                if (mapping.GetColumns(connectionString, cbTables.SelectedItem.ToString(), txtOutputLocation.Text, txtProjectName.Text, txtModelName.Text))
                {
                    MessageBox.Show("Modelo generado correctamente");
                }
                else
                {
                    MessageBox.Show("Ha ocurrido algun problema durante el proceso");
                }
                
            }
        }

        private void cbTables_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> TablesList = mapping.GetTableNames(connectionString);

            if (TablesList.Count > 0)
            {
                cbTables.ItemsSource = TablesList;

                cbTables.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Ha ocurrido algun problema al conectar con la base de datos");
            }
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            txtOutputLocation.Text = mapping.SelectFolder();
        }
    }
}
