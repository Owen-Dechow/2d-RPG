using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class GameUI : MonoBehaviour
    {
        private static GameUI _i;
        [SerializeField] GameObject textArea;
        [SerializeField] GameObject optionArea;
        [SerializeField] GameObject loadingScreen;

        void Awake()
        {
            _i = this;
            gameObject.SetActive(false);
        }

        public static void RenderAtPosition(GameObject go, Vector2 position)
        {
            float borderPad = 20;
            float normalPad = 5;
            Vector2 padding = new(1, -1);
            if (position.x == 0)
                padding.x *= borderPad;
            else
                padding.x *= normalPad;
            if (position.y == 0)
                padding.y *= borderPad;
            else
                padding.y *= normalPad;

            go.transform.SetParent(_i.transform, false);
            ((RectTransform)go.transform).anchoredPosition = position * new Vector2(1, -1) + padding;
        }

        public static IEnumerator TypeOut(string text, bool instant = false)
        {
            string newText = GameManager.GetCleanedText(text);

            GameObject textAreaObject = Instantiate(_i.textArea);
            yield return textAreaObject.GetComponent<TextBox>().TypeOut(newText, instant, Vector2.zero);
            Destroy(textAreaObject);
        }

        public static IEnumerator ChoiceMenu(string prompt, string[] options, int cols, bool allowCancel = false)
        {
            Vector2 position;
            GameObject textAreaObject;
            if (prompt != null && prompt != "")
            {
                textAreaObject = Instantiate(_i.textArea);
                yield return textAreaObject.GetComponent<TextBox>().TypeOut(prompt, false, Vector2.zero);

                RectTransform rt = textAreaObject.GetComponent<RectTransform>();
                if (rt.sizeDelta.y > 200)
                    position = new Vector2(rt.anchorMax.x + rt.offsetMax.x, 0);
                else
                    position = new Vector2(0, rt.anchorMin.y + rt.offsetMin.y * -1);
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

            GameObject optionsAreaObject = Instantiate(_i.optionArea);
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

        public static IEnumerator ToggleLoadingScreen(bool onOff, bool controlVol = true, bool instant = false)
        {
            _i.gameObject.SetActive(true);
            Image loadingScreenImage = _i.loadingScreen.GetComponent<Image>();

            if (instant)
            {
                loadingScreenImage.color = new Color(0, 0, 0, onOff ? 1 : 0);

                if (controlVol)
                    AudioListener.volume = onOff ? 0 : 1;

                yield break;
            }

            if (onOff)
            {
                while (loadingScreenImage.color.a < 1)
                {
                    loadingScreenImage.color = new Color(0, 0, 0, Mathf.Clamp(loadingScreenImage.color.a + Time.deltaTime, 0, 1));

                    if (controlVol)
                        AudioListener.volume = 1 - loadingScreenImage.color.a;

                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (loadingScreenImage.color.a > 0)
                {
                    loadingScreenImage.color = new Color(0, 0, 0, Mathf.Clamp(loadingScreenImage.color.a - Time.deltaTime, 0, 1));

                    if (controlVol)
                        AudioListener.volume = Mathf.Abs(loadingScreenImage.color.a - 1);

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        static void AddOptionPathToMenu(FullMenuOption fullMenu, string optionPath)
        {
            FullMenuOption depth = fullMenu;
            string[] optionPathArray = optionPath.Split('\\');
            for (int i = 0; i < optionPathArray.Length - 1; i++)
            {
                string pathSection = optionPathArray[i];

                FullMenuOption selectedPath = null;
                foreach (FullMenuOption possibleSelection in depth.Options)
                {
                    if (possibleSelection.DisplayText == pathSection)
                    {
                        selectedPath = possibleSelection;
                        break;
                    }
                }

                if (selectedPath == null)
                {
                    selectedPath = new FullMenuOption(pathSection);
                    depth.Options.Add(selectedPath);
                }

                depth = selectedPath;
            }
            depth.Options.Add(new(optionPathArray[^1]));
        }

        public static IEnumerator FullMenu(string[] options, bool allowCancel)
        {

            FullMenuOption fullMenu = new("");

            foreach (string option in options)
            {
                AddOptionPathToMenu(fullMenu, option);
            }

            yield return fullMenu.RunMenu(0, allowCancel);

            if (GameManager.AnswerIndex != -1)
            {
                GameManager.Answer = GameManager.Answer.Trim('\\');
                GameManager.AnswerIndex = System.Array.IndexOf(options, GameManager.Answer);
            }

            fullMenu.DestroyMenu();
            yield return new WaitForEndOfFrame();
        }

        class FullMenuOption
        {
            public readonly string DisplayText;
            public readonly List<FullMenuOption> Options;
            readonly GameObject choicePanel;
            private bool IsWrapper => Options.Count > 0;

            public FullMenuOption(string displayText)
            {
                this.DisplayText = displayText;
                Options = new List<FullMenuOption>();
                choicePanel = Instantiate(_i.optionArea);
                choicePanel.SetActive(false);
            }

            public IEnumerator RunMenu(float choicePanelOffset, bool allowCancel)
            {
                choicePanel.SetActive(true);

                string[] stringOptions = Options.Select(x => x.DisplayText).ToArray();
                yield return choicePanel.GetComponent<OptionMenu>().Options(stringOptions, new Vector2(choicePanelOffset, 0), 1, allowCancel);

                if (GameManager.AnswerIndex != -1)
                {
                    FullMenuOption selectedOption = Options[GameManager.AnswerIndex];
                    if (selectedOption.IsWrapper)
                    {
                        RectTransform rt = choicePanel.transform as RectTransform;
                        float xOffset = rt.anchorMax.x + rt.offsetMax.x;
                        yield return selectedOption.RunMenu(xOffset, true);

                        if (GameManager.AnswerIndex != -1)
                        {
                            GameManager.Answer = DisplayText + '\\' + GameManager.Answer;
                        }
                        else
                        {
                            yield return RunMenu(choicePanelOffset, allowCancel);
                        }
                    }
                    else
                    {
                        GameManager.Answer = DisplayText + '\\' + GameManager.Answer;
                    }
                }

                choicePanel.SetActive(false);
            }

            public void DestroyMenu()
            {
                foreach (FullMenuOption option in Options)
                {
                    option.DestroyMenu();
                }
                Destroy(choicePanel);
            }
        }
    }
}
