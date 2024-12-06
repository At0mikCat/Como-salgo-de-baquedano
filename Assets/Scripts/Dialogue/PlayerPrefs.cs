using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefs : MonoBehaviour
{
    public static PlayerPrefs current;

    private void Awake()
    {
        if (current == null)
            current = this;

        DontDestroyOnLoad(this.gameObject);
    }
    
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
    public float textSpeed;

    public GameObject lastDialogueBeforeLeaving;

    public int Minigame1Route;
    public bool Minigame1Complete;
    public int Minigame2Route;
    public bool Minigame2Complete;
    public int Minigame3Route;
    public bool Minigame4Complete;
}