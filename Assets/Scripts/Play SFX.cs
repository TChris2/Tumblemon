using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    private AudioSource audiosource;
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    void playSFX(string sfxPath)
    {
        AudioClip clip = Resources.Load<AudioClip>(sfxPath);
        audiosource.PlayOneShot(clip);
    }
}
