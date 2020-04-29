using System.Collections;
using UnityEngine;
using System;

public class ws_script : MonoBehaviour
{
  public int NetworkSpeed = 5;
  public string gameId = "jynkev";

  IEnumerator Start()
  {
    // Connect to Websocket
    WebSocket w = new WebSocket(new Uri("ws://" + Constants.IP + ":" + Constants.PORT));
    yield return StartCoroutine(w.Connect());

    w.SendString("UNITY|" + gameId + "|NEW");

    while (true)
    {
      string reply = w.RecvString();
      if (reply != null)
      {
        Debug.Log(reply);
      }
      if (w.error != null)
      {
        Debug.LogError("Error: " + w.error);
        break;
      }
      yield return 0;
    }
    w.SendString("UNITY|" + gameId + "|CLOSE"); //TODO: doesn't work
    w.Close();
  }
}

