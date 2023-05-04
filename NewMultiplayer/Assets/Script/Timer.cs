using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public int matchLength = 120;
    private int currentMatchTime;
    private Coroutine timerCoroutine;
    private bool state = false;
    private PlayerController _playerController;
    
    [SerializeField]private TMP_Text Text;
    
    public enum EventCodes : byte
    {
        RefreshTimer
    }

    private void Start()
    {
        InitializeTimer();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (state == true)
        {
            return;
        }
        
    }

    private void InitializeTimer()
    {
        currentMatchTime = matchLength;
        RefreshTimerUI();

        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(TimerUI());
        }
    }

    private Transform ui_endGame;
    private void InitializeUI()
    {
        ui_endGame = GameObject.Find("CanvasPlayer").transform.Find("End Game").transform;
    }

    private void RefreshTimerUI()
    {
        string minutes = (currentMatchTime / 60).ToString("00");
        string seconds = (currentMatchTime % 60).ToString("00");
        Text.text = $"{minutes}:{seconds}";
    }

    private void EndGame(bool _state)
    {
        _state = state;
        //set game state to ending
        state = true;
        //set timer to 0
        if (timerCoroutine != null ) StopCoroutine(timerCoroutine);
        currentMatchTime = 0;
        RefreshTimerUI();
        
        //send to result scene
        
        //PhotonNetwork.DestroyAll();
        Disconnect();
        //SceneManager.LoadSceneAsync("Result");
        
        //DisconnectPlayer();
    }
    
    
    public void Disconnect()
    {
        
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.PlayerTtl = 0;
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;
            //PhotonNetwork.RaiseEvent(raiseEventOptions: Caching )
            
            
            PhotonNetwork.Disconnect();
            PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
            //PhotonNetwork.LoadLevel(2);
            SceneManager.LoadSceneAsync(2);
        
    }

    /*
    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    private IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(Launcher.ResultScene());
        PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
    }*/

    private IEnumerator TimerUI()
    {
        yield return new WaitForSeconds(1f);

        currentMatchTime -= 1;

        if (currentMatchTime <= 0)
        {
            timerCoroutine = null;
            EndGame(true);
        }
        else
        {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(TimerUI());
        }
        
    }
    

    public void RefreshTimer_S()
    {
        object[] package = new object[] {currentMatchTime};
        PhotonNetwork.RaiseEvent(
            (byte) EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions {Receivers = ReceiverGroup.All},
            new SendOptions {Reliability = true}
            );
    }

    public void RefreshTimer_R(object[] data)
    {
        currentMatchTime = (int) data[0];
        RefreshTimerUI();
    }
    

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes e = (EventCodes) photonEvent.Code;
        object[] o = (object[]) photonEvent.CustomData;
        switch (e)
        {
            case EventCodes.RefreshTimer:
                RefreshTimer_R(o);
                break;
        }
        
    }
}
