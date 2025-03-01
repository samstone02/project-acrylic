using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionSceneManager : NetworkBehaviour
{
    [field: SerializeField] public string FindMatchSceneName { get; private set; }
    [field: SerializeField] public string LobbySceneName { get; private set; }
    [field: SerializeField] public string GameplaySceneName { get; private set; }
    [field: SerializeField] public string PostGameSceneName { get; private set; }

    private Scene LobbyScene { get; set; }
    private Scene GameplayScene { get; set; }
    private Scene PostGameScene { get; set; }

    private void Awake()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnSceneEvent += Handle_OnSceneEvent;
        }
    }

    public void LoadFindMatchScene()
    {
        SceneManager.LoadScene(FindMatchSceneName, LoadSceneMode.Additive);
    }

    public void UnloadFindMatchScene()
    {
        SceneManager.UnloadSceneAsync(FindMatchSceneName);
    }

    public void LoadLobbyScene()
    {
        if (GameplayScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(GameplayScene);
        }
        if (PostGameScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(PostGameScene);
        }

        NetworkManager.SceneManager.LoadScene(LobbySceneName, LoadSceneMode.Additive);
    }

    public void LoadGameplayScene()
    {
        if (LobbyScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(LobbyScene);
        }
        if (PostGameScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(PostGameScene);
        }

        NetworkManager.SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Additive);
    }

    public void LoadPostGameScene()
    {
        if (LobbyScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(LobbyScene);
        }
        if (PostGameScene.isLoaded)
        {
            NetworkManager.SceneManager.UnloadScene(GameplayScene);
        }

        NetworkManager.SceneManager.LoadScene(PostGameSceneName, LoadSceneMode.Additive);
    }

    private void Handle_OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete)
        {
            return;
        }

        if (sceneEvent.SceneName == LobbySceneName)
        {
            LobbyScene = sceneEvent.Scene;
        }
        else if (sceneEvent.SceneName == GameplaySceneName)
        {
            GameplaySceneName = sceneEvent.SceneName;
        }
        else if (sceneEvent.SceneName == PostGameSceneName)
        {
            PostGameScene = sceneEvent.Scene;
        }
        else
        {
            throw new Exception("Unrecognized scene name: " +  sceneEvent.SceneName);
        }
    }
}
