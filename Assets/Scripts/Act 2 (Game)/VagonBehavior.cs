using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VagonBehavior : MonoBehaviour
{
    private Animator animator;
    public AudioSource byebyeTrain;
    public AudioSource trainSound;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(RetireVagon());
    }

    IEnumerator RetireVagon()
    {
        float lifeTime = Random.Range(15, 20);
        yield return new WaitForSeconds(lifeTime);
        trainSound.Play();
        byebyeTrain.Play();
        yield return new WaitForSeconds(3f);
        animator.SetBool("escaping", true);
    }
}
