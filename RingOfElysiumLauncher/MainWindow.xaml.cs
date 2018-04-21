using System.Windows;
using RingOfElysiumLauncher.Data;

namespace RingOfElysiumLauncher {

	public partial class MainWindow : Window {

		LaunchParameters launchParameters = null;

		public MainWindow(string[] args) {
			InitializeComponent();

			launchParameters = new LaunchParameters(args);
		}
	}
}
