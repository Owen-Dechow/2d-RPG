using System.Collections;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshObject;
    public float delayBeforeTyping;

    public IEnumerator TypeOut(string text, bool close)
    {
        yield return TypeText(text.Trim(), close);
    }

    private IEnumerator TypeText(string text, bool close)
    {
        // Setup
        textMeshObject.text = "";
        yield return new WaitForSecondsRealtime(delayBeforeTyping);
        yield return new WaitWhile(() => MyInput.GetSelect() == 1);

        // Type out text
        foreach (char letter in text)
        {
            textMeshObject.text += letter;
            gameObject.SetActive(true);

            if (MyInput.GetSelect() == 1)
            {
                textMeshObject.text = text;
                yield return new WaitUntil(() => MyInput.GetSelect() == 0);
                break;
            }

            yield return new WaitForSecondsRealtime(0.05f);
        }

        // Wait for space click
        yield return MyInput.WaitForClick();

        // Finish
        if (close) gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
