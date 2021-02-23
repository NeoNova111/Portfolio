using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, PLAYERACTION, ENEMYTURN, SPEEDBATTLE, WON, LOST, PLAYERACTIONCHOICE}

public class BattleSystem : MonoBehaviour
{
    #region Singelton
    public string[] startMessage;
    public static BattleSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory instance");
            Destroy(gameObject);
            return;
        }

        instance = this;

        int enemySize = StaticInfo.enemyCurrentlyFightingRace.Length;
        if (enemySize > enemyStations.Length)
            enemySize = enemyStations.Length;
        enemies = new Unit[enemySize];
    }

    #endregion

    public float actionDelay;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerStation;
    public Transform[] enemyStations;

    public Text dialogueText;

    public BattleHUD playerBattleHUD;
    public BattleHUD enemyBattleHUD;
    public SlidersControll turnSliders;

    public BattleState battleState;

    bool playerWinsSpeedBattle;

    public Unit playerUnit;
    public Unit targetedEnemyUnit;
    public Unit[] enemies;
    private int activeEnemyUnitIdx;
    private int targetedEnemyIdx;

    #region Events

    public GameEvent playerTurn;
    public GameEvent PlayerDamaged;
    public GameEvent EnemyDamaged;
    public GameEvent Victory;
    public GameEvent Defeeat;
    public GameEvent HUDUpdated;

    #endregion

    #region Setup/Start

    void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(BattleSetup());
    }


    private void Update()
    {
        CheckForShadow();
    }

    IEnumerator BattleSetup()
    {
        GameObject player = Instantiate(playerPrefab, playerStation);
        playerUnit = player.GetComponent<Unit>();

        for (int i = 0; i < enemies.Length; i++) {
            if (StaticInfo.enemyCurrentlyFightingRace != null)
                LoadEnemy(enemyStations[i], i);
        }
        RandomTarget();
        turnSliders.CreateEnemySliders(enemies); //instantiates turnBars
        turnSliders.StartSpeed(playerUnit, enemies);
        dialogueText.text = startMessage[Random.Range(0, startMessage.Length)];

        yield return new WaitForSeconds(actionDelay);

        StartCoroutine(SpeedBattle());
    }

    void LoadEnemy(Transform station, int idx)
    {
        GameObject enemy = Instantiate(enemyPrefab, station);
        enemy.GetComponent<SpriteRenderer>().sprite = StaticInfo.enemyCurrentlyFightingRace[idx].apperance;
        enemies[idx] = enemy.GetComponent<Unit>();
        enemies[idx].race = StaticInfo.enemyCurrentlyFightingRace[idx];
        enemies[idx].level = StaticInfo.enemyCurrentlyFighting.enemyLevel;
        enemies[idx].job = StaticInfo.enemyCurrentlyFighting.enemyClass;
        enemies[idx].SetJobs();
        enemies[idx].maxHealth = StaticInfo.enemyCurrentlyFighting.enemyHealth;
        enemies[idx].health = StaticInfo.enemyCurrentlyFighting.enemyHealth;
        enemies[idx].armor = StaticInfo.enemyCurrentlyFighting.enemyArmor;
        enemies[idx].name = StaticInfo.enemyCurrentlyFightingRace[idx].name;

        //enemySpeed[idx] = enemies[idx].job.speed;

        if (targetedEnemyUnit == null)
            targetedEnemyUnit = enemies[idx];
    }

    #endregion

    void CheckForShadow()
    {
        foreach(Transform station in enemyStations)
        {
            if(station.childCount >= 2)
            {
                station.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                station.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public Unit UnitAtTurn()
    {
        if (battleState == BattleState.PLAYERACTION || battleState == BattleState.PLAYERTURN || battleState == BattleState.PLAYERACTIONCHOICE)
        {
            return playerUnit;
        }
        else if (battleState == BattleState.ENEMYTURN)
        {
            return enemies[activeEnemyUnitIdx];
        }
        return null;
    }

    public void SetPlayerUnit(Unit newUnit)
    {
        playerUnit = newUnit;
    }

    IEnumerator PlayerTurn()
    {
        battleState = BattleState.PLAYERTURN;
        //playerUnit.SetTurnPos(0);

        if (SkipTurnDueToStatus(playerUnit))
        {
            yield return new WaitForSeconds(actionDelay);
            StartCoroutine(EndTurn());
            yield break;
        }
        else if(playerUnit.AllAbilitiesOnCooldown())
        {
            dialogueText.text = "Out of options form now... ";
            yield return new WaitForSeconds(actionDelay);
            StartCoroutine(EndTurn());
            yield break;
        }

        playerTurn.Raise();
        yield return new WaitForSeconds(1f);
        battleState = BattleState.PLAYERACTIONCHOICE;
        dialogueText.text = "Choose an action:";
    }

    #region player Abilities

    public void OnAbilityOneClick()
    {
        if (battleState == BattleState.PLAYERACTIONCHOICE)
        {
            StartCoroutine(AbilityOne());
        }
    }

    IEnumerator AbilityOne()
    {
        battleState = BattleState.PLAYERACTION;
        TypeText(dialogueText, "Player uses " + playerUnit.job.abilityOne.name);
        yield return new WaitForSeconds(actionDelay);

        if (playerUnit.job.abilityOne.abilityType == AbilityType.ATTACK || playerUnit.job.abilityOne.abilityType == AbilityType.DEBUFF)
        {
            playerUnit.UseAbilityOne(targetedEnemyUnit);
            EnemyDamaged.Raise();
        }
        else
            playerUnit.UseAbilityOne(playerUnit);

        UpdateHud();
        StartCoroutine(EndTurn());
    }

    public void OnAbilityTwoClick()
    {
        if (battleState == BattleState.PLAYERACTIONCHOICE)
        {
            StartCoroutine(AbilityTwo());
        }
    }

    IEnumerator AbilityTwo()
    {
        battleState = BattleState.PLAYERACTION;
        TypeText(dialogueText, "Player uses " + playerUnit.job.abilityTwo.name);
        yield return new WaitForSeconds(actionDelay);

        if (playerUnit.job.abilityTwo.abilityType == AbilityType.ATTACK || playerUnit.job.abilityTwo.abilityType == AbilityType.DEBUFF)
        {
            playerUnit.UseAbilityTwo(targetedEnemyUnit);
            EnemyDamaged.Raise();
        }
        else
            playerUnit.UseAbilityTwo(playerUnit);

        UpdateHud();
        StartCoroutine(EndTurn());
    }

    #endregion

    bool SkipTurnDueToStatus(Unit unit)
    {
        if (unit.statusEffect != null)
        {
            if (unit.statusEffect.name == "Sleep")
            {
                if (Random.Range(0, 2) == 0)
                {
                    TypeText(dialogueText, unit.race.name + " woke up!");
                    unit.statusEffect = null;
                    UpdateHud();
                    return false;
                }
                else
                {
                    TypeText(dialogueText, unit.race.name + " is sound asleep...");
                    return true;
                }
            }
            else if (unit.statusEffect.name == "Stun")
            {
                TypeText(dialogueText, unit.race.name + " is stunned and can't act!");
                return true;
            }
        }
        return false;
    }

    bool EndOfTurnStatusEffect(Unit unit)
    {
        if (unit.statusEffect != null)
        {
            if (unit.statusEffect.name == "Poison")
            {
                unit.TakeDamage((int)(unit.maxHealth * unit.statusEffect.damagePercent));
                TypeText(dialogueText, unit.race.name + " is injured by the poison!");
                return true;
            }
        }
        return false;
    }

    IEnumerator StartEnemyTurn()
    {
        battleState = BattleState.ENEMYTURN;
        //enemies[activeEnemyUnitIdx].SetTurnPos(0);
        targetedEnemyUnit = enemies[activeEnemyUnitIdx];
        UpdateHud();

        if (SkipTurnDueToStatus(targetedEnemyUnit))
        {
            yield return new WaitForSeconds(actionDelay);
            StartCoroutine(EndTurn());
            yield break;
        }
        StartCoroutine(EnemyAction());
    }

    IEnumerator EnemyAction()
    {
        yield return new WaitForSeconds(actionDelay);
        dialogueText.text = "The enemy prepares to act";
        yield return new WaitForSeconds(actionDelay);

        if (targetedEnemyUnit.AllAbilitiesOnCooldown())
            TypeText(dialogueText, targetedEnemyUnit.name + " is out of options for now...");
        else
            EnemyDecideAction();

        UpdateHud();
        yield return new WaitForSeconds(actionDelay);
        StartCoroutine(EndTurn());
    }

    public void EnemyDecideAction()
    {
        if (Random.Range(0, 2) == 0 && targetedEnemyUnit.jobs[targetedEnemyUnit.currentJobIdx].cooldownOne == 0)
        {
            TypeText(dialogueText, targetedEnemyUnit.name + " uses " + targetedEnemyUnit.job.abilityOne.name);
            if (targetedEnemyUnit.job.abilityOne.abilityType == AbilityType.ATTACK || targetedEnemyUnit.job.abilityOne.abilityType == AbilityType.DEBUFF)
            {
                targetedEnemyUnit.UseAbilityOne(playerUnit);
                PlayerDamaged.Raise();
            }
            else
                targetedEnemyUnit.UseAbilityOne(targetedEnemyUnit);
        }
        else if(targetedEnemyUnit.jobs[targetedEnemyUnit.currentJobIdx].cooldownTwo == 0)
        {
            TypeText(dialogueText, targetedEnemyUnit.name + " uses " + targetedEnemyUnit.job.abilityTwo.name);
            if (targetedEnemyUnit.job.abilityTwo.abilityType == AbilityType.ATTACK || targetedEnemyUnit.job.abilityTwo.abilityType == AbilityType.DEBUFF)
            {
                targetedEnemyUnit.UseAbilityTwo(playerUnit);
                PlayerDamaged.Raise();
            }
            else
                targetedEnemyUnit.UseAbilityTwo(targetedEnemyUnit);
        }
    }

    //turn into coroutine
    IEnumerator EndTurn()
    {
        UnitAtTurn().SetTurnPos(0);

        if (EndOfTurnStatusEffect(UnitAtTurn()))
        {
            yield return new WaitForSeconds(actionDelay);
        }

        UnitAtTurn().DecreaseEffectDuration();
        UnitAtTurn().DecreaseCooldowns();

        UpdateHud();

        if (playerUnit.isDead())
        {
            battleState = BattleState.LOST;
            StartCoroutine(EndBattle());
            yield break;
        }

        int deadEnemies = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].isDead())
            {
                enemies[i].Die();
                turnSliders.DeactivateSlider(i); //ToDo: into die
                if(enemies[i] == targetedEnemyUnit)
                {
                    RandomTarget();
                }
                deadEnemies++;
            }
        }

        if(deadEnemies == enemies.Length)
        {
            battleState = BattleState.WON;
            StartCoroutine(EndBattle());
            yield break; ;
        }
        ;
        StartCoroutine(SpeedBattle());
    }

    IEnumerator SpeedBattle()
    {
        battleState = BattleState.SPEEDBATTLE;
        float delay;

        delay = actionDelay / 100;

        while (true)
        {
            bool breakOut = false;
            playerUnit.AddToTurnPos((float)playerUnit.job.speed / (10 / actionDelay) + Random.Range(-0.25f, 0.25f));
            playerBattleHUD.SetSpeedGauge(playerUnit.GetTurnPos());

            for (int i = 0; i < enemies.Length; i++)
            {
                if(!enemies[i].isDead())
                    enemies[i].AddToTurnPos((float)targetedEnemyUnit.job.speed / (10 / actionDelay) + Random.Range(-0.25f, 0.25f));

                enemyBattleHUD.SetSpeedGauge(enemies[i].GetTurnPos());
            }
            turnSliders.UpdateTurnBar(playerUnit, enemies);

            yield return new WaitForSeconds(delay);

            if (playerUnit.GetTurnPos() >= 100)
            {
                playerWinsSpeedBattle = true;
                breakOut = true;
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].GetTurnPos() >= 100)
                {
                    playerWinsSpeedBattle = false;
                    breakOut = true;
                    activeEnemyUnitIdx = i;
                }
            }

            if (breakOut)
                break;
        }

        if(playerWinsSpeedBattle)
        {
            StartCoroutine(PlayerTurn());
        }
        else
        {
            StartCoroutine(StartEnemyTurn());
        }
    }

    public void UpdateHud()
    {
        playerBattleHUD.SetHUD(playerUnit, playerUnit.GetTurnPos());
        if (targetedEnemyUnit != null)
        {
            enemyBattleHUD.SetHUD(targetedEnemyUnit, targetedEnemyUnit.GetTurnPos());
            turnSliders.UpdateTurnBar(playerUnit, enemies);

            if (targetedEnemyUnit.statusEffect != null)
            {
                enemyBattleHUD.SetStatusEffect(targetedEnemyUnit.statusEffect);
            }
            else
            {
                enemyBattleHUD.SetStatusEffect();
            }
        }

        if (playerUnit.statusEffect != null)
        {
            playerBattleHUD.SetStatusEffect(playerUnit.statusEffect);
        }
        else
        {
            playerBattleHUD.SetStatusEffect();
        }
        HUDUpdated.Raise();
    }

    IEnumerator EndBattle()
    {
        if (battleState == BattleState.WON)
        {
            Victory.Raise();
            StaticInfo.enemyDefeatStatus[StaticInfo.enemyCurrentlyFighting.enemyIdxInScene] = true;
            TypeText(dialogueText, "You showed em'");
        }
        else
        {
            Defeeat.Raise();
            TypeText(dialogueText, "Better luck next time...");
        }
        yield return new WaitForSeconds(actionDelay * 2);

        //reset muisik for overworld scene
        AudioManager.instance.TriggerDefaultSnapshot();
        AudioManager.instance.SetMusic(StaticInfo.previousMusic);
        AudioManager.instance.StartMusic();
        SceneManager.LoadScene(StaticInfo.currentOverworldSceneIdx);
    }

    void TypeText(Text textfieldToTypeIn, string textToType)
    {
        StartCoroutine(TypeTextCoroutine(textfieldToTypeIn, textToType));
    }

    IEnumerator TypeTextCoroutine(Text textfieldToTypeIn, string textToType)
    {
        textfieldToTypeIn.text = "";
        foreach (char character in textToType)
        {
            textfieldToTypeIn.text += character;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void TargetEnemy(Unit unit)
    {
        if(battleState == BattleState.PLAYERACTIONCHOICE)
        {
            targetedEnemyUnit = unit;
            UpdateHud();
        }
    }

    public void RandomTarget() //revistit when time
    {
        int deadEnemies = 0;
        foreach(Unit enemy in enemies)
        {
            if (enemy.isDead())
                deadEnemies++;
        }

        if(deadEnemies == enemies.Length)
        {
            targetedEnemyUnit = null;
            return;
        }
            
        while (true)
        {
            int rnd = Random.Range(0, enemies.Length);
            if (!enemies[rnd].isDead())
            {
                targetedEnemyIdx = rnd;
                break;
            }
        }
        targetedEnemyUnit = enemies[targetedEnemyIdx];
    }

    Unit FastestUnit()
    {
        Unit fast = playerUnit;
        foreach(Unit enemy in enemies)
        {
            if(enemy.job.speed > fast.job.speed && !enemy.isDead())
            {
                fast = enemy;
            }
        }
        return fast;
    }
}