using System.Collections;
using System.Collections.Generic;
using Data;
using Managers;
using Managers.CutScene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controllers
{
    public class LoadGame : MonoBehaviour
    {
        [SerializeField] GameObject gameManager;
        [SerializeField] GameObject buttonPrefab;
        [SerializeField] RectTransform buttonsContainer;
        [FormerlySerializedAs("newGameBTN")] [SerializeField] GameObject newGameBtn;
        [SerializeField] byte maxNumberOfGameProfiles;
        [SerializeField] EventSystem eventSystem;
        [SerializeField] GameObject getNamePanel;
        [SerializeField] GameObject confirmDeletePanel;

        private int idToDelete;
        private GameObject gameObjectToDelete;

        public void StartNewGame()
        {
            getNamePanel.SetActive(true);
            eventSystem.SetSelectedGameObject(getNamePanel.transform.Find("background").Find("input").gameObject);
            buttonsContainer.gameObject.SetActive(false);
        }

        public void Back_GetNamePanel()
        {
            getNamePanel.SetActive(false);
            eventSystem.SetSelectedGameObject(newGameBtn);
            buttonsContainer.gameObject.SetActive(true);
        }

        public void Start_GetNamePanel()
        {
            TMPro.TMP_InputField nameTextBox = getNamePanel.transform.Find("background").Find("input").GetComponent<TMPro.TMP_InputField>();

            if (nameTextBox.text == "") return;

            string fixedName = nameTextBox.text.ToUpper().Split(" ")[0];
            fixedName = fixedName[0] + fixedName[1..].ToLower();


            if (nameTextBox.text != fixedName)
            {
                nameTextBox.text = fixedName;
                return;
            }

            PlayerManager.SetName(fixedName);
 
            getNamePanel.SetActive(false);
            IEnumerator startGame()
            {
                DontDestroyOnLoad(gameObject);
                yield return GameUIManager.ToggleLoadingScreen(true);

                gameObject.GetComponent<Canvas>().enabled = false;
                GameManager.LoadLevel(LevelScene.ShipTop, Vector2.zero, AnimPlus.Direction.Down);

                yield return GameUIManager.ToggleLoadingScreen(false);
                SaveSystem.SaveGame();
                Destroy(gameObject);
            }
            StartCoroutine(startGame());
        }

        public void Yes_ConfirmDelete()
        {
            SaveSystem.RemoveSaveProfile(idToDelete);
            Destroy(gameObjectToDelete);
            int gamesCount = SaveSystem.GetSaveProfiles().Count;
            confirmDeletePanel.SetActive(false);
            buttonsContainer.gameObject.SetActive(true);
            ToggleNewGameBtn(gamesCount);
        }

        public void No_ConfirmDelete()
        {
            confirmDeletePanel.SetActive(false);
            buttonsContainer.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(newGameBtn);
        }

        public void JumpToFocus(GameObject focus)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(focus);
        }

        public void Start()
        {
            Dictionary<int, string> games = SaveSystem.GetSaveProfiles();

            foreach (KeyValuePair<int, string> game in games)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, buttonsContainer);
                GameObject load = buttonObj.transform.Find("load").gameObject;
                Button buttonLoad = load.GetComponent<Button>();
                Button buttonDelete = buttonObj.transform.Find("delete").GetComponent<Button>();

                buttonLoad.onClick.AddListener(() =>
                {
                    LoadFromSavePoint(game.Key);
                });
                buttonDelete.onClick.AddListener(() =>
                {
                    idToDelete = game.Key;
                    gameObjectToDelete = buttonObj;
                    confirmDeletePanel.SetActive(true);
                    buttonsContainer.gameObject.SetActive(false);
                    eventSystem.SetSelectedGameObject(confirmDeletePanel.transform.Find("background")
                        .Find("buttons").Find("Cancel").gameObject);
                });

                buttonLoad.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = game.Value;
            }

            ToggleNewGameBtn(games.Count);
        }

        void ToggleNewGameBtn(int numberOfGames)
        {
            if (numberOfGames < maxNumberOfGameProfiles)
            {
                newGameBtn.SetActive(true);
                eventSystem.SetSelectedGameObject(newGameBtn);
            }
            else
            {
                newGameBtn.SetActive(false);
                eventSystem.SetSelectedGameObject(buttonsContainer.GetChild(1).GetChild(0).gameObject);
            }
        }

        void LoadFromSavePoint(int key)
        {
            buttonsContainer.gameObject.SetActive(false);
            StartCoroutine(StartGame());
            return;

            IEnumerator StartGame()
            {
                using (new CutScene.Window(true))
                {
                    DontDestroyOnLoad(gameObject);
                    yield return GameUIManager.ToggleLoadingScreen(true);

                    gameObject.GetComponent<Canvas>().enabled = false;
                    GameManager.LoadFromSavePoint(key);

                    yield return GameUIManager.ToggleLoadingScreen(false);
                    Destroy(gameObject);
                }
            }
        }
    }
}
