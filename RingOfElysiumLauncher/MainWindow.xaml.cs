using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

using RingOfElysiumLauncher.Data;

namespace RingOfElysiumLauncher {

	public partial class MainWindow : Window {

		LaunchParameters launchParameters = null;

        public MainWindow(string[] args) {
			InitializeComponent();

			launchParameters = new LaunchParameters(args);
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

    }
}
