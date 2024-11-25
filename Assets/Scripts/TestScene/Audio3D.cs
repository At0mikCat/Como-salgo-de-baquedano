using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio3D : MonoBehaviour
{
    public enum Gender { Male, Female }
    public Gender npcGender;

    [Header("Audio Pools")]
    [SerializeField] private List<AudioClip> maleAudioPool;
    [SerializeField] private List<AudioClip> femaleAudioPool;

    [Header("Audio Source")]
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(ChooseAudio());
    }

    IEnumerator ChooseAudio()
    {
        while (true) 
        {
            AudioClip chosenClip = null;

            if (npcGender == Gender.Male && maleAudioPool.Count > 0)
            {
                chosenClip = maleAudioPool[Random.Range(0, maleAudioPool.Count)];
            }
            else if (npcGender == Gender.Female && femaleAudioPool.Count > 0)
            {
                chosenClip = femaleAudioPool[Random.Range(0, femaleAudioPool.Count)];
            }

            if (chosenClip != null)
            {
                audioSource.clip = chosenClip;
                audioSource.Play();

                yield return new WaitForSeconds(chosenClip.length + Random.Range(1f, 5f)); 
            }
            else
            {
                yield return null; 
            }
        }
    }
}
