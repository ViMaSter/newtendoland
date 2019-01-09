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

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for ConfigPopup.xaml
    /// </summary>
    partial class ConfigPopup : Window
    {
        TileTypes.TileType associatedType;

        public static void Show(TileTypes.TileType tileType)
        {
            ConfigPopup newPopup = new ConfigPopup();
            if (!tileType.PopulateFields(ref newPopup.CustomContent))
            {
                return;
            }

            newPopup.OK.Click += (object sender, RoutedEventArgs e) =>
            {
                tileType.ObtainData();
                newPopup.Close();
            };

            newPopup.Cancel.Click += (object sender, RoutedEventArgs e) =>
            {
                newPopup.Close();
            };
            newPopup.ShowDialog();
        }

        public ConfigPopup()
        {
            InitializeComponent();
        }
    }
}
