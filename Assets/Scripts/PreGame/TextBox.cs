using System.Collections;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshObject;
    public float delayBeforeTyping;

    public IEnumerator TypeOut(string text, bool instant)
    {
        if (instant)
        {
            textMeshObject.text = text;
        }
        else 
        {
            yield return TypeText(text.Trim());
        }
    }

    private IEnumerator TypeText(string text)
    {
        // Setup
        textMeshObject.text = "";
        yield return new WaitForSecondsRealtime(delayBeforeTyping);
        yield return new WaitWhile(() => MyInput.Select == 1);

        // Type out text
        foreach (char letter in text)
        {
            textMeshObject.text += letter;
            gameObject.SetActive(true);

            if (MyInput.Select == 1)
            {
                textMeshObject.text = text;
                yield return new WaitUntil(() => MyInput.Select == 0);
                break;
            }

            yield return new WaitForSecondsRealtime(0.05f);
        }

        // Wait for space click
        yield return MyInput.WaitForMenuNavigation();

        // Finish
        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
