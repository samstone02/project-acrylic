using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneManager : NetworkBehaviour
{
    [field: SerializeField] public string GameplayOverlaySceneName { get; private set; }

    public event Action GameplaySceneLoadEvent;

    public void LeaveGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelScene()
    {
        UnloadLobbySceneClientRpc();
        NetworkManager.SceneManager.LoadScene(SelectedLevelScene.SelectedLevelSceneName, LoadSceneMode.Additive);
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UnloadLobbySceneClientRpc()
    {
        SceneManager.UnloadSceneAsync("Lobby");
    }

    public void LoadGameplayOverlay()
    {
        SceneManager.LoadSceneAsync("GameplayOverlay", LoadSceneMode.Additive);
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete)
        {
            return;
        }

        if (sceneEvent.SceneName == SelectedLevelScene.SelectedLevelSceneName)
        {
            GameplaySceneLoadEvent?.Invoke();
        }
    }
}