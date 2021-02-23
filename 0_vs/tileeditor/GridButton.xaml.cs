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
using tileeditor.GridObjects;

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GridButton : Button
    {
        public GridButton()
        {
            InitializeComponent();
        }

        private void GridButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConfigPopup.Show(DataContext as BaseObject, Window.GetWindow(this) as Window);
        }
    }
}
