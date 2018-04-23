using System; 
using System.IO;
using System.Net;
using System.Windows;
using System.ComponentModel;

using IWshRuntimeLibrary;
using Microsoft.Win32;

using Octokit;

namespace RingOfElysiumLauncherInstaller { 

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        // Путь к игре
        private void PathButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Ring of Elysium Client | Europa_Client.exe";
            if (ofd.ShowDialog() == true) {
                PathTextBox.Text = ofd.FileName;

                InstallButton.Content = "INSTALL";
                InstallButton.IsEnabled = true;
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e) {
            Install();

            InstallButton.Content = "PLEASE WAIT";
            InstallButton.IsEnabled = false;
        }

        // Установка
        private async void Install() {
            // Получение ссылки на последнюю версию
            var client = new GitHubClient(new ProductHeaderValue("Ring-of-Elysium-Launcher"));
            var release = await client.Repository.Release.GetLatest("RoBit666", "Ring-of-Elysium-Launcher");

            foreach(ReleaseAsset ra in release.Assets) {
                if(ra.Name == "RingOfElysiumLauncher.exe") {

                    // Создание веб-клиента
                    WebClient webload = new WebClient();
                    webload.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                    webload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                    // Загружаем файл
                    webload.DownloadFileAsync(new Uri(ra.BrowserDownloadUrl), Path.GetDirectoryName(PathTextBox.Text) + "/DownloadLaucher");

                    break;
                }
            }

        }

        // Обработчик прогресса
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e){
            InstallButton.Content = $"DOWNLOAD: {e.ProgressPercentage}%";
        }

        // Завершение загрузки
        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e) {
            if(e.Error != null) {
                InstallButton.Content = "ERROR";
                MessageBox.Show(e.Error.Message, "Error");
            } else {

                if(!System.IO.File.Exists(Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe")) {
                    System.IO.File.Move(PathTextBox.Text, Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe");
                } else {
                    System.IO.File.Delete(PathTextBox.Text);
                }

                System.IO.File.Move(Path.GetDirectoryName(PathTextBox.Text) + "\\DownloadLaucher", PathTextBox.Text);

                // Настройки лаунчера
                RegistryKey launcherSettings = Registry.CurrentUser.OpenSubKey("RoELauncher", true); 
                if (launcherSettings == null) launcherSettings = Registry.CurrentUser.CreateSubKey("RoELauncher", RegistryKeyPermissionCheck.ReadWriteSubTree);
                launcherSettings.SetValue("PathToGame", Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe", RegistryValueKind.String);

                launcherSettings.SetValue("Token", "", RegistryValueKind.String);
                launcherSettings.SetValue("Uid", "", RegistryValueKind.String);
                launcherSettings.SetValue("Language", "en", RegistryValueKind.String);
                launcherSettings.SetValue("Server", "", RegistryValueKind.String);

                // Ярлык
                WshShell shell = new WshShell();
                string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Ring of Elysium.lnk";
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = PathTextBox.Text;
                shortcut.Save();

                InstallButton.Content = "INSTALLATION COMPLETED";
            }
        }
    }
}
