using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBattle : MonoBehaviour
{
    MonParty Trainer1Party;
    MonParty Trainer2Party;
    string winningTrainer = null;
    bool trainer1Loss = false;
    bool trainer2Loss = false;

    public TMPro.TMP_Text Mon1Text;
    public TMPro.TMP_Text Mon2Text;

    // Start is called before the first frame update
    void Start()
    {
        Trainer1Party = GameObject.Find("Trainer 1").GetComponent<MonParty>();
        Trainer2Party = GameObject.Find("Trainer 2").GetComponent<MonParty>();

        StartCoroutine(TumblemonBattle());
    }

    void UpdateText()
    {
        Mon1Text.text = $"{Trainer1Party.MonTeam[Trainer1Party.currentMon].name}: {Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health}";
        Mon2Text.text = $"{Trainer2Party.MonTeam[Trainer2Party.currentMon].name}: {Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health}";
    }

    private IEnumerator TumblemonBattle()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("BATTLE BEGINS NOW");
        Mon1Text.text = $"{Trainer1Party.MonTeam[Trainer1Party.currentMon].name}: {Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health}";
        Mon2Text.text = $"{Trainer2Party.MonTeam[Trainer2Party.currentMon].name}: {Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health}";

        // Loops until on of the trainers runs out of mons
        while (true)
        {
            yield return new WaitForSeconds(2f);

            // Trainer 1 action decision
            var (T1action, T1moveSelected, T1currentMon) = TrainerActionDecide(Trainer1Party, Trainer2Party);
            Trainer1Party.action = T1action;
            Trainer1Party.moveSelected = T1moveSelected;

            // Trainer 2 action decision
            var (T2action, T2moveSelected, T2currentMon) = TrainerActionDecide(Trainer2Party, Trainer1Party);
            Trainer2Party.action = T2action;
            Trainer2Party.moveSelected = T2moveSelected;

            // Intializes var
            var(AttackingParty, DefendingParty) = (Trainer1Party, Trainer1Party);

            // ADD HEAL & SWAP LOGIC HERE AS THOSE OCCUR BEFORE EITHER CAN ATTACK
            
            // Trainer 1 goes first
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed > Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed)
            {
                Debug.Log($"{Trainer1Party.Trainer.name} GOES FIRST!");
                
                if (Trainer1Party.action == "Attack")
                {
                    (AttackingParty, DefendingParty) = TrainerAttack(Trainer1Party, Trainer2Party);
                    Trainer1Party = AttackingParty;
                    Trainer2Party = DefendingParty;
                }
                UpdateText();
                if (Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health <= 0)
                {
                    Debug.Log($"{Trainer2Party.MonTeam[Trainer2Party.currentMon].name} HAS FAINTED!");

                    // Checks to see if trainer has run of mons
                    trainer2Loss = IsPartyDead(Trainer2Party);
                    if (trainer2Loss)
                    {
                        winningTrainer = Trainer1Party.Trainer.name;
                        break;
                    }
                    // ADD LOGIC HERE FOR SWAPPING IF MON IS DEAD
                }
                else if (Trainer2Party.action == "Attack")
                {
                    Debug.Log($"{Trainer2Party.Trainer.name} GOES SECOND!");
                    (AttackingParty, DefendingParty) = TrainerAttack(Trainer2Party, Trainer1Party);
                    Trainer2Party = AttackingParty;
                    Trainer1Party = DefendingParty;
                }
                UpdateText();
            }
            // Trainer 2 goes first
            else
            {
                Debug.Log($"{Trainer2Party.Trainer.name} GOES FIRST!");

                if (Trainer2Party.action == "Attack")
                {
                    (AttackingParty, DefendingParty) = TrainerAttack(Trainer2Party, Trainer1Party);
                    Trainer2Party = AttackingParty;
                    Trainer1Party = DefendingParty;
                }
                UpdateText();
                if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health <= 0)
                {
                    Debug.Log($"{Trainer1Party.MonTeam[Trainer1Party.currentMon].name} HAS FAINTED!");

                    // Checks to see if trainer has run of mons
                    trainer1Loss = IsPartyDead(Trainer1Party);
                    if (trainer1Loss)
                    {
                        winningTrainer = Trainer2Party.Trainer.name;
                        break;
                    }
                    // ADD LOGIC HERE FOR SWAPPING IF MON IS DEAD
                }
                else if (Trainer1Party.action == "Attack")
                {
                    Debug.Log($"{Trainer1Party.Trainer.name} GOES SECOND!");
                    (AttackingParty, DefendingParty) = TrainerAttack(Trainer1Party, Trainer2Party);
                    Trainer1Party = AttackingParty;
                    Trainer2Party = DefendingParty;
                }
                UpdateText();
            }

            // 2nd check to see if the trainer has run out of mons after they both do their actions
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
        }

        Debug.Log("BATTLE OVER");
        Debug.Log($"{winningTrainer} Won!");
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

    // Decides Trainer action
    (string, MoveInfo, int) TrainerActionDecide(MonParty AttackingParty, MonParty DefendingParty)
    {
        // Deciding which move is best +priority of it
        MoveInfo moveSelected = MoveSelect(AttackingParty.MonTeam[AttackingParty.currentMon].moves, AttackingParty, DefendingParty);

        // Thing for deciding to heal + priority

        // Thing for deciding to swap mons + their priority
        int currentMon = 0;

        //ADD IF STATEMENTS here to decide which has the highest priority
        string action = "Attack";
        return (action, moveSelected, currentMon);
    }

    // Enacts Trainer Attack
    (MonParty, MonParty) TrainerAttack(MonParty AttackingParty, MonParty DefendingParty)
    {
        Debug.Log($"{AttackingParty.Trainer.name} Attacks");
        DefendingParty.MonTeam[DefendingParty.currentMon].stats.health -= DamageCal(AttackingParty, DefendingParty);

        if (DefendingParty.MonTeam[DefendingParty.currentMon].stats.health < 0)
            DefendingParty.MonTeam[DefendingParty.currentMon].stats.health = 0;

        return (AttackingParty, DefendingParty);
    }

    MoveInfo MoveSelect(List<MoveInfo> moves, MonParty AttackingParty, MonParty DefendingParty)
    {
        /*Function recieves:
         * None
        */
        // ALSO UPDATE THE MOVES PRIORITY AFTER EACH CHECK
        /*Function returns:
         * INT 1-4
        */

        int bestDamage = 0;
        MoveInfo bestMove = moves[0];

        for (int i = 0; i < moves.Count; i++)
        {
            if (DamageCal(AttackingParty, DefendingParty) > bestDamage)
                bestMove = moves[i];
        }

        //int randomNum = Random.Range(0, moves.Count);
        //MoveInfo move = moves[randomNum];
        return bestMove;
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
            AttackingParty.moveSelected.type == AttackingParty.MonTeam[AttackingParty.currentMon].type2.name)
        {
            stab = 1.5f;
        }
        else
        {
            stab = 1;
        }
       
        float random = Random.Range(85f, 101f) / 100f;

        if (Random.Range(1, 256) == 1)
        {
            crit = ((2*AttackingParty.MonTeam[AttackingParty.currentMon].level) + 5) / 
                ((AttackingParty.MonTeam[AttackingParty.currentMon].level) + 5);
        }

        float damage = ( ((((2*AttackingParty.MonTeam[AttackingParty.currentMon].level)/5)+2) * AttackingParty.moveSelected.power
            * (attack/defense)) / (50) + 2) * crit * random * stab * type_modifier;

        return (int)damage;
    }

    // Checks type effectiveness for type modifier
    float TypeCheck(string attackType, TypeInfo defendType)
    {
        // No 2nd type
        if (defendType == null)
            return 1f;
        // Super Effective
        if (defendType.effective.Contains(attackType))
            return 2f;
        // Not Very Effective
        if (defendType.weak.Contains(attackType))
            return 0.5f;
        // No Effect
        if (defendType.no_effect.Contains(attackType))
            return 0f;
        // Normal
        else
            return 1f;
    }
}
