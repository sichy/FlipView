using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace FlipView
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		FlipViewViewController viewController;
		
		TickerView tickerView;
		UIView frontView;
		UIView backView;
		UIButton flipButton;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new FlipViewViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			// set the views there
			frontView = new UIView (new RectangleF (0, 0, 300, 300));
			frontView.BackgroundColor = UIColor.LightGray;
			
			backView = new UIView (new RectangleF (0, 0, 300, 300));
			backView.BackgroundColor = UIColor.Gray;
			
			tickerView = new TickerView (new RectangleF (10, 50, 300, 300));
			tickerView.FrontView = frontView;
			tickerView.BackView = backView;	
			tickerView.Duration = 1;
			
			flipButton = new UIButton (new RectangleF (10, 10, 80, 40));
			flipButton.SetTitle ("Flip", UIControlState.Normal);
			flipButton.TouchUpInside += delegate {
				tickerView.Flip (TickerViewTickDirection.Down, null);
			};
			
			viewController.View.AddSubview (tickerView);
			viewController.View.AddSubview (flipButton);
			
			return true;
		}
	}
}

