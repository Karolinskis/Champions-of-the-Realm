using Godot;
using System;

public partial class GUI : CanvasLayer
{
	private ProgressBar healthBar;
	private Label currencyLabel;
	private Label XPLabel;
	private Label currentAmmoLabel;
	private Label maxAmmoLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currencyLabel = GetNode<Label>("MarginContainer/Rows/TopRow/CurrenctContainer/CurrencyLabel");
		healthBar = GetNode<ProgressBar>("MarginContainer/Rows/BottomRow/HealthContainer/HealthBar");
		currentAmmoLabel = GetNode<Label>("MarginContainer/Rows/BottomRow/AmmoContainer/CurrentAmmo");
		XPLabel = GetNode<Label>("MarginContainer/Rows/TopRow/XPContainer/XPLabel");
		maxAmmoLabel = GetNode<Label>("MarginContainer/Rows/BottomRow/AmmoContainer/MaxAmmo");
	}

	private void ChangeCurrentHealth(float newHealthPoints)
	{
		healthBar.Value = newHealthPoints;
	}

	private void ChangeMaxHealth(float newHealthPoints)
	{
		healthBar.MaxValue = newHealthPoints;
	}

	private void ChangeCurrency(int newCurrency)
	{
		currencyLabel.Text = "Currency: " + newCurrency;
	}

    private void ChangeXP(int newXP)
    {
        XPLabel.Text = "XP: " + newXP;
    }

    private void ChangeCurrentAmmo(float newAmmo)
    {
		currentAmmoLabel.Text = newAmmo.ToString();
    }

    private void ChangeMaxAmmo(float newAmmo)
    {
        maxAmmoLabel.Text = newAmmo.ToString();
    }
}
