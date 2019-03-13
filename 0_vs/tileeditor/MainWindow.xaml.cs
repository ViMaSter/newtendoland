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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region UI constructor
        private void CreateGridDemo()
        {
            List<List<int>> lsts = new List<List<int>>();

            for (int i = 0; i < NintendoLand.DataFormats.MapData.COLUMNS_TOTAL; i++)
            {
                lsts.Add(new List<int>());

                for (int j = 0; j < NintendoLand.DataFormats.MapData.ROWS_TOTAL; j++)
                {
                    lsts[i].Add((i * NintendoLand.DataFormats.MapData.COLUMNS_TOTAL) + j);
                }
            }

            InitializeComponent();

            lst.ItemsSource = lsts;
        }
        #endregion

        public MainWindow()
        {
            // @TODO: detect remaining temp-folder
            InitializeComponent();

            NintendoLand.TileTypes.Registrar.Populate();
            GridObjects.Registrar.Populate();

            CreateGridDemo();
        }

        ~MainWindow()
        {
            // detect remaiming temp-folder and delete it
        }

        NintendoLand.DataFormats.GameDataContainer gameDataContainer;

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
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

                Parago.Windows.ProgressDialog.Report(progress, 80, "Extracting scene file...");
                if (!MoveAndExtract(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tmp/sourceFiles/Ysi_Cmn/Common/Scene/Ysi.szs"))
                {
                    throw new System.Exception();
                }

                Parago.Windows.ProgressDialog.Report(progress, 90, "Populating GameDataContainer...");
                gameDataContainer = new NintendoLand.DataFormats.GameDataContainer(Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "tmp",
                    "sourceFiles",
                    "Ysi"
                ));

                Parago.Windows.ProgressDialog.Report(progress, 95, "Populating dropdown...");
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    foreach (string map in gameDataContainer.MapsAvailable)
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
                }));
            });
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

            VisualizeMapData((e.AddedItems[0] as ComboBoxItem).Name + ".exbin");
        }

        #region Visualize map
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, System.Uri.EscapeUriString(resourcePath));
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

        private void VisualizeMapData(string mapDataFileName)
        {
            DataFormats.MapDescriptor descriptor = DataFormats.MapDescriptor.FromGameData(mapDataFileName, gameDataContainer);
            Debug.Assert(true, "Successful conversion");
        }
        #endregion
    }
}
