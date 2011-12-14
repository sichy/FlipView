using MonoTouch.UIKit;
using System;
using MonoTouch.CoreGraphics;

namespace FlipView
{
	public static class UIView_Extensions
	{
		public static UIImage GetImage (this UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, false, UIScreen.MainScreen.Scale);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());			
			var image = UIGraphics.GetImageFromCurrentImageContext ();			
			UIGraphics.EndImageContext ();			
			
			return image; 
		}
	}
}

