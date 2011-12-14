using System;
using MonoTouch.CoreAnimation;

namespace FlipView
{
	public class DoubleSidedLayer : CATransformLayer
	{
		public DoubleSidedLayer ()
		{
			DoubleSided = true;
		}
		
		#region implementation
		
		public override void LayoutSublayers ()
		{
			base.LayoutSublayers ();
			
			FrontLayer.Frame = this.Bounds;
			BackLayer.Frame = this.Bounds;
		}
		
		#endregion
		
		#region properties
		
		private GradientLayer _frontLayer;
		private GradientLayer _backLayer;
		
		public GradientLayer FrontLayer 
		{ 
			get
			{
				return _frontLayer;
			}
			
			set
			{
				if (value == null)
					throw new ArgumentException ();
				
				if (_frontLayer != value) 
				{
					if (_frontLayer != null)
						_frontLayer.RemoveFromSuperLayer ();				
				
					_frontLayer = value;				
					_frontLayer.DoubleSided = false;
				
					this.AddSublayer (_frontLayer);
				
					this.SetNeedsLayout ();
				}
			}
		}
		
		public GradientLayer BackLayer 
		{ 
			get
			{
				return _backLayer;
			}
			
			set
			{
				if (_backLayer != value) 
				{
					if (_backLayer != null)
						_backLayer.RemoveFromSuperLayer ();	
					
					_backLayer = value;
					_backLayer.DoubleSided = false;
					
					var transform = CATransform3D.MakeRotation ((float)Math.PI, 1.0f, 0.0f, 0.0f);
					_backLayer.Transform = transform;
					
					this.AddSublayer (_backLayer);
					this.SetNeedsLayout ();
				}
			}
		}
		
		#endregion
	}
}

