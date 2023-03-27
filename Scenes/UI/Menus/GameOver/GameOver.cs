using Godot;
using System;

public partial class GameOver : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	/// <summary>
	/// Button for restarting scene.
	/// </summary>
	private void ButtonRestartPressed()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
	
	/// <summary>
	/// Button for loading an existing save file.
	/// </summary>
	private void ButtonLoadPressed()
	{
		// todo: waiting for save/load functionality.
	}

	/// <summary>
	/// Button for quitting to main menu.
	/// </summary>
	private void ButtonQuitMainPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Scenes/UI/Menus/Main/MainMenu.tscn");
	}

	/// <summary>
	/// Button for quiting to desktop.
	/// </summary>
	private void ButtonQuitPressed()
	{
		GetTree().Quit();
	}
}
