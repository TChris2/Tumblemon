using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    private MoveDatabase moveDatabase;
    [SerializeField]
    // Used if a mon has run out power points on all of their moves
    private MoveInfo struggle;
    MonParty Trainer1Party;
    MonParty Trainer2Party;
    string winningTrainer = null;
    bool trainer1Loss = false;
    bool trainer2Loss = false;
    // Displays Action Text
    private TMPro.TMP_Text ActionText;

    // Start is called before the first frame update
    void Start()
    {
        moveDatabase = FindObjectOfType<MoveDatabase>();
        struggle = moveDatabase.GetMoveByName("Struggle");
        Trainer1Party = GameObject.Find("Trainer 1").GetComponent<MonParty>();
        Trainer2Party = GameObject.Find("Trainer 2").GetComponent<MonParty>();
        
        // Gets Text Boxes
        ActionText = GameObject.Find("Action Text").GetComponent<TMPro.TMP_Text>();
        // Trainer 1
        Trainer1Party.HealthBarText = GameObject.Find("Trainer 1 Health Bar Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.ThoughtText1 = GameObject.Find("Trainer 1 Thought Text 1").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.ThoughtText2 = GameObject.Find("Trainer 1 Thought Text 2").GetComponent<TMPro.TMP_Text>();
        // Trainer 2
        Trainer2Party.HealthBarText = GameObject.Find("Trainer 2 Health Bar Text").GetComponent<TMPro.TMP_Text>();
        Trainer2Party.ThoughtText1 = GameObject.Find("Trainer 2 Thought Text 1").GetComponent<TMPro.TMP_Text>();
        Trainer2Party.ThoughtText2 = GameObject.Find("Trainer 2 Thought Text 2").GetComponent<TMPro.TMP_Text>();

        StartCoroutine(TumblemonBattle());
    }

    void UpdateHealthBarText(MonParty Party1, MonParty Party2)
    {
        Party1.HealthBarText.text = $"{Party1.MonTeam[Party1.currentMon].name}: {Party1.MonTeam[Party1.currentMon].stats.health}";
        Party2.HealthBarText.text = $"{Party2.MonTeam[Party2.currentMon].name}: {Party2.MonTeam[Party2.currentMon].stats.health}";
    }

    void UpdateThoughtText(MonParty ActingParty, string newText)
    {
        string temp = ActingParty.HealthBarText.text;
        ActingParty.ThoughtText1.text = newText;
        ActingParty.ThoughtText2.text = temp;
    }
    
    private IEnumerator TumblemonBattle()
    {
        // Intializes vars
        var(AttackingParty, DefendingParty) = (Trainer1Party, Trainer1Party);

        yield return new WaitForSeconds(1);
        ActionText.text = "BATTLE BEGINS NOW";
        Debug.Log("BATTLE BEGINS NOW");
        UpdateHealthBarText(Trainer1Party, Trainer2Party);

        // Loops until on of the trainers runs out of mons
        while (true)
        {
            Debug.Log("NEW TURN");
            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(2f);

            // Trainer 1 action decision
            var (T1action, T1moveSelected, T1currentMon) = TrainerActionDecide(Trainer1Party, Trainer2Party);
            Trainer1Party.action = T1action;
            Trainer1Party.moveSelected = T1moveSelected;

            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(2f);

            // Trainer 2 action decision
            var (T2action, T2moveSelected, T2currentMon) = TrainerActionDecide(Trainer2Party, Trainer1Party);
            Trainer2Party.action = T2action;
            Trainer2Party.moveSelected = T2moveSelected;

            Debug.Log("-------------------------------------------------------------------------------------");
            yield return new WaitForSeconds(2f);

            // Swapping current mon
            if (Trainer1Party.action == "Swap")
            {
                Swap(Trainer1Party, T1currentMon);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(2f);
            }
            if (Trainer2Party.action == "Swap")
            {
                Swap(Trainer2Party, T2currentMon);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(2f);
            }
            
            // Healing current mon
            if (Trainer1Party.action == "Heal")
            {
                Heal(Trainer1Party);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(2f);
            }
            if (Trainer2Party.action == "Heal")
            {
                Heal(Trainer2Party);
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return new WaitForSeconds(2f);
            }

            // Trainer 1 goes first
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed > Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed)
            {
                yield return new WaitForSeconds(2f);

                Debug.Log($"{Trainer1Party.Trainer.name} GOES FIRST! {Trainer2Party.Trainer.name} GOES SECOND!");

                if (Trainer1Party.action == "Attack") 
                {
                    (AttackingParty, DefendingParty) = Attack(Trainer1Party, Trainer2Party, true);
                    Trainer1Party = AttackingParty;
                    Trainer2Party = DefendingParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(2f);
                }

                if (Trainer2Party.action == "Attack" && Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health >= 0) 
                {
                    (AttackingParty, DefendingParty) = Attack(Trainer2Party, Trainer1Party, false);
                    Trainer2Party = AttackingParty;
                    Trainer1Party = DefendingParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(2f);
                }
            }
            // Trainer 2 goes first
            else
            {
                yield return new WaitForSeconds(2f);

                Debug.Log($"{Trainer2Party.Trainer.name} GOES FIRST! {Trainer1Party.Trainer.name} GOES SECOND!");

                if (Trainer2Party.action == "Attack") 
                {
                    (AttackingParty, DefendingParty) = Attack(Trainer2Party, Trainer1Party, false);
                    Trainer2Party = AttackingParty;
                    Trainer1Party = DefendingParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(2f);
                }

                if (Trainer1Party.action == "Attack" && Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health >= 0) 
                {
                    (AttackingParty, DefendingParty) = Attack(Trainer1Party, Trainer2Party, true);
                    Trainer1Party = AttackingParty;
                    Trainer2Party = DefendingParty;
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return new WaitForSeconds(2f);
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
                Trainer1Party = FaintedMonSwap(Trainer1Party, Trainer2Party);
            if (Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health <= 0)
                Trainer2Party = FaintedMonSwap(Trainer2Party, Trainer1Party);
        }
        Debug.Log("-------------------------------------------------------------------------------------");
        yield return new WaitForSeconds(2f);
        Debug.Log("BATTLE OVER");
        ActionText.text = "BATTLE OVER";
        Debug.Log("-------------------------------------------------------------------------------------");
        yield return new WaitForSeconds(2f);
        Debug.Log($"{winningTrainer} Won!");
        ActionText.text = $"{winningTrainer} Won!";
    }

    // Checks to see if trainer has run of mons
    bool IsPartyDead(MonParty TrainerParty)
    {
        // Checks each mon on the trainer's team
        foreach (MonInfo mon in TrainerParty.MonTeam)
        {
            // Returns false if there is atleast one alive mon on their team
            if (mon.stats.health > 0)
                return false;
        }
        return true;
    }

    MonParty FaintedMonSwap(MonParty ActingParty, MonParty NonActingParty)
    {
        Debug.Log($"{ActingParty.Trainer.name}'s {ActingParty.MonTeam[ActingParty.currentMon].name} HAS FAINTED!");
        ActionText.text = $"{ActingParty.Trainer.name}'s {ActingParty.MonTeam[ActingParty.currentMon].name} HAS FAINTED!";

            float bestScore = float.NegativeInfinity;
            int newMon = 0;
            // Evaluates score for swaping
            for (int i = 0; i < ActingParty.MonTeam.Count; i++)
            {
                // Skips current & fainted mons
                if (ActingParty.MonTeam[i].stats.health <= 0) 
                    continue;

                float score = EvaluateSwap(ActingParty.MonTeam[i], NonActingParty.MonTeam[NonActingParty.currentMon]);
                // If swapping is currently the best action
                if (score > bestScore)
                {
                    bestScore = score;
                    newMon = i;
                }
            }
            ActingParty = Swap(ActingParty, newMon);

        return ActingParty;
    }

    // Decides Trainer action
    (string, MoveInfo, int) TrainerActionDecide(MonParty myParty, MonParty opponentParty)
    {
        Debug.Log($"{myParty.Trainer.name} IS DECIDING ...");
        UpdateThoughtText(myParty, $"{myParty.Trainer.name} IS DECIDING ...");

        MonInfo myMon = myParty.MonTeam[myParty.currentMon];
        MonInfo opponentMon = opponentParty.MonTeam[opponentParty.currentMon];

        float bestScore = float.NegativeInfinity;
        float aScore = float.NegativeInfinity;
        float hScore = float.NegativeInfinity;
        float sScore = float.NegativeInfinity;
        // Defaults action is to attack
        string bestAction = "Attack";
        MoveInfo bestMove = null;
        int bestSwapMon = -1;

        // Evaluates score for attacking
        foreach (var move in myMon.moves)
        {
            // Skips moves which are out of power points
            if (move.pp <= 0) 
                continue;

            float score = EvaluateAttack(myParty, opponentParty, myParty.currentMon, move);
            score += myParty.Trainer.attack_priority;
            // If attacking is currently the best action
            if (score > bestScore)
            {
                aScore = score;
                bestScore = score;
                bestAction = "Attack";
                bestMove = move;
            }
        }
        // If the mon ran out pp on all of its moves it does calculation for struggle
        if (bestScore == float.NegativeInfinity)
        {
            float score = EvaluateAttack(myParty, opponentParty, myParty.currentMon, struggle);
            score += myParty.Trainer.attack_priority/4;
            // If attacking is currently the best action
            if (score > bestScore)
            {
                aScore = score;
                bestScore = score;
                bestAction = "Attack";
                bestMove = struggle;
            }
        }

        Debug.Log($"{myParty.Trainer.name} Attack Score {aScore}");
        UpdateThoughtText(myParty, $"{myParty.Trainer.name} Attack Score {aScore}");

        // Evaluates score for healing
        if (myParty.potionCount > 0 && (float)myParty.MonTeam[myParty.currentMon].stats.health / 
            (float)myParty.MonTeam[myParty.currentMon].stats.total_health <= (float)myParty.Trainer.heal_threshold / 100)
        {
            float healScore = myParty.Trainer.heal_priority;
            hScore=healScore;

            // If mons health is critical / half its heal_threshold
            if ((float)myParty.MonTeam[myParty.currentMon].stats.health /
                (float)myParty.MonTeam[myParty.currentMon].stats.total_health <= (float)((float)myParty.Trainer.heal_threshold / 2) / 100)
                    healScore *= 2;

            // If healing is currently the best action
            if (healScore > bestScore)
            {
                bestScore = healScore;
                bestAction = "Heal";
            }
        }
        Debug.Log($"{myParty.Trainer.name} Heal Score {hScore}");
        UpdateThoughtText(myParty, $"{myParty.Trainer.name} Heal Score {hScore}");

        // Evaluates score for swaping
        for (int i = 0; i < myParty.MonTeam.Count; i++)
        {
            // Skips current & fainted mons
            if (i == myParty.currentMon || myParty.MonTeam[i].stats.health <= 0) 
                continue;

            float score = EvaluateSwap(myParty.MonTeam[i], opponentMon);
            score += myParty.Trainer.swap_priority;
            sScore = score;
            // If swapping is currently the best action
            if (score > bestScore)
            {
                bestScore = score;
                bestAction = "Swap";
                bestSwapMon = i;
            }
        }
        Debug.Log($"{myParty.Trainer.name} Swap Score {sScore}");
        UpdateThoughtText(myParty, $"{myParty.Trainer.name} Swap Score {sScore}");

        Debug.Log($"{myParty.Trainer.name} IS GOING TO {bestAction} WHEN ITS THEIR TURN");
        UpdateThoughtText(myParty, $"{myParty.Trainer.name} IS GOING TO {bestAction} WHEN ITS THEIR TURN");
        return (bestAction, bestMove, bestSwapMon);
    }

    private float EvaluateAttack(MonParty AttackingParty, MonParty DefendingParty, int mon, MoveInfo move)
    {   
        AttackingParty.moveSelected = move;
        AttackingParty.currentMon = mon;
        float damage = DamageCal(AttackingParty, DefendingParty);
        float score = damage;

        // Check if move could KO the opponent mon
        if (damage >= DefendingParty.MonTeam[DefendingParty.currentMon].stats.health)
        {
            // Applies double priority if the move can KO
            score += 2*AttackingParty.Trainer.attack_priority;
        }

        return score;
    }

    private float EvaluateSwap(MonInfo candidate, MonInfo opponent)
    {
        float defensiveScore = 1f;

        // Evaluate how well the candidate resists opponent’s moves
        foreach (var move in opponent.moves)
        {
            defensiveScore *= TypeCheck(move.type, candidate.type1);
            if (candidate.type2 != null)
                defensiveScore *= TypeCheck(move.type, candidate.type2);
        }

        // Defensive score: lower is better (less damage taken)
        defensiveScore = 1f / Mathf.Max(defensiveScore, 0.1f); // avoid division by zero

        float offensiveScore = 0f;

        // Now consider candidate’s attacking potential
        foreach (var move in candidate.moves)
        {
            offensiveScore += TypeCheck(move.type, opponent.type1);
            if (opponent.type2 != null)
                offensiveScore += TypeCheck(move.type, opponent.type2);
        }
        offensiveScore = offensiveScore / Mathf.Max(candidate.moves.Count, 1);

        // Final score = heavily weight defensive score, add offensive bonus
        return (defensiveScore * 2f) + offensiveScore;
    }

    // Enacts Trainer Attack
    (MonParty, MonParty) Attack(MonParty AttackingParty, MonParty DefendingParty, bool isTrainer1)
    {
        int trainerDmg = 0;

        Debug.Log($"{AttackingParty.Trainer.name}'s {AttackingParty.MonTeam[AttackingParty.currentMon].name} USES {AttackingParty.moveSelected.name}");
        ActionText.text = $"{AttackingParty.Trainer.name}'s {AttackingParty.MonTeam[AttackingParty.currentMon].name} USES {AttackingParty.moveSelected.name}";
        // Decreases the attacking mon move's power points
        AttackingParty.moveSelected.pp -= 1;

        // Gets damage info
        trainerDmg = DamageCal(AttackingParty, DefendingParty);
        // Deals damage to defending mon
        DefendingParty.MonTeam[DefendingParty.currentMon].stats.health -= trainerDmg;

        if (DefendingParty.MonTeam[DefendingParty.currentMon].stats.health < 0)
            DefendingParty.MonTeam[DefendingParty.currentMon].stats.health = 0;

        // Updates damage for counter moves
        if (isTrainer1)
        {
            AttackingParty.currentDmg = trainerDmg;
            UpdateHealthBarText(AttackingParty, DefendingParty);
        }
        else
        {
            DefendingParty.currentDmg = trainerDmg;
            UpdateHealthBarText(DefendingParty, AttackingParty);
        }

        if (AttackingParty.moveSelected.recoilType != "None")
        {
            Debug.Log($"{AttackingParty.Trainer.name}'s {AttackingParty.MonTeam[AttackingParty.currentMon].name} SUFFERED RECOIL DAMAGE");
            ActionText.text = $"{AttackingParty.Trainer.name}'s {AttackingParty.MonTeam[AttackingParty.currentMon].name} SUFFERED RECOIL DAMAGE";

            if (AttackingParty.moveSelected.recoilType != "Total Health")
            AttackingParty.MonTeam[AttackingParty.currentMon].stats.health -= (int)(
                AttackingParty.MonTeam[AttackingParty.currentMon].stats.total_health * AttackingParty.moveSelected.recoil);
        }

        UpdateHealthBarText(AttackingParty, DefendingParty);

        return (AttackingParty, DefendingParty);
    }

    // Heals Current Mon
    MonParty Heal(MonParty ActingParty)
    {
        Debug.Log($"{ActingParty.Trainer.name} Heals {ActingParty.MonTeam[ActingParty.currentMon].name}");
        ActionText.text = $"{ActingParty.Trainer.name} Heals {ActingParty.MonTeam[ActingParty.currentMon].name}";

        ActingParty.MonTeam[ActingParty.currentMon].stats.health = ActingParty.MonTeam[ActingParty.currentMon].stats.total_health;
        ActingParty.MonTeam[ActingParty.currentMon].status = "None";
        ActingParty.potionCount -= 1;

        return ActingParty;
    }

    // Swaps Current Mon
    MonParty Swap(MonParty ActingParty, int newMon)
    {
        ActionText.text = $"{ActingParty.Trainer.name} Swaps {ActingParty.MonTeam[ActingParty.currentMon].name} to {ActingParty.MonTeam[newMon].name}";
        Debug.Log($"{ActingParty.Trainer.name} Swaps {ActingParty.MonTeam[ActingParty.currentMon].name} to {ActingParty.MonTeam[newMon].name}"); 
        ActingParty.currentMon = newMon;        

        return ActingParty;
    }

    int DamageCal(MonParty AttackingParty, MonParty DefendingParty)
    {
        float attack = 0;
        float defense = 0;
        float stab = 0;
        float crit = 1;
        
        if (AttackingParty.moveSelected.attackType == "Physical")
        {
            attack = AttackingParty.MonTeam[AttackingParty.currentMon].stats.attack;
            defense = DefendingParty.MonTeam[DefendingParty.currentMon].stats.defense;
        }
        if (AttackingParty.moveSelected.attackType == "Special")
        {
            attack = AttackingParty.MonTeam[AttackingParty.currentMon].stats.special_attack;
            defense = DefendingParty.MonTeam[DefendingParty.currentMon].stats.special_defense;
        }

        // Checks for type effectiveness
        float type_modifier = TypeCheck(AttackingParty.moveSelected.type, DefendingParty.MonTeam[DefendingParty.currentMon].type1) *
            TypeCheck(AttackingParty.moveSelected.type, DefendingParty.MonTeam[DefendingParty.currentMon].type2);
        
        // Stab
        if (AttackingParty.moveSelected.type == AttackingParty.MonTeam[AttackingParty.currentMon].type1.name || 
            (AttackingParty.MonTeam[AttackingParty.currentMon].type2 != null &&
            AttackingParty.moveSelected.type == AttackingParty.MonTeam[AttackingParty.currentMon].type2.name))
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
            crit = ((2*AttackingParty.MonTeam[AttackingParty.currentMon].level) + 5) / 
                ((AttackingParty.MonTeam[AttackingParty.currentMon].level) + 5);
        }

        float damage = ( ((((2*AttackingParty.MonTeam[AttackingParty.currentMon].level)/5)+2) * AttackingParty.moveSelected.power
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
}
