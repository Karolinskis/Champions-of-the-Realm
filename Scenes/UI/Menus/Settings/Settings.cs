using Godot;
using System;
using System.Collections.Generic;

public partial class Settings : Control
{	
	// audio variables
	private int musicBus;
	private int sfxBus;

	// resolution variables
	private OptionButton resolutionMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// audio
		musicBus = AudioServer.GetBusIndex("New Bus");		// audio buses should be named Music and SFX, but I crash while renaming them so it will stay like this for now.
		sfxBus = AudioServer.GetBusIndex("New Bus 2");

		// resolution
		resolutionMenu = GetNode<OptionButton>("CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ResolutionContainer/ResolutionDropDown");
		AddItems();
		setStartResolution();
	}

	/// <summary>
	/// Method for enabling or disabling music.
	/// </summary>
	/// <param name="button_pressed">Music toggle</param>
	private void MusicToggled(bool buttonPressed)
	{
		AudioServer.SetBusMute(musicBus, !AudioServer.IsBusMute(musicBus));
	}

	/// <summary>
	/// Method for enabling or disabling sfx.
	/// </summary>
	/// <param name="button_pressed">Sfx toggle</param>
	private void SfxToggled(bool buttonPressed)
	{
		AudioServer.SetBusMute(sfxBus, !AudioServer.IsBusMute(sfxBus));
	}
	
	/// <summary>
	/// Music volume slider
	/// </summary>
	private void MusicSliderVolumeChanged(double value)
	{
		AudioServer.SetBusVolumeDb(musicBus, (float)value);
	}
	
	/// <summary>
	/// SFX volume slider
	/// </summary>
	private void SfxSliderVolumeChanged(double value)
	{
		AudioServer.SetBusVolumeDb(sfxBus, (float)value);
	}

	private Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>
	{
		{ "1920x1080", new Vector2I(1920, 1080) },
		{ "1600x900", new Vector2I(1600, 900) },
		{ "1366x768", new Vector2I(1366, 768) },
		{ "1280x720", new Vector2I(1280, 720) },
		{ "1024x768", new Vector2I(1024, 768) },
		{ "800x600", new Vector2I(800, 600) }
	};
	
	/// <summary>
	/// Adds dictionary items to the resolution drop down menu
	/// </summary>
	private void AddItems()
	{
		foreach (var resolution in Resolutions) 
		{
			resolutionMenu.AddItem(resolution.Key);
		}
	}

	/// <summary>
	/// Selects the starting resolution from the resolutionMenu.
	/// </summary>
	private void setStartResolution() 
	{
		resolutionMenu.Selected = 3;
	}
	
	/// <summary>
	/// Resolution drop down menu
	/// </summary>
	/// <param name="index">The index of selected entry</param>
	private void ResolutionDropDownItemSelected(long index)
	{
		var size = Resolutions[resolutionMenu.GetItemText((int)index)];
		DisplayServer.WindowSetSize(size);
	}

	/// <summary>
	/// Goes back to the previous scene.
	/// </summary>
	private void ButtonBackPressed()
	{
		var parent = GetParent();
		var control = parent.GetNode<Control>("CanvasLayer/Control");
		control.Show();
		QueueFree();
	}
}