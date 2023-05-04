using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus;
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if(menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }
    
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void MainMenu()
    {
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
