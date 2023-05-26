using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    public float playerSpread;
    public float panelSpread;

    public GameObject enemyGameObject;
    public BattleUnit[] players;
    public BattleUnit[] enemies;

    [SerializeField] Transform playerBattleStation;
    [SerializeField] Transform enemyBattleStation;
    [SerializeField] GameObject panelPrefab;
    [SerializeField] Canvas canvas;

    private Panel[] playerPanels;
    private BattleTurn battleState;
    private bool setBattleWinnerToRun = false;
    private BattleUnit selectedUnit = null;
    private bool blinkUnit = false;

    private string PlayerTitle { get => $"{GameManager.player.playerBattleUnit.title}{(players.Length == 1 ? "" : " and his companion" + (players.Length == 2 ? "" : "s"))}"; }
    private string EnemyTitle { get => $"the {enemies[0].title}{(enemies.Length == 1 ? "" : enemies.Length == 2 ? " and his brother" : "'s mob")}"; }

    enum BattleTurn
    {
        PlayerTurn,
        EnemyTurn,
    }
    enum BattleWinner
    {
        Player,
        Enemy,
        None,
        Run,
    }
    enum EnemySelectCammand
    {
        Left,
        Right,
        Select,
        Cancel,
        None,
    }

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

    void DisplayUnitsOnPanel()
    {
        foreach (Panel panel in playerPanels)
        {
            panel.DisplayUnit();
        }
    }

    BattleWinner GetBattleWinner()
    {
        if (setBattleWinnerToRun) return BattleWinner.Run;

        // Check if player is alive
        bool playerAlive = false;
        foreach (BattleUnit player in players)
        {
            if (player.Alive)
            {
                playerAlive = true;
                break;
            }
        }

        // Check if enemy is alive
        bool enemyAlive = false;
        foreach (BattleUnit enemy in enemies)
        {
            if (enemy.Alive)
            {
                enemyAlive = true;
                break;
            }
        }

        if (enemyAlive && !playerAlive) return BattleWinner.Enemy;
        else if (!enemyAlive && playerAlive) return BattleWinner.Player;
        return BattleWinner.None;

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
            yield return GameManager.TypeOut($"{GameManager.player.Name} engages {enemyGroupName}.");
        }

        // Pick who goes first
        if (Random.Range(0, 2) == 0) battleState = BattleTurn.PlayerTurn;
        else battleState = BattleTurn.EnemyTurn;

        // clear extra data
        foreach (BattleUnit unit in players) { unit.onDefence = false; }
        foreach (BattleUnit unit in enemies) { unit.onDefence = false; }

        // Battle Loop
        while (GetBattleWinner() == BattleWinner.None)
        {
            // Turn start
            yield return new WaitForEndOfFrame();
            if (GetBattleWinner() != BattleWinner.None)
            {
                break;
            }

            // Handel player turn
            if (battleState == BattleTurn.PlayerTurn)
            {
                foreach (BattleUnit player in players)
                {
                    player.onDefence = false;
                    if (GetBattleWinner() == BattleWinner.None)
                    {
                        if (player.Alive)
                        {
                            yield return PlayerUnitTurn(player);
                        }
                    }
                }
                battleState = BattleTurn.EnemyTurn;
                GetBattleWinner();
                continue;
            }

            // Handel enemy turn
            if (battleState == BattleTurn.EnemyTurn)
            {
                foreach (BattleUnit enemy in enemies)
                {
                    enemy.onDefence = false;
                    if (GetBattleWinner() == BattleWinner.None)
                    {
                        if (enemy.Alive)
                        {
                            yield return EnemyUnitTurn(enemy);
                        }
                    }
                    GetBattleWinner();
                }
                GetBattleWinner();
                battleState = BattleTurn.PlayerTurn;
                continue;
            }
        }

        yield return EndBattle();
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    IEnumerator PlayerUnitTurn(BattleUnit player)
    {
        // Get desired action
        List<string> possibleActions = new(System.Enum.GetNames(typeof(BattleUnit.TurnOptions)));
        if (player.magicOptionsForUnit.Count == 0) possibleActions.Remove("Magic");
        if (player.items.Count == 0) possibleActions.Remove("Item");

        yield return GameManager.GetChoice(null, possibleActions.ToArray());
        System.Enum.TryParse(GameManager.Answer, out BattleUnit.TurnOptions choice);

        // Run choice
        if (choice == BattleUnit.TurnOptions.Run)
        {
            yield return GameManager.TypeOut($"{player.title} attempts to {GetActionStatement(player, runText, EnemyTitle)}.");

            bool run;
            if (enemies[0].escapePercentageAllowed == 0) run = false;
            else run = Random.Range(0, 100 / enemies[0].escapePercentageAllowed) == 0;

            if (run)
            {
                yield return GameManager.TypeOut("And got away!");
                setBattleWinnerToRun = true;
            }
            else yield return GameManager.TypeOut("But couldn't get away.");

        }
        else if (choice == BattleUnit.TurnOptions.Magic)
        {
            // Get desired magic choice
            string[] magicOptions = new string[player.magicOptionsForUnit.Count];
            for (int i = 0; i < magicOptions.Length; i++)
            {
                magicOptions[i] = player.magicOptionsForUnit[i].ToString();
                magicOptions[i] = magicOptions[i] + " : " + new Magic.Data(player.magicOptionsForUnit[i]).price;
            }
            yield return GameManager.GetChoice(null, magicOptions, alowCancel: true);

            // Handel cancel
            if (GameManager.Answer == "CANCEL")
            {
                yield return new WaitForEndOfFrame();
                yield return PlayerUnitTurn(player);
                yield break;
            }

            // Get data for magic choice
            Magic.Data data = Magic.GetDataForOption(GameManager.Answer.Split(" : ")[0]);

            // Choose attack target
            if (data.Attack)
            {
                if (!data.applyAll)
                {
                    yield return ChooseUnitPlayer(enemies);
                    if (selectedUnit == null)
                    {
                        yield return PlayerUnitTurn(player);
                    }
                }
            }

            // Choose heal target
            if (data.Heal)
            {
                if (!data.applyAll)
                {
                    yield return ChooseUnitPlayer(players);
                    if (selectedUnit == null)
                    {
                        yield return PlayerUnitTurn(player);
                    }
                }
            }

            // Preliminary text
            yield return GameManager.TypeOut($"{player.title} tried {data.title}.", close: false);
            if (data.HasEnoughMagic(player)) player.magic -= data.price;
            else
            {
                yield return GameManager.TypeOut($"But didn't have enough magic.");
                yield break;
            }

            // Run magic
            if (data.Attack)
            {
                IEnumerator AttackUnit(BattleUnit target)
                {
                    yield return new WaitForEndOfFrame();

                    int hit = target.badges.GetDefenseChange(data.attackPower, target.onDefence);
                    yield return GameManager.TypeOut($"{hit} damage to {target.title}.", close: false);
                    yield return ChangeLifeOnEnemyUnit(target, -hit);
                }

                if (data.applyAll)
                {
                    foreach (BattleUnit target in enemies)
                    {
                        if (target.Alive) yield return AttackUnit(target);
                    }
                }
                else { yield return AttackUnit(selectedUnit); }

                GameManager.CloseUI();
            }
            if (data.Heal)
            {
                IEnumerator HealUnit(BattleUnit target)
                {
                    yield return new WaitForEndOfFrame();
                    yield return GameManager.TypeOut($"{target.title} gained {data.healingPower} HP.", close: false);
                    yield return ChangeLifeOnPlayerUnit(target, data.healingPower);
                }

                if (data.applyAll)
                {
                    foreach (BattleUnit target in players)
                    {
                        if (target.Alive) yield return HealUnit(target);
                    }
                }
                else yield return HealUnit(selectedUnit);

                GameManager.CloseUI();
            }

        }
        else if (choice == BattleUnit.TurnOptions.Attack)
        {
            yield return ChooseUnitPlayer(enemies);
            if (selectedUnit == null)
            {
                yield return PlayerUnitTurn(player);
                yield break;
            }

            int attack = player.badges.GetAttack();
            int defense = selectedUnit.badges.GetDefenseChange(attack, selectedUnit.onDefence);

            yield return GameManager.TypeOut($"{player.title} {GetActionStatement(player, attackText, selectedUnit.title)}!");
            yield return GameManager.TypeOut($"{defense} damage to {selectedUnit.title}.");
            yield return ChangeLifeOnEnemyUnit(selectedUnit, -defense);
        }
        else if (choice == BattleUnit.TurnOptions.Defend)
        {
            player.onDefence = true;
            yield return GameManager.TypeOut($"{player.title} {GetActionStatement(player, defendText, EnemyTitle)}.");
        }
        else if (choice == BattleUnit.TurnOptions.Item)
        {
            // Get desired magic choice
            string[] itemOptions = new string[player.items.Count];
            for (int i = 0; i < itemOptions.Length; i++)
            {
                itemOptions[i] = player.items[i].ToString();
            }
            yield return GameManager.GetChoice(null, itemOptions, alowCancel: true);
            if (GameManager.Answer == "CANCEL")
            {
                yield return new WaitForEndOfFrame();
                yield return PlayerUnitTurn(player);
                yield break;
            }

            // Get data for magic choice
            Items.Data data = Items.GetDataForOption(GameManager.Answer);

            // Choose attack target
            if (data.Attack)
            {

                yield return ChooseUnitPlayer(enemies);
                if (selectedUnit == null)
                {
                    yield return PlayerUnitTurn(player);
                }

            }

            // Choose heal target
            if (data.Heal)
            {
                yield return ChooseUnitPlayer(players);
                if (selectedUnit == null)
                {
                    yield return PlayerUnitTurn(player);
                }
            }

            // Preliminary text
            yield return GameManager.TypeOut($"{player.title} tried using {data.title}.", close: false);
            player.items.Remove(data.type);

            // Run Item
            if (data.Attack)
            {
                yield return new WaitForEndOfFrame();

                int hit = selectedUnit.badges.GetDefenseChange(data.attackPower, selectedUnit.onDefence);
                yield return GameManager.TypeOut($"{hit} damage to {selectedUnit.title}.");
                yield return ChangeLifeOnEnemyUnit(selectedUnit, -hit);
            }
            if (data.Heal)
            {
                yield return new WaitForEndOfFrame();
                yield return GameManager.TypeOut($"{selectedUnit.title} gained {data.healingPower} HP.");
                yield return ChangeLifeOnPlayerUnit(selectedUnit, data.healingPower);
            }

        }
    }

    IEnumerator ChooseUnitPlayer(BattleUnit[] units)
    {
        // Set up select
        selectedUnit = null;
        yield return new WaitForEndOfFrame();
        bool multipleAlive = false;
        int selected = -1;
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

        bool exitLoop = false;
        while (!exitLoop)
        {
            EnemySelectCammand cammand = EnemySelectCammand.None;
            IEnumerator waitForClick()
            {
                EnemySelectCammand getInput()
                {
                    int direction = (int)(MyInput.GetMoveHorizontal() - MyInput.GetMoveVertical());
                    int select = MyInput.GetSelect();

                    if (select == 1) return EnemySelectCammand.Select;
                    if (select == -1) return EnemySelectCammand.Cancel;
                    if (direction < 0) return EnemySelectCammand.Left;
                    if (direction > 0) return EnemySelectCammand.Right;
                    else return EnemySelectCammand.None;
                }

                yield return new WaitUntil(() => getInput() == EnemySelectCammand.None);
                yield return new WaitWhile(() => getInput() == EnemySelectCammand.None);
                cammand = getInput();
                yield return new WaitUntil(() => getInput() == EnemySelectCammand.None);
            }
            yield return waitForClick();

            int jumpSelected(int currentSelected, int step)
            {
                int newSelected = currentSelected;
                newSelected += step;
                if (newSelected < 0) newSelected = units.Length - 1;
                if (newSelected >= units.Length) newSelected = 0;
                if (!units[newSelected].Alive) return jumpSelected(newSelected, step);
                return newSelected;
            }
            switch (cammand)
            {
                case EnemySelectCammand.Left:
                    selected = jumpSelected(selected, -1);
                    break;
                case EnemySelectCammand.Right:
                    selected = jumpSelected(selected, 1);
                    if (selected < 0) selected = 0;
                    break;
                case EnemySelectCammand.Select:
                    exitLoop = true;
                    break;
                case EnemySelectCammand.Cancel:
                    selectedUnit.spr.color = Color.white;
                    selectedUnit = null;
                    exitLoop = true;
                    break;
            }

            if (cammand != EnemySelectCammand.Cancel)
            {
                selectedUnit.spr.color = Color.white;
                selectedUnit = units[selected];
            }
        }

        blinkUnit = false;
    }

    IEnumerator EnemyUnitTurn(BattleUnit enemy)
    {
        yield return new WaitForEndOfFrame();
        BattleUnit.TurnOptions turnChoice = GetEnemyTurnChoice(enemy);

        if (turnChoice == BattleUnit.TurnOptions.Run)
        {
            yield return GameManager.TypeOut($"{enemy.title} tried to {GetActionStatement(enemy, runText, PlayerTitle)}.");

            bool run;
            if (players[0].escapePercentageAllowed == 0) run = false;
            else run = Random.Range(0, 100 / players[0].escapePercentageAllowed) == 0;

            if (run)
            {
                yield return GameManager.TypeOut("And got away!");
                setBattleWinnerToRun = true;
            }
            else yield return GameManager.TypeOut("But couldn't get away.");
        }
        else if (turnChoice == BattleUnit.TurnOptions.Magic)
        {
            // Get desired magic choice
            string choice = enemy.magicOptionsForUnit[Random.Range(0, enemy.magicOptionsForUnit.Count)].ToString();
            Magic.Data data = Magic.GetDataForOption(choice);

            // Choose attack target
            if (data.Attack)
            {
                if (!data.applyAll)
                {
                    ChooseUnitEnemy(players);
                }
            }

            // Choose heal target
            if (data.Heal)
            {
                if (!data.applyAll)
                {
                    ChooseUnitEnemy(enemies);
                }
            }

            // Preliminary text
            yield return GameManager.TypeOut($"{enemy.title} tried {data.title}.", close: false);
            if (data.HasEnoughMagic(enemy)) enemy.magic -= data.price;
            else
            {
                yield return GameManager.TypeOut($"But didn't have enough magic.");
                yield break;
            }

            // Run magic
            if (data.Attack)
            {
                IEnumerator AttackUnit(BattleUnit target)
                {
                    yield return new WaitForEndOfFrame();

                    int hit = target.badges.GetDefenseChange(data.attackPower, target.onDefence);
                    yield return GameManager.TypeOut($"{hit} damage to {target.title}.", close: false);
                    yield return ChangeLifeOnPlayerUnit(target, -hit);
                }

                if (data.applyAll)
                {
                    foreach (BattleUnit target in players)
                    {
                        if (target.Alive) yield return AttackUnit(target);
                    }
                }
                else { yield return AttackUnit(selectedUnit); }

                GameManager.CloseUI();
            }
            if (data.Heal)
            {
                IEnumerator HealUnit(BattleUnit target)
                {
                    yield return new WaitForEndOfFrame();
                    yield return GameManager.TypeOut($"{target.title} gained {data.healingPower} HP.", close: false);
                    yield return ChangeLifeOnPlayerUnit(target, data.healingPower);
                }

                if (data.applyAll)
                {
                    foreach (BattleUnit target in enemies)
                    {
                        if (target.Alive) yield return HealUnit(target);
                    }
                }
                else yield return HealUnit(selectedUnit);

                GameManager.CloseUI();
            }

        }
        else if (turnChoice == BattleUnit.TurnOptions.Attack)
        {
            ChooseUnitEnemy(players);

            int attack = enemy.badges.GetAttack();
            int defense = selectedUnit.badges.GetDefenseChange(attack, selectedUnit.onDefence);

            yield return GameManager.TypeOut($"{enemy.title} {GetActionStatement(enemy, attackText, selectedUnit.title)}!");
            yield return GameManager.TypeOut($"{defense} damage to {selectedUnit.title}.");
            yield return ChangeLifeOnPlayerUnit(selectedUnit, -defense);
        }
        else if (turnChoice == BattleUnit.TurnOptions.Defend)
        {
            enemy.onDefence = true;
            yield return GameManager.TypeOut($"{enemy.title} {GetActionStatement(enemy, defendText, PlayerTitle)}.");
        }
        else if (turnChoice == BattleUnit.TurnOptions.Item)
        {
            // Get desired magic choice
            string choice = enemy.items[Random.Range(0, enemy.items.Count)].ToString();
            Items.Data data = Items.GetDataForOption(choice);

            // Choose attack target
            if (data.Attack)
            {

                ChooseUnitEnemy(players);

            }

            // Choose heal target
            if (data.Heal)
            {
                ChooseUnitEnemy(enemies);
            }

            // Preliminary text
            yield return GameManager.TypeOut($"{enemy.title} tried using {data.title}.", close: false);
            enemy.items.Remove(data.type);

            // Run Item
            if (data.Attack)
            {
                yield return new WaitForEndOfFrame();

                int hit = selectedUnit.badges.GetDefenseChange(data.attackPower, selectedUnit.onDefence);
                yield return GameManager.TypeOut($"{hit} damage to {selectedUnit.title}.");
                yield return ChangeLifeOnPlayerUnit(selectedUnit, -hit);
            }
            if (data.Heal)
            {
                yield return new WaitForEndOfFrame();
                yield return GameManager.TypeOut($"{selectedUnit.title} gained {data.healingPower} HP.");
                yield return ChangeLifeOnPlayerUnit(selectedUnit, data.healingPower);
            }

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
            yield return GameManager.TypeOut($"{unit.title}'s life is maxed out.");
        }
        if (!unit.Alive)
        {
            yield return GameManager.TypeOut($"{unit.title} has been destroyed.");

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
            yield return GameManager.TypeOut($"{unit.title}'s life is maxed out.");
        }
        if (!unit.Alive)
        {
            yield return GameManager.TypeOut($"{unit.title} died.");
        }
    }

    IEnumerator EndBattle()
    {
        yield return new WaitForEndOfFrame();

        switch (GetBattleWinner())
        {
            case BattleWinner.Player:
                Destroy(enemyGameObject);
                GameManager.player.playerObject.SetInactive();
                yield return GameManager.TypeOut($"{GameManager.player.Name} won the battle!");
                break;

            case BattleWinner.Enemy:
                yield return GameManager.TypeOut($"{GameManager.player.Name} lost the battle!");
                GameManager.LostBattle();
                yield return new WaitWhile(() => true);
                break;

            case BattleWinner.Run:
                GameManager.player.playerObject.SetInactive();
                break;
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
}
