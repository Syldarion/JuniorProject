﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkInitializer : MonoBehaviour
{
    [Header("Network Info")]
    public string PlayerName;
    public string IPAddress;
    public int Port;
    public string Password;

    void Start()
    {
        PlayerName = "Default";
        IPAddress = "localhost";
        Port = 5666;
        Password = string.Empty;
	}
	
	void Update()
    {
        	
	}

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    public void SetIPAddress(string address)
    {
        IPAddress = System.Net.IPAddress.Parse(address).ToString();
    }

    public void SetPassword(string password)
    {
        Password = password;
    }

    public void ConnectAsHost()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.networkPort = 5666;
        NetworkManager.singleton.StartHost();
    }

    public void ConnectAsClient()
    {
        NetworkManager.singleton.networkAddress = IPAddress;
        NetworkManager.singleton.networkPort = Port;
        NetworkManager.singleton.StartClient();
    }
}
