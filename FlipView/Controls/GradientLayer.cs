using MonoTouch.CoreAnimation;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace FlipView
{
	public enum GradientLayerType
	{
		Face,
		Flip
	}
	
	public enum GradientLayerAreaType
	{
		Top,
		Bottom
	}
	
	public class GradientLayer : CALayer
	{
		float _minimumOpacity;
		float _maximumOpacity;
		
		CAGradientLayer _gradientLayer;
		CALayer _gradientMaskLayer;
		
		GradientLayerType _type;
		GradientLayerAreaType _segment;
		
		CGColor [] colors1 = new CGColor [] { UIColor.FromWhiteAlpha (0f, 0.5f).CGColor,
	                                       				UIColor.FromWhiteAlpha (0f, 1f).CGColor };
		NSNumber [] locations1 = new NSNumber[] { NSNumber.FromFloat (0), NSNumber.FromFloat (1f) };
		
		CGColor [] colors2 = new CGColor [] { UIColor.FromWhiteAlpha (0f, 0f).CGColor,
														UIColor.FromWhiteAlpha (0f, 0.5f).CGColor,
	                                       				UIColor.FromWhiteAlpha (0f, 1f).CGColor };
		NSNumber [] locations2 = new NSNumber [] { NSNumber.FromFloat (0.2f), 
															NSNumber.FromFloat (0.4f),
				                                          NSNumber.FromFloat (1f) };
		
		public GradientLayer (GradientLayerType type, GradientLayerAreaType segment)
		{
			_type = type;
        	_segment = segment;
		
        	_minimumOpacity = 0f;
        
        	_gradientMaskLayer = CALayer.Create ();
			_gradientMaskLayer.ContentsScale = UIScreen.MainScreen.Scale;
			
			_gradientLayer = new CAGradientLayer ();
        	_gradientLayer.Frame = this.Bounds;
        	_gradientLayer.Mask = _gradientMaskLayer;
			
			this.MasksToBounds = true;
			this.AddSublayer (_gradientLayer);
        	this.ContentsScale = UIScreen.MainScreen.Scale;
        
			if (_type == GradientLayerType.Face) 
			{
				_gradientLayer.Colors = colors1;
	            _gradientLayer.Locations = locations1;
				
				_maximumOpacity = .75f;
			} 
			else 
			{
				_gradientLayer.Colors = colors2;
				_gradientLayer.Locations = locations2;
				
				_maximumOpacity = 1f;
			}
        
	        if (_segment == GradientLayerAreaType.Top) 
			{
	            this.ContentsGravity = "bottom";
	            
	            _gradientLayer.StartPoint = new PointF (0, 0);
	            _gradientLayer.EndPoint = new PointF (0, 1);
	            
	            _gradientMaskLayer.ContentsGravity = "bottom";
	        } 
			else 
			{
	            this.ContentsGravity = "top";
	            
	            _gradientLayer.StartPoint = new PointF (0, 1);
	            _gradientLayer.EndPoint = new PointF (0, 0);
	            
	            _gradientMaskLayer.ContentsGravity = "top";
	        }
        
			_gradientLayer.Opacity = _minimumOpacity;        
		}
		
		public override void LayoutSublayers ()
		{
			base.LayoutSublayers ();
			
			_gradientLayer.Frame = this.Bounds;
    		_gradientMaskLayer.Frame = this.Bounds;
		}
		
		public float GradientOpacity 
		{
			get
			{
				return _gradientLayer.Opacity;
			}
			
			set
			{
				_gradientLayer.Opacity = (value * (_maximumOpacity - _minimumOpacity) + _minimumOpacity);	
			}
		}

		public override CGImage Contents 
		{
			get 
			{
				return base.Contents;
			}
			
			set 
			{
				base.Contents = value;
				
				_gradientMaskLayer.Contents = Contents;
			}
		}
	}
}

