using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource backgroundMusic;
    private void Start()
    {
        backgroundMusic = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        backgroundMusic.volume = StaticData.musicValue;
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
    }
    private void Update()
    {
        backgroundMusic.volume = StaticData.musicValue;
    }
}
