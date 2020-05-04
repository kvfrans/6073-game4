using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMixer : MonoBehaviour
{

    public static AudioSource bgm;

    public AudioClip track;


    // Start is called before the first frame update
    void Start()
    {

        bgm = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        bgm.loop = true;
        bgm.clip = track;
        bgm.volume = 0.25f;
        bgm.Play();

    }

    // Update is called once per frame
    void Update()
    {
    }

    /*    public static void PlaySound(AudioSource source, string sound, float volume = 1.0f)
        {
            source.PlayOneShot(sounds[sound], volume);
        }*/

}

