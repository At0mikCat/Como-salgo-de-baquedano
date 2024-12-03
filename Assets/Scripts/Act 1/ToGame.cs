using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToGame : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(gotogame());
    }

    IEnumerator gotogame()
    {
        yield return new WaitForSeconds(90f);
        SceneManager.LoadScene("Acto2");
    }
}
