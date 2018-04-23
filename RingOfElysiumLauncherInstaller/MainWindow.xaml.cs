using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Octokit;

namespace RingOfElysiumLauncherInstaller {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            LastRelease();

        }

        private async void LastRelease() {
            var client = new GitHubClient(new ProductHeaderValue("Ring-of-Elysium-Launcher"));
            var releases = await client.Repository.Release.GetAll("RoBit666", "Ring-of-Elysium-Launcher");

            var assets = releases[0].Assets;
            MessageBox.Show(assets[0].BrowserDownloadUrl);
        }
    }
}
