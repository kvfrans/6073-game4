using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{

    protected static GameFlow instance;
    public static GameFlow Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<GameFlow>();
            return instance;
        }
    }
    public bool isScholar = true;
    public string state = "Ready";
    public TextMesh roleText;
    public TextMesh timerText;
    private float timer = 10;

    public static string word;

    void Start() {
        isScholar = GameObject.Find("MasterControl").GetComponent<MasterControl>().isScholar;
        roleText.text = "";
        timer = 10;
        if (isScholar)
        {
            roleText.text = "You are the scholar! The word is " + GameObject.Find("MasterControl").GetComponent<MasterControl>().word;
        }
        else
        {
            roleText.text = "You are the cheater!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0) {
            SceneManager.LoadScene("DiscussionScene");
        }
        timerText.text = "" + Mathf.Round(timer * 10f) / 10f;
    }
}
