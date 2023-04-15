using Godot;
using System;
public partial class LootManager : Node2D
{
    [Export] private int goldenCount = 20; // Max amount of gold coins in a scene.
    [Export] private int silverCount = 100; // Max amount of silver coins in a scene.
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
        goldenCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Utility/Coin/GoldenCoin.tscn");
        silverCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Utility/Coin/SilverCoin.tscn");
        bronzeCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Utility/Coin/BronzeCoin.tscn");

        // Creating and filling coin pools.
        goldenCoinsPool = new ObjectPool<Coin>();
        silverCoinsPool = new ObjectPool<Coin>();
        bronzeCoinsPool = new ObjectPool<Coin>();
        for (int i = 0; i < goldenCount; i++)
        {
            Coin temp = goldenCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseGoldenCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        for (int i = 0; i < silverCount; i++)
        {
            Coin temp = silverCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseSilverCoin"));
            AddChild(temp);
            temp.RemoveFromScene();

            temp = bronzeCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseBronzeCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        HandleCoinsSpawned(1, new Vector2(400, 400));
    }

    /// <summary>
    /// Handles dropped coins.
    /// </summary>
    /// <param name="coin">Silver coin scene.</param>
    /// <param name="position">Coin position.</param>
    public void HandleCoinsSpawned(int coins, Vector2 position)
    {
        Coin temp;
        for (int i = 0; i < 2; i++)
        {
            if (coins >= 50)
            {
                temp = goldenCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 50;
            }
            else if (coins >= 10)
            {
                temp = silverCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 10;
            }
            else if (coins > 0)
            {
                temp = bronzeCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 1;
            }
            else
            {
                return;
            }
            temp.AddToScene();
        }
        if (coins >= 50)
        {
            temp = goldenCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position;
        }
        else if (coins >= 10)
        {
            temp = silverCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position;
        }
        else if (coins > 0)
        {
            temp = bronzeCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position;
        }
    }

    /// <summary>
    /// Method for releasing gold coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Gold coin scene</param>
    private void ReleaseGoldenCoin(Coin coin)
    {
        coin.Gold = 50;
        goldenCoinsPool.Release(coin);
    }

    /// <summary>
    /// Method for releasing silver coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Silver coin scene</param>
    private void ReleaseSilverCoin(Coin coin)
    {
        coin.Gold = 10;
        silverCoinsPool.Release(coin);
    }

    /// <summary>
    /// Method for releasing bronze coin to pool (made non-usable).
    /// </summary>
    /// <param name="coin">Bronze coin scene</param>
    private void ReleaseBronzeCoin(Coin coin)
    {
        coin.Gold = 1;
        bronzeCoinsPool.Release(coin);
    }
}