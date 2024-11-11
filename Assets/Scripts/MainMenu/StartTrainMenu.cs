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
        animator = GetComponent<Animator>();
    }

    public void GoToGame()
    {
        panel.SetActive(false);
        animator.SetBool("Go", true);
        StartCoroutine(CooldownForAnimation());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator CooldownForAnimation()
    {
        yield return new WaitForSeconds(1.6f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); 
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("TestScene");
    }
}
