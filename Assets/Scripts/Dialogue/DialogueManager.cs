using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;


[Serializable]
public class KeyValuePair 
{
    public int key;
    public string val;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueBox;
    public GameObject nextDialogueBox;

    public GameObject characterNameBox;
    
    public TextMeshProUGUI characterTextBox;
    public TextMeshProUGUI dialogueTextBox;

    
    public float dialogueDelay;

    private Queue<DialogueLine> lines;

    private bool isDialogueActive;
    public bool isTyping;

    public float typingSpeed;
    public float OGtypingSpeed;

    public bool isDialougeBoxActive;
    public bool waitForAudio;

    public List<GameObject> allDialogues = new List<GameObject>();

    public bool isBeingDelayed;

    public List<GameObject> characters = new List<GameObject>();

    public GameObject relatedCinematic;


    public List<KeyValuePair> characterIndexList = new List<KeyValuePair>();
    Dictionary<int, string> indexDictionary = new Dictionary<int, string>();

    public AudioClip voiceEstudiante;
    public AudioClip voiceMamá;
    public AudioClip voiceGuardia1;
    public AudioClip voiceGuardia2;
    public AudioSource SFX;

    public int sceneNumber;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    void Start()
    {
        typingSpeed = PlayerPrefs.current.textSpeed;
        OGtypingSpeed = typingSpeed;
        
        allDialogues.Add(GameObject.FindGameObjectWithTag("Dialogue"));
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(0);
        }

        if (isBeingDelayed)
        {
            dialogueDelay -= Time.deltaTime;
            EndDialogue();
        }
        if(dialogueDelay < 0)
        {
            isBeingDelayed = false;
        }

        if (lines.Count > 0)
        {
            dialogueBox.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    typingSpeed = 0f;
                }
                else
                {
                    DisplayNextDialogueLine();
                    typingSpeed = OGtypingSpeed;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping == false)
                {
                    dialogueBox.SetActive(false);
                    typingSpeed = OGtypingSpeed;
                    EndDialogue();

                }
                else
                {
                    typingSpeed = 0;
                }
            }

        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (var kvp in characterIndexList)
        {
            indexDictionary[kvp.key] = kvp.val;
        }

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void RelatedDialogue(GameObject dialogue)
    {
        relatedCinematic = dialogue;
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        StopAllCoroutines();
        CharacterName(currentLine);
        StartCoroutine(TypeSentence(currentLine));
    }

    public void CharacterName(DialogueLine dialogueName)
    {
        characterTextBox.text = indexDictionary[dialogueName.speakCharIndex];
        if (indexDictionary[dialogueName.speakCharIndex] == " ")
        {
            characterNameBox.SetActive(false);
        }
        else
        {
            characterNameBox.SetActive(true);
        }
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueTextBox.text = "";
        isTyping = true;

        if (typingSpeed != 0f)
        {
            foreach (char letter in dialogueLine.line.ToCharArray())
            {
                if (typingSpeed != 0f && isTyping)
                {
                    dialogueTextBox.text += letter;
                    yield return new WaitForSeconds(typingSpeed);
                    
                    if (characterTextBox.text == "Estudiante")
                    {
                        if (waitForAudio == false)
                        {
                            SFX.pitch = Random.Range(0.90f, 1.10f);
                            SFX.PlayOneShot(voiceEstudiante);
                            waitForAudio = true;
                        }
                        else
                        {
                            waitForAudio = false;
                        }
                    } 
                    if (characterTextBox.text == "Mamá")
                    {
                        if (waitForAudio == false)
                        {
                            SFX.pitch = Random.Range(0.90f, 1.10f);
                            SFX.PlayOneShot(voiceMamá);
                            waitForAudio = true;
                        }
                        else
                        {
                            waitForAudio = false;
                        }
                    }       
                    if (characterTextBox.text == "Guardia 1")
                    {
                        if (waitForAudio == false)
                        {
                            SFX.pitch = Random.Range(0.90f, 1.10f);
                            SFX.PlayOneShot(voiceGuardia1);
                            waitForAudio = true;
                        }
                        else
                        {
                            waitForAudio = false;
                        }
                    }
                    if (characterTextBox.text == "Guardia 2")
                    {
                        if (waitForAudio == false)
                        {
                            SFX.pitch = Random.Range(0.90f, 1.10f);
                            SFX.PlayOneShot(voiceGuardia2);
                            waitForAudio = true;
                        }
                        else
                        {
                            waitForAudio = false;
                        }
                    }

                }
                else
                {
                    dialogueTextBox.text = dialogueLine.line;
                    isTyping = false;
                }
            }
            isTyping = false;
        }
        else
        {
            dialogueTextBox.text = dialogueLine.line;
            isTyping = false;
        }
        typingSpeed = OGtypingSpeed;
    }
    public void EndDialogue()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(false);
        }
        
        isBeingDelayed = true;
        
        if (nextDialogueBox)
        {
            PlayerPrefs.current.lastDialogueBeforeLeaving = nextDialogueBox;
            if (dialogueDelay > 0)
            {
            }
            else
            {
                nextDialogueBox.SetActive(true);
            }
            relatedCinematic.SetActive(false);
        }
        else
        {
            dialogueBox.SetActive(false);
            characterNameBox.SetActive(false);
            StartCoroutine(CTM());
        }

        
    }

    IEnumerator CTM()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneNumber);
    }
}