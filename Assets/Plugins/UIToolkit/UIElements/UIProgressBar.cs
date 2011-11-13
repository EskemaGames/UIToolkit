using UnityEngine;
using System;


public class UIProgressBar : UISprite
{
	public bool resizeTextureOnChange = false;
	public bool rightToLeft;
	
	private float _value = 0;
	private UISprite _bar;
	private float _barOriginalWidth;
	private float _barOriginalHeight;
	private UIUVRect _barOriginalUVframe;
	
	
	
	public static UIProgressBar create( string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos )
	{
		return create( UI.firstToolkit, barFilename, borderFilename, barxPos, baryPos, borderxPos, borderyPos, false );
	}


	public static UIProgressBar create( string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos, bool rightToLeft )
	{
		return create( UI.firstToolkit, barFilename, borderFilename, barxPos, baryPos, borderxPos, borderyPos, rightToLeft );
	}

	
	// the bars x/y coordinates should be relative to the borders
	public static UIProgressBar create( UIToolkit manager, string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos, bool rightToLeft )
	{
		var borderTI = manager.textureInfoForFilename( borderFilename );
	
		var borderFrame = new Rect( borderxPos, borderyPos, borderTI.frame.width, borderTI.frame.height );
		
		UISprite bar;
		
		if( rightToLeft )
			bar = manager.addSprite( barFilename, borderxPos - barxPos + ((int)borderTI.frame.width), borderyPos + baryPos, 1 );
		else
			bar = manager.addSprite( barFilename, borderxPos + barxPos, borderyPos + baryPos, 1 );

		var progressBar = new UIProgressBar( manager, borderFrame, 1, borderTI.uvRect, bar, borderTI.rotated );
		progressBar.rightToLeft = rightToLeft;
		
		return progressBar;
	}
	
	
	public UIProgressBar( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UISprite bar, bool rotated ):base( frame, depth, uvFrame, rotated )
	{
		// Save the bar and make it a child of the container/border for organization purposes
		_bar = bar;
		_bar.parentUIObject = this;
		
		_barOriginalWidth = _bar.width;
		_barOriginalHeight = _bar.height;
		
		/*
		// Save the bars original size
		if (rotated)
		{
			_barOriginalWidth = _bar.width;
			_barOriginalHeight = _bar.height;
		}
		else
		{
			_barOriginalWidth = _bar.width;
			_barOriginalHeight = _bar.height;
		}
		*/
		
		/*if (!_rotated)
					normalFrame = new Rect( clientTransform.position.x, -clientTransform.position.y, width * touchesScale.x, height * touchesScale.y);
				else
					normalFrame = new Rect( clientTransform.position.x, -clientTransform.position.y - (width * touchesScale.y), height * touchesScale.x, width * touchesScale.y);
					*/
		_barOriginalUVframe = _bar.uvFrame;
		
		manager.addSprite( this );
	}


    public override bool hidden
    {
        get { return ___hidden; }
        set
        {
            // No need to do anything if we're already in this state:
            if( value == ___hidden )
                return;
			
			base.hidden = value;
			
			// pass the call down to our bar
			_bar.hidden = value;
        }
    }
	
	
	public override void destroy()
	{
		_bar.destroy();
		base.destroy();
	}

	// Current value of the progress bar.  Value is always between 0 and 1.
	public float value
	{
		get { return _value; }
		
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0, 1 );
				
				// Update the bar UV's if resizeTextureOnChange is set
				if( resizeTextureOnChange )
				{
					// Set the uvFrame's width based on the value
					UIUVRect newUVframe = _barOriginalUVframe;
					
					//check the rotation
					if (_rotated)
					{
						newUVframe.uvDimensions.y *= _value;
					}
					else
					{
						newUVframe.uvDimensions.x *= _value;
					}
					
					_bar.uvFrame = newUVframe;
				}

				// Update the bar size based on the value
				if( rightToLeft )
				{
					//check the rotation
					if (_rotated)
					{
						_bar.setSize( _value * -_barOriginalHeight, _bar.width );	
					}
					else
					{
						_bar.setSize( _value * -_barOriginalWidth, _bar.height );
					}
				}
				else
				{
					//check the rotation
					if (_rotated)
					{
						_bar.setSize( _bar.width, _value * _barOriginalHeight );
					}
					else
					{
						_bar.setSize( _value * _barOriginalWidth, _bar.width );
					}
				}
			}
		}
	}

	
}