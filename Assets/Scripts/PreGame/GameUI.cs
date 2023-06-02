using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    static GameUI i;
    [SerializeField] GameObject textArea;
    [SerializeField] GameObject optionArea;
    [SerializeField] GameObject loadingScreen;

    void Awake()
    {
        i = this;
        gameObject.SetActive(false);
    }

    public static void ClearUI()
    {
        foreach (Transform child in i.transform)
        {
            Destroy(child.gameObject);
        }

    }

    public static void RenderAtPosition(GameObject go, Vector2 position)
    {
        go.transform.SetParent(i.transform);
        (go.transform as RectTransform).anchoredPosition = position * new Vector2(1, -1) + new Vector2(5, -5);
    }

    public static IEnumerator TypeOut(string text, bool instant = false)
    {
        string newText = GameManager.GetCleanedText(text);

        GameObject textAreaObject = Instantiate(i.textArea);
        yield return textAreaObject.GetComponent<TextBox>().TypeOut(newText, instant, Vector2.zero);
        Destroy(textAreaObject);
    }

    public static IEnumerator ChoiceMenu(string prompt, string[] options, int cols, bool allowCancel = false)
    {   
        Vector2 position;
        GameObject textAreaObject;
        if (prompt != null && prompt != "")
        {
            textAreaObject = Instantiate(i.textArea);
            yield return textAreaObject.GetComponent<TextBox>().TypeOut(prompt, false, Vector2.zero);

            RectTransform rt = textAreaObject.GetComponent<RectTransform>();
            if (rt.sizeDelta.y > 200)
                position = new Vector2(rt.sizeDelta.x - 11, 0);
            else
                position = new Vector2(0, rt.sizeDelta.y + 3);
        }
        else
        {
            textAreaObject = null;
            position = Vector2.zero;
        }

        string[] newOptions = options;
        for (int i = 0; i < options.Length; i++)
        {
            newOptions[i] = GameManager.GetCleanedText(options[i]);
        }

        GameObject optionsAreaObject = Instantiate(i.optionArea);
        yield return optionsAreaObject.GetComponent<OptionMenu>().Options(newOptions, position, cols, allowCancel);

        Destroy(optionsAreaObject);
        Destroy(textAreaObject);
    }

    public static IEnumerator GetYesNo(string prompt, string thirdOption = null, bool allowCancel = false)
    {
        string[] options;
        if (thirdOption != null && thirdOption.Length > 0)
            options = new string[] { "Yes", "No", GameManager.GetCleanedText(thirdOption) };
        else
            options = new string[] { "Yes", "No" };

        yield return ChoiceMenu(prompt, options, 1, allowCancel);
    }

    public static IEnumerator ToggleLoadingScreen(bool onOff, bool instant = false)
    {
        i.gameObject.SetActive(true);
        Image loadingScreenImage = i.loadingScreen.GetComponent<Image>();

        if (instant)
        {
            loadingScreenImage.color = new Color(0, 0, 0, onOff ? 1 : 0);
        }

        if (onOff)
        {
            while (loadingScreenImage.color.a < 1)
            {
                loadingScreenImage.color = new Color(0, 0, 0, Mathf.Clamp(loadingScreenImage.color.a + Time.unscaledDeltaTime, 0, 1));
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (loadingScreenImage.color.a > 0)
            {
                loadingScreenImage.color = new Color(0, 0, 0, Mathf.Clamp(loadingScreenImage.color.a - Time.unscaledDeltaTime, 0, 1));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
