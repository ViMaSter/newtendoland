using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Resource helper
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }
        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            return GetResourcePaths(assembly)
                .Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (System.Collections.DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
        #endregion

        #region UI constructor
        private void CreateObjectPicker()
        {
            // create object picker
            //  create rows
            int rowsRequired = (int)Math.Ceiling(Tile.PickableTypes.Length / (float)ObjectPicker.ColumnDefinitions.Count);
            for (int i = 0; i < rowsRequired; i++)
            {
                ObjectPicker.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            //  create buttons
            int row = 0;
            int column = 0;
            TileTypes.TileType.ForEach((TileTypes.TileType type) =>
            {
                Button newButton = new Button
                {
                    Content = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/TileTypes/" + (char)type.MemoryIdentifier + ".png", UriKind.Absolute)),
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    ToolTip = type.DisplayName
                };
                newButton.Click += (object sender, RoutedEventArgs e) => ConfigPopup.Show(type, this);
                Grid.SetColumn(newButton, column);
                Grid.SetRow(newButton, row);
                ObjectPicker.Children.Add(newButton);

                column++;
                if (column == ObjectPicker.ColumnDefinitions.Count)
                {
                    column = 0;
                    row++;
                }
            });
        }

        private void CreatePlacementGrid()
        {
            // create placement grid
            for (int row = 0; row < MapData.ROWS_VISIBLE; row++)
            {
                for (int column = 0; column < MapData.COLUMNS_VISIBLE; column++)
                {
                    imageElements[row, column] = new Image();
                    Grid.SetRow(imageElements[row, column], row);
                    Grid.SetColumn(imageElements[row, column], column);
                    ImageGrid.Children.Add(imageElements[row, column]);

                    Border borderElement = new Border();
                    borderElement.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    borderElement.BorderThickness = new Thickness(1, 1, (column + 1) == MapData.COLUMNS_VISIBLE ? 1 : 0, (row + 1) == MapData.ROWS_VISIBLE ? 1 : 0);
                    Grid.SetRow(borderElement, row);
                    Grid.SetColumn(borderElement, column);
                    ImageGrid.Children.Add(borderElement);

                    textBoxElements[row, column] = new TextBox();
                    Grid.SetRow(textBoxElements[row, column], row);
                    Grid.SetColumn(textBoxElements[row, column], column);
                    textBoxElements[row, column].TextWrapping = TextWrapping.NoWrap;
                    textBoxElements[row, column].TextAlignment = TextAlignment.Center;
                    textBoxElements[row, column].VerticalAlignment = VerticalAlignment.Center;
                    textBoxElements[row, column].Text = "";
                    textBoxElements[row, column].Visibility = Visibility.Hidden;
                    textBoxElements[row, column].Background = new SolidColorBrush(Color.FromArgb(0xDF, 0xFF, 0xFF, 0xFF));
                    textBoxElements[row, column].Foreground = Brushes.Black;
                    textBoxElements[row, column].BorderBrush = Brushes.Transparent;
                    ImageGrid.Children.Add(textBoxElements[row, column]);
                }
            }
        }
        #endregion

        Image[,] imageElements = new Image[MapData.ROWS_VISIBLE, MapData.COLUMNS_VISIBLE];
        TextBox[,] textBoxElements = new TextBox[MapData.ROWS_VISIBLE, MapData.COLUMNS_VISIBLE];
        public MainWindow()
        {
            // @TODO: detect remaining temp-folder
            InitializeComponent();

            TileTypes.TileType.PopulateRegistar();

            CreateObjectPicker();
            CreatePlacementGrid();
        }

        ~MainWindow()
        {
            // detect remaiming temp-folder and delete it
        }

        bool MoveAndExtract(string filePath)
        {
            string newTargetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tmp/sourceFiles/";
            Directory.CreateDirectory(newTargetFolder);
            File.Copy(filePath, newTargetFolder + Path.GetFileName(filePath), true);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "py",
                    Arguments = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Scripts/SARC/SARCExtract.py " + newTargetFolder + Path.GetFileName(filePath),
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();

            string extractedFolderName = newTargetFolder + Path.GetFileNameWithoutExtension(filePath) + "/";
            DirectoryInfo dirInfo = new DirectoryInfo(extractedFolderName);
            return dirInfo.Exists;
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            // open and extract SZS
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Yoshi Fruit Cart package file (Ysi_Cmn.pack)|Ysi_Cmn.pack";
            if (openFileDialog.ShowDialog() != true)
            {
                // no file selected
                return;
            }

            Parago.Windows.ProgressDialogResult result = Parago.Windows.ProgressDialog.Execute(this, "Loading data...", (progress) => {
                Parago.Windows.ProgressDialog.Report(progress, 40, "Extracting game package file...");
                if (!MoveAndExtract(openFileDialog.FileName))
                {
                    throw new System.Exception();
                }

                Parago.Windows.ProgressDialog.Report(progress, 90, "Extracting scene file...");
                if (!MoveAndExtract(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tmp/sourceFiles/Ysi_Cmn/Common/Scene/Ysi.szs"))
                {
                    throw new System.Exception();
                }

                Parago.Windows.ProgressDialog.Report(progress, 95, "Populating list...");
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    PopulateList();
                }));
            });

        }

        void PopulateList()
        {
            IEnumerable<string> maps = Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp", "sourceFiles", "Ysi"), "MapData*.exbin");
            foreach (string map in maps)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = Path.GetFileNameWithoutExtension(map);
                int levelIndex = int.Parse(Path.GetFileNameWithoutExtension(map).Replace("MapData", ""));
                if (levelIndex == 99)
                {
                    item.Content = "Tutorial stage";
                }
                else if (levelIndex > 49)
                {
                    item.Content = "Gate " + (levelIndex + 1) + " (UNUSED)";
                }
                else
                {
                    item.Content = "Gate " + (levelIndex + 1);
                }
                LevelSelector.Items.Add(item);
            }
            LevelSelector.IsEnabled = true;
            LevelSelector.SelectedIndex = 1;
        }

        private void ParseFile_Click(object sender, RoutedEventArgs e)
        {
            // open and parse .exbin
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp", "sourceFiles", "Ysi");
            openFileDialog.Filter = "Extracted binary-files (*.exbin)|*.exbin";
            if (openFileDialog.ShowDialog() != true)
            {
                // no file selected
                return;
            }

        }

        private void LevelSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                return;
            }

            if ((e.AddedItems[0] as ComboBoxItem).Name == "default")
            {
                if (e.RemovedItems.Count > 0)
                {
                    ((ComboBox)sender).SelectedItem = e.RemovedItems[0];
                }
                return;
            }

            using (var memStream = new MemoryStream())
            {
                MapData mapData = MapData.Load(Path.Combine(
                                      Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                      "tmp",
                                      "sourceFiles",
                                      "Ysi",
                                      (e.AddedItems[0] as ComboBoxItem).Name + ".exbin"
                                  ));
                //LevelData.Text = mapData.Visualize();
                for (int row = 0; row < MapData.ROWS_VISIBLE; row++)
                {
                    for (int column = 0; column < MapData.COLUMNS_VISIBLE; column++)
                    {
                        Tile tile = mapData.GetItem(row, column);

                        // update text
                        textBoxElements[row, column].Text = tile.GetIndex();

                        // update image
                        if (tile.ToString().Length == 0)
                        {
                            imageElements[row, column].Source = new BitmapImage();
                            continue;
                        }

                        string path = "Resources/TileTypes/" + tile.ToString() + ".png";
                        if (!ResourceExists(path))
                        {
                            // fallback to edge-case image
                            path = "Resources/TileTypes/" + tile.GetTileType() + "_.png";
                            if (!ResourceExists(path))
                            {
                                // fallback to non-indexed image
                                path = "Resources/TileTypes/" + tile.GetTileType() + ".png";
                                if (!ResourceExists(path))
                                {
                                    path = "Resources/TileTypes/unknown.png";
                                }
                            }
                        }

                        imageElements[row, column].Source = new BitmapImage(new Uri("pack://application:,,,/" + path, UriKind.Absolute));
                    }
                }

            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl)
            {
                foreach (TextBox textBox in textBoxElements)
                {
                    if (textBox.Text.Length > 0)
                    {
                        textBox.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl)
            {
                foreach (TextBox textBox in textBoxElements)
                {
                    textBox.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
