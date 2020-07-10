using System.Collections;
using System.Collections.Generic;
using System;
using SkillzSDK;
using UnityEngine;
using Nakama;

public class NakamaClient : MonoBehaviour
{
    private string deviceId;

    private readonly IClient client = new Client("http", "192.168.1.121", 7350, "defaultkey");

    private string serverMatchId;

    private ISocket socket;
    // Start is called before the first frame update
    async void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        Dictionary<string, string> authParams = new Dictionary<string, string>();
        authParams.Add("skillz_match_token", "1234567890");

        var session = await client.AuthenticateCustomAsync(deviceId, "test-client-2", false, authParams);
        //var session = await client.AuthenticateDeviceAsync(deviceId, "test-client-2", false);

        Debug.Log("Device authenticated with token:");
        Debug.Log(session.AuthToken); // raw JWT token
        Debug.LogFormat("Session user id: '{0}'", session.UserId);
        Debug.LogFormat("Session user username: '{0}'", session.Username);
        Debug.LogFormat("Session has expired: {0}", session.IsExpired);
        Debug.LogFormat("Session expires at: {0}", session.ExpireTime); // in seconds.

        socket = client.NewSocket();

        socket.Connected += () => Debug.Log("Socket Connected");
        socket.Closed += () => Debug.Log("Socket Closed");
        socket.ReceivedMatchState += ReceiveMatchStateMessage;

        await socket.ConnectAsync(session);



        //var match = await socket.CreateMatchAsync();
        //Debug.LogFormat("New match with id '{0}'.", match.Id);
        //var matchId = "01234567890123456789.nakama";

        var resp = await socket.RpcAsync("create_skillz_match", "{\"match_id\": \"01234567890123456789.nakama\"}");
        GameMsg gMsg = JsonUtility.FromJson<GameMsg>(resp.Payload);
        Debug.LogFormat("New Match ID: {0}", gMsg.match_id);

        serverMatchId = gMsg.match_id;
        var match = await socket.JoinMatchAsync(serverMatchId);


    }

    // Update is called once per frame
    void Update()
    {
        //SkillzCrossPlatform.LaunchSkillz(new GameManager());
    }

    private void ReceiveMatchStateMessage(IMatchState matchState)
    {
        string messageJson = System.Text.Encoding.UTF8.GetString(matchState.State);
        Debug.LogFormat("Message: {0}", messageJson);
    }

    public void SendMessageToServer()
    {
        var opCode = 1;
        socket.SendMatchStateAsync(serverMatchId, opCode, "{\"hello\":\"from client 1\"}");
    }

    private void OnApplicationQuit()
    {
        socket?.CloseAsync();
    }
}

public class GameMsg
{
    public string match_id;
}