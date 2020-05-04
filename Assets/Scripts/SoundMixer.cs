using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMixer : MonoBehaviour
{

    public static AudioSource bgm;
    public static AudioSource soundeffect;

    public AudioClip track;

    public AudioClip test;

    public static readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();


    // Start is called before the first frame update
    void Start()
    {

        bgm = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        soundeffect = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        bgm.loop = true;
        bgm.clip = track;
        bgm.volume = 0.5f;
        bgm.Play();

        sounds.Add("test sound effect", test);

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

