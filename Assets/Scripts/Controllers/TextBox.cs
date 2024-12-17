using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class TextBox : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI textMeshObject;
        public float delayBeforeTyping;

        public IEnumerator TypeOut(string text, bool instant, Vector2 position)
        {
            GameUI.RenderAtPosition(gameObject, position);

            // Set Size
            GameObject model = Instantiate(gameObject);
            model.GetComponent<TextBox>().textMeshObject.text = text;
            yield return new WaitForEndOfFrame();
            Vector2 size = (model.transform as RectTransform).sizeDelta;
            Destroy(model);
            GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            (transform as RectTransform).sizeDelta = size;


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
            yield return MyInput.WaitForClickSelect();
        }
    }
}
