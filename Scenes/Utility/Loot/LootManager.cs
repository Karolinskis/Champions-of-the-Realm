using Godot;
using System;
public partial class LootManager : Node2D
{
    [Export] private int goldenCount = 100; // Max amount of gold coins in a scene.
    [Export] private int silverCount = 100; // Max amount of silver and bronze coins in a scene.
    private PackedScene goldenCoinScene; // Gold coin resource
    private PackedScene silverCoinScene; // Silver coin resource
    private PackedScene bronzeCoinScene; // Bronze coin resource
    private ObjectPool<Coin> goldenCoinsPool; // Gold coin pool
    private ObjectPool<Coin> silverCoinsPool; // Silver coin pool
    private ObjectPool<Coin> bronzeCoinsPool; // Bronze coin pool
    public override void _Ready()
    {
        base._Ready();
        // Loading coin scenes
        goldenCoinScene = ResourceLoader.Load<PackedScene>("res://Scenes/Utility/Loot/Coins/GoldenCoin.tscn");
        silverCoinScene = ResourceLoader.Load<PackedScene>("res://Scenes/Utility/Loot/Coins/SilverCoin.tscn");
        bronzeCoinScene = ResourceLoader.Load<PackedScene>("res://Scenes/Utility/Loot/Coins/BronzeCoin.tscn");

        // Creating and filling coin pools.
        goldenCoinsPool = new ObjectPool<Coin>();
        silverCoinsPool = new ObjectPool<Coin>();
        bronzeCoinsPool = new ObjectPool<Coin>();
        for (int i = 0; i < goldenCount; i++)
        {
            Coin temp = goldenCoinScene.Instantiate<Coin>();
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseGoldenCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        for (int i = 0; i < silverCount; i++)
        {
            Coin temp = silverCoinScene.Instantiate<Coin>();
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseSilverCoin"));
            AddChild(temp);
            temp.RemoveFromScene();

            temp = bronzeCoinScene.Instantiate<Coin>();
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseBronzeCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        //HandleCoinsSpawned(159, new Vector2(400, 400)); //For debugging purposes
    }

    /// <summary>
    /// Handles dropped coins.
    /// </summary>
    /// <param name="coin">Silver coin scene.</param>
    /// <param name="position">Coin position.</param>
    public void HandleCoinsSpawned(int coins, Vector2 position)
    {
        GD.Print(coins);
        Coin temp;
        while (coins > 0)
        {
            if (coins >= 100)
            {
                temp = goldenCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 100;
                continue;
            }
            else if (coins >= 25)
            {
                temp = silverCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 25;
                continue;
            }
            else
            {
                temp = bronzeCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 1;
                continue;
            }
            temp.AddToScene();
        }
    }

    /// <summary>
    /// Method for releasing gold coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Gold coin scene</param>
    private void ReleaseGoldenCoin(Coin coin)
    {
        goldenCoinsPool.Release(coin);
    }

    /// <summary>
    /// Method for releasing silver coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Silver coin scene</param>
    private void ReleaseSilverCoin(Coin coin)
    {
        silverCoinsPool.Release(coin);
    }

    /// <summary>
    /// Method for releasing bronze coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Bronze coin scene</param>
    private void ReleaseBronzeCoin(Coin coin)
    {
        bronzeCoinsPool.Release(coin);
    }
}