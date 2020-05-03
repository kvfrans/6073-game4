using UnityEngine;
using System.Collections;
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
        if (guess == GlobalVariables.word)
        {
            textDisplay.GetComponent<Text>().text = "Correct, you win!";
        } else
        {
            textDisplay.GetComponent<Text>().text = "Incorrect, you lose!";
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("LOOK");
        Debug.Log(GameFlow.Instance.isScholar);
        if (GameFlow.Instance.isScholar)
        {
            promptText.GetComponent<Text>().text = "Who was the cheater?";
        } else
        {
            promptText.GetComponent<Text>().text = "What was the word?";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
