using UnityEngine;
using UnityEngine.SceneManagement;

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
    private static string[] levelSceneNames = { "Level1" };

    // The name of the start menu scene
    private static string startMenuSceneName = "StartMenu";

    // The name of the level complete menu scene
    private static string levelCompleteMenuSceneName = "LevelCompleteMenu";

    // The name of the level failed menu scene
    private static string levelFailedMenuSceneName = "LevelFailedMenu";

    // The name of the game complete menu scene
    private static string gameCompleteMenuSceneName = "GameCompleteMenu";

    // The index of the current level's scene name in levelScenesNames
    private static int currentLevelIndex = 0;

    public static void Reset()
    {
        currentLevelIndex = 0;
        SceneManager.LoadScene(startMenuSceneName);
    }

    public static void CompleteLevel()
    {
        if(currentLevelIndex < levelSceneNames.Length - 1) 
        { 
            currentLevelIndex++;
            SceneManager.LoadScene(levelCompleteMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameCompleteMenuSceneName);
        }
    }

    public static void RetryLevel()
    {
        SceneManager.LoadScene(levelFailedMenuSceneName);
    }

    public static void LoadLevel()
    {
        SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
    }

    public static void DisplayStartMenu()
    {
        SceneManager.LoadScene(startMenuSceneName);
    }

    public static float Score
    { get; set; } = 10000f;
}