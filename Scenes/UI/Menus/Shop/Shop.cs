using Godot;
using System;

public partial class Shop : Control
{
    private Globals globals; // global variables and functionality

    private Label DamageLabel;
    private ProgressBar DamageBar;
    private Label KnockbackLabel;
    private ProgressBar KnockbackBar;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

        // Damage bar and label nodes
        DamageBar = GetNode<ProgressBar>(
            "CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Sword/DamagePanel/VBoxContainer/DamageLevelBar"
        );
        DamageLabel = GetNode<Label>(
            "CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Sword/DamagePanel/VBoxContainer/CurrentDamage"
        );

        KnockbackBar = GetNode<ProgressBar>(
            "CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Sword/KnockbackPanel/VBoxContainer/KnockbackLevelBar"
        );
        KnockbackLabel = GetNode<Label>(
            "CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Sword/KnockbackPanel/VBoxContainer/CurrentKnockback"
        );
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void ButtonStartPressed()
    {
        globals.ChangeScene("res://Scenes/Maps/Main/Main.tscn");
        QueueFree();
    }

    /// <summary>
    /// Button for loading an existing save file.
    /// </summary>
    private void ButtonBackPressed()
    {
        var parent = GetParent();
        Control control = parent.GetNode<Control>("CanvasLayer/Control");
        control.Show();
        QueueFree();
    }

    private void UpgradeDamageButtonPressed()
    {
        DamageBar.Value += 25;
        DamageLabel.Text = "Current: " + (100+DamageBar.Value) + "%";
    }

    private void UpgradeKnockbackButtonPressed()
    {
        KnockbackBar.Value += 25;
        KnockbackLabel.Text = "Current: " + (100+KnockbackBar.Value) + "%";
    }
}
