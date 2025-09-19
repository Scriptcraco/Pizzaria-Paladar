using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // Adjust to your actual scene names/indices
    private const string MainMenu = "StartMenu";
    private const string GameLoop = "GameLoop";
    private const string Customization = "Customization"; // create later

    public static void LoadMainMenu() => SceneManager.LoadScene(MainMenu);
    public static void LoadGameLoop() => SceneManager.LoadScene(GameLoop);
    public static void LoadCustomization() => SceneManager.LoadScene(Customization);
}
