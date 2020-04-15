using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MasterControl : MonoBehaviour {
  
  public int NetworkSpeed = 5;
  public string ip = "localhost";
  public string port = "8000";
  public string gameId = "CMS";

  private int client_id = -1;

  void Start() {
    StartCoroutine(Socket());
  }

  IEnumerator Socket() {
    // Connect to Websocket
    WebSocket w = new WebSocket(new Uri("ws://" + ip + ":" + port));
    yield return StartCoroutine(w.Connect());
    int uniqueid = (int) (Random.value * 99999);
    w.SendString("JOIN|" + uniqueid);
    Debug.Log("Sent joinZ");
    while (true)
    {
      string reply = w.RecvString();
      while (reply != null)
      {
        Debug.Log(reply);
        string[] split = reply.Split('|');
        if (split[0] == "CONFIRM") {
          string uid = split[1];
          if (uid == uniqueid.ToString()) {
            client_id = System.Convert.ToInt32(split[2]);;
            Debug.Log("My id is now " + client_id);
          }
        }
        reply = w.RecvString();
      }
      yield return 0;
    }
  }
}