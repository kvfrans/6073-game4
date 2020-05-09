using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Discussion : MonoBehaviour
{
    public string guess;
    public GameObject promptText;
    public GameObject inputField;
    public GameObject textDisplay;

    public void GuessWord()
    {
        string guess = inputField.GetComponent<Text>().text;
        GameObject.Find("MasterControl").GetComponent<MasterControl>().CheaterGuess(guess);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
