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
    public bool isScholar = false;
    public string state = "Ready";
    public TextMesh roleText;
    public TextMesh timerText;
    private float timer = 0;

    public static string word;

    void Start()
    {
        GlobalVariables.word = WordBank.GetRandomWord();
        roleText.text = "";
        timer = 10;
        if (isScholar)
        {
            roleText.text = "You are the scholar! The word is " + GlobalVariables.word;
        }
        else
        {
            roleText.text = "You are the cheater!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "Done")
        {
            if (isScholar)
            {
                SceneManager.LoadScene("ScholarDiscussionScene");
            } else
            {
                SceneManager.LoadScene("CheaterDiscussionScene");
            }
        }
        else if (state == "Ready")
        {
            if (Input.GetMouseButtonDown(0))
            {
                state = "Drawing";
            }
        }
        else if (state == "Drawing")
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                state = "Done";
            }
            timerText.text = "" + Mathf.Round(timer * 10f) / 10f;
        }
    }
}
