using Godot;

/// <summary>
/// Class for handeling xp, level ups and obtaining new skills
/// </summary>
public partial class LevelSystem : Control
{
	[Export] public int Level { get; set; }
	[Export] public float CurrrentXp
	{
		// TODO: need implementation for level up
		get { return CurrrentXp; }
		set { CurrrentXp = value; }
	}
	[Export] public float LevelUpXp { get; set; }

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
			Level++;
			leftXp = CurrrentXp + obtainedXp - LevelUpXp;
		}
        CurrrentXp = leftXp;
	}
}
