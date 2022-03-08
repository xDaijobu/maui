using System;
using Maui.Controls.Sample.Pages.Base;
using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample.Pages
{
	public partial class MultiWindowPage : BasePage
	{
		static int windowCounter = 1;

		public MultiWindowPage()
		{
			InitializeComponent();

			label.Text = "Window Count: " + (windowCounter++).ToString();
		}

		void OnNewWindowClicked(object sender, EventArgs e)
		{
			Application.Current.OpenWindow(new Window(new MultiWindowPage()));
		}

		void OnCloseWindowClicked(object sender, EventArgs e)
		{
			if (Window is not null)
				Application.Current.CloseWindow(Window);
		}
	}
}