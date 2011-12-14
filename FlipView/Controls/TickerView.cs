using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;

namespace FlipView
{
	public enum TickerViewTickDirection : int
	{
		Down = 0,
		Up = 1
	}
	 
	public class TickerView : UIView
	{
		private UIView _frontView;
		public UIView FrontView 
		{ 
			get { return _frontView; } 
			
			set
			{
				 if (_frontView != null &&_frontView.Superview != null)
        			_frontView.RemoveFromSuperview ();
    
    			_frontView = value;
    			this.AddSubview (_frontView);
			}
		}
		
		public UIView BackView { get; set; }
		public double Duration { get; set; }
		
		public GradientOverlayLayer TopFaceLayer { get; set; }
		public GradientOverlayLayer BottomFaceLayer { get; set; }
		public DoubleSidedLayer TickLayer { get; set; }
		CALayer FlipLayer { get; set; }
		
		bool ticking = false;
		
		public TickerView (RectangleF frame) : base (frame)
		{
			Setup ();
		}
		
		private void Setup ()
		{
		    Duration = 0.5;
			
    		this.BackgroundColor = UIColor.Clear;
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			FrontView.Frame = Bounds;
    		BackView.Frame = Bounds;

		}
		
		UIImage frontImage;
    	UIImage backImage;
		UIView fv;
		
		public void Flip (TickerViewTickDirection direction, NSAction completion) 
		{
    		if (ticking || FrontView == null || BackView == null)
        		return;
			
    		ticking = true;
    
    		this.FlipLayer = CALayer.Create ();
    		FlipLayer.Frame = this.Layer.Bounds;
    
    		var perspective = CATransform3D.Identity;
    		float zDistanse = 350f;
    		perspective.m34 = 1f / -zDistanse;
    		FlipLayer.SublayerTransform = perspective;
    
    		this.Layer.AddSublayer (FlipLayer);
    
    		frontImage = FrontView.GetImage ();
    		backImage = BackView.GetImage ();
    
		    // Face layers
		    // Top
		    TopFaceLayer = new GradientOverlayLayer (GradientOverlayLayerType.TypeFace, GradientOverlayLayerSegmentType.SegmentTop);
		    TopFaceLayer.Frame = new RectangleF (0f, 0f, FlipLayer.Frame.Size.Width, (float)Math.Floor (FlipLayer.Frame.Size.Height/2f));
		    
		    // Bottom
		    BottomFaceLayer = new GradientOverlayLayer (GradientOverlayLayerType.TypeFace, GradientOverlayLayerSegmentType.SegmentBottom);
		    BottomFaceLayer.Frame = new RectangleF (0f, (float)Math.Floor (FlipLayer.Frame.Size.Height / 2), FlipLayer.Frame.Size.Width, (float)Math.Floor (FlipLayer.Frame.Size.Height/2));
		
		    // Flip layer
		    TickLayer = new DoubleSidedLayer ();
		    
		    TickLayer.AnchorPoint = new PointF (1f, 1f);
		    TickLayer.Frame = new RectangleF (0f, 0f, FlipLayer.Frame.Size.Width, (float)Math.Floor (FlipLayer.Frame.Size.Height/2));
		    TickLayer.ZPosition = 1f;
		    
		    TickLayer.FrontLayer = new GradientOverlayLayer (GradientOverlayLayerType.TypeTick, GradientOverlayLayerSegmentType.SegmentTop);
		    TickLayer.BackLayer = new GradientOverlayLayer (GradientOverlayLayerType.TypeTick, GradientOverlayLayerSegmentType.SegmentBottom);
		    
		    // Images
		    if (direction == TickerViewTickDirection.Down) 
			{
		        TopFaceLayer.Contents = backImage.CGImage;
		        BottomFaceLayer.Contents = frontImage.CGImage;
		        TickLayer.FrontLayer.Contents = frontImage.CGImage;
		        TickLayer.BackLayer.Contents = backImage.CGImage;
		        
		        TopFaceLayer.SetGradientOpacity (1f);
		        
		        TickLayer.Transform = CATransform3D.Identity;
		    } 
			else 
			{
		        TopFaceLayer.Contents = frontImage.CGImage;
		        BottomFaceLayer.Contents = backImage.CGImage;
		        TickLayer.FrontLayer.Contents = backImage.CGImage;
		        TickLayer.BackLayer.Contents = frontImage.CGImage;
		        
		        BottomFaceLayer.SetGradientOpacity (1f);
		        
		        TickLayer.Transform = CATransform3D.MakeRotation ((float)-Math.PI, 1f, 0f, 0f);
		    }
		
		    // Add layers
		    FlipLayer.AddSublayer (TopFaceLayer);
		    FlipLayer.AddSublayer (BottomFaceLayer);
		    FlipLayer.AddSublayer (TickLayer);				
		    
		    //dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, .01 * NSEC_PER_SEC); // WTF!
		    //dispatch_after(popTime, dispatch_get_main_queue(), ^(void){
		    
			NSTimer.CreateScheduledTimer (0.01, delegate {
				
				this.InvokeOnMainThread (delegate {
			
			CATransaction.Begin ();
			CATransaction.AnimationDuration = Duration;
			CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			CATransaction.CompletionBlock = delegate {
				ticking = false;
				
				fv = this.FrontView;
	            FrontView = BackView;
	            BackView = fv;
			};
			
	        float angle = (float)Math.PI * 1f - (float)direction;
	        TickLayer.Transform = CATransform3D.MakeRotation (angle, 1f, 0f, 0f);
	        
	        TopFaceLayer.SetGradientOpacity ((float)direction);
	        BottomFaceLayer.SetGradientOpacity (1f - (float)direction);
	        
	        ((GradientOverlayLayer)TickLayer.FrontLayer).SetGradientOpacity (1f - (float)direction);
	        ((GradientOverlayLayer)TickLayer.BackLayer).SetGradientOpacity ((float)direction);
	        
	        CATransaction.Commit ();
				
			});
				
			});
   
		}
	}
}

