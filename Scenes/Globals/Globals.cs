using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Class for handeling global actions (Scene switching, save/load functionality)
/// </summary>
public partial class Globals : Node
{
	private const string SaveDir = "user://saves/"; // save files directory path
	private string savePath = SaveDir + "save.dat"; // save file path
	public enum LoadingForms
	{
		New,
		Save,
		Load
	}
	public LoadingForms LoadingForm { get; set; } = LoadingForms.New;

    public Dictionary<string, Variant> Player { get; set; } // For transfering player between scenes
    public override void _Ready()
	{
	}

	public bool SaveGame()
	{
		if (!DirAccess.DirExistsAbsolute(SaveDir))
		{
			DirAccess.MakeDirRecursiveAbsolute(SaveDir);
		}
		// opening file and encrpypting filewith password
		FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Write, "nekompiliuoja");
		if (saveFile.GetError() != Error.Ok)
		{
			GD.PushError("Failed to save data!"); // for debuging purposes
			return false;
		}
		Array<Node> saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			if (!saveNode.HasMethod("Save") || saveNode.Name.IsEmpty)
			{
				continue;
			}
			Variant data = saveNode.Call("Save"); // Getting dictionary from every Persist node
			GD.Print(data); // for debugging purposes
			saveFile.StoreLine(Json.Stringify(data));
		}
		return true;
	}

	public async void LoadGame()
	{
		if (!FileAccess.FileExists(savePath))
		{
			return;
		}

		Array<Node> saveNodes = GetTree().GetNodesInGroup("Presist"); // getting all unwanted save nodes in current scene
		foreach (Node saveNode in saveNodes)
		{
			saveNode.QueueFree(); // removing all unwanted save nodes
		}
		GetTree().CurrentScene.QueueFree();
		if (saveNodes.Count > 0)
		{
			// Giving small amount of time to fully remove all unwanted nodes
			await ToSignal(GetTree().CreateTimer(0.00001f), "timeout"); // TODO implement better solution, without async
		}
		FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, Godot.FileAccess.ModeFlags.Read, "nekompiliuoja");
		if (saveFile.GetError() != Error.Ok) // Checking if saveFile was loaded without any errors
		{
			GD.PushError("Failed to load data!");
		}
		while (saveFile.GetPosition() < saveFile.GetLength()) // reading every line
		{
			Dictionary<string, Variant> data = (Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());
			PackedScene newObjectScene = ResourceLoader.Load<PackedScene>(data["Filename"].ToString());
			Node newObject = newObjectScene.Instantiate();
			newObject.Set("position", new Vector2((float)data["PosX"], (float)data["PosY"]));
			GetNode(data["Parent"].ToString()).AddChild(newObject);
			newObject.Call("Load", data);
		}
		GetTree().CurrentScene.Call("LoadSavedPlayer");
	}

}
