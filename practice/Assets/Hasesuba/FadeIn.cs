
using System.Collections;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        fadeGroup.alpha = 1f;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            fadeGroup.alpha = 1f - Mathf.Clamp01(time / fadeDuration);

            yield return null;
        }

        fadeGroup.alpha = 0f;
    }
}
