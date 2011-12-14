using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;

namespace FlipView
{
	public enum FlipDirection : int
	{
		Down = 0,
		Up = 1
	}
	 
	public class FlipView : UIView
	{
		#region private members
		
		private GradientLayer topFaceLayer;
		private GradientLayer bottomFaceLayer;
		private DoubleSidedLayer mainFlipLayer;
		private CALayer projectionLayer;
		private UIView frontView;
		private UIImage frontImage;
    	private UIImage backImage;
		private UIView fv;
		
		private bool flippingInProgress = false;
		
		#endregion
		
		public FlipView (RectangleF frame) : base (frame)
		{
			Duration = 0.75; // that is a nice default
			
    		this.BackgroundColor = UIColor.Clear;
		}
		
		#region properties
		
		public UIView FrontView 
		{ 
			get { return frontView; } 
			
			set
			{
				 if (frontView != null &&frontView.Superview != null)
        			frontView.RemoveFromSuperview ();
    
    			frontView = value;
    			this.AddSubview (frontView);
			}
		}
		
		public UIView BackView { get; set; }
		public double Duration { get; set; }
		
		#endregion
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			FrontView.Frame = Bounds;
    		BackView.Frame = Bounds;

		}
		
		public void Flip (FlipDirection direction, NSAction completion) 
		{
    		if (flippingInProgress || FrontView == null || BackView == null)
        		return;
			
    		flippingInProgress = true;
    
    		projectionLayer = CALayer.Create (); // this is to make the perspective and 2.5D effect
    		projectionLayer.Frame = this.Layer.Bounds;
    
    		var perspective = CATransform3D.Identity;
    		float zDistanse = 350f; // change this to the actual perspective and distance of the projection plane
    		perspective.m34 = 1f / -zDistanse; 
    		projectionLayer.SublayerTransform = perspective;
    
    		this.Layer.AddSublayer (projectionLayer);
    
    		frontImage = FrontView.GetImage ();
    		backImage = BackView.GetImage ();
    
		    topFaceLayer = new GradientLayer (GradientLayerType.Face, GradientLayerAreaType.Top);
		    topFaceLayer.Frame = new RectangleF (0f, 0f, projectionLayer.Frame.Size.Width, (float)Math.Floor (projectionLayer.Frame.Size.Height/2f));
		    
		    bottomFaceLayer = new GradientLayer (GradientLayerType.Face, GradientLayerAreaType.Bottom);
		    bottomFaceLayer.Frame = new RectangleF (0f, (float)Math.Floor (projectionLayer.Frame.Size.Height / 2f), projectionLayer.Frame.Size.Width, (float)Math.Floor (projectionLayer.Frame.Size.Height/2f));
		
		    mainFlipLayer = new DoubleSidedLayer ();
		    
		    mainFlipLayer.AnchorPoint = new PointF (1f, 1f);
		    mainFlipLayer.Frame = new RectangleF (0f, 0f, projectionLayer.Frame.Size.Width, (float)Math.Floor (projectionLayer.Frame.Size.Height/2f));
		    mainFlipLayer.ZPosition = 1f;
		    
		    mainFlipLayer.FrontLayer = new GradientLayer (GradientLayerType.Flip, GradientLayerAreaType.Top);
		    mainFlipLayer.BackLayer = new GradientLayer (GradientLayerType.Flip, GradientLayerAreaType.Bottom);
				
			// offscreen rendering optimization to reuse the offscreen buffer
			topFaceLayer.ShouldRasterize = true;
			bottomFaceLayer.ShouldRasterize = true;
		    
		    if (direction == FlipDirection.Down) 
			{
		        topFaceLayer.Contents = backImage.CGImage;
		        bottomFaceLayer.Contents = frontImage.CGImage;
		        mainFlipLayer.FrontLayer.Contents = frontImage.CGImage;
		        mainFlipLayer.BackLayer.Contents = backImage.CGImage;
		        
		        topFaceLayer.GradientOpacity = 1f;
		        
		        mainFlipLayer.Transform = CATransform3D.Identity;
		    } 
			else 
			{
		        topFaceLayer.Contents = frontImage.CGImage;
		        bottomFaceLayer.Contents = backImage.CGImage;
		        mainFlipLayer.FrontLayer.Contents = backImage.CGImage;
		        mainFlipLayer.BackLayer.Contents = frontImage.CGImage;
		        
		        bottomFaceLayer.GradientOpacity = 1f;
		        
		        mainFlipLayer.Transform = CATransform3D.MakeRotation ((float)-Math.PI, 1f, 0f, 0f);
		    }
		
		    // Add layers to the projection layer, so we apply the projections to it
		    projectionLayer.AddSublayer (topFaceLayer);
		    projectionLayer.AddSublayer (bottomFaceLayer);
		    projectionLayer.AddSublayer (mainFlipLayer);				
		    
			NSTimer.CreateScheduledTimer (0.01, delegate {
				CATransaction.Begin ();
				CATransaction.AnimationDuration = Duration;
				CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
				CATransaction.CompletionBlock = delegate {
					fv = this.FrontView;
		            FrontView = BackView;
		            BackView = fv;
					flippingInProgress = false;
					
					if (completion != null)
						completion.Invoke ();
				};
		
				// this is the whole trick, change the angle in timing function
		        float angle = (float)Math.PI * (1f - (float)direction);
        		mainFlipLayer.Transform = CATransform3D.MakeRotation (angle, 1f, 0f, 0f);
        
        		topFaceLayer.GradientOpacity = (float)direction;
        		bottomFaceLayer.GradientOpacity = 1f - (float)direction;
        
        		mainFlipLayer.FrontLayer.GradientOpacity = 1f - (float)direction;
        		mainFlipLayer.BackLayer.GradientOpacity = (float)direction;
        
        		CATransaction.Commit ();
			});
		}
	}
}

