using UnityEngine;
using TMPro;

public class SimpleDialoguePopup : MonoBehaviour
{
    [TextArea]
    public string dialogueLine = "I remember... the garden. Someone was with me.";

    public TMP_Text dialogueText;
    public KeyCode closeKey = KeyCode.Space;

    void Start()
    {
        if (dialogueText != null)
            dialogueText.text = dialogueLine;

        // Optional: pause game while dialogue is showing
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(closeKey))
        {
            // Resume game and hide panel
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}
