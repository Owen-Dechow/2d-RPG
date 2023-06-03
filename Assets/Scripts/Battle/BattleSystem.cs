using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    #region Public
    public float playerSpread;
    public float panelSpread;

    public GameObject enemyGameObject;
    public BattleUnit[] players;
    public BattleUnit[] enemies;

    [SerializeField] Transform playerBattleStation;
    [SerializeField] Transform enemyBattleStation;
    [SerializeField] GameObject panelPrefab;
    [SerializeField] Canvas canvas;
    #endregion

    #region AltText
    private readonly string[] attackText = {
        "attacks",
        "swings at the enemy",
        "throws himself at the enemy",
        "assaults the enemy",
        "strikes at the enemy",
        "bombards the enemy",
        "rushes the enemy",
        "charges the enemy",
        "hurls himself at the enemy",
        "pelts the enemy",
        "bashes the enemy"
    };
    private readonly string[] defendText = {
        "is on defense",
        "is cowering in fear",
        "takes a step back",
        "shields himself",
        "defends himself from the enemy",
        "is on guard",
        "protects himself from the enemy",
        "screens himself from the enemy",
        "walls himself from the enemy",
        "recoils",
        "shrinks in fear",
        "whitens in fear",
    };
    private readonly string[] runText = {
        "run",
        "flee",
        "get away",
        "fly from the battle",
        "scamper away from the battle",
        "gallop away from the enemy",
        "leave the battle",
        "remove himself from the battle",
    };
    #endregion

    Panel[] playerPanels;
    BattleUnit selectedUnit = null;
    bool blinkUnit = false;

    BattleState battleState;
    BattleFinish battleFinish;

    string PlayerTitle { get => $"{GameManager.player.playerBattleUnit.title}{(players.Length == 1 ? "" : " and his companion" + (players.Length == 2 ? "" : "s"))}"; }
    string EnemyTitle { get => $"the {enemies[0].title}{(enemies.Length == 1 ? "" : enemies.Length == 2 ? " and his brother" : "'s mob")}"; }

    enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        Exit,
    }

    enum BattleFinish
    {
        PlayerWin,
        EnemyWin,
        PlayerRun,
        EnemyRun,
        None,
    }

    void Start()
    {
        // Instantiate player units
        playerPanels = new Panel[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            players[i].stationGO = new($"PlayerBattleUnit_{i}", new System.Type[] { typeof(SpriteRenderer) });
            players[i].stationGO.transform.parent = playerBattleStation;

            players[i].spr = players[i].stationGO.GetComponent<SpriteRenderer>();
            players[i].spr.sprite = players[i].sprite;
            players[i].spr.sortingLayerName = "Battle";

            float playerMove;
            playerMove = (i * playerSpread) - (players.Length * playerSpread / 2);
            playerMove += playerSpread / 2;
            players[i].stationGO.transform.localPosition = Vector3.zero;
            players[i].stationGO.transform.Translate(playerMove, 0, 0);

            float panelMove;
            panelMove = (i * panelSpread) - (players.Length * panelSpread / 2);
            panelMove += panelSpread / 2;

            playerPanels[i] = Instantiate(panelPrefab, canvas.transform).GetComponent<Panel>();
            playerPanels[i].unit = players[i];
            playerPanels[i].transform.Translate(panelMove, 0, 0);
        }

        // Instantiate enemy units
        Dictionary<string, int> enemyNumbers = new();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].stationGO = new($"EnemyBattleUnit_{i}", new System.Type[] { typeof(SpriteRenderer) });
            enemies[i].stationGO.transform.parent = enemyBattleStation;

            enemies[i].spr = enemies[i].stationGO.GetComponent<SpriteRenderer>();
            enemies[i].spr.sprite = enemies[i].sprite;
            enemies[i].spr.sortingLayerName = "Battle";

            float enemyMove;
            enemyMove = (i * playerSpread) - (enemies.Length * playerSpread / 2);
            enemyMove += playerSpread / 2;
            enemies[i].stationGO.transform.localPosition = Vector3.zero;
            enemies[i].stationGO.transform.Translate(enemyMove, 0, 0);

            enemies[i] = enemies[i].Copy();
            if (enemyNumbers.ContainsKey(enemies[i].title))
            {
                enemyNumbers[enemies[i].title]++;
                enemies[i].title += " " + enemyNumbers[enemies[i].title];

            }
            else enemyNumbers[enemies[i].title] = 1;
        }

        // Set Battle Position
        GameObject camera = GameObject.Find("Main Camera");
        transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0);

        DisplayUnitsOnPanel();
        StartCoroutine(Battle());
    }
    private void Update()
    {
        foreach (Panel panel in playerPanels) { panel.DisplayUnitGradual(Time.unscaledDeltaTime * 4); }
        if (blinkUnit)
        {
            if (selectedUnit != null)
            {
                selectedUnit.spr.color = Mathf.Floor(Time.unscaledTime * 10) % 2 == 0 ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.3f);
            }
        }
    }

    IEnumerator Battle()
    {
        yield return new WaitForEndOfFrame();

        // Battle enter message
        {
            // Set enemy group name
            string enemyGroupName;
            if (enemies.Length == 1) enemyGroupName = enemies[0].title;
            else if (enemies.Length == 2) enemyGroupName = $"{enemies[0].title} and his brother";

            // Select a or an
            else enemyGroupName = $"{enemies[0].title} and his cohorts";
            if ("aeiouAEIOU".Contains(enemyGroupName[0])) enemyGroupName = "an " + enemyGroupName;
            else enemyGroupName = "a " + enemyGroupName;

            // Display Message
            yield return GameUI.TypeOut($"{GameManager.player.Name} engages {enemyGroupName}.");
        }

        // Pick randomly who goes first
        if (Random.Range(0, 2) == 0) battleState = BattleState.PlayerTurn;
        else battleState = BattleState.EnemyTurn;

        // Clear extra data extra data
        foreach (BattleUnit unit in players) { unit.onDefence = false; }
        foreach (BattleUnit unit in enemies) { unit.onDefence = false; }

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
                    if (!player.Alive) continue;

                    // Remove player defense
                    player.onDefence = false;

                    // Run player
                    yield return PlayerUnitTurn(player);

                    if (battleState == BattleState.Exit)
                    {
                        break;
                    }
                }

                if (battleState != BattleState.Exit)
                {
                    if (CheckLoss(enemies))
                    {
                        battleState = BattleState.Exit;
                        battleFinish = BattleFinish.PlayerWin;
                    }
                    else
                    {
                        battleState = BattleState.EnemyTurn;
                    }
                }
            }

            // Handel enemy turn
            if (battleState == BattleState.EnemyTurn)
            {
                foreach (BattleUnit enemy in enemies)
                {
                    // Check if enemy is alive
                    if (!enemy.Alive) continue;

                    // Remove enemy defense
                    enemy.onDefence = false;

                    // Run enemy
                    yield return EnemyUnitTurn(enemy);

                    if (battleState == BattleState.Exit)
                    {
                        break;
                    }
                }

                if (battleState != BattleState.Exit)
                {
                    if (CheckLoss(players))
                    {
                        battleState = BattleState.Exit;
                        battleFinish = BattleFinish.EnemyWin;
                    }
                    else
                    {
                        battleState = BattleState.PlayerTurn;
                    }
                }
            }
        }

        // Exit battle
        yield return EndBattle();
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    IEnumerator PlayerUnitTurn(BattleUnit unit)
    {
        // Wait for end of frame
        yield return new WaitForEndOfFrame();

        // Get possible actions
        List<string> possibleActions = new(System.Enum.GetNames(typeof(BattleUnit.TurnOptions)));
        if (unit.magicOptionsForUnit.Count == 0) possibleActions.Remove("Magic");
        if (unit.items.Count == 0) possibleActions.Remove("Item");

        // Get desired action
        yield return GameUI.ChoiceMenu(null, possibleActions.ToArray(), 1);
        System.Enum.TryParse(GameManager.Answer, out BattleUnit.TurnOptions choice);

        // Run choice
        if (choice == BattleUnit.TurnOptions.Run)
        {
            // Attempt message
            yield return GameUI.TypeOut($"{unit.title} attempts to {GetActionStatement(unit, runText, EnemyTitle)}.");

            // Check if run is successful
            bool run;
            if (enemies[0].escapePercentageAllowed == 0) run = false;
            else run = Random.Range(0, 100 / enemies[0].escapePercentageAllowed) == 0;

            // Finish turn
            if (run)
            {
                yield return GameUI.TypeOut("And got away!");
                battleState = BattleState.Exit;
                battleFinish = BattleFinish.PlayerRun;
            }
            else yield return GameUI.TypeOut("But couldn't get away.");

        }
        else if (choice == BattleUnit.TurnOptions.Attack)
        {
            // Get enemy to attack
            yield return ChooseUnitPlayer(enemies);

            // Check if player changes mind
            if (selectedUnit == null)
            {
                yield return PlayerUnitTurn(unit);
                yield break;
            }

            // Get attack power
            int preDefenseAttack = unit.badges.GetAttack();
            int attack = selectedUnit.badges.GetDefenseChange(preDefenseAttack, selectedUnit.onDefence);

            // Send message
            yield return GameUI.TypeOut($"{unit.title} {GetActionStatement(unit, attackText, selectedUnit.title)}!");
            yield return GameUI.TypeOut($"{attack} damage to {selectedUnit.title}.");
            yield return ChangeLifeOnEnemyUnit(selectedUnit, -attack);
        }
        else if (choice == BattleUnit.TurnOptions.Defend)
        {
            unit.onDefence = true;
            yield return GameUI.TypeOut($"{unit.title} {GetActionStatement(unit, defendText, EnemyTitle)}.");
        }
        else if (choice == BattleUnit.TurnOptions.Item)
        {
            // Get desired item
            {
                // Get available items
                string[] itemOptions = new string[unit.items.Count];
                for (int i = 0; i < itemOptions.Length; i++)
                {
                    itemOptions[i] = unit.items[i].ToString();
                }

                // Get display settings
                int cols;
                {
                    int itemCount = unit.items.Count;
                    if (itemCount <= 3) cols = 1;
                    else if (itemCount <= 6) cols = 2;
                    else cols = 3;
                }

                // Get choice
                yield return GameUI.ChoiceMenu(null, itemOptions, cols, true);

                // Check if player changes mind
                if (GameManager.AnswerIndex == -1)
                {
                    yield return PlayerUnitTurn(unit);
                    yield break;
                }
            }

            // Get data for item choice
            Items.DataSet data = Items.GetDataForOption(GameManager.Answer);

            // Choose target
            BattleUnit target;
            if (data.scriptable.Type == ItemScriptable.ItemType.Attack)
            {
                yield return ChooseUnitPlayer(enemies);
                if (selectedUnit == null)
                {
                    yield return PlayerUnitTurn(unit);
                }

                target = selectedUnit;
            }
            else if (data.scriptable.Type == ItemScriptable.ItemType.Heal)
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

            // Preliminary message
            yield return GameUI.TypeOut($"{unit.title} tried using {data.Title}.");

            // Remove item from inventory
            unit.items.Remove(data.identity);

            // Run Item
            yield return new WaitForEndOfFrame();
            if (data.scriptable.Type == ItemScriptable.ItemType.Attack)
            {

                int hit = target.badges.GetDefenseChange(data.scriptable.Power, target.onDefence);
                yield return GameUI.TypeOut($"{hit} damage to {target.title}.");
                yield return ChangeLifeOnEnemyUnit(target, -hit);
            }
            else if (data.scriptable.Type == ItemScriptable.ItemType.Heal)
            {
                int heal = data.scriptable.Power;
                yield return GameUI.TypeOut($"{target.title} gained {heal} HP.");
                yield return ChangeLifeOnPlayerUnit(target, heal);
            }

        }
        else if (choice == BattleUnit.TurnOptions.Magic)
        {
            // Get desired magic
            {
                // Get magic options
                string[] magicOptions = new string[unit.magicOptionsForUnit.Count];
                for (int i = 0; i < magicOptions.Length; i++)
                {
                    string stringName = unit.magicOptionsForUnit[i].ToString();
                    magicOptions[i] = stringName;
                    magicOptions[i] = magicOptions[i] + " : " + Magic.GetDataForOption(stringName).scriptable.Price;
                }
                yield return GameUI.ChoiceMenu(null, magicOptions, 1, allowCancel: true);

                // Check if player changes mind
                if (GameManager.AnswerIndex == -1)
                {
                    yield return PlayerUnitTurn(unit);
                    yield break;
                }
            }

            // Get data for magic choice
            Magic.DataSet data = Magic.GetDataForOption(GameManager.Answer.Split(" : ")[0]);

            // Choose target
            BattleUnit[] targets;
            if (data.scriptable.Type == MagicScriptable.MagicType.Attack)
            {
                if (data.scriptable.EffectsAll)
                {
                    targets = enemies;
                }
                else
                {
                    yield return ChooseUnitPlayer(enemies);
                    if (selectedUnit == null)
                    {
                        yield return PlayerUnitTurn(unit);
                    }

                    targets = new BattleUnit[] { selectedUnit };
                }
            }
            else if (data.scriptable.Type == MagicScriptable.MagicType.Heal)
            {
                if (data.scriptable.EffectsAll)
                {
                    targets = players;
                }
                else
                {
                    yield return ChooseUnitPlayer(players);
                    if (selectedUnit == null)
                    {
                        yield return PlayerUnitTurn(unit);
                    }

                    targets = new BattleUnit[] { selectedUnit };
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }

            // Preliminary message
            yield return GameUI.TypeOut($"{unit.title} tried {data.Title}.");

            // Handel tax
            if (unit.magic >= data.scriptable.Price)
            {
                unit.magic -= data.scriptable.Price;
            }
            else
            {
                yield return GameUI.TypeOut($"But didn't have enough magic.");
                yield break;
            }

            // Run magic
            yield return new WaitForEndOfFrame();
            if (data.scriptable.Type == MagicScriptable.MagicType.Attack)
            {
                foreach (var target in targets)
                {
                    int hit = target.badges.GetDefenseChange(data.scriptable.Power, target.onDefence);
                    yield return GameUI.TypeOut($"{hit} damage to {target.title}.");
                    yield return ChangeLifeOnEnemyUnit(target, -hit);
                }
            }
            else if (data.scriptable.Type == MagicScriptable.MagicType.Heal)
            {
                foreach (var target in targets)
                {
                    int heal = data.scriptable.Power;
                    yield return GameUI.TypeOut($"{target.title} gained {heal} HP.");
                    yield return ChangeLifeOnPlayerUnit(target, heal);
                }
            }
        }
    }

    IEnumerator EnemyUnitTurn(BattleUnit unit)
    {
        // Wait for end of frame
        yield return new WaitForEndOfFrame();

        // Get possible actions
        List<string> possibleActions = new(System.Enum.GetNames(typeof(BattleUnit.TurnOptions)));
        if (unit.magicOptionsForUnit.Count == 0) possibleActions.Remove("Magic");
        if (unit.items.Count == 0) possibleActions.Remove("Item");

        // Get desired action
        System.Enum.TryParse(possibleActions[Random.Range(0, possibleActions.Count)], out BattleUnit.TurnOptions choice);

        // Run choice
        if (choice == BattleUnit.TurnOptions.Run)
        {
            // Attempt message
            yield return GameUI.TypeOut($"{unit.title} attempts to {GetActionStatement(unit, runText, PlayerTitle)}.");

            // Check if run is successful
            bool run;
            if (players[0].escapePercentageAllowed == 0) run = false;
            else run = Random.Range(0, 100 / players[0].escapePercentageAllowed) == 0;

            // Finish turn
            if (run)
            {
                yield return GameUI.TypeOut("And got away!");
                battleState = BattleState.Exit;
                battleFinish = BattleFinish.EnemyRun;
            }
            else yield return GameUI.TypeOut("But couldn't get away.");

        }
        else if (choice == BattleUnit.TurnOptions.Attack)
        {
            // Get enemy to attack
            ChooseUnitEnemy(players);

            // Get attack power
            int preDefenseAttack = unit.badges.GetAttack();
            int attack = selectedUnit.badges.GetDefenseChange(preDefenseAttack, selectedUnit.onDefence);

            // Send message
            yield return GameUI.TypeOut($"{unit.title} {GetActionStatement(unit, attackText, selectedUnit.title)}!");
            yield return GameUI.TypeOut($"{attack} damage to {selectedUnit.title}.");
            yield return ChangeLifeOnPlayerUnit(selectedUnit, -attack);
        }
        else if (choice == BattleUnit.TurnOptions.Defend)
        {
            unit.onDefence = true;
            yield return GameUI.TypeOut($"{unit.title} {GetActionStatement(unit, defendText, PlayerTitle)}.");
        }
        else if (choice == BattleUnit.TurnOptions.Item)
        {
            // Get item
            Items.DataSet data;
            {
                // Get available items
                string[] itemOptions = new string[unit.items.Count];
                for (int i = 0; i < itemOptions.Length; i++)
                {
                    itemOptions[i] = unit.items[i].ToString();
                }

                // Get choice
                data = Items.GetDataForOption(itemOptions[Random.Range(0, itemOptions.Length)]);
            }

            // Choose target
            BattleUnit target;
            if (data.scriptable.Type == ItemScriptable.ItemType.Attack)
            {
                ChooseUnitEnemy(players);
                target = selectedUnit;
            }
            else if (data.scriptable.Type == ItemScriptable.ItemType.Heal)
            {
                ChooseUnitEnemy(enemies);
                target = selectedUnit;
            }
            else
            {
                throw new System.NotImplementedException();
            }

            // Preliminary message
            yield return GameUI.TypeOut($"{unit.title} tried using {data.Title}.");

            // Remove item from inventory
            unit.items.Remove(data.identity);

            // Run Item
            yield return new WaitForEndOfFrame();
            if (data.scriptable.Type == ItemScriptable.ItemType.Attack)
            {

                int hit = target.badges.GetDefenseChange(data.scriptable.Power, target.onDefence);
                yield return GameUI.TypeOut($"{hit} damage to {target.title}.");
                yield return ChangeLifeOnPlayerUnit(target, -hit);
            }
            else if (data.scriptable.Type == ItemScriptable.ItemType.Heal)
            {
                int heal = data.scriptable.Power;
                yield return GameUI.TypeOut($"{target.title} gained {heal} HP.");
                yield return ChangeLifeOnEnemyUnit(target, heal);
            }

        }
        else if (choice == BattleUnit.TurnOptions.Magic)
        {
            // Get desired magic
            Magic.DataSet data;
            {
                // Get magic options
                string[] magicOptions = new string[unit.magicOptionsForUnit.Count];
                for (int i = 0; i < magicOptions.Length; i++)
                {
                    string stringName = unit.magicOptionsForUnit[i].ToString();
                    magicOptions[i] = stringName;
                }

                // Get choice
                data = Magic.GetDataForOption(magicOptions[Random.Range(0, magicOptions.Length)]);
            }

            // Choose target
            BattleUnit[] targets;
            if (data.scriptable.Type == MagicScriptable.MagicType.Attack)
            {
                if (data.scriptable.EffectsAll) { targets = players; }
                else
                {
                    ChooseUnitEnemy(players);
                    targets = new BattleUnit[] { selectedUnit };
                }
            }
            else if (data.scriptable.Type == MagicScriptable.MagicType.Heal)
            {
                if (data.scriptable.EffectsAll) { targets = enemies; }
                else
                {
                    ChooseUnitEnemy(enemies);
                    targets = new BattleUnit[] { selectedUnit };
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }

            // Preliminary message
            yield return GameUI.TypeOut($"{unit.title} tried {data.Title}.");

            // Handel tax
            if (unit.magic >= data.scriptable.Price)
            {
                unit.magic -= data.scriptable.Price;
            }
            else
            {
                yield return GameUI.TypeOut($"But didn't have enough magic.");
                yield break;
            }

            // Run magic
            yield return new WaitForEndOfFrame();
            if (data.scriptable.Type == MagicScriptable.MagicType.Attack)
            {
                foreach (BattleUnit target in targets)
                {
                    int hit = target.badges.GetDefenseChange(data.scriptable.Power, target.onDefence);
                    yield return GameUI.TypeOut($"{hit} damage to {target.title}.");
                    yield return ChangeLifeOnPlayerUnit(target, -hit);
                }
            }
            else if (data.scriptable.Type == MagicScriptable.MagicType.Heal)
            {
                foreach (BattleUnit target in targets)
                {
                    int heal = data.scriptable.Power;
                    yield return GameUI.TypeOut($"{target.title} gained {heal} HP.");
                    yield return ChangeLifeOnEnemyUnit(target, heal);
                }
            }
        }
    }

    IEnumerator ChooseUnitPlayer(BattleUnit[] units)
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
                if (units[i].Alive)
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
                    selected = SkipToNextLivingUnit(units, selected, 1);
                    break;

                case MyInput.Action.Right:
                case MyInput.Action.Down:
                    selected = SkipToNextLivingUnit(units, selected, -1);
                    break;

                case MyInput.Action.Select:
                    blinkUnit = false;
                    yield break;

                case MyInput.Action.Cancel:
                    blinkUnit = false;
                    selectedUnit.spr.color = Color.white;
                    selectedUnit = null;
                    yield break;
            }

            selectedUnit.spr.color = Color.white;
            selectedUnit = units[selected];
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ChangeLifeOnEnemyUnit(BattleUnit unit, int lifeChange)
    {
        if (lifeChange < 0)
        {
            for (int i = 0; i < 2; i++)
            {
                unit.spr.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
                unit.spr.color = Color.white;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        unit.life += lifeChange;
        if (unit.life >= unit.maxLife)
        {
            unit.life = unit.maxLife;
            yield return GameUI.TypeOut($"{unit.title}'s life is maxed out.");
        }
        if (!unit.Alive)
        {
            yield return GameUI.TypeOut($"{unit.title} has been destroyed.");

            int iterations = 6;
            for (float i = 0; i < iterations; i++)
            {
                unit.spr.color = new Color(1, 1, 1, Mathf.Clamp(1 - (i / iterations), 0, 1));
                yield return new WaitForSecondsRealtime(0.1f);
                unit.spr.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
            }

        }

    }

    IEnumerator ChangeLifeOnPlayerUnit(BattleUnit unit, int lifeChange)
    {
        if (lifeChange < 0)
        {
            for (int i = 0; i < 2; i++)
            {
                unit.spr.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
                unit.spr.color = Color.white;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        unit.life += lifeChange;
        if (unit.life >= unit.maxLife)
        {
            unit.life = unit.maxLife;
            yield return GameUI.TypeOut($"{unit.title}'s life is maxed out.");
        }
        if (!unit.Alive)
        {
            yield return GameUI.TypeOut($"{unit.title} died.");
        }
    }

    IEnumerator EndBattle()
    {
        yield return new WaitForEndOfFrame();

        switch (battleFinish)
        {
            case BattleFinish.PlayerWin:
                Destroy(enemyGameObject);
                GameManager.player.playerObject.SetInactive();
                yield return GameUI.TypeOut($"{GameManager.player.Name} won the battle!");
                break;

            case BattleFinish.EnemyWin:
                yield return GameUI.TypeOut($"{GameManager.player.Name} lost the battle!");
                GameManager.LostBattle();
                yield return new WaitWhile(() => true);
                break;

            case BattleFinish.EnemyRun:
            case BattleFinish.PlayerRun:
                GameManager.player.playerObject.SetInactive();
                break;
            case BattleFinish.None:
                throw new System.Exception("Battle finish may not be None");
        }
    }

    BattleUnit.TurnOptions GetEnemyTurnChoice(BattleUnit enemy)
    {
        EnemyAI ai = enemy.enemyAI;
        ai.CorrectData();
        int choiceInt = Random.Range(1, 101);

        int bar = ai.attack;
        if (choiceInt <= bar) return BattleUnit.TurnOptions.Attack;

        bar += ai.defend;
        if (choiceInt <= bar) return BattleUnit.TurnOptions.Defend;

        bar += ai.item;
        if (choiceInt <= bar)
        {
            if (enemy.items.Count == 0) return GetEnemyTurnChoice(enemy);
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

    void DisplayUnitsOnPanel()
    {
        foreach (Panel panel in playerPanels)
        {
            panel.DisplayUnit();
        }
    }

    bool CheckLoss(BattleUnit[] side)
    {
        foreach (BattleUnit battleUnit in side)
        {
            if (battleUnit.Alive) return false;
        }
        return true;
    }

    void ChooseUnitEnemy(BattleUnit[] units)
    {
        BattleUnit target = null;
        while (target == null || !target.Alive)
        {
            target = units[Random.Range(0, units.Length)];
        }

        selectedUnit = target;
    }

    string GetActionStatement(BattleUnit unit, string[] statements, string opposition)
    {
        string choice = statements[Random.Range(0, statements.Length)];
        choice = choice.Replace("himself", unit.refexive.ToString());
        if (opposition != null)
        {
            choice = choice.Replace("the enemy", opposition);
        }
        return choice;
    }

    int SkipToNextLivingUnit(BattleUnit[] units, int selected, int step)
    {
        selected += step;
        if (selected > units.Length - 1) selected = 0;
        else if (selected < 0) selected = units.Length - 1;

        if (!units[selected].Alive)
        {
            return SkipToNextLivingUnit(units, selected, step);
        }

        return selected;
    }
}
