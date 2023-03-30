using Godot;
using System;

public partial class PauseMenu : Control
{
	private PackedScene settingsScene;	// Settings resources.
	private Control control;
	private Globals globals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Pausing the game
		GetTree().Paused = true;

		// To be used for showing/hiding pause menu.
		control = GetNodeOrNull<Control>("CanvasLayer/Control");
		globals = GetNode<Globals>("/root/Globals");


		// Loading settings scene
		settingsScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Settings/Settings.tscn");
	}

	/// <summary>
	/// Method for handeling Input
	/// </summary>
	/// <param name="event">Input event</param>
	public override void _Input(InputEvent @event)
	{	
			if (@event is InputEventKey eventKeyboardKey)
			{
					if (eventKeyboardKey.Keycode == Key.Escape && eventKeyboardKey.IsPressed())
					{
				GetTree().Paused = false;
				QueueFree();
					}
			}
	}

	/// <summary>
	/// Button for resuming the game.
	/// </summary>
	private void ButtonResumePressed()
	{
		GetTree().Paused = false;
		QueueFree();
	}

	/// <summary>
	/// Button for saving current game session.
	/// </summary>
	private void ButtonSavePressed()
	{
		globals.SaveGame();
	}

	/// <summary>
	/// Button for loading existing save file.
	/// </summary>
	private void ButtonLoadPressed()
	{
		globals.LoadingForm = Globals.LoadingForms.Save;
		globals.LoadGame(); // Changing scene through globals
		GetTree().Paused = false; // resuming process
		QueueFree(); // removing pauseMenu
	}

	/// <summary>
	/// Button for accessing the settings menu.
	/// </summary>
	private void ButtonSettingsPressed()
	{
		Settings settingsScreen = settingsScene.Instantiate() as Settings;
		AddChild(settingsScreen);
		control.Hide();
	}

	/// <summary>
	/// Button for quitting the game.
	/// </summary>
	private void ButtonQuitMainMenuPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Scenes/UI/Menus/Main/MainMenu.tscn");
	}

	/// <summary>
	/// Button for quitting the game.
	/// </summary>
	private void ButtonQuitPressed()
	{
		GetTree().Quit();
	}
}
