using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using Managers;
using Managers.CutScene;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using Transform = UnityEngine.Transform;

namespace Battle
{
    public class BattleSystem : MonoBehaviour
    {
        #region Fields/Properties

        [Header("UI Settings")]

        #region UI Settings

        [SerializeField]
        private float playerSpread;

        [SerializeField] private float panelSpread;
        [SerializeField] private Transform playerBattleStation;
        [SerializeField] private Transform enemyBattleStation;
        [SerializeField] private Transform playerBadges;
        [SerializeField] private Transform enemyBadges;
        [SerializeField] private Transform damageNumberCanvas;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private GameObject panelPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject pointer;
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject badgePrefab;

        #endregion

        [Header("Action Messages")] [SerializeField]
        private string[] attackText;

        [SerializeField] private string[] defendText;
        [SerializeField] private string[] runText;

        [Header("Audio")] [SerializeField] private AudioClip damageSound;
        private AudioSource audioSource;

        [HideInInspector] public GameObject enemyGameObject;
        [HideInInspector] public BattleUnit[] players;
        [HideInInspector] public BattleUnit[] enemies;

        private Panel[] playerPanels;
        private BattleUnit selectedUnit;
        private bool blinkUnit;

        private BattleState battleState;
        private BattleFinish battleFinish;

        private Max100 attackPower;

        private string PlayerTitle =>
            PlayerManager.SyncTextToSex(
                PlayerManager.Name +
                (players.Length == 1 ? "" : " and his companion" + (players.Length == 2 ? "" : "s")));

        private string EnemyTitle =>
            enemies[0].SyncSex(
                $@"{enemies[0].data.title}{
                    enemies.Length switch
                    {
                        1 => "",
                        2 => " and his sibling",
                        _ => " and his mob"
                    }
                }");

        #endregion

        private enum BattleState
        {
            PlayerTurn,
            EnemyTurn,
            Exit,
        }

        private enum BattleFinish
        {
            PlayerWin,
            EnemyWin,
            Run,
            None,
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            
            // Instantiate player units
            playerPanels = new Panel[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                // Player sprite
                players[i].stationGo = new GameObject($"PlayerBattleUnit_{i}", typeof(SpriteRenderer))
                {
                    transform =
                    {
                        parent = playerBattleStation
                    }
                };

                players[i].spriteRenderer = players[i].stationGo.GetComponent<SpriteRenderer>();
                players[i].spriteRenderer.sprite = players[i].sprite;
                players[i].spriteRenderer.sortingLayerName = "Battle";

                var playerMove = (i * playerSpread) - (players.Length * playerSpread / 2);
                playerMove += playerSpread / 2;
                players[i].stationGo.transform.localPosition = Vector3.zero;
                players[i].stationGo.transform.Translate(playerMove, 0, 0);


                // Player panel
                var panelMove = (i * panelSpread) - (players.Length * panelSpread / 2);
                panelMove += panelSpread / 2;

                playerPanels[i] = Instantiate(panelPrefab, canvas.transform).GetComponent<Panel>();
                playerPanels[i].Unit = players[i];
                playerPanels[i].transform.Translate(panelMove, 0, 0);
            }

            // Instantiate enemy units
            Dictionary<string, int> enemyNumbers = new();
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].stationGo = new GameObject($"EnemyBattleUnit_{i}", typeof(SpriteRenderer))
                {
                    transform =
                    {
                        parent = enemyBattleStation
                    }
                };

                enemies[i].spriteRenderer = enemies[i].stationGo.GetComponent<SpriteRenderer>();
                enemies[i].spriteRenderer.sprite = enemies[i].sprite;
                enemies[i].spriteRenderer.sortingLayerName = "Battle";

                var enemyMove = (i * playerSpread) - (enemies.Length * playerSpread / 2);
                enemyMove += playerSpread / 2;
                enemies[i].stationGo.transform.localPosition = Vector3.zero;
                enemies[i].stationGo.transform.Translate(enemyMove, 0, 0);

                enemies[i] = enemies[i].CopyAtStationGo();
                if (!enemyNumbers.TryAdd(enemies[i].data.title, 1))
                {
                    enemyNumbers[enemies[i].data.title]++;
                    enemies[i].data.title += " " + enemyNumbers[enemies[i].data.title];
                }
            }

