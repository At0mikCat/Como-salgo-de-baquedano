using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTrainMenu : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject panel;

    [SerializeField] private CanvasGroup fadeCanvasGroup; 
    [SerializeField] private CanvasGroup startButtonCanvas; 
    [SerializeField] private CanvasGroup exitButtonCanvas; 
    [SerializeField] private CanvasGroup logoCanvas; 
    [SerializeField] private float fadeDuration = 1.5f;

    [SerializeField] private bool hasPressedButton;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        hasPressedButton = false;

        animator = GetComponent<Animator>();
        StartCoroutine(Start());
    }

    public void GoToGame()
    {
        animator.SetBool("Go", true);
        hasPressedButton = true;
        StartCoroutine(CooldownForAnimation());
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        hasPressedButton = true;
        Application.Quit();
    }

    IEnumerator Start()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            logoCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            if (hasPressedButton)
            {
                startButtonCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                exitButtonCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            }

            yield return null;
        }
        
        fadeCanvasGroup.alpha = 0f;

    }

    IEnumerator CooldownForAnimation()
    {
        
        float elapsedTime1 = 0f;

        while (elapsedTime1 < fadeDuration)
        {
            if (hasPressedButton)
            {
                elapsedTime1 += Time.deltaTime;
                
                startButtonCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime1 / fadeDuration);
                exitButtonCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime1 / fadeDuration);

                yield return null;
            }
        }
        
        yield return new WaitForSeconds(2.4f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); 
            logoCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("Acto1");
    }
}
