using Godot;
using System;

public partial class PauseMenu : Control
{
	private PackedScene settingsScene;	// Settings resources.
	private Map mapInstance;	// Map instance to be controlled.
	private bool paused = false;	// Indicates if game is paused, default is false.
	private Control control;	// for hiding/showing pause menu.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// To be used for showing/hiding pause menu.
		control = GetNodeOrNull<Control>("CanvasLayer/Control");

		// Loading settings scene
		settingsScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Menus/Settings/Settings.tscn");
	}

	/// <summary>
    /// Sets map instance that is controller by PauseMenu.
    /// </summary>
    /// <param name="mapInstance">Map instance to be controlled by PauseMenu.</param>
    public void SetMapInstance(Map mapInstance)
    {
        mapInstance = mapInstance;
    }

	/// <summary>
    /// Method for pausing and unpausing the game with "ESC".
    /// </summary>
    /// <param name="@event">Keyboard input.</param>
    public override void _UnhandledInput(InputEvent @event)
    {
		// Using _UnhandledInput() instead of _Input() because _Input() won't detect key presses when game is paused.
		// Works fine but throws exception everytime "ESC" is pressed.
		// Could change to _Input, but then won't be able to pause/unpause with the same key, would have to use a button for resuming.
		try
		{
			// Open pause menu and pause the Map instance
        	if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Escape)
        	{
				if (!paused)
				{
					paused = true;
					mapInstance.Pause();
					control.Show();
				}
				else
				{
					paused = false;
					mapInstance.Resume();
					control.Hide();
				}
        	}
		}
		catch (Exception e)
		{
			GD.Print("ESC key was pressed.");
		}
    }

	/// <summary>
	/// Button for resuming the game.
	/// </summary>
	private void ButtonResumePressed()
	{
		paused = false;
		mapInstance.Resume();
		control.Hide();
	}

	/// <summary>
	/// Button for saving current game session.
	/// </summary>
	private void ButtonSavePressed()
	{
		// todo: waiting for save/load system to be implemented.
	}

	/// <summary>
	/// Button for loading existing save file.
	/// </summary>
	private void ButtonLoadPressed()
	{
		// todo: waiting for save/load system to be implemented.
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
	private void ButtonQuitPressed()
	{
		GetTree().Quit();
	}
}