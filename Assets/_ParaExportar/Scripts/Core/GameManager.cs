using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    public GameState State { get; private set; } = GameState.None;

    [Header("Meta")] 
    public string PlayerName { get; private set; } = "";
    public string PlayerGender { get; private set; } = ""; // future: enum

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        State = newState;
        // TODO: raise events if needed
    }

    // Basic hooks for external UI
    public void NewGame(string playerName, string gender)
    {
        PlayerName = playerName;
        PlayerGender = gender;
        SaveSystem.NewProfile(playerName, gender);
        SetState(GameState.Loading);
        SceneLoader.LoadGameLoop();
    }

    public void LoadGame()
    {
        var data = SaveSystem.Load();
        if (data != null)
        {
            PlayerName = data.playerName;
            PlayerGender = data.gender;
        }
        SetState(GameState.Loading);
        SceneLoader.LoadGameLoop();
    }

    public void SaveGame()
    {
        SaveSystem.Save(PlayerName, PlayerGender);
    }
}
