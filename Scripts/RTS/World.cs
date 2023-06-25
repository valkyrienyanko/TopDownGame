namespace RTS;

public partial class World : Node
{
    // Static is convienant but arguably makes code more confusing to read / decouple
    public static World Instance { get; private set; }

    public static List<TileLayer> TileLayers { get; } = new();

    public static Dictionary<Vector2I, Chunk> Chunks { get; } = new();
    public static int ChunkSize { get; } = 10;
    public static int TileSize { get; } = 16;
    public static int SpawnRadius { get; } = 3;

    Node2D parentChunks;

    public override void _Ready()
    {
        Instance = this;

        parentChunks = new Node2D
        {
            Name = "Chunks"
        };
        AddChild(parentChunks);

        // DEBUG
        var prefab = GD.Load<PackedScene>("res://Scenes/Prefabs/test.tscn");
        var definedChunk = prefab.Instantiate<DefinedChunk>();
        definedChunk.Init();
        var tileLayers = definedChunk.TileLayers;
        //GD.Print(tileLayers.Count);
        definedChunk.QueueFree();

        SetupTileLayers();
        GenerateChunk(1, 0);
        //GenerateSpawn();
    }

    public void GenerateChunk(int x, int y)
    {
        var chunk = new Chunk(parentChunks, x, y);
        Chunks[new Vector2I(x, y)] = chunk;
    }

    void SetupTileLayers()
    {
        var grassNoise = new FastNoiseLite
        {
            Frequency = 0.1f
        };

        var treeNoise = new FastNoiseLite
        {
            Frequency = 0.05f,
            Offset = new Vector3(1000, 0, 0)
        };

        TileLayers.Add(new(
            zindex: -10,
            tileSetImagePath: "Sprites/basictiles.png",
            fnl: grassNoise,
            tileData: new()
            {
                { new TileData(new Vector2I(3, 1), 10f) { Name = "grass_1" } },
                { new TileData(new Vector2I(0, 8), 10f) { Name = "grass_2" } }
            }, 0f));

        TileLayers.Add(new(
            zindex: -9,
            tileSetImagePath: "Sprites/basictiles.png",
            fnl: treeNoise,
            tileData: new()
            {
                { new TileData(new Vector2I(6, 4), 50f, true) { Name = "tree_1" } }
            }, 30f));
    }

    void GenerateSpawn()
    {
        for (int x = -SpawnRadius; x <= SpawnRadius; x++)
        {
            for (int y = -SpawnRadius; y <= SpawnRadius; y++)
            {
                GenerateChunk(x, y);
            }
        }
    }
}
