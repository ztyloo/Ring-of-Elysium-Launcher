using System;
using System.Windows;
using System.Windows.Interop;

namespace RingOfElysiumLauncherInstaller.Styles {

	internal static class LocalExtensions {

		public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action) {
            if ( ((FrameworkElement)templateFrameworkElement).TemplatedParent is Window window )
                action(window);
        }

		public static IntPtr GetWindowHandle(this Window window) {
			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle;
		}
	}

	public partial class WindowStyle {

		void CloseButtonClick(object sender, RoutedEventArgs e) {
			sender.ForWindowFromTemplate(w => SystemCommands.CloseWindow(w));
		}

		void MinButtonClick(object sender, RoutedEventArgs e) {
			sender.ForWindowFromTemplate(w => SystemCommands.MinimizeWindow(w));
		}
	}
}
