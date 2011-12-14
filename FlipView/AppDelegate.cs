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
		
		FlipView flipView;
		UIImageView frontView;
		UIImageView backView;
		UIButton flipButtonDown;
		UIButton flipButtonUp;

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
			frontView = new UIImageView (new RectangleF (0, 0, 300, 226)); // make sure the height is even number, so it can easily make the half of it
			frontView.Image = UIImage.FromFile ("galaxy1.png");
			
			backView = new UIImageView (new RectangleF (0, 0, 300, 226));
			backView.Image = UIImage.FromFile ("galaxy2.png");
			
			flipView = new FlipView (new RectangleF (10, 128, 300, 226));
			flipView.FrontView = frontView;
			flipView.BackView = backView;	
			flipView.Duration = 1;
			
			flipButtonDown = UIButton.FromType (UIButtonType.RoundedRect);
			flipButtonDown.Frame = new RectangleF (10, 10, 80, 40);
			flipButtonDown.SetTitle ("Flip down", UIControlState.Normal);
			flipButtonDown.TouchUpInside += delegate {
				flipView.Flip (FlipDirection.Down, null);
			};
			
			flipButtonUp = UIButton.FromType (UIButtonType.RoundedRect);
			flipButtonUp.Frame = new RectangleF (100, 10, 80, 40);
			flipButtonUp.SetTitle ("Flip up", UIControlState.Normal);
			flipButtonUp.TouchUpInside += delegate {
				flipView.Flip (FlipDirection.Up, null);
			};
			
			viewController.View.AddSubview (flipView);
			viewController.View.AddSubview (flipButtonDown);
			viewController.View.AddSubview (flipButtonUp);
			
			return true;
		}
	}
}

