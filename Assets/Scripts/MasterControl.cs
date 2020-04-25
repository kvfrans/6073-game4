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
  private WebSocket w;

  public SpriteRenderer canvas;
  public SpriteRenderer canvas2;

  void Start() {
    StartCoroutine(Socket());
    StartCoroutine(DrawableSend());
  }

  IEnumerator Socket() {
    // Connect to Websocket
    w = new WebSocket(new Uri("ws://" + ip + ":" + port));
    yield return StartCoroutine(w.Connect());
    int uniqueid = (int) (Random.value * 99999);
    w.SendString("JOIN|" + uniqueid);
    Debug.Log("Sent joinZ");
    while (true)
    {
      string reply = w.RecvString();
      while (reply != null)
      {
//        Debug.Log(reply);
        string[] split = reply.Split('|');
        if (split[0] == "CONFIRM") {
          string uid = split[1];
          if (uid == uniqueid.ToString()) {
            client_id = System.Convert.ToInt32(split[2]);;
            Debug.Log("My id is now " + client_id);
          }
        }
        else if (split[0] == "DRAW") {
//          Debug.Log(reply);
          string b64 = split[2];
          byte[] imageData = Convert.FromBase64String(b64);
          canvas2.sprite.texture.LoadImage(imageData);
        }

        reply = w.RecvString();
      }
      yield return 0;
    }
  }

  IEnumerator DrawableSend() {
    while (true) {
      yield return new WaitForSeconds(1.0f);
      Debug.Log("sending bytes");
      byte[] imageData;
      imageData = canvas.sprite.texture.EncodeToPNG();
      string s = Convert.ToBase64String(imageData);
      SendStringServer("DRAW", s);
    }
  }

  public void SendStringServer(string command, string s) {
    w.SendString(command + "|" + client_id.ToString() + "|" + s);
  }
}