using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTrainMenu : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject panel;

    [SerializeField] private CanvasGroup fadeCanvasGroup; 
    [SerializeField] private float fadeDuration = 1.5f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        animator = GetComponent<Animator>();
        StartCoroutine(Start());
    }

    public void GoToGame()
    {
        panel.SetActive(false);
        animator.SetBool("Go", true);
        StartCoroutine(CooldownForAnimation());
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator Start()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;

    }

    IEnumerator CooldownForAnimation()
    {
        yield return new WaitForSeconds(2.4f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); 
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("Acto1");
    }
}
