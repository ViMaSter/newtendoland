﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for ConfigPopup.xaml
    /// </summary>
    partial class ConfigPopup : Window
    {
        public static void Show(GridObjects.BaseObject tileType, Window owner)
        {
            ConfigPopup newPopup = new ConfigPopup();
            newPopup.Title = "Configure tile: "+tileType.DisplayName;
            newPopup.Owner = owner;
            newPopup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (!tileType.PopulateFields(ref newPopup.CustomContent))
            {
                return;
            }

            newPopup.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Escape)
                {
                    newPopup.Close();
                }

                if (args.Key == Key.Enter)
                {
                    tileType.ObtainData();
                    newPopup.Close();
                }
            };

            newPopup.OK.Click += (object sender, RoutedEventArgs e) =>
            {
                tileType.ObtainData();
                newPopup.Close();
            };

            newPopup.Cancel.Click += (object sender, RoutedEventArgs e) =>
            {
                newPopup.Close();
            };
            newPopup.WindowStartupLocation = WindowStartupLocation.Manual;
            newPopup.Left = owner.Left + owner.ActualWidth;
            newPopup.Top = owner.Top;
            newPopup.ShowDialog();
        }

        public ConfigPopup()
        {
            InitializeComponent();
        }
    }
}
