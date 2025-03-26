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

    void UpdateText(MonParty Party1, MonParty Party2)
    {
        Mon1Text.text = $"{Party1.MonTeam[Party1.currentMon].name}: {Party1.MonTeam[Party1.currentMon].stats.health}";
        Mon2Text.text = $"{Party2.MonTeam[Party2.currentMon].name}: {Party2.MonTeam[Party2.currentMon].stats.health}";
    }

    private IEnumerator TumblemonBattle()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("BATTLE BEGINS NOW");
        UpdateText(Trainer1Party, Trainer2Party);

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

            // Intializes vars
            var(AttackingParty, DefendingParty, isGameOver) = (Trainer1Party, Trainer1Party, false);

            // ADD HEAL & SWAP LOGIC HERE AS THOSE OCCUR BEFORE EITHER CAN ATTACK
            
            // Trainer 1 goes first
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed > Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed)
            {
                Debug.Log($"{Trainer1Party.Trainer.name} GOES FIRST!");
                (AttackingParty, DefendingParty, isGameOver) = TrainerActionEnact(Trainer1Party, Trainer2Party, true, true);
                Trainer1Party = AttackingParty;
                Trainer2Party = DefendingParty;

                Debug.Log($"{Trainer2Party.Trainer.name} GOES SECOND!");
                (AttackingParty, DefendingParty, isGameOver) = TrainerActionEnact(Trainer2Party, Trainer1Party, false, false);
                Trainer2Party = AttackingParty;
                Trainer1Party = DefendingParty;

                if (isGameOver)
                    break;
            }
            // Trainer 2 goes first
            else
            {
                Debug.Log($"{Trainer2Party.Trainer.name} GOES FIRST!");
                (AttackingParty, DefendingParty, isGameOver) = TrainerActionEnact(Trainer2Party, Trainer1Party, true, false);
                Trainer2Party = AttackingParty;
                Trainer1Party = DefendingParty;

                Debug.Log($"{Trainer1Party.Trainer.name} GOES SECOND!");
                (AttackingParty, DefendingParty, isGameOver) = TrainerActionEnact(Trainer1Party, Trainer2Party, false, true);
                Trainer1Party = AttackingParty;
                Trainer2Party = DefendingParty;

                if (isGameOver)
                    break;
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
        int attack_priority = 0;
        int heal_priority = 0;
        int swap_priority = 0;
        string action = "";

        // Deciding which move is best +priority of it
        MoveInfo moveSelected = MoveSelect(AttackingParty.MonTeam[AttackingParty.currentMon].moves, AttackingParty, DefendingParty);

        // Thing for deciding to heal + priority
        if ((float)AttackingParty.MonTeam[AttackingParty.currentMon].stats.health / 
            (float)AttackingParty.MonTeam[AttackingParty.currentMon].stats.total_health <= (float)AttackingParty.Trainer.heal_threshold / 100)
            heal_priority = AttackingParty.Trainer.heal_priority;

        // Thing for deciding to swap mons + their priority
        int currentMon = 0;

        // ADD IF STATEMENTS here to decide which has the highest priority
        if (attack_priority >= heal_priority && attack_priority >= swap_priority)
            action = "Attack";
        else if (heal_priority >= attack_priority && heal_priority >= swap_priority)
            action = "Heal";
        else if (swap_priority >= attack_priority && swap_priority >= heal_priority)
            action = "Swap";
        return (action, moveSelected, currentMon);
    }

    // Enacts Trainer's Attack
    (MonParty, MonParty, bool) TrainerActionEnact(MonParty ActingParty, MonParty NonActingParty, bool isFirst, bool isTrainer1)
    {
        // Intializes var
        var(AttackingParty, DefendingParty) = (ActingParty, ActingParty);

        if (isFirst) {
            if (ActingParty.action == "Attack")
            {
                (AttackingParty, DefendingParty) = Attack(ActingParty, NonActingParty);
                ActingParty = AttackingParty;
                NonActingParty = DefendingParty;

                if (isTrainer1)
                    UpdateText(ActingParty, NonActingParty);
                else   
                    UpdateText(NonActingParty, ActingParty);
            }
        }    
        else
        {
            if (ActingParty.MonTeam[ActingParty.currentMon].stats.health <= 0)
            {
                Debug.Log($"{ActingParty.MonTeam[ActingParty.currentMon].name} HAS FAINTED!");

                // Checks to see if trainer has run of mons
                trainer2Loss = IsPartyDead(ActingParty);
                if (trainer2Loss)
                {
                    winningTrainer = NonActingParty.Trainer.name;
                    return (ActingParty, NonActingParty, true);
                }
                // ADD LOGIC HERE FOR SWAPPING IF MON IS DEAD
            }
            else if (ActingParty.action == "Attack")
            {
                (AttackingParty, DefendingParty) = Attack(ActingParty, NonActingParty);
                ActingParty = AttackingParty;
                NonActingParty = DefendingParty;
            }
            if (isTrainer1)
                UpdateText(ActingParty, NonActingParty);
            else   
                UpdateText(NonActingParty, ActingParty);
        }

        return (ActingParty, NonActingParty, false);
    }

    // Enacts Trainer Attack
    (MonParty, MonParty) Attack(MonParty AttackingParty, MonParty DefendingParty)
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
            AttackingParty.moveSelected = moves[i];
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
