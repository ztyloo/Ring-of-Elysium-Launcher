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

            // Настройки лаунчера
            launcherSettings = Registry.CurrentUser.OpenSubKey("RoELauncher", true);
            if(launcherSettings == null) {
                PlayButton.IsEnabled = false;
                PlayButton.Content = "SELECT PATH";

                SettingsGrid.Visibility = Visibility.Visible;
                SettingsButton.IsEnabled = false;
            } else {
                PathToGame = launcherSettings.GetValue("PathToGame").ToString();
            }

            launchParameters = new LaunchParameters(args);

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
            ofd.Filter = "Ring of Elysium Client | Europa_Client.exe";
            if (ofd.ShowDialog() == true) {
                PathToGame = ofd.FileName;

                if (launcherSettings == null) launcherSettings = Registry.CurrentUser.CreateSubKey("RoELauncher", RegistryKeyPermissionCheck.ReadWriteSubTree);
                launcherSettings.SetValue("PathToGame", ofd.FileName, RegistryValueKind.String);

                PlayButton.IsEnabled = true;
                PlayButton.Content = "PLAY";
            }
        }
    }
}
