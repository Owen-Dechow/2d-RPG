using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pointer;
        [SerializeField] private GameObject textAreaPrefab;

        [SerializeField] private float pointerOriginY;
        [SerializeField] private float pointerSpaceY;
        [SerializeField] private float pointerSpaceX;

        List<TMPro.TextMeshProUGUI> textboxes;

        void SetUp(string[] options, Vector2 position, int cols)
        {
            // Clear preexisting data
            if (textboxes != null)
            {
                while (textboxes.Count > 0)
                {
                    Destroy(textboxes[0].gameObject);
                    textboxes.RemoveAt(0);
                }
            }

            // Place in UI
            GameUI.RenderAtPosition(gameObject, position);

            // Create text boxes
            textboxes = new();
            for (int i = 0; i < cols; i++)
            {
                GameObject go = Instantiate(textAreaPrefab, transform.Find("Background"));
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
            yield return new WaitForEndOfFrame();
            PositionPointer(0, cols);

            int selected = 0;
            while (true)
            {
                yield return MyInput.WaitForMenuNavigation();

                MyInput.Action action = MyInput.MenuNavigation;
                if (action == MyInput.Action.Up)
                {
                    selected -= cols;
                    if (selected < 0)
                    {
                        selected += cols;
                        if (selected == 0) selected = options.Length - 1;
                        else selected = 0;
                    }
                }
                else if (action == MyInput.Action.Down)
                {
                    selected += cols;
                    if (selected >= options.Length)
                    {
                        selected -= cols;
                        if (selected == options.Length - 1) selected = 0;
                        else selected = options.Length - 1;
                    }
                }
                else if (action == MyInput.Action.Left)
                {
                    selected -= 1;
                    if (selected < 0) selected = options.Length - 1;
                }
                else if (action == MyInput.Action.Right)
                {
                    selected += 1;
                    if (selected >= options.Length) selected = 0;
                }
                else if (action == MyInput.Action.Select)
                {
                    GameManager.Answer = options[selected];
                    GameManager.AnswerIndex = selected;
                    break;
                }
                else if (action == MyInput.Action.Cancel)
                {
                    if (allowCancel)
                    {
                        GameManager.Answer = "{{CANCEL}}";
                        GameManager.AnswerIndex = -1;
                        break;
                    }
                }

                PositionPointer(selected, cols);
                yield return new WaitForEndOfFrame();
            }
        }

        void PositionPointer(int selected, int cols)
        {
            int row = selected / cols;
            int col = selected % cols;

            float x = textboxes[col].gameObject.transform.localPosition.x + pointerSpaceX;
            float y = row * pointerSpaceY + pointerOriginY;

            (pointer.transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }
    }
}