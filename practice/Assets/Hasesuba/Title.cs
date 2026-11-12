
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public AudioClip decideSound;
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.5f;

    private AudioSource audioSource;
    private bool isTransitioning = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // ç≈èâÇÕìßñæ
        fadeGroup.alpha = 0f;
    }

    public void StartGame()
    {
        if (isTransitioning) return;

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        isTransitioning = true;

        if (decideSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(decideSound);
        }

        // çïÇ≠Ç»ÇÈÇ‹Ç≈ë“Ç¬
        yield return FadeToBlack();

        SceneManager.LoadScene("Main Scene");
    }

    private IEnumerator FadeToBlack()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;
    }

    public void QuitGame()
    {
        if (isTransitioning) return;

        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator QuitGameRoutine()
    {
        isTransitioning = true;

        if (decideSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(decideSound);
        }

        yield return FadeToBlack();

        Application.Quit();
    }
}
