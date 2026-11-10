
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    public Button[] buttons;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;

    public AudioClip selectSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateButtonColors();
    }

    void Update()
    {
        if (Gamepad.current == null) return;

        if (Gamepad.current.dpad.up.wasPressedThisFrame)
        {
            selectedIndex--;

            if (selectedIndex < 0)
                selectedIndex = buttons.Length - 1;

            audioSource.PlayOneShot(selectSound);

            UpdateButtonColors();
        }
        else if (Gamepad.current.dpad.down.wasPressedThisFrame)
        {
            selectedIndex++;

            if (selectedIndex >= buttons.Length)
                selectedIndex = 0;

            audioSource.PlayOneShot(selectSound);

            UpdateButtonColors();
        }

        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colors = buttons[i].colors;
            colors.normalColor =
                (i == selectedIndex) ? selectedColor : normalColor;
            buttons[i].colors = colors;
        }
    }
}
