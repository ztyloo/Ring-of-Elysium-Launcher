using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using RingOfElysiumLauncher.Data;

namespace RingOfElysiumLauncher {

	public partial class MainWindow : Window {

        private RegistryKey launcherSettings; // Ссылка в реестре для хранения настроек лаунчера

        public LaunchParameters launchParameters = null; // Объект параметров запуска
        private string pathToGame = ""; // Путь к игре
        public string PathToGame { get { return pathToGame; } set { PathTextBox.Text = pathToGame = value; } }


        public MainWindow(string[] args) {
			InitializeComponent();

            launchParameters = new LaunchParameters(args);

            // Настройки лаунчера
            launcherSettings = Registry.CurrentUser.OpenSubKey("RoELauncher", true);
            if(launcherSettings == null) {
                launcherSettings = Registry.CurrentUser.CreateSubKey("RoELauncher", RegistryKeyPermissionCheck.ReadWriteSubTree);

                launcherSettings.SetValue("Token", "", RegistryValueKind.String);
                launcherSettings.SetValue("Uid", "", RegistryValueKind.String);
                launcherSettings.SetValue("Language", "", RegistryValueKind.String);
                launcherSettings.SetValue("Server", "", RegistryValueKind.String);

                PlayButton.IsEnabled = false;
                PlayButton.Content = "SELECT PATH";

                SettingsGrid.Visibility = Visibility.Visible;
                SettingsButton.IsEnabled = false;
            } else {
                PathToGame = launcherSettings.GetValue("PathToGame").ToString();

                if (launchParameters.IsEmpty()) {

                    string token = launcherSettings.GetValue("Token").ToString();
                    string uid = launcherSettings.GetValue("Uid").ToString();
                    string language = launcherSettings.GetValue("Language").ToString();
                    string server = launcherSettings.GetValue("Server").ToString();
                    launchParameters = new LaunchParameters(token, uid, language, server);

                    if (launchParameters.IsEmpty()) {
                        PlayButton.IsEnabled = false;
                        PlayButton.Content = "STARTUP THROUNG GARENA";
                    }

                } else {
                    if (launchParameters.Language == "th") launchParameters.Language = "en";

                    launcherSettings.SetValue("Token", launchParameters.Token, RegistryValueKind.String);
                    launcherSettings.SetValue("Uid", launchParameters.Uid, RegistryValueKind.String);
                    launcherSettings.SetValue("Language", launchParameters.Language, RegistryValueKind.String);
                    launcherSettings.SetValue("Server", launchParameters.Server, RegistryValueKind.String);
                }
            }

            LanguageTexBox.Text = launchParameters.Language;

            // Пинг
            PingAsync("203.205.147.187", 1000, 500);
        }

        // Запуск асинхронного пинга
        private async void PingAsync(string server, int timeout, int updateTime) {
            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(PingEvent);
            while (true)  {
                await ping.SendPingAsync(server, timeout);
                await Task.Delay(updateTime);
            }
        }

        // Событие получения пинга
        private void PingEvent(object sender, PingCompletedEventArgs e) {
            if (e.Reply.Status == IPStatus.Success)
                PingLabel.Content = $"Ping: {e.Reply.RoundtripTime}";
        }

        // Настройки
        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            SettingsGrid.Visibility = Visibility.Visible;
            SettingsButton.IsEnabled = false;
        }

        // Закрыть настройки 
        private void CloseSettingsButton_Click(object sender, RoutedEventArgs e) {
            SettingsGrid.Visibility = Visibility.Collapsed;
            SettingsButton.IsEnabled = true;
        }

        // Выбор пути к игре
        private void PathButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Ring of Elysium Client | roe_game.exe";
            if (ofd.ShowDialog() == true) {
                PathToGame = ofd.FileName;

                launcherSettings.SetValue("PathToGame", ofd.FileName, RegistryValueKind.String);

                if (launchParameters.IsEmpty()) {
                    PlayButton.Content = "STARTUP THROUNG GARENA";
                } else {
                    PlayButton.IsEnabled = true;
                    PlayButton.Content = "PLAY";
                }
            }
        }

        // Запуск игры
        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            ProcessStartInfo roeProcessInfo = new ProcessStartInfo();
            roeProcessInfo.FileName = PathToGame;
            roeProcessInfo.WorkingDirectory = Path.GetDirectoryName(PathToGame);

            roeProcessInfo.Arguments = $" -garena -token={launchParameters.Token} -uid={launchParameters.Uid} -language={launchParameters.Language} -server={launchParameters.Server}";

            Process.Start(roeProcessInfo);
        }

        // Обработка изменения языка
        private void LanguageTexBox_SelectionChanged(object sender, RoutedEventArgs e) {
            launcherSettings.SetValue("Language", LanguageTexBox.Text, RegistryValueKind.String);
        }
    }
}
