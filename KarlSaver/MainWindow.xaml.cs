using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace KarlSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            win10Path = System.IO.File.ReadAllText("win10Path");
            win10PathText.Text = win10Path;
            steamPath = System.IO.File.ReadAllText("steamPath");
            steamPathText.Text = steamPath;

        }
        public string win10Path;
        public string steamPath;

        void BackupFile(string filePath)
        {
            string unixTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string[] splitPath = filePath.Split('\\');
            string file = splitPath[splitPath.Length - 1];
            Console.WriteLine("file: " + file);

            File.Copy(filePath, "backup\\" + unixTimeStamp + file);
        }

        private void btnOpenFileSteam_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                steamPath = openFileDialog.FileName;
                steamPathText.Text = steamPath;
                File.WriteAllText("steamPath", steamPath);
            }
        }
        private void btnOpenFileXbox_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                win10Path = openFileDialog.FileName;
                win10PathText.Text = win10Path;
                File.WriteAllText("win10Path", win10Path);
            }
        }

        private void SteamToXboxClick(object sender, RoutedEventArgs e)
        {
            BackupFile(win10Path);
            BackupFile(steamPath);
            string[] splitPath = win10Path.Split('\\');
            string xboxFile = splitPath[splitPath.Length - 1];

            string[] splitWin10 = win10Path.Split('\\');
            splitWin10[splitWin10.Length - 1] = xboxFile;
            string newPath = String.Join("\\", splitWin10);
            Console.WriteLine("steampath: " + steamPath);
            Console.WriteLine("newXBOXPath: " + newPath);

            File.Copy(steamPath, newPath, true);
        }

        private void XboxToSteamClick(object sender, RoutedEventArgs e)
        {
            BackupFile(win10Path);
            BackupFile(steamPath);
            string[] splitPath = steamPath.Split('\\');
            string steamFile = splitPath[splitPath.Length - 1];

            string[] splitSteam = steamPath.Split('\\');
            splitSteam[splitSteam.Length - 1] = steamFile;
            string newPath = String.Join("\\", splitSteam);
            Console.WriteLine("xboxPath: " + win10Path);
            Console.WriteLine("newSTEAMPath: " + newPath);

            File.Copy(win10Path, newPath, true);
        }

        private void steamPathText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
