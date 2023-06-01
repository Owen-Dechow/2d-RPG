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
        yield return textAreaObject.GetComponent<TextBox>().TypeOut(newText, instant);
    }

    public static IEnumerator ChoiceMenu(string prompt, string[] options, int cols, bool allowCancel = false)
    {
        string[] newOptions = options;
        for (int i = 0; i < options.Length; i++)
        {
            newOptions[i] = GameManager.GetCleanedText(options[i]);
        }

        GameObject optionsAreaObject = Instantiate(i.optionArea);
        yield return optionsAreaObject.GetComponent<OptionMenu>().Options(newOptions, Vector2.zero, cols, allowCancel);
    }

    public static IEnumerator GetYesNo(string prompt, string thirdOption = null, bool allowCancel = false)
    {
        string[] options;
        if (thirdOption != null && thirdOption.Length > 0)
            options = new string[] { "Yes", "No", GameManager.GetCleanedText(thirdOption) };
        else
            options = new string[] { "Yes", "No" };

        GameObject optionsAreaObject = Instantiate(i.optionArea);
        yield return optionsAreaObject.GetComponent<OptionMenu>().Options(options, Vector2.zero, 2, allowCancel);
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
