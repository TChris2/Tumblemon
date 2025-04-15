using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    private MoveDatabase moveDatabase;
    private PartySelect partySelect;
    // Used if a mon has run out power points on all of their moves
    private MoveInfo struggle;
    public MonParty Trainer1Party;
    public MonParty Trainer2Party;
    string winningTrainer = null;
    bool trainer1Loss = false;
    bool trainer2Loss = false;
    // Displays Action Text
    TMPro.TMP_Text ActionText;
    [SerializeField]
    private float waitTime = 1;
    private ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect typewriter;
    private bool isTextDone = false;


    // Start is called before the first frame update
    void Start()
    {
        moveDatabase = FindObjectOfType<MoveDatabase>();
        partySelect = moveDatabase.GetComponent<PartySelect>();
        struggle = moveDatabase.GetMoveByName("Struggle");
        Trainer1Party = partySelect.Trainer1Party;
        Trainer2Party = partySelect.Trainer2Party;
        
        // Gets Text Boxes
        ActionText = GameObject.Find("Action Text").GetComponent<TMPro.TMP_Text>();
        typewriter = ActionText.GetComponent<ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect>();

        // Trainer 1
        Trainer1Party.HealthBar = GameObject.Find("Back Health Bar").GetComponent<Image>();
        Trainer1Party.NameText = GameObject.Find("Back Name Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.HealthText = GameObject.Find("Back Health Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.TotalHealthText = GameObject.Find("Back Total Health Text").GetComponent<TMPro.TMP_Text>();
        // Trainer 2
        Trainer2Party.HealthBar = GameObject.Find("Fore Health Bar").GetComponent<Image>();
        Trainer2Party.NameText = GameObject.Find("Fore Name Text").GetComponent<TMPro.TMP_Text>();

        StartCoroutine(TumblemonBattle());
    }
    

    private void OnTypewriterDone()
    {
        isTextDone = true;
    }

    void OnEnable()
    {
        ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect.CompleteTextRevealed += OnTypewriterDone;
    }

    void OnDisable()
    {
        ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect.CompleteTextRevealed -= OnTypewriterDone;
    }
    
    private IEnumerator TumblemonBattle()
    {
        yield return null;
        ActionText.text = $"aaaaaaaaaaaaaaa";
        yield return new WaitUntil(() => isTextDone);
        
        ActionText.text = $"I went to the beach today yes yes";
        yield return new WaitUntil(() => isTextDone);

        // Intializes vars
        var(AtkParty, DefParty) = (Trainer1Party, Trainer1Party);

        Trainer1Party = partySelect.Trainer1Party;
        Trainer2Party = partySelect.Trainer2Party;
        
        ActionText.text = $"{Trainer1Party.Trainer.name} is challenged by {Trainer2Party.Trainer.name}!";
        Debug.Log("BATTLE BEGINS NOW");

        // Loops until on of the trainers runs out of mons
        while (true)
        {
            Debug.Log("NEW TURN");
            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(waitTime);

            // Trainer 1 action decision
            var (T1action, T1moveSelected, T1currentMon) = TrainerActionDecide(Trainer1Party, Trainer2Party);
            Trainer1Party.action = T1action;
            Trainer1Party.moveSelected = T1moveSelected;

            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(waitTime);

            // Trainer 2 action decision
            var (T2action, T2moveSelected, T2currentMon) = TrainerActionDecide(Trainer2Party, Trainer1Party);
            Trainer2Party.action = T2action;
            Trainer2Party.moveSelected = T2moveSelected;

            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(waitTime);

            // Swapping current mon
            if (Trainer1Party.action == "Swap")
            {
                Swap(Trainer1Party, T1currentMon);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(waitTime);
            }
            if (Trainer2Party.action == "Swap")
            {
                Swap(Trainer2Party, T2currentMon);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(waitTime);
            }
            
            // Healing current mon
            if (Trainer1Party.action == "Heal")
            {
                Heal(Trainer1Party);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(waitTime);
            }
            if (Trainer2Party.action == "Heal")
            {
                Heal(Trainer2Party);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(waitTime);
            }

            // Trainer 1 goes first
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed > Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed)
            {
                yield return new WaitForSeconds(waitTime);

                Debug.Log($"{Trainer1Party.Trainer.name} GOES FIRST! {Trainer2Party.Trainer.name} GOES SECOND!");

                if (Trainer1Party.action == "Attack") 
                {
                    (AtkParty, DefParty) = Attack(Trainer1Party, Trainer2Party, true);
                    Trainer1Party = AtkParty;
                    Trainer2Party = DefParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(waitTime);
                }

                if (Trainer2Party.action == "Attack" && Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health > 0) 
                {
                    (AtkParty, DefParty) = Attack(Trainer2Party, Trainer1Party, false);
                    Trainer2Party = AtkParty;
                    Trainer1Party = DefParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(waitTime);
                }
            }
            // Trainer 2 goes first
            else
            {
                yield return new WaitForSeconds(waitTime);

                Debug.Log($"{Trainer2Party.Trainer.name} GOES FIRST! {Trainer1Party.Trainer.name} GOES SECOND!");

                if (Trainer2Party.action == "Attack") 
                {
                    (AtkParty, DefParty) = Attack(Trainer2Party, Trainer1Party, false);
                    Trainer2Party = AtkParty;
                    Trainer1Party = DefParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(waitTime);
                }

                if (Trainer1Party.action == "Attack" && Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health > 0) 
                {
                    (AtkParty, DefParty) = Attack(Trainer1Party, Trainer2Party, true);
                    Trainer1Party = AtkParty;
                    Trainer2Party = DefParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(waitTime);
                }
            }

            // Checks to see if the trainer has run out of mons after they both do their actions
            trainer1Loss = IsPartyDead(Trainer1Party);
            trainer2Loss = IsPartyDead(Trainer2Party);
            if (trainer1Loss || trainer2Loss)
            {
                if (trainer1Loss)
                    winningTrainer = Trainer2Party.Trainer.name;
                else    
                    winningTrainer = Trainer1Party.Trainer.name;
                break;
            }

            // If their current mon has fainted
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health <= 0)
                Trainer1Party.currentMon = FaintedMonSwap(Trainer1Party, Trainer2Party);
            if (Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health <= 0)
                Trainer2Party.currentMon = FaintedMonSwap(Trainer2Party, Trainer1Party);
            
        }
        Debug.Log("-------------------------------------------------------------------------------------");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("BATTLE OVER");
        ActionText.text = "BATTLE OVER";
        Debug.Log("-------------------------------------------------------------------------------------");
        yield return new WaitForSeconds(waitTime);
        Debug.Log($"{winningTrainer} WON!");
        ActionText.text = $"{winningTrainer} WON!";
    }

    // Checks to see if trainer has run of mons
    bool IsPartyDead(MonParty p) => p.MonTeam.TrueForAll(mon => mon.stats.health <= 0);

    // If a trainer's mon died in battle but has not run out of mons to swap to
    int FaintedMonSwap(MonParty ActingParty, MonParty OpParty)
    {
        Debug.Log($"{ActingParty.Trainer.name}'s {ActingParty.MonTeam[ActingParty.currentMon].name} HAS FAINTED!");
        ActionText.text = $"{ActingParty.Trainer.name}'s {ActingParty.MonTeam[ActingParty.currentMon].name} HAS FAINTED!";

        // Clones parties to simulate them
        MonParty simMy = ActingParty.Clone();
        MonParty simOpponent = OpParty.Clone();

        Minimax(simMy, simOpponent, 4, true, float.NegativeInfinity, float.PositiveInfinity, true,
                out string action, out MoveInfo move, out int swapIndex);

        ActionText.text = $"{ActingParty.Trainer.name} SWAPS {ActingParty.MonTeam[ActingParty.currentMon].name} TO {ActingParty.MonTeam[swapIndex].name}";
        Debug.Log($"{ActingParty.Trainer.name} SWAPS {ActingParty.MonTeam[ActingParty.currentMon].name} TO {ActingParty.MonTeam[swapIndex].name}"); 
        return swapIndex;
    }

    // Enacts Trainer Attack
    (MonParty, MonParty) Attack(MonParty AtkParty, MonParty DefParty, bool isTrainer1)
    {
        int trainerDmg = 0;

        Debug.Log($"{AtkParty.Trainer.name}'s {AtkParty.MonTeam[AtkParty.currentMon].name} USES {AtkParty.moveSelected.name}");
        ActionText.text = $"{AtkParty.Trainer.name}'s {AtkParty.MonTeam[AtkParty.currentMon].name} USES {AtkParty.moveSelected.name}";
        
        // Decreases the attacking mon move's power points
        AtkParty.moveSelected.pp--;

        // Gets damage info
        trainerDmg = DamageCal(AtkParty, DefParty);
        // Deals damage to defending mon
        DefParty.MonTeam[DefParty.currentMon].stats.health -= trainerDmg;

        if (DefParty.MonTeam[DefParty.currentMon].stats.health < 0)
            DefParty.MonTeam[DefParty.currentMon].stats.health = 0;

        // Updates damage for counter moves
        if (isTrainer1)
        {
            AtkParty.currentDmg = trainerDmg;
        }
        else
        {
            DefParty.currentDmg = trainerDmg;
        }

        if (AtkParty.moveSelected.recoilType != "None")
        {
            Debug.Log($"{AtkParty.Trainer.name}'s {AtkParty.MonTeam[AtkParty.currentMon].name} SUFFERED RECOIL DAMAGE");
            ActionText.text = $"{AtkParty.Trainer.name}'s {AtkParty.MonTeam[AtkParty.currentMon].name} SUFFERED RECOIL DAMAGE";

            if (AtkParty.moveSelected.recoilType != "Total Health")
            AtkParty.MonTeam[AtkParty.currentMon].stats.health -= (int)(
                AtkParty.MonTeam[AtkParty.currentMon].stats.total_health * AtkParty.moveSelected.recoil);
        }

        return (AtkParty, DefParty);
    }

    // Heals Current Mon
    MonParty Heal(MonParty ActingParty)
    {
        Debug.Log($"{ActingParty.Trainer.name} HEALS {ActingParty.MonTeam[ActingParty.currentMon].name}");
        ActionText.text = $"{ActingParty.Trainer.name} HEALS {ActingParty.MonTeam[ActingParty.currentMon].name}";

        ActingParty.MonTeam[ActingParty.currentMon].stats.health = ActingParty.MonTeam[ActingParty.currentMon].stats.total_health;
        ActingParty.MonTeam[ActingParty.currentMon].status = "None";
        ActingParty.potionCount--;

        return ActingParty;
    }

    // Swaps Current Mon
    MonParty Swap(MonParty ActingParty, int newMon)
    {
        ActionText.text = $"{ActingParty.Trainer.name} SWAPS {ActingParty.MonTeam[ActingParty.currentMon].name} TO {ActingParty.MonTeam[newMon].name}";
        Debug.Log($"{ActingParty.Trainer.name} SWAPS {ActingParty.MonTeam[ActingParty.currentMon].name} TO {ActingParty.MonTeam[newMon].name}"); 
        ActingParty.currentMon = newMon;        

        return ActingParty;
    }

    // Decides Trainer action
    (string, MoveInfo, int) TrainerActionDecide(MonParty myParty, MonParty opponentParty)
    {
        Debug.Log($"{myParty.Trainer.name} IS DECIDING ...");
        // Clones parties to simulate them
        MonParty simMy = myParty.Clone();
        MonParty simOpponent = opponentParty.Clone();

        Minimax(simMy, simOpponent, 4, true, float.NegativeInfinity, float.PositiveInfinity, false,
                out string action, out MoveInfo move, out int swapIndex);

        Debug.Log($"{myParty.Trainer.name} IS GOING TO {action}");
        return (action, move, swapIndex);
    }

    // Does the minimax
    float Minimax(MonParty my, MonParty opp, int depth, bool isMax, float alpha, float beta, bool isSwapping,
                  out string bestAction, out MoveInfo bestMove, out int bestSwap)
    {
        bestAction = "Attack"; bestMove = null; bestSwap = -1;
        if (depth == 0 || IsPartyDead(my) || IsPartyDead(opp))
            return EvaluateGameState(my, opp);

        float bestScore = isMax ? float.NegativeInfinity : float.PositiveInfinity;
        var actions = GetAllActions(my, opp, isSwapping);

        foreach (var (act, mv, idx) in actions)
        {
            var myClone = my.Clone();
            var oppClone = opp.Clone();
            ApplyAction(myClone, oppClone, act, mv, idx);
            float score = Minimax(oppClone, myClone, depth - 1, !isMax, alpha, beta, isSwapping, out _, out _, out _);

            if (act == "Attack")
            {
                if (mv.name == "Struggle")
                    score += 0.25f * my.Trainer.attack_priority;
                else
                {   
                    int predictedDamage = DamageCal(my, opp);
                    int targetHP = opp.MonTeam[opp.currentMon].stats.health;
                    if (predictedDamage >= targetHP || my.MonTeam[my.currentMon].stats.health >= 1){
                        Debug.Log($" Attack KO Score {score}");
                        score += 2 * my.Trainer.attack_priority;}
                    else
                    {
                        score += my.Trainer.attack_priority;
                        Debug.Log($" Attack Normal Score {score}");}
                }
            }
            else if (act == "Heal")
            {
                float healthRatio = my.MonTeam[my.currentMon].stats.health / (float)my.MonTeam[my.currentMon].stats.total_health;
                if (healthRatio <= my.Trainer.heal_threshold / 100f){
                    score += 2 * my.Trainer.heal_priority;
                    Debug.Log($" Heal Critical Score {score}");
                }
                else {
                    score += my.Trainer.heal_priority;
                    Debug.Log($" Heal Normal Score {score}");}
            }
            else if (act == "Swap")
            {
                score += my.Trainer.swap_priority;
                Debug.Log($" Swap Normal Score {score}");
            }

            if (isMax && score > bestScore)
            {
                bestScore = score; bestAction = act; bestMove = mv; bestSwap = idx;
                alpha = Mathf.Max(alpha, score);
            }
            else if (!isMax && score < bestScore)
            {
                bestScore = score;
                beta = Mathf.Min(beta, score);
            }
            if (beta <= alpha) break;
        }
        return bestScore;
    }

    // Gets all possible actions the trainer can do that turn
    List<(string, MoveInfo, int)> GetAllActions(MonParty party, MonParty opponent, bool isSwapping)
    {
        var actions = new List<(string, MoveInfo, int)>();
        var mon = party.MonTeam[party.currentMon];

        if (!isSwapping)
        {
            // Gets all moves that have power points
            foreach (var move in mon.moves)
                if (move.pp > 0) actions.Add(("Attack", move, -1));
            
            // If the current mon has 0 power points in any of their moves
            if (actions.Count == 0) actions.Add(("Attack", struggle, -1));

            // Adds heal action if the mon is at a certian threshold for healing
            if (party.potionCount > 0 && mon.stats.health / (float)mon.stats.total_health <= party.Trainer.heal_threshold / 100f)
                actions.Add(("Heal", null, -1));
        }

        // Gets all available mons which have not fainted yet
        for (int i = 0; i < party.MonTeam.Count; i++)
            if (i != party.currentMon && party.MonTeam[i].stats.health > 0)
                actions.Add(("Swap", null, i));

        return actions;
    }

    void ApplyAction(MonParty my, MonParty opp, string action, MoveInfo move, int swapIndex)
    {
        switch (action)
        {
            case "Attack":
                my.moveSelected = move;
                AttackSim(my, opp);
                break;
            case "Heal":
                var mon = my.MonTeam[my.currentMon];
                mon.stats.health = mon.stats.total_health;
                mon.status = "None";
                my.potionCount--;
                break;
            case "Swap":
                my.currentMon = swapIndex;
                break;
        }
    }

    void AttackSim(MonParty AtkParty, MonParty DefParty)
    {
        int dmg = DamageCal(AtkParty, DefParty);
        DefParty.MonTeam[DefParty.currentMon].stats.health -= dmg;
        if (DefParty.MonTeam[DefParty.currentMon].stats.health < 0)
            DefParty.MonTeam[DefParty.currentMon].stats.health = 0;
    }

    // Caculates damage dealt to defending party
    int DamageCal(MonParty AtkParty, MonParty DefParty) 
    {
        float attack = 0;
        float defense = 0;
        float stab = 0;
        float crit = 1;
        
        if (AtkParty.moveSelected.attackType == "Physical")
        {
            attack = AtkParty.MonTeam[AtkParty.currentMon].stats.attack;
            defense = DefParty.MonTeam[DefParty.currentMon].stats.defense;
        }
        if (AtkParty.moveSelected.attackType == "Special")
        {
            attack = AtkParty.MonTeam[AtkParty.currentMon].stats.special_attack;
            defense = DefParty.MonTeam[DefParty.currentMon].stats.special_defense;
        }

        // Checks for type effectiveness
        float type_modifier = TypeCheck(AtkParty.moveSelected.type, DefParty.MonTeam[DefParty.currentMon].type1) *
            TypeCheck(AtkParty.moveSelected.type, DefParty.MonTeam[DefParty.currentMon].type2);
        
        // Stab
        if (AtkParty.moveSelected.type == AtkParty.MonTeam[AtkParty.currentMon].type1.name || 
            (AtkParty.MonTeam[AtkParty.currentMon].type2 != null &&
            AtkParty.moveSelected.type == AtkParty.MonTeam[AtkParty.currentMon].type2.name))
        {
            stab = 1.5f;
        }
        else
        {
            stab = 1;
        }
       
        float random = UnityEngine.Random.Range(85f, 101f) / 100f;

        if (UnityEngine.Random.Range(1, 256) == 1)
        {
            crit = ((2*AtkParty.MonTeam[AtkParty.currentMon].level) + 5) / 
                ((AtkParty.MonTeam[AtkParty.currentMon].level) + 5);
        }

        float damage = ( ((((2*AtkParty.MonTeam[AtkParty.currentMon].level)/5)+2) * AtkParty.moveSelected.power
            * (attack/defense)) / (50) + 2) * crit * random * stab * type_modifier;

        return (int)damage;
    }

    // Checks type effectiveness for type modifier
    float TypeCheck(string moveType, TypeInfo targetType)
    {
        if (moveType == "Typeless")
            return 1f;
        // No 2nd type
        if (targetType == null)
            return 1f;
        // Super Effective
        if (targetType.effective.Contains(moveType))
            return 2f;
        // Not Very Effective
        if (targetType.weak.Contains(moveType))
            return 0.5f;
        // No Effect
        if (targetType.no_effect.Contains(moveType))
            return 0f;
        // Normal
        else
            return 1f;
    }

    float EvaluateGameState(MonParty myParty, MonParty opponentParty)
    {
        float myScore = 0;
        float opponentScore = 0;

        foreach (var mon in myParty.MonTeam)
            myScore += mon.stats.health;

        foreach (var mon in opponentParty.MonTeam)
            opponentScore += mon.stats.health;

        return myScore - opponentScore;
    }
}
