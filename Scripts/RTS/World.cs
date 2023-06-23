namespace RTS;

public partial class World : Node
{
    // Static is convienant but arguably makes code more confusing to read / decouple
    public static World Instance { get; private set; }

    public static List<Atlas> Atlases { get; } = new();

    public static Dictionary<Vector2I, Chunk> Chunks { get; } = new();
    public static int ChunkSize { get; } = 10;
    public static int TileSize { get; } = 16;
    public static int SpawnRadius { get; } = 3;

    [Export] public TileMap Grass { get; set; }
    [Export] public TileMap Trees { get; set; }

    Dictionary<string, TileData> tileDataGrass { get; } = new()
    {  
        { "grass_1", new TileData(new Vector2I(3, 1), 10f) },
        { "grass_2", new TileData(new Vector2I(0, 8), 10f) }
    };

    Dictionary<string, TileData> tileDataTrees { get; } = new()
    {
        { "tree_1",  new TileData(new Vector2I(6, 4), 10f) }
    };

    Node2D parentChunks;

    public override void _Ready()
    {
        Instance = this;

        parentChunks = new Node2D
        {
            Name = "Chunks"
        };
        AddChild(parentChunks);

        var grassNoise = new FastNoiseLite
        {
            Frequency = 0.1f
        };

        var treeNoise = new FastNoiseLite
        {
            Frequency = 0.3f,
            Offset = new Vector3(1000, 0, 0)
        };

        Atlases.Add(new(-10, Grass, grassNoise, tileDataGrass));
        Atlases.Add(new(-9 , Trees, treeNoise,  tileDataTrees, 30f));

        GenerateChunk(0, 0);
        GenerateSpawn();
    }

    public void GenerateChunk(int x, int y)
    {
        var chunk = new Chunk(parentChunks, x, y);
        Chunks[new Vector2I(x, y)] = chunk;
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
