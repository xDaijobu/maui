using System;

namespace Maui.Controls.Sample.Pages
{
	public partial class OthersPage
	{
		TestWindowOverlay overlay;

		public OthersPage()
		{
			InitializeComponent();
		}

		void TestAddOverlayWindow(object sender, EventArgs e)
		{
			overlay ??= new TestWindowOverlay(Window);
			Window.AddOverlay(overlay);
		}

		void TestRemoveOverlayWindow(object sender, EventArgs e)
		{
			if (overlay is not null)
			{
				Window.RemoveOverlay(overlay);
				overlay = null;
			}
		}

		void TestVisualTreeHelper(object sender, EventArgs e)
		{
			var overlay = Window.VisualDiagnosticsOverlay;
			overlay.RemoveAdorners();
			overlay.AddAdorner(TestButton, false);
		}

		void EnableElementPicker(object sender, EventArgs e)
		{
			Window.VisualDiagnosticsOverlay.EnableElementSelector = true;
		}
	}
}