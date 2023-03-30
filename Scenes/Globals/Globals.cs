using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Class for handling global actions (Scene switching, save/load functionality)
/// </summary>
public partial class Globals : Node
{
	private const string SaveDir = "user://saves/"; // save files directory path
	private string savePath = SaveDir + "save.dat"; // save file path

	// Loading forms which indicate how map scenes should be loaded
	// (Loading new game, loading from save file or just certain scenes,
	// for example when map level is switched)
	public enum LoadingForms 
	{
		New,
		Save,
		Load
	}
	public LoadingForms LoadingForm { get; set; } = LoadingForms.New;

	public Dictionary<string, Variant> Player { get; set; } // For transfering player between scenes

	/// <summary>
	/// Method which saves all nodes in Persist group by parsing dictionary to JSON file
	/// </summary>
	/// <returns>If saved successfully</returns>
	public bool SaveGame()
	{
		if (!DirAccess.DirExistsAbsolute(SaveDir)) // Checks if directory exists
		{
			DirAccess.MakeDirRecursiveAbsolute(SaveDir);
		}
		// opening file and encrpypting file with password
		FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Write, "nekompiliuoja");
		if (saveFile.GetError() != Error.Ok) // checking if opened JSON file without errors
		{
			GD.PushError("Failed to save data!"); // for debuging purposes
			return false;
		}
		Array<Node> saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			// Checking if can be saved
			if (!saveNode.HasMethod("Save") || saveNode.Name.IsEmpty)
			{
				continue;
			}
			// Getting a dictionary of data about every node by calling their save method
			Variant data = saveNode.Call("Save");
			GD.Print(data); // for debugging purposes
			saveFile.StoreLine(Json.Stringify(data)); // parsing dictionary to JSON file
		}
		return true;
	}
	/// <summary>
	/// Method for loading saved game and its state from JSON file
	/// </summary>
	public async void LoadGame()
	{
		// Checking if save path exists
		if (!FileAccess.FileExists(savePath))
		{
			// TODO in future scene shouldn't be changed
			GetTree().ChangeSceneToFile("res://Scenes/UI/Menus/Main/MainMenu.tscn");
			return;
		}
		// getting all unwanted save nodes in current scene
		Array<Node> saveNodes = GetTree().GetNodesInGroup("Presist");
		foreach (Node saveNode in saveNodes)
		{
			saveNode.QueueFree(); // removing all unwanted save nodes
		}
		GetTree().CurrentScene.QueueFree();
		if (saveNodes.Count > 0)
		{
			// Giving small amount of time to fully remove all unwanted nodes (It takes time to free from memory)
			await ToSignal(GetTree().CreateTimer(0.00001f), "timeout"); // TODO implement better solution, without async
		}
		FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, Godot.FileAccess.ModeFlags.Read, "nekompiliuoja");
		if (saveFile.GetError() != Error.Ok) // Checking if saveFile was loaded without any errors
		{
			GD.PushError("Failed to load data!"); // for debuging purposes
		}
		while (saveFile.GetPosition() < saveFile.GetLength()) // reading every JSON line
		{
			// Loading data about saved nodes and parsing it to dictionaries
			Dictionary<string, Variant> data = (Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());

			// Creating Persist node, adding to scene and loading data
			PackedScene newObjectScene = ResourceLoader.Load<PackedScene>(data["Filename"].ToString());
			Node newObject = newObjectScene.Instantiate();
			newObject.Set("position", new Vector2((float)data["PosX"], (float)data["PosY"]));
			GetNode(data["Parent"].ToString()).AddChild(newObject);
			newObject.Call("Load", data);
		}
		// Calling method to load player inside scene
		GetTree().CurrentScene.Call("LoadSavedPlayer");
	}
}