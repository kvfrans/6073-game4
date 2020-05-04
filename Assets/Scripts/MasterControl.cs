using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MasterControl : MonoBehaviour {
  
    public int NetworkSpeed = 5;
    public string gameId = "CMS";

    private int client_id = -1;
    private WebSocket socket;

    public DrawingControl dc;

    // start scene
    private Button joinButton;

    // lobby scene
    private GameObject[] lobbyPlayerAvatars;
    private Text[] lobbyPlayerUsernames;
    private GameObject waitingMsg;
    private Button startButton;

    // game variables
    private string username;
    private string[] players;
    private int cheater_id;
    private string word;

    // sound effects
    public static AudioSource soundeffect;
    public static readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    public AudioClip joinGreeting;

    void Start() {
        UnityEngine.Object.DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        joinButton = GameObject.Find("Join Button").GetComponent<Button>();
        joinButton.onClick.AddListener(() => JoinButtonClicked());
        joinButton.gameObject.SetActive(true);
        Debug.Log("Start scene initiated and join button activated");
        // StartCoroutine(DrawableSend());
        username = "ezou";

        // Initialize sounds
        soundeffect = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        sounds.Add("JoinGreeting", joinGreeting);
    }

    void JoinButtonClicked() {
        Debug.Log("Join button clicked");
        StartCoroutine(Socket());
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    void StartButtonClicked() {
        Debug.Log("Start button clicked");
        int cheater = Random.Range(1, 1 + Constants.NUM_PLAYERS);
        string word = Constants.DICTIONARY[Random.Range(0, Constants.DICTIONARY.Length)];
        socket.SendString("START|" + cheater + "|" + word);
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("OnSceneLoaded: " + scene.name + ", mode: " + mode);
        if (scene.name == "LobbyScene")
            StartLobby();
    }

    void StartLobby() {
        lobbyPlayerAvatars = new GameObject[Constants.NUM_PLAYERS];
        lobbyPlayerUsernames = new Text[Constants.NUM_PLAYERS];
        for (int i = 1; i <= Constants.NUM_PLAYERS; i++) {
            // find player objects in lobby
            lobbyPlayerAvatars[i - 1] = GameObject.Find("player" + i);
            lobbyPlayerUsernames[i - 1] = GameObject.Find("player" + i + "_text").GetComponent<Text>();
            // by default, disable all player objects
            lobbyPlayerAvatars[i - 1].SetActive(false);
            lobbyPlayerUsernames[i - 1].gameObject.SetActive(false);
        }

        // find other objects
        waitingMsg = GameObject.Find("waiting");
        startButton = GameObject.Find("Start Button").GetComponent<Button>();
        // setup button
        startButton.onClick.AddListener(() => StartButtonClicked());
        startButton.gameObject.SetActive(false);
    }

    IEnumerator Socket() {
        // Connect to Websocket
        socket = new WebSocket(new Uri("ws://e6c9c783.ngrok.io"));
//        socket = new WebSocket(new Uri("ws://localhost:8000"));
        yield return StartCoroutine(socket.Connect());
        socket.SendString("JOIN|" + username);
        Debug.Log("Sent join request");
        while (true) {
            string reply = socket.RecvString();
            while (reply != null) {
                Debug.Log(reply);
                string[] split = reply.Split('|');
                if (split[0] == "CONFIRM") {
                    string uid = split[1];
                    if (uid == username) {
                        client_id = System.Convert.ToInt32(split[2]);;
                        Debug.Log("My id is now " + client_id);
                    }
                }
                else if (split[0] == "PLAYERS") {
                    Debug.Log(split[1]);
                    players = split[1].Split('_');
                    for (int i = 0; i < players.Length; i++) {
                        lobbyPlayerAvatars[i].SetActive(true);
                        lobbyPlayerUsernames[i].text = players[i];
                        lobbyPlayerUsernames[i].gameObject.SetActive(true);
                    }
                    if (players.Length == Constants.NUM_PLAYERS) {
                        waitingMsg.SetActive(false);
                        startButton.gameObject.SetActive(true);
                    }
                    soundeffect.PlayOneShot(sounds["JoinGreeting"], 0.5f);
                }
                else if (split[0] == "START") {
                    cheater_id = System.Convert.ToInt32(split[1]);
                    word = split[2];
                    SceneManager.LoadScene("ExampleDrawingSceneKevinTest");
                }
                else if (split[0] == "DRAW") {
                    // Debug.Log(reply);
                    string b64 = split[2];
                    byte[] imageData = Convert.FromBase64String(b64);
                    int who_drew = System.Convert.ToInt32(split[1]);
                    if (client_id == 1) {
                        if (who_drew == 2) {
                            dc.canvas1.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 3) {
                            dc.canvas2.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 4) {
                            dc.canvas3.sprite.texture.LoadImage(imageData);
                        }
                    }
                    if (client_id == 2) {
                        if (who_drew == 1) {
                            dc.canvas1.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 3) {
                            dc.canvas2.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 4) {
                            dc.canvas3.sprite.texture.LoadImage(imageData);
                        }
                    }
                    if (client_id == 3) {
                        if (who_drew == 1) {
                            dc.canvas1.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 2) {
                            dc.canvas2.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 4) {
                            dc.canvas3.sprite.texture.LoadImage(imageData);
                        }
                    }
                    if (client_id == 4) {
                        if (who_drew == 1) {
                            dc.canvas1.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 2) {
                            dc.canvas2.sprite.texture.LoadImage(imageData);
                        }
                        if (who_drew == 3) {
                            dc.canvas3.sprite.texture.LoadImage(imageData);
                        }
                    }
                }

                reply = socket.RecvString();
            }
            yield return 0;
        }
    }



    public void SendStringServer(string command, string s) {
        socket.SendString(command + "|" + client_id.ToString() + "|" + s);
    }
}