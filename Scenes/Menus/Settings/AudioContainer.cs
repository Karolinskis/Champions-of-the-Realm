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
	/// Toggles music audio bus
	/// </summary>
	private void _on_music_toggle_toggled(bool button_pressed)
	{
		AudioServer.SetBusMute(musicBus, !AudioServer.IsBusMute(musicBus));
	}

	/// <summary>
	/// Toggles sfx audio bus
	/// </summary>
	private void _on_sfx_toggle_toggled(bool button_pressed)
	{
		AudioServer.SetBusMute(sfxBus, !AudioServer.IsBusMute(sfxBus));
	}
	
	/// <summary>
	/// Music audio bus volume slider
	/// </summary>
	private void _on_music_slider_value_changed(double value)
	{
		AudioServer.SetBusVolumeDb(musicBus, (float)value);
	}
	
	/// <summary>
	/// SFX audio bus volume slider
	/// </summary>
	private void _on_sfx_slider_value_changed(double value)
	{
		// will probably need to add more sfxBuses
		AudioServer.SetBusVolumeDb(sfxBus, (float)value);
	}
}
