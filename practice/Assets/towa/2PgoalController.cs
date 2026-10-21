using UnityEngine;
using TMPro;

public class goalContoroller : MonoBehaviour
{
    [SerializeField] private GameObject goalText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("1PÇÉSÅ[ÉãÇ…êÅÇ¡îÚÇŒÇµÇΩ");

            if (goalText != null)
            {
                goalText.SetActive(true);
            }
        }
    }
}
