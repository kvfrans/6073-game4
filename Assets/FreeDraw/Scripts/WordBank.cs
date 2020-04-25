using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordBank : MonoBehaviour
{

    private static string[] words = new string[5] { "pug", "lion", "San Francisco", "ramen", "boba" };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static string GetRandomWord()
    {
        //System.Random random = new System.Random();
        int index = Random.Range(0, words.Length);
        return words[index];
    }
}
