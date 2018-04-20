using System;
using System.Windows;

namespace RingOfElysiumLauncher {

	public partial class App : Application {

		private void Application_Startup(object sender, StartupEventArgs e) {
			MainWindow mw = new MainWindow(e.Args);
			mw.Show();
		}

	}
}
