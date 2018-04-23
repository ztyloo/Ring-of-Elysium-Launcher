using System; 
using System.IO;
using System.Net;
using System.Windows;
using System.ComponentModel;

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
            InstallButton.IsEnabled = false;
        }

        // Установка
        private async void Install() {
            // Получение ссылки на последнюю версию
            var client = new GitHubClient(new ProductHeaderValue("Ring-of-Elysium-Launcher"));
            var releases = await client.Repository.Release.GetAll("RoBit666", "Ring-of-Elysium-Launcher");
            var urlDownload = releases[0].Assets[0].BrowserDownloadUrl;

            WebClient webload = new WebClient();
            webload.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
            webload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

            // Загружаем файл
            webload.DownloadFileAsync(new Uri(urlDownload), Path.GetDirectoryName(PathTextBox.Text) + "/DownloadLaucher");
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

                if(!File.Exists(Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe")) { 
                    File.Move(PathTextBox.Text, Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe");
                } else {
                    File.Delete(PathTextBox.Text);
                }

                File.Move(Path.GetDirectoryName(PathTextBox.Text) + "\\DownloadLaucher", PathTextBox.Text);

                // Настройки лаунчера
                RegistryKey launcherSettings = Registry.CurrentUser.OpenSubKey("RoELauncher", true); 
                if (launcherSettings == null) launcherSettings = Registry.CurrentUser.CreateSubKey("RoELauncher", RegistryKeyPermissionCheck.ReadWriteSubTree);
                launcherSettings.SetValue("PathToGame", Path.GetDirectoryName(PathTextBox.Text) + "\\roe_game.exe", RegistryValueKind.String);

                InstallButton.Content = "INSTALLATION COMPLETED";
            }
        }
    }
}
