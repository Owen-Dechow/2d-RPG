using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] GameObject pointer;
    [SerializeField] TMPro.TextMeshProUGUI[] textMeshProUGUIs;

    readonly float orginY = -28;
    readonly float spaceY = 17.6f;

    public enum KeyCommand
    {
        Up,
        Right,
        Down,
        Left,
        Select,
        Cancel,
        None,
    }

    public IEnumerator AskChoice(string prompt, string[] options, bool alowCancel)
    {
        yield return SelectChoice(prompt, options, alowCancel);
    }

    int GetNumberOfColumns(int numberOfOptions)
    {
        if (numberOfOptions < 4) return 1;
        if (numberOfOptions < 6) return 2;
        return 3;
    }

    void PositionPointer(int selected, int numberOfColumns)
    {
        int row = selected / numberOfColumns;
        int col = selected % numberOfColumns;

        float x = textMeshProUGUIs[col].rectTransform.offsetMin.x;
        float y = orginY - (row * spaceY);

        Vector3 newPos = new(x, y, 0);
        ((RectTransform)pointer.transform).anchoredPosition = newPos;
    }

    static KeyCommand GetInput()
    {
        int Yaxis = (int)MyInput.GetMoveVertical();
        int Xaxis = (int)MyInput.GetMoveHorizontal();
        int selected = MyInput.GetSelect();

        KeyCommand cammand;

        if (selected == -1) cammand = KeyCommand.Cancel;
        else if (selected == 1) cammand = KeyCommand.Select;
        else if (Yaxis == 1) cammand = KeyCommand.Up;
        else if (Yaxis == -1) cammand = KeyCommand.Down;
        else if (Xaxis == 1) cammand = KeyCommand.Right;
        else if (Xaxis == -1) cammand = KeyCommand.Left;
        else cammand = KeyCommand.None;

        return cammand;
    }

    IEnumerator Setup(string prompt, string[] options, int numberOfColumns)
    {
        Time.timeScale = 0;

        if (prompt != "" && prompt != null)
        {
            yield return GameManager.TypeOut(prompt, close: false);
        }

        foreach (TMPro.TextMeshProUGUI textMeshProUGUI in textMeshProUGUIs)
        {
            textMeshProUGUI.text = "";
        }

        for (int i = 0; i < options.Length; i++)
        {
            textMeshProUGUIs[i % numberOfColumns].text += options[i] + "\n";
        }

        for (int i = 0; i < textMeshProUGUIs.Length; i++)
        {
            textMeshProUGUIs[i].gameObject.SetActive(i < numberOfColumns);
        }

        gameObject.SetActive(true);
    }

    IEnumerator SelectChoice(string prompt, string[] options, bool allowCancel)
    {
        int selected = 0;
        int numberOfColumns = GetNumberOfColumns(options.Length);
        PositionPointer(selected, numberOfColumns);

        yield return Setup(prompt, options, numberOfColumns);

        // Get choice
        while (true)
        {
            // Wait for click
            yield return new WaitUntil(() => GetInput() == KeyCommand.None);
            yield return new WaitUntil(() => GetInput() != KeyCommand.None);

            KeyCommand click = GetInput();

            // Handel action
            if (click == KeyCommand.Down)
            {
                selected += numberOfColumns;
                if (selected >= options.Length)
                {
                    selected -= numberOfColumns;
                    if (selected == options.Length - 1) selected = 0;
                    else selected = options.Length - 1;
                }
            }
            else if (click == KeyCommand.Up)
            {
                selected -= numberOfColumns;
                if (selected < 0)
                {
                    selected += numberOfColumns;
                    if (selected == 0) selected = options.Length - 1;
                    else selected = 0;
                }
            }
            else if (click == KeyCommand.Right)
            {
                selected += 1;
                if (selected >= options.Length) selected = 0;
            }
            else if (click == KeyCommand.Left)
            {
                selected -= 1;
                if (selected < 0) selected = options.Length - 1;
            }
            else if (click == KeyCommand.Select)
            {
                break;
            }
            else if (click == KeyCommand.Cancel)
            {
                if (!allowCancel) continue;

                yield return new WaitUntil(() => GetInput() == KeyCommand.None);
                GameManager.Answer = "CANCEL";
                GameManager.AnswerIndex = -1;
                gameObject.SetActive(false);
                GameManager.CloseUI();
                yield break;
            }

            PositionPointer(selected, numberOfColumns);
        }

        // Finish
        yield return new WaitUntil(() => GetInput() == KeyCommand.None);
        GameManager.Answer = options[selected];
        GameManager.AnswerIndex = selected + 1;
        gameObject.SetActive(false);
        GameManager.CloseUI();

    }
}