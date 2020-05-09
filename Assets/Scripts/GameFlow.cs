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
    private float timer = 50;
    public Transform cheaterBlock;

    public static string word;

    // sound effects
    public static AudioSource soundeffect;
    public AudioClip narratorWarning;
    public AudioClip narratorDiscussion1;
    public AudioClip narratorDiscussion2;
    private bool warningPlayed = false;
    private string scene;
    private bool playedSound = false;

    void Start() {
        isScholar = GameObject.Find("MasterControl").GetComponent<MasterControl>().isScholar;
        if (!isScholar) {
            cheaterBlock.gameObject.SetActive(true);
        }
        
        roleText.text = "";
        timer = 50;
        if (isScholar)
        {
            roleText.text = "You are the scholar! The word is " + GameObject.Find("MasterControl").GetComponent<MasterControl>().word;
        }
        else
        {
            roleText.text = "You are the cheater!";
        }

        //init sounds
        soundeffect = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 10 && !warningPlayed)
        {
            soundeffect.PlayOneShot(narratorWarning, 0.3f);
            warningPlayed = true;
        }
        else if (timer < 0) {
            if (isScholar)
            {
                scene = "ScholarDiscussionScene";

            } else
            {
                scene = "CheaterDiscussionScene";
            }

            if (!playedSound) {
                playedSound = true;
                soundeffect.PlayOneShot(narratorDiscussion1, 0.3f);
                StartCoroutine(waitForSound(scene));
            }
        }
        timerText.text = "" + Mathf.Round(timer * 10f) / 10f;
        
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

}
