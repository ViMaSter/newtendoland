using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace tileeditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // detect remaining temp-folder
            InitializeComponent();
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
                    CreateNoWindow = true
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
            openFileDialog.Filter = "Yoshi's package file (Ysi_Cmn.pack)|Ysi_Cmn.pack";
            if (openFileDialog.ShowDialog() != true)
            {
                // no file selected
                return;
            }

            if (!MoveAndExtract(openFileDialog.FileName))
            {
                throw new System.Exception();
            }

            if (!MoveAndExtract(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tmp/sourceFiles/Ysi_Cmn/Common/Scene/Ysi.szs"))
            {
                throw new System.Exception();
            }

            PopulateList();
        }

        void PopulateList()
        {
            IEnumerable<string> maps = Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp", "sourceFiles", "Ysi"), "MapData*.exbin");
            LevelSelector.Items.Clear();
            foreach (string map in maps)
            {
                LevelSelector.Items.Add(Path.GetFileNameWithoutExtension(map));
            }
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

            using (var memStream = new MemoryStream())
            {
                MapData mapData = MapData.Load(Path.Combine(
                                      Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                      "tmp",
                                      "sourceFiles",
                                      "Ysi",
                                      (e.AddedItems[0] as string) + ".exbin"
                                  ));
                LevelData.Text = mapData.Visualize();
            }
        }
    }
}
