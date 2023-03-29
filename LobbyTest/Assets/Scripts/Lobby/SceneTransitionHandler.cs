using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : NetworkBehaviour
{
    static public SceneTransitionHandler sceneTransitionHandler { get; internal set; }
    [SerializeField] public string DefaultMainMenu = "StartMenu";

    [HideInInspector]
    public delegate void ClientLoadedSceneDelegateHandler(ulong clientID);

    [HideInInspector] public event ClientLoadedSceneDelegateHandler OnClientLoadedScene;

    [HideInInspector]
    public delegate void SceneStateChangedDelegateHandler(SceneStates newState);

    [HideInInspector] public event SceneStateChangedDelegateHandler OnSceneStateChanged;

    private int m_numberOfClientLoaded;
    
    // example scene
    public enum SceneStates
    {
        Init,
        Start,
        Lobby,
        Ingame
    }

    private SceneStates m_SceneState;

    //awake
    //set our scene state to INIT
    private void Awake()
    {
        if (sceneTransitionHandler != this && sceneTransitionHandler != null)
        {
            GameObject.Destroy(sceneTransitionHandler.gameObject);
        }

        sceneTransitionHandler = this;
        SetSceneState(SceneStates.Init);
        DontDestroyOnLoad(this);
    }
    
    //SetSceneState

    public void SetSceneState(SceneStates sceneStates)
    {
        m_SceneState = sceneStates;
        if (OnSceneStateChanged != null)
        {
            OnSceneStateChanged.Invoke(m_SceneState);
        }
    }
    
    //GetCurrentSceneState
    public SceneStates GetCurrentSceneState()
    {
        return m_SceneState;
    }
    
    //Start
    private void Start()
    {
        if (m_SceneState == SceneStates.Init)
        {
            SceneManager.LoadScene(DefaultMainMenu);
        }
    }
    
    //Registers callbacks to the NetworkSceneManager. should be called when starting the server
    public void RegisterCallbacks()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }
    
    //Switches to a new scene
    public void SwitchScene(string sceneName)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            m_numberOfClientLoaded = 0;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        m_numberOfClientLoaded += 1;
        OnClientLoadedScene?.Invoke(clientId);
    }

    public bool AllClientsAreLoaded()
    {
        return m_numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count;
    }
    
    //Exit

    public void ExitAndLoadStartMenu()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        OnClientLoadedScene = null;
        SetSceneState(SceneStates.Start);
        SceneManager.LoadScene(1);
    }
}
