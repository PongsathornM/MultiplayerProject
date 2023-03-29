using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Text m_HostIpInput;
    [SerializeField] private string m_LobbySceneName = "Lobby";

    public void StartLocalGame()
    {
        //update the current HostNameInput
        var utpTransport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (utpTransport) m_HostIpInput.text = "127.0.0.1";
        if (NetworkManager.Singleton.StartHost())
        {
            SceneTransitionHandler.sceneTransitionHandler.RegisterCallbacks();
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public void JoinLocalGame()
    {
        if (m_HostIpInput.text != "Hostname")
        {
            var utpTransport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            if (utpTransport)
            {
                utpTransport.SetConnectionData(Sanitize(m_HostIpInput.text), 7777);
            }

            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogError("Failed to start client.");
            }
        }
    }

    public static string Sanitize(string dirtyString)
    {
        //sanitize
        return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
    }
}
