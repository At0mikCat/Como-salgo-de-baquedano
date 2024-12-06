using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeEndEnd : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private CanvasGroup text1;
    [SerializeField] private CanvasGroup text2;
    [SerializeField] private CanvasGroup image;
    [SerializeField] private CanvasGroup text3;
    [SerializeField] private CanvasGroup ToBlack;
    void Start()
    {
        StartCoroutine(endend());
    }

    IEnumerator endend()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text1.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        text1.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text2.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        text2.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            image.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        image.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text3.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        text3.alpha = 1f;
        yield return new WaitForSeconds(1.5f);
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            ToBlack.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        ToBlack.alpha = 1f;
        SceneManager.LoadScene("Credits");
    }
}
