using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField]
    Button joinButton;
    
    // Start is called before the first frame update
    void Start()
    {
        joinButton.onClick.AddListener(() => joinButtonClicked());
        joinButton.gameObject.SetActive(true);
        Debug.Log("Start scene initiated and join button activated");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void joinButtonClicked()
    {
        Debug.Log("Join button clicked");
        
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }
}