            // Set Battle Position
            GameObject camera = Camera.main!.gameObject;
            transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0);

            DisplayUnitsOnPanel();
            StartCoroutine(Battle());
        }

        private void Update()
        {
            foreach (Panel panel in playerPanels)
            {
                panel.DisplayUnitGradual(Time.deltaTime * 4);
            }

            if (blinkUnit)
            {
                if (selectedUnit != null)
                {
                    selectedUnit.spriteRenderer.color = Mathf.Floor(Time.time * 10) % 2 == 0
                        ? Color.white
                        : new Color(0.3f, 0.3f, 0.3f, 0.3f);
                }
            }
        }

        private IEnumerator EnterBattleAnimation()
        {
            SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.clear;

            while (spriteRenderer.color.a < 0.9f)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.black, Time.deltaTime * 2);
                yield return new WaitForEndOfFrame();
            }

            spriteRenderer.color = Color.black;

            ShowObjects();
        }

        private void ShowObjects()
        {
            playerBattleStation.gameObject.SetActive(true);
            enemyBattleStation.gameObject.SetActive(true);

            foreach (Panel p in playerPanels)
            {
                p.gameObject.SetActive(true);
            }
        }

        private IEnumerator Battle()
        {
            using (new CutScene.Window())
            {
                yield return new WaitForEndOfFrame();

                yield return EnterBattleAnimation();

                string aOrAn = "AEIOUAEIOU".Contains(EnemyTitle[0])
                    ? "an"
                    : "a";

                // Display Message
                yield return GameUIManager.TypeOut($"{PlayerManager.Name} engages {aOrAn} {EnemyTitle}.");

                // Pick randomly who goes first
                battleState = Random.Range(0, 1) == 0
                    ? BattleState.PlayerTurn
                    : BattleState.EnemyTurn;

                // Clear extra data
                foreach (BattleUnit unit in players)
                {
                    unit.onDefense = false;
                }

                foreach (BattleUnit unit in enemies)
                {
                    unit.onDefense = false;
                }

                // Battle Loop
                while (battleState != BattleState.Exit)
                {
                    // Wait for frame
                    yield return new WaitForEndOfFrame();

                    // Handel player turn
                    if (battleState == BattleState.PlayerTurn)
                    {
                        foreach (BattleUnit player in players)
                        {
                            // Check if player is alive
                            if (!player.data.Alive) continue;

                            // Remove player defense
                            player.onDefense = false;

                            // Run player
                            yield return PlayerUnitTurn(player);

                            if (CheckLoss(enemies))
                            {
                                battleState = BattleState.Exit;
                                battleFinish = BattleFinish.PlayerWin;
                            }

                            if (battleState == BattleState.Exit)
                            {
                                break;
                            }
                        }

                        if (battleState != BattleState.Exit)
                        {
                            battleState = BattleState.EnemyTurn;
                        }
                    }

                    // Handel enemy turn
                    if (battleState == BattleState.EnemyTurn)
                    {
                        foreach (BattleUnit enemy in enemies)
                        {
                            // Check if enemy is alive
                            if (!enemy.data.Alive) continue;

                            // Remove enemy defense
                            enemy.onDefense = false;

                            // Run enemy
                            yield return EnemyUnitTurn(enemy);

                            if (CheckLoss(players))
                            {
                                battleState = BattleState.Exit;
                                battleFinish = BattleFinish.EnemyWin;
                            }

                            if (battleState == BattleState.Exit)
                            {
                                break;
                            }
                        }

                        if (battleState != BattleState.Exit)
                        {
                            battleState = BattleState.PlayerTurn;
                        }
                    }
                }

                // Exit battle
                yield return EndBattle();
                Destroy(gameObject);
            }
        }

        private IEnumerator EndBattle()
        {
            yield return new WaitForEndOfFrame();

            switch (battleFinish)
            {
                case BattleFinish.PlayerWin:
                    Destroy(enemyGameObject);
                    PlayerController.playerController.SetInactive();
                    yield return GameUIManager.TypeOut($"{PlayerTitle} won the battle!");

                    int gold = enemies.Sum(x => x.data.gold);
                    yield return GameUIManager.TypeOut($"{PlayerTitle} gained {gold} gold.");
                    PlayerManager.Gold += gold;

                    foreach (BattleUnit player in players)
                    {
                        if (player.data.life < 1)
                            player.data.life = 1;
                    }

                    break;

                case BattleFinish.EnemyWin:
                    yield return GameUIManager.TypeOut($"{PlayerTitle} lost the battle!");
                    BattleManager.LostBattle();
                    yield return new WaitWhile(() => true);
                    break;

                case BattleFinish.Run:
                    PlayerController.playerController.SetInactive();
                    break;

                case BattleFinish.None:
                    throw new System.Exception("Battle finish may not be None");
            }
        }

        private IEnumerator ChooseUnitPlayer(BattleUnit[] units)
        {
            // Clear Selected
            yield return new WaitForEndOfFrame();
            selectedUnit = null;

            // Select first alive
            int selected = -1;
            {
                bool multipleAlive = false;
                for (int i = 0; i < units.Length; i++)
                {
                    if (units[i].data.Alive)
                    {
                        if (selected == -1) selected = i;
                        else
                        {
                            multipleAlive = true;
                            break;
                        }
                    }
                }

                selectedUnit = units[selected];

                if (!multipleAlive) yield break;
                else blinkUnit = true;
            }

            while (true)
            {
                yield return MyInput.WaitForMenuNavigation();

                switch (MyInput.MenuNavigation)
                {
                    case MyInput.Action.Left:
                    case MyInput.Action.Up:
                        selected = SkipToNextLivingUnit(units, selected, -1);
                        break;

                    case MyInput.Action.Right:
                    case MyInput.Action.Down:
                        selected = SkipToNextLivingUnit(units, selected, 1);
                        break;

                    case MyInput.Action.Select:
                        blinkUnit = false;
                        selectedUnit.spriteRenderer.color = Color.white;
                        yield break;

                    case MyInput.Action.Cancel:
                        blinkUnit = false;
                        selectedUnit.spriteRenderer.color = Color.white;
                        selectedUnit = null;
                        yield break;
                }

                selectedUnit.spriteRenderer.color = Color.white;
                selectedUnit = units[selected];
                yield return new WaitForEndOfFrame();
            }
        }

        private void ChooseUnitEnemy(BattleUnit[] units)
        {
            BattleUnit target = null;
            while (target == null || !target.data.Alive)
            {
                target = units[Random.Range(0, units.Length)];
            }

            selectedUnit = target;
        }

        private IEnumerator DamageBlink(BattleUnit unit)
        {
            audioSource.PlayOneShot(damageSound);
            
            for (int i = 0; i < 2; i++)
            {
                unit.spriteRenderer.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
                unit.spriteRenderer.color = Color.white;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        private static IEnumerator ChangeLifeOnEnemyUnit(BattleUnit unit, int lifeChange)
        {
            Max100 maxLife = unit.GetMaxHealth();
            unit.data.life += lifeChange;
            if (unit.data.life >= maxLife)
            {
                unit.data.life = maxLife;
                yield return GameUIManager.TypeOut($"{unit.data.title}'s life is maxed out.");
            }

            if (!unit.data.Alive)
            {
                yield return GameUIManager.TypeOut($"{unit.data.title} has been destroyed.");

                int iterations = 6;
                for (float i = 0; i < iterations; i++)
                {
                    unit.spriteRenderer.color = new Color(1, 1, 1, Mathf.Clamp(1 - (i / iterations), 0, 1));
                    yield return new WaitForSecondsRealtime(0.1f);
                    unit.spriteRenderer.color = Color.clear;
                    yield return new WaitForSecondsRealtime(0.05f);
                }
            }
        }

        private static IEnumerator ChangeLifeOnPlayerUnit(BattleUnit unit, int lifeChange)
        {
            unit.data.life += lifeChange;
            Max100 maxLife = unit.GetMaxHealth();
            if (unit.data.life >= maxLife)
            {
                unit.data.life = maxLife;
                yield return GameUIManager.TypeOut($"{unit.data.title}'s life is maxed out.");
            }

            if (!unit.data.Alive)
            {
                yield return GameUIManager.TypeOut($"{unit.data.title} died.");
            }
        }

        private IEnumerator PlayerUnitTurn(BattleUnit unit)
        {
            // Indicate which player is going
            Panel playerPanel = playerPanels.First(p => p.Unit == unit);
            playerPanel.Bump = true;
            PositionPointer(unit);

            // Wait for end of frame
            yield return new WaitForEndOfFrame();

            // Get possible actions
            string[] possibleActions = GetPossibleActions(unit);

            // Get desired action
            yield return GameUIManager.ChoiceMenu(null, possibleActions.ToArray(), 1);
            System.Enum.TryParse(GameUIManager.Answer, out BattleUnit.TurnOptions choice);

            // Run choice
            switch (choice)
            {
                case BattleUnit.TurnOptions.Run:
                {
                    yield return GameUIManager.TypeOut(GetActionStatement(unit, runText, EnemyTitle));
                    yield return Run(GetCanRun(players[0]));
                    break;
                }

                case BattleUnit.TurnOptions.Attack:
                {
                    yield return ChooseUnitPlayer(enemies);

                    // Check if player changes mind
                    if (selectedUnit == null)
                    {
                        yield return PlayerUnitTurn(unit);
                        yield break;
                    }

                    yield return Attack(unit, selectedUnit, ChangeLifeOnEnemyUnit);
                    break;
                }

                case BattleUnit.TurnOptions.Defend:
                    unit.onDefense = true;
                    yield return GameUIManager.TypeOut(GetActionStatement(unit, defendText, EnemyTitle));
                    break;

                case BattleUnit.TurnOptions.Item:
                {
                    // Get desired item
                    {
                        // Get available items
                        string[] itemOptions = GetPossibleItems(unit);

                        // Get display settings
                        int cols;
                        {
                            int itemCount = unit.itemOptionsForUnit.Count;
                            cols = itemCount switch
                            {
                                <= 3 => 1,
                                <= 6 => 2,
                                _ => 3
                            };
                        }

                        // Get choice
                        yield return GameUIManager.ChoiceMenu(null, itemOptions, cols, true);

                        // Check if player changes mind
                        if (GameUIManager.AnswerIndex == -1)
                        {
                            yield return PlayerUnitTurn(unit);
                            yield break;
                        }
                    }

                    // Get data for item choice
                    ItemScriptable item = unit.itemOptionsForUnit[GameUIManager.AnswerIndex];

                    // Choose target
                    BattleUnit target;
                    if (item.Type == ItemScriptable.ItemType.Attack)
                    {
                        yield return ChooseUnitPlayer(enemies);
                        if (selectedUnit == null)
                        {
                            yield return PlayerUnitTurn(unit);
                        }

                        target = selectedUnit;
                    }
                    else if (item.Type == ItemScriptable.ItemType.Heal)
                    {
                        yield return ChooseUnitPlayer(players);
                        if (selectedUnit == null)
                        {
                            yield return PlayerUnitTurn(unit);
                        }

                        target = selectedUnit;
                    }
                    else
                    {
                        throw new System.NotImplementedException();
                    }

                    yield return Item(unit, target, item, ChangeLifeOnPlayerUnit, ChangeLifeOnEnemyUnit);
                    break;
                }

                case BattleUnit.TurnOptions.Magic:
                {
                    // Get desired magic
                    {
                        string[] magicOptions = unit.magicOptionsForUnit.Select(x => x.Title).ToArray();

                        yield return GameUIManager.ChoiceMenu(null, magicOptions, 1, allowCancel: true);

                        // Check if player changes mind
                        if (GameUIManager.AnswerIndex == -1)
                        {
                            yield return PlayerUnitTurn(unit);
                            yield break;
                        }
                    }

                    MagicScriptable magic = unit.magicOptionsForUnit[GameUIManager.AnswerIndex];

                    // Choose target
                    BattleUnit[] targets;
                    switch (magic.Type)
                    {
                        case MagicScriptable.MagicType.Attack when magic.EffectsAll:
                            targets = enemies;
                            break;
                        case MagicScriptable.MagicType.Attack:
                        {
                            yield return ChooseUnitPlayer(enemies);
                            if (selectedUnit == null)
                            {
                                yield return PlayerUnitTurn(unit);
                            }

                            targets = new[] { selectedUnit };
                            break;
                        }
                        case MagicScriptable.MagicType.Heal when magic.EffectsAll:
                            targets = players;
                            break;
                        case MagicScriptable.MagicType.Heal:
                        {
                            yield return ChooseUnitPlayer(players);
                            if (selectedUnit == null)
                            {
                                yield return PlayerUnitTurn(unit);
                            }

                            targets = new[] { selectedUnit };
                            break;
                        }
                        default:
                            throw new System.NotImplementedException();
                    }

                    yield return Magic(unit, targets, magic, ChangeLifeOnPlayerUnit, ChangeLifeOnEnemyUnit);
                    break;
                }
            }

            // Return player panel to original position
            playerPanel.Bump = false;
        }

        private IEnumerator EnemyUnitTurn(BattleUnit unit)
        {
            // Wait for end of frame
            yield return new WaitForEndOfFrame();

            // Indicate which player is going
            PositionPointer(unit);

            yield return new WaitForSecondsRealtime(.5f);

            BattleUnit.TurnOptions choice = GetEnemyTurnChoice(unit);

            // Run choice
            switch (choice)
            {
                case BattleUnit.TurnOptions.Run:
                {
                    yield return GameUIManager.TypeOut(GetActionStatement(unit, runText, PlayerTitle));
                    yield return Run(GetCanRun(players[0]));
                    break;
                }

                case BattleUnit.TurnOptions.Attack:
                {
                    ChooseUnitEnemy(players);
                    yield return Attack(unit, selectedUnit, ChangeLifeOnPlayerUnit);
                    break;
                }

                case BattleUnit.TurnOptions.Defend:
                {
                    unit.onDefense = true;
                    yield return GameUIManager.TypeOut(GetActionStatement(unit, defendText, PlayerTitle));
                    break;
                }

                case BattleUnit.TurnOptions.Item:
                {
                    // Get item
                    string[] itemOptions = GetPossibleItems(unit);
                    ItemScriptable item = unit.itemOptionsForUnit[Random.Range(0, itemOptions.Length)];

                    // Choose target
                    BattleUnit target;
                    if (item.Type == ItemScriptable.ItemType.Attack)
                    {
                        ChooseUnitEnemy(players);
                        target = selectedUnit;
                    }
                    else if (item.Type == ItemScriptable.ItemType.Heal)
                    {
                        ChooseUnitEnemy(enemies);
                        target = selectedUnit;
                    }
                    else
                    {
                        throw new System.NotImplementedException();
                    }

                    yield return Item(unit, target, item, ChangeLifeOnEnemyUnit, ChangeLifeOnPlayerUnit);
                    break;
                }

                case BattleUnit.TurnOptions.Magic:
                {
                    // Get desired magic
                    string[] magicOptions = GetPossibleMagic(unit);
                    MagicScriptable magic = unit.magicOptionsForUnit[Random.Range(0, magicOptions.Length)];

                    // Choose target
                    BattleUnit[] targets;
                    if (magic.Type == MagicScriptable.MagicType.Attack)
                    {
                        if (magic.EffectsAll)
                        {
                            targets = players;
                        }
                        else
                        {
                            ChooseUnitEnemy(players);
                            targets = new[] { selectedUnit };
                        }
                    }
                    else if (magic.Type == MagicScriptable.MagicType.Heal)
                    {
                        if (magic.EffectsAll)
                        {
                            targets = enemies;
                        }
                        else
                        {
                            ChooseUnitEnemy(enemies);
                            targets = new[] { selectedUnit };
                        }
                    }
                    else
                    {
                        throw new System.NotImplementedException();
                    }

                    yield return Magic(unit, targets, magic, ChangeLifeOnEnemyUnit, ChangeLifeOnPlayerUnit);

                    break;
                }
            }
        }

        private static BattleUnit.TurnOptions GetEnemyTurnChoice(BattleUnit enemy)
        {
            EnemyAI ai = enemy.data.enemyAI;
            ai.CorrectData();
            int choiceInt = Random.Range(1, 101);
            int bar = ai.attack;
            if (choiceInt <= bar) return BattleUnit.TurnOptions.Attack;

            bar += ai.defend;
            if (choiceInt <= bar) return BattleUnit.TurnOptions.Defend;

            bar += ai.item;
            if (choiceInt <= bar)
            {
                if (enemy.itemOptionsForUnit.Count == 0) return GetEnemyTurnChoice(enemy);
                return BattleUnit.TurnOptions.Item;
            }

            bar += ai.magic;
            if (choiceInt <= bar)
            {
                if (enemy.magicOptionsForUnit.Count == 0) return GetEnemyTurnChoice(enemy);
                return BattleUnit.TurnOptions.Magic;
            }

            bar += ai.run;
            if (choiceInt <= bar) return BattleUnit.TurnOptions.Run;

            throw new System.Exception("AI did not find correct turn option.");
        }

        private IEnumerator Run(bool run)
        {
            if (run)
            {
                yield return GameUIManager.TypeOut("And got away!");
                battleState = BattleState.Exit;
                battleFinish = BattleFinish.Run;
            }
            else
            {
                yield return GameUIManager.TypeOut("But couldn't get away.");
            }
        }

        private static bool GetCanRun(BattleUnit enemyUnit)
        {
            bool run;
            if (enemyUnit.data.escapePercentageAllowed == 0) run = false;
            else run = Random.Range(0, 100 / enemyUnit.data.escapePercentageAllowed) == 0;
            return run;
        }

        private IEnumerator Attack(BattleUnit attacking, BattleUnit defending,
            System.Func<BattleUnit, int, IEnumerator> lifeChangeFunc)
        {
            yield return GameUIManager.TypeOut(GetActionStatement(attacking, attackText, defending.data.title));
            yield return AttackPower(attacking, defending);
            yield return GameUIManager.TypeOut($"{attackPower} damage to {defending.data.title}.");
            yield return lifeChangeFunc(defending, -attackPower);
        }

        private IEnumerator AttackPower(BattleUnit attacking, BattleUnit defending)
        {
            bool isPlayer = players.Contains(attacking);

            BadgesScriptable[] attackingBadges = attacking.GetAttackBadges();
            BadgesScriptable[] defendingBadges = defending.GetDefenseBadges();

            List<BadgeStation> attackingBadgeStations = new();

            for (int i = 0; i < attackingBadges.Length; i++)
            {
                BadgesScriptable badge = attackingBadges[i];
                GameObject badgeStation = Instantiate(badgePrefab, isPlayer ? playerBadges : enemyBadges, false);
                SpriteRenderer sr = badgeStation.GetComponent<SpriteRenderer>();
                sr.sprite = badge.Sprite;
                badgeStation.transform.Translate(Vector3.right * (isPlayer ? playerSpread * i : -playerSpread * i));

                attackingBadgeStations.Add(new BadgeStation(sr, badgeStation.transform, badge));
            }

            List<BadgeStation> defendingBadgeStations = new();
            for (int i = 0; i < defendingBadges.Length; i++)
            {
                BadgesScriptable badge = defendingBadges[i];
                GameObject badgeStation = Instantiate(badgePrefab, isPlayer ? enemyBadges : playerBadges, false);
                SpriteRenderer sr = badgeStation.GetComponent<SpriteRenderer>();
                sr.sprite = badge.Sprite;
                badgeStation.transform.Translate(Vector3.left * (isPlayer ? playerSpread * i : -playerSpread * i));

                defendingBadgeStations.Add(new BadgeStation(sr, badgeStation.transform, badge));
            }

            attackPower = 1;
            if (attackingBadgeStations.Count > 0)
            {
                bool first = true;
                foreach (BadgeStation badgeStation in attackingBadgeStations)
                {
                    yield return badgeStation.AnimationIn();

                    if (first)
                        damageNumberCanvas.position = badgeStation.GetTargetPosition();
                    else
                        yield return badgeStation.LerpObjectToPosition(damageNumberCanvas);

                    attackPower += badgeStation.Badge.Power * Random.Range(0.75f, 1);
                    damageText.text = attackPower.ToString();
                    yield return new WaitForSeconds(0.25f);
                    yield return badgeStation.AnimationOut();
                    first = false;
                }
            }
            else
            {
                damageNumberCanvas.transform.position = isPlayer
                    ? playerBadges.position
                    : enemyBadges.position;

                damageText.text = "1";
            }

            foreach (BadgeStation badgeStation in defendingBadgeStations)
            {
                yield return badgeStation.AnimationIn();
                yield return badgeStation.LerpObjectToPosition(damageNumberCanvas);
                attackPower -= badgeStation.Badge.Power * Random.Range(0.75f, 1);
                attackPower.Min1();
                damageText.text = attackPower.ToString();
                yield return new WaitForSeconds(0.25f);
                yield return badgeStation.AnimationOut();
            }

            if (defending.onDefense)
            {
                yield return BadgeStation.LerpObjectToPosition(damageNumberCanvas.transform,
                    defending.stationGo.transform.position + Vector3.down * .5f);
                attackPower *= .5f;
                attackPower.Min1();
                damageText.text = attackPower.ToString();
                yield return new WaitForSeconds(0.25f);
            }

            yield return BadgeStation.LerpObjectToPosition(damageNumberCanvas.transform,
                defending.stationGo.transform.position);

            damageText.text = "";
            yield return DamageBlink(defending);
        }

        private IEnumerator AttackPowerNoAttackBadge(BattleUnit defending, Max100 initialAttack)
        {
            bool isPlayer = enemies.Contains(defending);

            BadgesScriptable[] defendingBadges = defending.GetDefenseBadges();

            List<BadgeStation> defendingBadgeStations = new();
            for (int i = 0; i < defendingBadges.Length; i++)
            {
                BadgesScriptable badge = defendingBadges[i];
                GameObject badgeStation = Instantiate(badgePrefab, isPlayer ? enemyBadges : playerBadges, false);
                SpriteRenderer sr = badgeStation.GetComponent<SpriteRenderer>();
                sr.sprite = badge.Sprite;
                badgeStation.transform.Translate(Vector3.left * (isPlayer ? playerSpread * i : -playerSpread * i));

                defendingBadgeStations.Add(new BadgeStation(sr, badgeStation.transform, badge));
            }

            damageNumberCanvas.transform.position = isPlayer
                ? playerBadges.position
                : enemyBadges.position;

            attackPower = initialAttack;
            damageText.text = initialAttack.ToString();

            foreach (BadgeStation badgeStation in defendingBadgeStations)
            {
                yield return badgeStation.AnimationIn();
                yield return badgeStation.LerpObjectToPosition(damageNumberCanvas);
                attackPower -= badgeStation.Badge.Power * Random.Range(0.75f, 1);
                attackPower.Min1();
                damageText.text = attackPower.ToString();
                yield return badgeStation.AnimationOut();
            }

            if (defending.onDefense)
            {
                yield return BadgeStation.LerpObjectToPosition(damageNumberCanvas.transform,
                    defending.stationGo.transform.position + Vector3.down * .5f);
                attackPower *= .5f;
                attackPower.Min1();
                damageText.text = attackPower.ToString();
                yield return new WaitForSeconds(0.25f);
            }

            yield return BadgeStation.LerpObjectToPosition(damageNumberCanvas.transform,
                defending.stationGo.transform.position);
            damageText.text = "";
            yield return DamageBlink(defending);
        }

        private IEnumerator Item(BattleUnit unit, BattleUnit target, ItemScriptable item,
            System.Func<BattleUnit, int, IEnumerator> changeLifeOnFriendFunc,
            System.Func<BattleUnit, int, IEnumerator> changeLifeOnOpponentFunc)
        {
            // Preliminary message
            yield return GameUIManager.TypeOut($"{unit.data.title} tried using {item.Title}.");
            unit.itemOptionsForUnit.Remove(item);

            // Run Item
            yield return new WaitForEndOfFrame();
            if (item.Type == ItemScriptable.ItemType.Attack)
            {
                yield return AttackPowerNoAttackBadge(target, item.Power);
                yield return GameUIManager.TypeOut($"{attackPower} damage to {target.data.title}.");
                yield return changeLifeOnOpponentFunc(target, -attackPower);
            }
            else if (item.Type == ItemScriptable.ItemType.Heal)
            {
                int heal = item.Power;
                yield return GameUIManager.TypeOut($"{target.data.title} gained {heal} HP.");
                yield return changeLifeOnFriendFunc(target, heal);
            }
        }

        private static string[] GetPossibleItems(BattleUnit unit)
        {
            return unit.itemOptionsForUnit.Select(item => item.ToString()).ToArray();
        }

        private IEnumerator Magic(BattleUnit unit, BattleUnit[] targets, MagicScriptable magic,
            System.Func<BattleUnit, int, IEnumerator> changeLifeOnFriendFunc,
            System.Func<BattleUnit, int, IEnumerator> changeLifeOnOpponentFunc)
        {
            // Preliminary message
            yield return GameUIManager.TypeOut($"{unit.data.title} tried {magic.Title}.");

            // Handel tax
            if (unit.data.magic >= magic.Price)
            {
                unit.data.magic -= magic.Price;
            }
            else
            {
                yield return GameUIManager.TypeOut($"But didn't have enough magic.");
                yield break;
            }

            // Run magic
            yield return new WaitForEndOfFrame();
            if (magic.Type == MagicScriptable.MagicType.Attack)
            {
                foreach (BattleUnit target in targets)
                {
                    yield return AttackPowerNoAttackBadge(target, magic.Power);
                    yield return GameUIManager.TypeOut($"{attackPower} damage to {target.data.title}.");
                    yield return changeLifeOnOpponentFunc(target, -attackPower);
                }
            }
            else if (magic.Type == MagicScriptable.MagicType.Heal)
            {
                foreach (BattleUnit target in targets)
                {
                    int heal = magic.Power;
                    yield return GameUIManager.TypeOut($"{target.data.title} gained {heal} HP.");
                    yield return changeLifeOnFriendFunc(target, heal);
                }
            }
        }

        private static string[] GetPossibleMagic(BattleUnit unit)
        {
            string[] magicOptions = new string[unit.magicOptionsForUnit.Count];
            for (int i = 0; i < magicOptions.Length; i++)
            {
                string stringName = unit.magicOptionsForUnit[i].ToString();
                magicOptions[i] = stringName;
            }

            return magicOptions;
        }

        private static string[] GetPossibleActions(BattleUnit unit)
        {
            List<string> possibleActions = new(System.Enum.GetNames(typeof(BattleUnit.TurnOptions)));

            if (unit.magicOptionsForUnit.Count == 0)
                possibleActions.Remove("Magic");

            if (unit.itemOptionsForUnit.Count == 0)
                possibleActions.Remove("Item");

            return possibleActions.ToArray();
        }

        private static int SkipToNextLivingUnit(BattleUnit[] units, int selected, int step)
        {
            selected += step;
            if (selected > units.Length - 1) selected = 0;
            else if (selected < 0) selected = units.Length - 1;

            if (!units[selected].data.Alive)
            {
                return SkipToNextLivingUnit(units, selected, step);
            }

            return selected;
        }

        private static string GetActionStatement(BattleUnit unit, string[] statements, string opposition)
        {
            string choice = statements[Random.Range(0, statements.Length)];
            choice = choice.Replace("Player", unit.data.title)
                .Replace("Enemy", opposition);

            return unit.SyncSex(choice);
        }

        private static bool CheckLoss(BattleUnit[] side)
        {
            foreach (BattleUnit battleUnit in side)
            {
                if (battleUnit.data.Alive) return false;
            }

            return true;
        }

        private void DisplayUnitsOnPanel()
        {
            foreach (Panel panel in playerPanels)
            {
                panel.InstantiateToUnit();
            }
        }

        private void PositionPointer(BattleUnit unit)
        {
            pointer.transform.position = unit.spriteRenderer.transform.position;

            int moveDir;
            if (Camera.main!.WorldToScreenPoint(pointer.transform.position).y - Camera.main.scaledPixelHeight / 2 > 0)
            {
                moveDir = -1;
            }
            else
            {
                moveDir = 1;
            }

            pointer.transform.position += .2f * moveDir * Vector3.up;
            pointer.transform.rotation = Quaternion.Euler(0, 0, moveDir * -90);
        }

        private readonly struct BadgeStation
        {
            private readonly SpriteRenderer sr;
            private readonly Transform tr;
            public readonly BadgesScriptable Badge;
            private const byte AnimationSpeed = 5;

            public BadgeStation(SpriteRenderer sr, Transform transform, BadgesScriptable badge)
            {
                this.sr = sr;
                tr = transform;
                Badge = badge;
            }

            public IEnumerator AnimationIn()
            {
                while (sr.color.a < .9)
                {
                    sr.color = Color.Lerp(sr.color, Color.white, Time.deltaTime * AnimationSpeed);
                    tr.Translate(Vector3.up * Time.deltaTime * 0.15f);
                    yield return new WaitForEndOfFrame();
                }

                sr.color = Color.white;
            }

            public IEnumerator AnimationOut()
            {
                while (sr.color.a > .1)
                {
                    sr.color = Color.Lerp(sr.color, Color.clear, Time.deltaTime * AnimationSpeed);
                    tr.Translate(Vector3.down * Time.deltaTime * 0.15f);
                    yield return new WaitForEndOfFrame();
                }

                sr.color = Color.clear;
            }

            public IEnumerator LerpObjectToPosition(Transform objectTransform)
            {
                yield return LerpObjectToPosition(objectTransform, GetTargetPosition());
            }

            public static IEnumerator LerpObjectToPosition(Transform objectTransform, Vector3 endPoint)
            {
                const float duration = 0.25f;
                Vector3 startPoint = objectTransform.position;
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    objectTransform.position = Vector3.Lerp(startPoint, endPoint, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                objectTransform.position = endPoint;
            }

            public Vector3 GetTargetPosition()
            {
                return tr.position + Vector3.down * .175f;
            }
        }
    }
}