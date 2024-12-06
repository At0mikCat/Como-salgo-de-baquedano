using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
[System.Serializable]
public enum SpriteState{
    Neutral,
    Happy,
    Sad,
    Angry1,
    Angry2,
    Angry3,
}

public enum SpritePosition
{
    Left,
    Right,
    Center
}

public enum SpriteLookAt
{
    Left,
    Right
}
public enum SpriteAnimation
{
    None,
    Jump,
    Sway
}

[System.Serializable]
public class Character1
{
    public bool isSpeaking;
    public bool isInUse;
    public GameObject spriteCointainer;
    public SpriteState state;
    public SpritePosition position;
    public SpriteLookAt lookAt;
    public bool isAnimated;
    public SpriteAnimation animation;
}
[System.Serializable]
public class Character2
{
    public bool isSpeaking;
    public bool isInUse;
    public GameObject spriteCointainer;
    public SpriteState state;
    public SpritePosition position;
    public SpriteLookAt lookAt;
    public bool isAnimated;
    public SpriteAnimation animation;
}
[System.Serializable]
public class Character3
{
    public bool isSpeaking;
    public bool isInUse;
    public GameObject spriteCointainer;
    public SpriteState state;
    public SpritePosition position;
    public SpriteLookAt lookAt;
    public bool isAnimated;
    public SpriteAnimation animation;
}

[System.Serializable]
public class DialogueLine
{
    public int speakCharIndex;
    public Color speakCharColor;
    [TextArea(3, 10)] 
    public string line;

    public Character1 Char1Data;
    public Character2 Char2Data;
    public Character3 Char3Data;
}
 
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public bool hasSentDialogue;
    public Dialogue dialogue;
    public GameObject nextDialogue;
    public GameObject player;
    public Sprite background;
    public GameObject relatedDialogue;

    public float dialogueDelay;
    public bool startTimer;
    public bool sentDialogue;

    private void Start()
    {
        startTimer = false;
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.isDialougeBoxActive = true;
        DialogueManager.Instance.StartDialogue(dialogue);
        DialogueManager.Instance.RelatedDialogue(relatedDialogue);
        //DialogueManager.Instance.background.sprite = background;
    }

    private void Update()
    {
        if (sentDialogue == false)
        {
            DialogueManager.Instance.dialogueDelay = dialogueDelay;
            TriggerDialogue();
            DialogueManager.Instance.nextDialogueBox = nextDialogue;
            sentDialogue = true;
        }
    }
}
    