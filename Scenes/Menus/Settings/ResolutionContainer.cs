using Godot;
using System;
using System.Collections.Generic;


public partial class ResolutionContainer : VBoxContainer
{
	
	// todo: no default selected resolution
	// todo: back button to main menu
	OptionButton resolutionMenu;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resolutionMenu = GetNode<OptionButton>("ResolutionDropDown");
		AddItems();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>
	{
		{ "1920x1080", new Vector2I(1920, 1080) },
		{ "1600x900", new Vector2I(1600, 900) },
		{ "1366x768", new Vector2I(1366, 768) },
		{ "1280x720", new Vector2I(1280, 720) },
		{ "1024x768", new Vector2I(1024, 768) },
		{ "800x600", new Vector2I(800, 600) }
	};
	

	/// <summary>
	/// Adds dictionary items to the drop down menu
	/// </summary>
	public void AddItems()
	{
		foreach (var resolution in Resolutions) 
		{
			resolutionMenu.AddItem(resolution.Key);
		}
	}
	
	/// <summary>
	/// Changes resolution to the selected one from the drop down menu
	/// </summary>
	private void _on_resolution_drop_down_item_selected(long index)
	{
		var size = Resolutions[resolutionMenu.GetItemText((int)index)];
		DisplayServer.WindowSetSize(size);
	}
}



