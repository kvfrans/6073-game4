﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private TextMeshProUGUI usernameText;

    // lobby scene
    private GameObject[] lobbyPlayerAvatars;
    private Text[] lobbyPlayerUsernames;
    private GameObject waitingMsg;
    private Button startButton;

    // scholar discussion scene
    private Button[] playerButtons;

    // game variables
    private string username;
    private string[] players;
    private int cheater_id;
    public string word;
    public bool isScholar = true;

    // sound effects
    public static AudioSource soundeffect;
    public AudioClip joinGreeting;
    public AudioClip narratorIntro;

    void Start() {
        UnityEngine.Object.DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        joinButton = GameObject.Find("Join Button").GetComponent<Button>();
        joinButton.onClick.AddListener(() => JoinButtonClicked());
        joinButton.gameObject.SetActive(true);
        usernameText = GameObject.Find("Username Text").GetComponent<TextMeshProUGUI>();
        Debug.Log("Start scene initiated and join button activated");
        username = "";

        // Initialize sounds
        soundeffect = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
    }

    void JoinButtonClicked() {
        Debug.Log("Join button clicked");
        username = usernameText.text;
        username = username.Substring(0, username.Length - 1); // for some reason theres always ASCII character 8203 (zero width space) at the end
        Debug.Log("Username is: " + username);
        if (username.Length < Constants.MIN_USERNAME_LENGTH) {
            Debug.Log("Username must be between 3-10 characters");
            return;
        }
        StartCoroutine(Socket());
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    void StartButtonClicked() {
        Debug.Log("Start button clicked");

        int cheater = Random.Range(1, 1 + Constants.NUM_PLAYERS);
        string word = Constants.DICTIONARY[Random.Range(0, Constants.DICTIONARY.Length)];
        socket.SendString("START|" + cheater + "|" + word);
    }

    void PlayerButtonClicked(Int32 player) {
        Debug.Log("Player " + player + " button clicked");
        socket.SendString("SCHOLAR_GUESS|" + username + "|" + player);
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("OnSceneLoaded: " + scene.name + ", mode: " + mode);
        if (scene.name == "LobbyScene")
            StartLobby();
        if (scene.name == "ScholarDiscussionScene")
            StartScholarDiscussion();
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

    void StartScholarDiscussion() {
        playerButtons = new Button[Constants.NUM_PLAYERS];
        for (int i = 1; i <= Constants.NUM_PLAYERS; i++) {
            playerButtons[i - 1] = GameObject.Find("Button" + i).GetComponent<Button>();
            playerButtons[i - 1].onClick.AddListener(() => PlayerButtonClicked(i));
            GameObject.Find("Text" + i).GetComponent<Text>().text = players[i - 1];
        }
    }

    public void CheaterGuess(string guess) {
        Debug.Log("Cheater has guessed the word " + guess);
        socket.SendString("CHEATER_GUESS|" + username + "|" + guess);
    }

    IEnumerator Socket() {
        // Connect to Websocket
        // socket = new WebSocket(new Uri("ws://e6c9c783.ngrok.io"));
        socket = new WebSocket(new Uri("ws://7cf7cbc7.ngrok.io"));
        // http://10ec0522.ngrok.io
        // socket = new WebSocket(new Uri("ws://localhost:8000"));

        yield return StartCoroutine(socket.Connect());
        socket.SendString("JOIN|" + username);
        Debug.Log("Sent join request");
        while (true) {
            string reply = socket.RecvString();
            while (reply != null) {
                // Debug.Log(reply);
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
                    soundeffect.PlayOneShot(joinGreeting, 0.5f);
                }
                else if (split[0] == "START") {
                    cheater_id = System.Convert.ToInt32(split[1]);
                    word = split[2];
                    if (cheater_id == client_id) {
                        isScholar = false;
                    }
                    soundeffect.PlayOneShot(narratorIntro, 0.3f);
                    startButton.gameObject.SetActive(false);
                    StartCoroutine(waitForSound("ExampleDrawingSceneKevinTest"));
                }
                else if (split[0] == "DRAW" && SceneManager.GetActiveScene().name == "ExampleDrawingSceneKevinTest") {
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
                else if (split[0] == "CHEATER_GUESS") {
                    if (split[2] == word) {
                        Debug.Log(word);
                        Debug.Log(split[2]);
                        SceneManager.LoadScene("EndSceneCheater");
                    }
                }

                reply = socket.RecvString();
            }
            yield return 0;
        }
    }

    IEnumerator waitForSound(string scene)
    {
        //Wait Until Sound has finished playing
        while (soundeffect.isPlaying)
        {
            yield return null;
        }

        //Auidio has finished playing, change scene
        SceneManager.LoadScene(scene);
    }

    public void SendStringServer(string command, string s) {
        socket.SendString(command + "|" + client_id.ToString() + "|" + s);
    }
}