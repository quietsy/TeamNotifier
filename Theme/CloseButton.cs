using System;
using System.Windows;
using TeamNotifier;
using System.Windows.Controls.Primitives;

namespace CustomChromeLibrary
{
	public class CloseButton : CaptionButton
	{
		static CloseButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseButton), new FrameworkPropertyMetadata(typeof(CloseButton)));
		}

		protected override void OnClick()
		{
			base.OnClick();
            Window.GetWindow(this).Hide();
        }
	}
}
