using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
        const string steamPathTemplate = @"C:\Program Files (x86)\Steam)\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames\[NUMBERS]_Player.sav";
        const string win10PathTemplate = @"C:\Users\(Your username here)\AppData\Local\Packages\CoffeeStainStudios.[NUMBERS]\SystemAppData\wgs\[NUMBERS]\[NUMBERS]\[NUMBERS]";

        public string win10Path;
        public string steamPath;

        public MainWindow()
        {
            InitializeComponent();

            if(!File.Exists("steamPath"))
            {
                try
                {
                    steamPath = FindSteamPath();
                    steamPathText.Text = steamPath;
                    File.WriteAllText("steamPath", steamPath);
                }
                catch
                {
                    steamPathText.Text = steamPathTemplate;
                    File.WriteAllText("steamPath", steamPathTemplate);
                }
            }
            else
            {
                steamPath = System.IO.File.ReadAllText("steamPath");
                steamPathText.Text = steamPath;
            }
            if (!File.Exists("win10Path"))
            {
                try
                {
                    win10Path = FindWin10Path();
                    win10PathText.Text = win10Path;
                    File.WriteAllText("win10Path", win10Path);
                }
                catch
                {
                    win10PathText.Text = win10PathTemplate;
                    File.WriteAllText("win10Path", win10PathTemplate);
                }
            }
            else
            {
                win10Path = System.IO.File.ReadAllText("win10Path");
                win10PathText.Text = win10Path;
            }
        }

        void BackupFile(string filePath)
        {
            string unixTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string[] splitPath = filePath.Split('\\');
            string file = splitPath[splitPath.Length - 1];
            Console.WriteLine("file: " + file);

            File.Copy(filePath, "backup\\" +"["+ unixTimeStamp +"] "+ file);
        }

        string FindSteamPath()
        {
            string _steamPath = @"C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames\";
            string[] steamSaveFolderFiles = Directory.GetFiles(_steamPath);

            foreach (var file in steamSaveFolderFiles)
            {
                string[] splitString = file.Split('_');

                if (splitString[splitString.Length - 1] == "Player.sav")
                {
                    return  file;
                }
            }

            return null;
        }

        string FindWin10Path()
        {
            string _win10Path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\";
            string[] win10Games = Directory.GetDirectories(_win10Path);

            foreach(var filePath in win10Games)
            {
                if(filePath.Contains("CoffeeStainStudios.DeepRockGalactic"))
                {
                    _win10Path = filePath + @"\SystemAppData\wgs\";
                    _win10Path = getLongestFolderDir(_win10Path) + @"\";
                    _win10Path = getLongestFolderDir(_win10Path) + @"\";
                    _win10Path = getLongestFile(_win10Path);
                    return _win10Path;
                }
            }

            return null;
        }

        string getLongestFile(string path)
        {
            return Directory.GetFiles(path).OrderByDescending(s => s.Length).First();
        }
        string getLongestFolderDir(string path)
        {
            return Directory.GetDirectories(path).OrderByDescending(s => s.Length).First();
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to copy your Steam save to Xbox/Win10?", "Steam To Xbox/Win10", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    return;
            }

            try
            {
                BackupFile(win10Path);
                BackupFile(steamPath);
            }
            catch (Exception error)
            {
                MessageBox.Show("Error, Unable to Backup Save files. Cancelling Conversion\n" + error + "\n" + win10Path + "\n" + steamPath, "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string[] splitPath = win10Path.Split('\\');
                string xboxFile = splitPath[splitPath.Length - 1];

                string[] splitWin10 = win10Path.Split('\\');
                splitWin10[splitWin10.Length - 1] = xboxFile;
                string newPath = String.Join("\\", splitWin10);
                Console.WriteLine("steampath: " + steamPath);
                Console.WriteLine("newXBOXPath: " + newPath);

                File.Copy(steamPath, newPath, true);
            }
            catch(Exception error)
            {
                MessageBox.Show("Error in savegame conversion\n" + error, "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PlaySound();
            MessageBox.Show("Savegame successfully converted!", "Conversion Success", MessageBoxButton.OK);
        }

        private void XboxToSteamClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to copy your Xbox/Win10 save to Steam?", "Xbox/Win10 To Steam", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    return;
            }

            try
            {
                BackupFile(win10Path);
                BackupFile(steamPath);
            }
            catch (Exception error)
            {
                MessageBox.Show("Error, Unable to Backup Save files. Cancelling Conversion\n" + error, "Backup Error" , MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string[] splitPath = steamPath.Split('\\');
                string steamFile = splitPath[splitPath.Length - 1];

                string[] splitSteam = steamPath.Split('\\');
                splitSteam[splitSteam.Length - 1] = steamFile;
                string newPath = String.Join("\\", splitSteam);
                Console.WriteLine("xboxPath: " + win10Path);
                Console.WriteLine("newSTEAMPath: " + newPath);

                File.Copy(win10Path, newPath, true);
            }
            catch (Exception error)
            {
                MessageBox.Show("Error in savegame conversion\n" + error, "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Savegame successfully converted!", "Conversion Success", MessageBoxButton.OK);
            PlaySound();
        }

        private void btnSteamAutoPath_Click(object sender, RoutedEventArgs e)
        {
            string autoPath;

            try
            {
                autoPath = FindSteamPath();
            }
            catch
            {
                MessageBox.Show("Unable to find save file automatically, please enter the path manually", "File Find Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (autoPath != null)
            {
                steamPath = autoPath;
                steamPathText.Text = steamPath;
                File.WriteAllText("steamPath", steamPath);
                MessageBox.Show("Successfully got Steam save file location", "Find Find Success", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Unable to find save file automatically, please enter the path manually", "File Find Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXboxAutoPath_Click(object sender, RoutedEventArgs e)
        {
            string autoPath;

            try
            {
                autoPath = FindWin10Path();
            }
            catch
            {
                MessageBox.Show("Unable to find save file automatically, please enter the path manually", "File Find Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (autoPath != null)
            {
                win10Path = autoPath;
                win10PathText.Text = win10Path;
                File.WriteAllText("win10Path", win10Path);
                MessageBox.Show("Successfully got Xbox/Win10 save file location", "Find Find Success", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Unable to find save file automatically, please enter the path manually", "File Find Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void PlaySound()
        {
            /*SoundPlayer player = new SoundPlayer(@"C:\Users\Dylan\source\repos\KarlSaver\KarlSaver\bin\Debug\rock_and_stone.wav");
            player.Load();
            player.Play();
            */

            MediaPlayer mediaplayer = new MediaPlayer();
            mediaplayer.Open(new Uri(@"C:\Users\Dylan\source\repos\KarlSaver\KarlSaver\bin\Debug\rock_and_stone.wav"));
            mediaplayer.Volume = 0.2;
            mediaplayer.Play();
        }
        
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(win10PathTemplate + "/n" + steamPathTemplate);
        }
    }
}
