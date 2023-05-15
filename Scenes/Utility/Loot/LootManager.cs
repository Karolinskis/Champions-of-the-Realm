namespace ChampionsOfTheRealm;

public partial class LootManager : Node2D
{
    [Export] private int goldenCount = 100; // Maximum number of gold coins in a scene.
    [Export] private int silverCount = 100; // Max number of silver and bronze coins in a scene.
    private PackedScene goldenCoinScene; // Gold coin resource
    private PackedScene silverCoinScene; // Silver coin resource
    private PackedScene bronzeCoinScene; // Bronze coin resource
    private ObjectPool<Coin> goldenCoinsPool; // Pool for gold coins
    private ObjectPool<Coin> silverCoinsPool; // Pool for silver coins
    private ObjectPool<Coin> bronzeCoinsPool; // Pool for bronze coins

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

        HandleCoinsSpawned(159, new Vector2(400, 400)); //For debugging purposes
    }

    /// <summary>
    /// Handles the spawning of dropped coins
    /// </summary>
    /// <param name="coin">The number of coins to spawn</param>
    /// <param name="position">The position where the coins should be spawned</param>
    public void HandleCoinsSpawned(int coins, Vector2 position)
    {
        Coin temp;

        while (coins > 0)
        {
            if (coins >= 100)
            {
                temp = goldenCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 100;
            }
            else if (coins >= 25)
            {
                temp = silverCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 25;
            }
            else
            {
                temp = bronzeCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 1;
            }

            temp.AddToScene();
        }
    }

    /// <summary>
    /// Releases a gold coin back to the pool (makes it non-usable)
    /// </summary>
    /// <param name="coin">Gold coin scene</param>
    private void ReleaseGoldenCoin(Coin coin) => goldenCoinsPool.Release(coin);

    /// <summary>
    /// Releases a silver coin back to the pool (makes it non-usable)
    /// </summary>
    /// <param name="coin">Silver coin scene</param>
    private void ReleaseSilverCoin(Coin coin) => silverCoinsPool.Release(coin);

    /// <summary>
    /// Releases a bronze coin back to the pool (makes it non-usable)
    /// </summary>
    /// <param name="coin">Bronze coin scene</param>
    private void ReleaseBronzeCoin(Coin coin) => bronzeCoinsPool.Release(coin);
}