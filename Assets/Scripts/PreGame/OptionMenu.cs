using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] GameObject pointer;
    [SerializeField] GameObject textareaPrefab;

    [SerializeField] float pointerOriginY;
    [SerializeField] float pointerOriginX;
    [SerializeField] float pointerSpaceY;

    List<TMPro.TextMeshProUGUI> textboxes;

    int cols;
    int rows;

    void SetUp(string[] options, Vector2 position, int cols)
    {
        // Set number of cols/rows
        this.cols = cols;
        rows = options.Length / cols;

        // Place in UI
        GameUI.RenderAtPosition(gameObject, position);

        // Create text boxes
        textboxes = new();
        for (int i = 0; i < cols; i++)
        {
            GameObject go = Instantiate(textareaPrefab, transform.Find("Background"));
            textboxes.Add(go.GetComponent<TMPro.TextMeshProUGUI>());
        }

        // Add text to text boxes
        for (int i = 0; i < options.Length; i++)
        {
            int boxNumber = i % cols;
            textboxes[boxNumber].text += options[i] + "\n";
        }
    }

    public IEnumerator Options(string[] options, Vector2 position, int cols, bool allowCancel)
    {
        SetUp(options, position, cols);

        int row = 0;
        int col = 0;
        while (true)
        {
            yield return MyInput.WaitForMenuNavigation();

            switch (MyInput.MenuNavigation)
            {
                case MyInput.Action.Up:
                    col -= 1;
                    if (col <= -1) col = cols;
                    break;
                case MyInput.Action.Down:
                    break;
                case MyInput.Action.Left:
                    break;
                case MyInput.Action.Right:
                    break;
                case MyInput.Action.Select:
                    break;
                case MyInput.Action.Cancel:
                    break;
                case MyInput.Action.None:
                    break;
            }
        }

        yield return null;
    }
}