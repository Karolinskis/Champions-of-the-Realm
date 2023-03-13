using Godot;
using System;

public partial class AudioContainer : VBoxContainer
{
	int musicBus;
	int sfxBus;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		musicBus = AudioServer.GetBusIndex("Music");
		sfxBus = AudioServer.GetBusIndex("SFX");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	/// <summary>
	/// Method for enabling or disabling music bus
	/// </summary>
	private void _on_music_toggle_toggled(bool button_pressed)
	{
		AudioServer.SetBusMute(musicBus, !AudioServer.IsBusMute(musicBus));
	}

	/// <summary>
	/// Method for enabling or disabling sfx bus
	/// </summary>
	private void _on_sfx_toggle_toggled(bool button_pressed)
	{
		AudioServer.SetBusMute(sfxBus, !AudioServer.IsBusMute(sfxBus));
	}
	
	/// <summary>
	/// Music volume slider
	/// </summary>
	private void _on_music_slider_value_changed(double value)
	{
		AudioServer.SetBusVolumeDb(musicBus, (float)value);
	}
	
	/// <summary>
	/// SFX volume slider
	/// </summary>
	private void _on_sfx_slider_value_changed(double value)
	{
		AudioServer.SetBusVolumeDb(sfxBus, (float)value);
	}
}