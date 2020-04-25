using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    private bool isScholar = true;
    public string state = "Ready";
    public TextMesh roleText;
    public TextMesh timerText;
    private float timer = 0;

    public static string word;

    void Start()
    {
        word = WordBank.GetRandomWord();
        roleText.text = "";
        timer = 30;
        if (isScholar)
        {
            roleText.text = "You are the scholar! The word is " + word;
        }
        else
        {
            roleText.text = "You are the cheater!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            roleText.text = "DONE!";
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
            timerText.text = "" + Mathf.Round(timer * 10f) / 10f;
        }
    }
}
