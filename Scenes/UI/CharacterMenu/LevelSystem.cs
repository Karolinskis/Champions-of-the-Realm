using Godot;
using Godot.Collections;

/// <summary>
/// Class for handeling xp, level ups and obtaining new skills (Lacking UI, skills)
/// </summary>
public partial class LevelSystem : Control
{
	[Export] public int Level { get; set; } = 0;
	[Export] public float CurrrentXp { get; set; } = 0;
	[Export] public float LevelUpXp { get; set; } = 0;

	//public override void _Ready()
	//{
	//}

	/// <summary>
	/// Method of handeling obtained xp
	/// </summary>
	/// <param name="obtainedXp"></param>
	public void GetXp(float obtainedXp)
	{
		float leftXp = 0;
		if (CurrrentXp + obtainedXp  > LevelUpXp)
		{
			Level++; // LevelUp
			leftXp = CurrrentXp + obtainedXp - LevelUpXp;
			// TODO call signals
		}
		CurrrentXp = leftXp;
	}

	/// <summary>
	/// Method for parsing LevelSystem data to dictionary
	/// </summary>
	/// <returns>Dictionary with data to save</returns>
	public Dictionary<string, Variant> Save()
	{
		return new Dictionary<string, Variant>()
		{
			{ "Level", Level },
			{ "CurrrentXp", CurrrentXp },
			{ "LevelUpXp", LevelUpXp }
		};
	}

	/// <summary>
	/// Method for loading LevelSystem data from dictionary
	/// </summary>
	/// <param name="data">Dictionary filled with read data</param>
	public void Load(Dictionary<string, Variant> data)
	{
		Level = (int)data["Level"];
		CurrrentXp = (float)data["CurrrentXp"];
		LevelUpXp = (float)data["LevelUpXp"];
		//TODO call signals
	}
}
