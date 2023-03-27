using Godot;
using System;

public partial class MainMenu : Control
{
	private PackedScene settingsScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		settingsScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Menus/Settings/Settings.tscn");
	}
	
	/// <summary>
	/// Button for starting a new game.
	/// </summary>
	private void ButtonStartNewPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Maps/Main/Main.tscn");
	}
	
	/// <summary>
	/// Button for loading an existing save file.
	/// </summary>
	private void ButtonLoadPressed()
	{
		// Replace with function body.
	}

	/// <summary>
	/// Button for accessing the settings menu.
	/// </summary>
	private void ButtonSettingsPressed()
	{
		Settings settingsScreen = settingsScene.Instantiate() as Settings;
		AddChild(settingsScreen);
		Control control = GetNode<Control>("CanvasLayer/Control");
		control.Hide();
	}

	/// <summary>
	/// Button for quiting the game.
	/// </summary>
	private void ButtonQuitPressed()
	{
		GetTree().Quit();
	}
}
