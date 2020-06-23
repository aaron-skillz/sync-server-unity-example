using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Nakama;

public class NakamaClient : MonoBehaviour
{
    private string deviceId;

    private readonly IClient client = new Client("http", "192.168.1.139", 7350, "defaultkey");

    private ISocket socket;
    // Start is called before the first frame update
    async void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;

        var session = await client.AuthenticateDeviceAsync(deviceId, "test-client", false);
        Debug.Log("Device authenticated with token:");
        Debug.Log(session.AuthToken); // raw JWT token
        Debug.LogFormat("Session user id: '{0}'", session.UserId);
        Debug.LogFormat("Session user username: '{0}'", session.Username);
        Debug.LogFormat("Session has expired: {0}", session.IsExpired);
        Debug.LogFormat("Session expires at: {0}", session.ExpireTime); // in seconds.

        socket = client.NewSocket();

        socket.Connected += () => Debug.Log("Socket Connected");
        socket.Closed += () => Debug.Log("Socket Closed");

        await socket.ConnectAsync(session);


        var match = await socket.CreateMatchAsync();
        Debug.LogFormat("New match with id '{0}'.", match.Id);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        socket?.CloseAsync();
    }
}
