using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] float playerSpread;
    [SerializeField] float panelSpread;
    [SerializeField] Transform playerBattleStation;
    [SerializeField] Transform enemyBattleStation;
    [SerializeField] GameObject panelPrefab;
    [SerializeField] Canvas canvas;

    [Header("Action Messages")]
    [SerializeField] string[] attackText;
    [SerializeField] string[] defendText;
    [SerializeField] string[] runText;

    [HideInInspector] public GameObject enemyGameObject;
    [HideInInspector] public BattleUnit[] players;
    [HideInInspector] public BattleUnit[] enemies;

    Panel[] playerPanels;
    BattleUnit selectedUnit = null;
    bool blinkUnit = false;

    BattleState battleState;
    BattleFinish battleFinish;

    string PlayerTitle { get => $"{Player.Name}{(players.Length == 1 ? "" : " and his companion" + (players.Length == 2 ? "" : "s"))}"; }
    string EnemyTitle { get => $"the {enemies[0].data.title}{(enemies.Length == 1 ? "" : enemies.Length == 2 ? " and his brother" : "'s mob")}"; }

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
        Run,
        None,
    }

    void Start()
    {
        // Instantiate player units
        playerPanels = new Panel[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            // Player sprite
            players[i].stationGO = new($"PlayerBattleUnit_{i}", new System.Type[] { typeof(SpriteRenderer) });
            players[i].stationGO.transform.parent = playerBattleStation;

            players[i].spriteRenderer = players[i].stationGO.GetComponent<SpriteRenderer>();
            players[i].spriteRenderer.sprite = players[i].sprite;
            players[i].spriteRenderer.sortingLayerName = "Battle";

            float playerMove;
            playerMove = (i * playerSpread) - (players.Length * playerSpread / 2);
            playerMove += playerSpread / 2;
            players[i].stationGO.transform.localPosition = Vector3.zero;
            players[i].stationGO.transform.Translate(playerMove, 0, 0);


            // Player panel
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

            enemies[i].spriteRenderer = enemies[i].stationGO.GetComponent<SpriteRenderer>();
            enemies[i].spriteRenderer.sprite = enemies[i].sprite;
            enemies[i].spriteRenderer.sortingLayerName = "Battle";

            float enemyMove;
            enemyMove = (i * playerSpread) - (enemies.Length * playerSpread / 2);
            enemyMove += playerSpread / 2;
            enemies[i].stationGO.transform.localPosition = Vector3.zero;
            enemies[i].stationGO.transform.Translate(enemyMove, 0, 0);

            enemies[i] = enemies[i].CopyAtStationGO();
            if (enemyNumbers.ContainsKey(enemies[i].data.title))
            {
                enemyNumbers[enemies[i].data.title]++;
                enemies[i].data.title += " " + enemyNumbers[enemies[i].data.title];

            }
            else enemyNumbers[enemies[i].data.title] = 1;
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
                selectedUnit.spriteRenderer.color = Mathf.Floor(Time.unscaledTime * 10) % 2 == 0 ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.3f);
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
            if (enemies.Length == 1) enemyGroupName = enemies[0].data.title;
            else if (enemies.Length == 2) enemyGroupName = $"{enemies[0].data.title} and his brother";

            // Select a or an
            else enemyGroupName = $"{enemies[0].data.title} and his cohorts";
            if ("aeiouAEIOU".Contains(enemyGroupName[0])) enemyGroupName = "an " + enemyGroupName;
            else enemyGroupName = "a " + enemyGroupName;

            // Display Message
            yield return GameUI.TypeOut($"{Player.Name} engages {enemyGroupName}.");
        }

        // Pick randomly who goes first
        if (Random.Range(0, 1) == 0) battleState = BattleState.PlayerTurn;
        else battleState = BattleState.EnemyTurn;

        // Clear extra data extra data
        foreach (BattleUnit unit in players) { unit.onDefense = false; }
        foreach (BattleUnit unit in enemies) { unit.onDefense = false; }

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
        Time.timeScale = 1;
        Destroy(gameObject);
    }
    IEnumerator EndBattle()
    {
        yield return new WaitForEndOfFrame();

        switch (battleFinish)
        {
            case BattleFinish.PlayerWin:
                Destroy(enemyGameObject);
                Player.PlayerController.SetInactive();
                yield return GameUI.TypeOut($"{PlayerTitle} won the battle!");

                int exp = enemies.Sum(x => x.data.expAward);
                yield return GameUI.TypeOut($"{PlayerTitle} gained {exp} experience.");

                int gold = enemies.Sum(x => x.data.goldAward);
                yield return GameUI.TypeOut($"{PlayerTitle} gained {gold} gold.");
                Player.Gold += gold;

                players[0].data.exp += exp;
                yield return players[0].LevelUpUnit();

                foreach (BattleUnit player in players)
                {
                    if (player.data.life < 1)
                        player.data.life = 1;
                }

                break;

            case BattleFinish.EnemyWin:
                yield return GameUI.TypeOut($"{PlayerTitle} lost the battle!");
                GameManager.LostBattle();
                yield return new WaitWhile(() => true);
                break;

            case BattleFinish.Run:
                Player.SetInactive();
                break;

            case BattleFinish.None:
                throw new System.Exception("Battle finish may not be None");
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
                    selected = SkipToNextLivingUnit(units, selected, 1);
                    break;

                case MyInput.Action.Right:
                case MyInput.Action.Down:
                    selected = SkipToNextLivingUnit(units, selected, -1);
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
    void ChooseUnitEnemy(BattleUnit[] units)
    {
        BattleUnit target = null;
        while (target == null || !target.data.Alive)
        {
            target = units[Random.Range(0, units.Length)];
        }

        selectedUnit = target;
    }

    IEnumerator ChangeLifeOnEnemyUnit(BattleUnit unit, int lifeChange)
    {
        if (lifeChange < 0)
        {
            for (int i = 0; i < 2; i++)
            {
                unit.spriteRenderer.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
                unit.spriteRenderer.color = Color.white;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        unit.data.life += lifeChange;
        if (unit.data.life >= unit.data.maxLife)
        {
            unit.data.life = unit.data.maxLife;
            yield return GameUI.TypeOut($"{unit.data.title}'s life is maxed out.");
        }
        if (!unit.data.Alive)
        {
            yield return GameUI.TypeOut($"{unit.data.title} has been destroyed.");

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
    IEnumerator ChangeLifeOnPlayerUnit(BattleUnit unit, int lifeChange)
    {
        if (lifeChange < 0)
        {
            for (int i = 0; i < 2; i++)
            {
                unit.spriteRenderer.color = Color.clear;
                yield return new WaitForSecondsRealtime(0.05f);
                unit.spriteRenderer.color = Color.white;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        unit.data.life += lifeChange;
        if (unit.data.life >= unit.data.maxLife)
        {
            unit.data.life = unit.data.maxLife;
            yield return GameUI.TypeOut($"{unit.data.title}'s life is maxed out.");
        }
        if (!unit.data.Alive)
        {
            yield return GameUI.TypeOut($"{unit.data.title} died.");
        }
    }

    IEnumerator PlayerUnitTurn(BattleUnit unit)
    {
        // Wait for end of frame
        yield return new WaitForEndOfFrame();

        // Get possible actions
        string[] possibleActions = GetPossibleActions(unit);

        // Get desired action
        yield return GameUI.ChoiceMenu(null, possibleActions.ToArray(), 1);
        System.Enum.TryParse(GameManager.Answer, out BattleUnit.TurnOptions choice);

        // Run choice
        switch (choice)
        {
            case BattleUnit.TurnOptions.Run:
                {
                    yield return GameUI.TypeOut(GetActionStatement(unit, runText, EnemyTitle));
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
                yield return GameUI.TypeOut(GetActionStatement(unit, defendText, EnemyTitle));
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
                            int itemCount = unit.data.itemOptionsForUnit.Count;
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
                    GameItems.DataSet itemData = GameItems.GetDataForOption(GameManager.Answer);

                    // Choose target
                    BattleUnit target;
                    if (itemData.scriptable.Type == ItemScriptable.ItemType.Attack)
                    {
                        yield return ChooseUnitPlayer(enemies);
                        if (selectedUnit == null)
                        {
                            yield return PlayerUnitTurn(unit);
                        }

                        target = selectedUnit;
                    }
                    else if (itemData.scriptable.Type == ItemScriptable.ItemType.Heal)
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

                    yield return Item(unit, target, itemData, ChangeLifeOnPlayerUnit, ChangeLifeOnEnemyUnit);
                    break;
                }

            case BattleUnit.TurnOptions.Magic:
                {
                    // Get desired magic
                    {
                        // Get magic options
                        string[] magicOptions = new string[unit.data.magicOptionsForUnit.Count];
                        for (int i = 0; i < magicOptions.Length; i++)
                        {
                            string stringName = unit.data.magicOptionsForUnit[i].ToString();
                            magicOptions[i] = stringName;
                            magicOptions[i] = magicOptions[i] + " : " + GameMagic.GetDataForOption(stringName).scriptable.Price;
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
                    GameMagic.DataSet magicData = GameMagic.GetDataForOption(GameManager.Answer.Split(" : ")[0]);

                    // Choose target
                    BattleUnit[] targets;
                    if (magicData.scriptable.Type == MagicScriptable.MagicType.Attack)
                    {
                        if (magicData.scriptable.EffectsAll)
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
                    else if (magicData.scriptable.Type == MagicScriptable.MagicType.Heal)
                    {
                        if (magicData.scriptable.EffectsAll)
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

                    yield return Magic(unit, targets, magicData, ChangeLifeOnPlayerUnit, ChangeLifeOnEnemyUnit);
                    break;
                }
        }
    }
    IEnumerator EnemyUnitTurn(BattleUnit unit)
    {
        // Wait for end of frame
        yield return new WaitForEndOfFrame();

        // Get possible actions
        string[] possibleActions = GetPossibleActions(unit);

        // Get desired action
        System.Enum.TryParse(possibleActions[Random.Range(0, possibleActions.Length)], out BattleUnit.TurnOptions choice);

        // Run choice
        switch (choice)
        {
            case BattleUnit.TurnOptions.Run:
                {
                    yield return GameUI.TypeOut(GetActionStatement(unit, runText, PlayerTitle));
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
                    yield return GameUI.TypeOut(GetActionStatement(unit, defendText, PlayerTitle));
                    break;
                }

            case BattleUnit.TurnOptions.Item:
                {
                    // Get item
                    string[] itemOptions = GetPossibleItems(unit);
                    GameItems.DataSet itemData = GameItems.GetDataForOption(itemOptions[Random.Range(0, itemOptions.Length)]);

                    // Choose target
                    BattleUnit target;
                    if (itemData.scriptable.Type == ItemScriptable.ItemType.Attack)
                    {
                        ChooseUnitEnemy(players);
                        target = selectedUnit;
                    }
                    else if (itemData.scriptable.Type == ItemScriptable.ItemType.Heal)
                    {
                        ChooseUnitEnemy(enemies);
                        target = selectedUnit;
                    }
                    else
                    {
                        throw new System.NotImplementedException();
                    }

                    yield return Item(unit, target, itemData, ChangeLifeOnEnemyUnit, ChangeLifeOnPlayerUnit);
                    break;
                }

            case BattleUnit.TurnOptions.Magic:
                {
                    // Get desired magic
                    string[] magicOptions = GetPossibleMagic(unit);
                    GameMagic.DataSet magicData = GameMagic.GetDataForOption(magicOptions[Random.Range(0, magicOptions.Length)]);

                    // Choose target
                    BattleUnit[] targets;
                    if (magicData.scriptable.Type == MagicScriptable.MagicType.Attack)
                    {
                        if (magicData.scriptable.EffectsAll) { targets = players; }
                        else
                        {
                            ChooseUnitEnemy(players);
                            targets = new BattleUnit[] { selectedUnit };
                        }
                    }
                    else if (magicData.scriptable.Type == MagicScriptable.MagicType.Heal)
                    {
                        if (magicData.scriptable.EffectsAll) { targets = enemies; }
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

                    yield return Magic(unit, targets, magicData, ChangeLifeOnEnemyUnit, ChangeLifeOnPlayerUnit);

                    break;
                }
        }
    }
    BattleUnit.TurnOptions GetEnemyTurnChoice(BattleUnit enemy)
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
            if (enemy.data.itemOptionsForUnit.Count == 0) return GetEnemyTurnChoice(enemy);
            return BattleUnit.TurnOptions.Item;
        }

        bar += ai.magic;
        if (choiceInt <= bar)
        {
            if (enemy.data.magicOptionsForUnit.Count == 0) return GetEnemyTurnChoice(enemy);
            return BattleUnit.TurnOptions.Magic;
        }

        bar += ai.run;
        if (choiceInt <= bar) return BattleUnit.TurnOptions.Run;

        throw new System.Exception("AI did not find correct turn option.");
    }

    IEnumerator Run(bool run)
    {
        if (run)
        {
            yield return GameUI.TypeOut("And got away!");
            battleState = BattleState.Exit;
            battleFinish = BattleFinish.Run;
        }
        else
        {
            yield return GameUI.TypeOut("But couldn't get away.");
        }
    }
    bool GetCanRun(BattleUnit enemyUnit)
    {
        bool run;
        if (enemyUnit.data.escapePercentageAllowed == 0) run = false;
        else run = Random.Range(0, 100 / enemyUnit.data.escapePercentageAllowed) == 0;
        return run;
    }

    IEnumerator Attack(BattleUnit attacking, BattleUnit defending, System.Func<BattleUnit, int, IEnumerator> lifeChangeFunc)
    {
        int attackPower = GetAttackPower(attacking, defending);
        yield return GameUI.TypeOut(GetActionStatement(attacking, attackText, defending.data.title));
        yield return GameUI.TypeOut($"{attackPower} damage to {defending.data.title}.");
        yield return lifeChangeFunc(defending, -attackPower);
    }
    int GetAttackPower(BattleUnit attacking, BattleUnit defending)
    {
        int preDefenseAttack = attacking.GetAttack();
        int attack = defending.GetDefenseChange(preDefenseAttack);
        return attack;
    }

    IEnumerator Item(BattleUnit unit, BattleUnit target, GameItems.DataSet itemData, System.Func<BattleUnit, int, IEnumerator> changeLifeOnFriendFunc, System.Func<BattleUnit, int, IEnumerator> changeLifeOnOpponentFunc)
    {
        // Preliminary message
        yield return GameUI.TypeOut($"{unit.data.title} tried using {itemData.Title}.");

        // Remove item from inventory
        unit.data.itemOptionsForUnit.Remove(itemData.identity);

        // Run Item
        yield return new WaitForEndOfFrame();
        if (itemData.scriptable.Type == ItemScriptable.ItemType.Attack)
        {

            int hit = target.GetDefenseChange(itemData.scriptable.Power);
            yield return GameUI.TypeOut($"{hit} damage to {target.data.title}.");
            yield return changeLifeOnOpponentFunc(target, -hit);
        }
        else if (itemData.scriptable.Type == ItemScriptable.ItemType.Heal)
        {
            int heal = itemData.scriptable.Power;
            yield return GameUI.TypeOut($"{target.data.title} gained {heal} HP.");
            yield return changeLifeOnFriendFunc(target, heal);
        }
    }
    string[] GetPossibleItems(BattleUnit unit)
    {
        string[] itemOptions = new string[unit.data.itemOptionsForUnit.Count];
        for (int i = 0; i < itemOptions.Length; i++)
        {
            itemOptions[i] = unit.data.itemOptionsForUnit[i].ToString();
        }
        return itemOptions;
    }

    IEnumerator Magic(BattleUnit unit, BattleUnit[] targets, GameMagic.DataSet magicData, System.Func<BattleUnit, int, IEnumerator> changeLifeOnFriendFunc, System.Func<BattleUnit, int, IEnumerator> changeLifeOnOpponentFunc)
    {
        // Preliminary message
        yield return GameUI.TypeOut($"{unit.data.title} tried {magicData.Title}.");

        // Handel tax
        if (unit.data.magic >= magicData.scriptable.Price)
        {
            unit.data.magic -= magicData.scriptable.Price;
        }
        else
        {
            yield return GameUI.TypeOut($"But didn't have enough magic.");
            yield break;
        }

        // Run magic
        yield return new WaitForEndOfFrame();
        if (magicData.scriptable.Type == MagicScriptable.MagicType.Attack)
        {
            foreach (BattleUnit target in targets)
            {
                int hit = target.GetDefenseChange(magicData.scriptable.Power);
                yield return GameUI.TypeOut($"{hit} damage to {target.data.title}.");
                yield return changeLifeOnOpponentFunc(target, -hit);
            }
        }
        else if (magicData.scriptable.Type == MagicScriptable.MagicType.Heal)
        {
            foreach (BattleUnit target in targets)
            {
                int heal = magicData.scriptable.Power;
                yield return GameUI.TypeOut($"{target.data.title} gained {heal} HP.");
                yield return changeLifeOnFriendFunc(target, heal);
            }
        }
    }
    string[] GetPossibleMagic(BattleUnit unit)
    {
        string[] magicOptions = new string[unit.data.magicOptionsForUnit.Count];
        for (int i = 0; i < magicOptions.Length; i++)
        {
            string stringName = unit.data.magicOptionsForUnit[i].ToString();
            magicOptions[i] = stringName;
        }
        return magicOptions;
    }

    string[] GetPossibleActions(BattleUnit unit)
    {
        List<string> possibleActions = new(System.Enum.GetNames(typeof(BattleUnit.TurnOptions)));

        if (unit.data.magicOptionsForUnit.Count == 0)
            possibleActions.Remove("Magic");

        if (unit.data.itemOptionsForUnit.Count == 0)
            possibleActions.Remove("Item");

        return possibleActions.ToArray();

    }

    int SkipToNextLivingUnit(BattleUnit[] units, int selected, int step)
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
    string GetActionStatement(BattleUnit unit, string[] statements, string opposition)
    {
        string reflexive = unit.data.sex switch
        {
            BattleUnit.UnitSex.Male => "himself",
            BattleUnit.UnitSex.Female => "herself",
            _ => throw new System.NotImplementedException(),
        };

        string choice = statements[Random.Range(0, statements.Length)];
        choice = choice.Replace("himself", reflexive).Replace("Player", unit.data.title).Replace("Enemy", opposition);
        return choice;
    }
    bool CheckLoss(BattleUnit[] side)
    {
        foreach (BattleUnit battleUnit in side)
        {
            if (battleUnit.data.Alive) return false;
        }
        return true;
    }
    void DisplayUnitsOnPanel()
    {
        foreach (Panel panel in playerPanels)
        {
            panel.DisplayUnit();
        }
    }
}
