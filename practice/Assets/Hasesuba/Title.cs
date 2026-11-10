
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public AudioClip decideSound;
    private AudioSource audioSource;

    private bool isTransitioning = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(StartGameRoutine());
        }
    }

    private IEnumerator StartGameRoutine()
    {
        isTransitioning = true;

        if (decideSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(decideSound, 8f);
            yield return new WaitForSecondsRealtime(0.2f);
        }

        SceneManager.LoadScene("Main Scene");
    }

    public void QuitGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(QuitGameRoutine());
        }
    }

    private IEnumerator QuitGameRoutine()
    {
        isTransitioning = true;

        if (decideSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(decideSound, 8f);
            yield return new WaitForSecondsRealtime(0.2f);
        }

        Application.Quit();
    }
}
