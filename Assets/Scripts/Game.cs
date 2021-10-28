public class Game
{
    // This class is a singleton
    private static Game instance;

    private Game() { }

    public static Game GetInstance()
    {
        if (instance == null) { instance = new Game(); }
        return instance;
    }

    // Stores the names of the level scenes in order
    private static string[] levelScenesNames = { "Level1" };

    // The name of the scene holding the start menu
    public static string StartMenuSceneName { get; } = "StartMenu";

    // The name of the scene holding the menu displayed upon level completion
    public static string LevelCompleteMenuSceneName { get; } = "LevelCompleteMenu";

    // The name of the scene holding the menu displayed upon game completion
    public static string gameCompleteMenuSceneName { get; } = "LevelCompleteMenu";

    // The index of the current level's scene name in levelScenesNames
    public static int currentLevelIndex = 0;

    public static string GetCurrentLevelSceneName()
    {
        if(currentLevelIndex >= levelScenesNames.Length) { return gameCompleteMenuSceneName; }
        if(currentLevelIndex < 0) { currentLevelIndex = 0; return StartMenuSceneName; }
        return levelScenesNames[currentLevelIndex];
    }

    public static float Score
    { get; set; } = 10000f;
}