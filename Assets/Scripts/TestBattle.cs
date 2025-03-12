using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBattle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int mon_selection = Random.Range(1,number of mons);
        //Pick two random mons

        int move_selection = 1;
        int mon_1_hp = 100;
        int mon_2_hp = 100;
        int mon_1_speed = 50;
        int mon_2_speed = 60;
        /*Need to know:
         * HP stat of mon 1
         * Hp stat of mon 2
        */
        
        //while loop runs while both mons have more than 0 health
        while (mon_1_hp > 0 && mon_2_hp > 0)
        {
            if (mon_1_speed > mon_2_speed)
            {
                //mon 1 picks a random ove 1-4
                move_selection = select_move();

                //damage calculation is performed and mon 2 loses hp
                mon_2_hp = mon_2_hp - damage_calc();

                //mon 2 picks a random move
                move_selection = select_move();

                //damage calculation is performed and mon 1 loses hp
                mon_1_hp = mon_1_hp - damage_calc();
            }
           
            else
            {
                //mon 2 picks a random move
                move_selection = select_move();

                //damage calculation is performed and mon 1 loses hp
                mon_1_hp = mon_1_hp - damage_calc();

                //mon 1 picks a random ove 1-4
                move_selection = select_move();

                //damage calculation is performed and mon 2 loses hp
                mon_2_hp = mon_2_hp - damage_calc();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int select_move()
    {
        /*Function recieves:
         * None
        */

        /*Function returns:
         * INT 1-4
        */

        int ms = Random.Range(1, 4);
        return ms;
    }

    int damage_calc()
    {
        /*Function needs to recieve:
         * mon level
         * mon power
         * mon defense
         * mon attack
         * move type
         * reciver type
         * attacker type
        */

        /*Function returns
         * An int damage dealt to reciever, greated than 1, rounded down to nearest int
        */

        double damage;
        double level = 50;
        double power = 60;
        double defense = 90;
        double attack = 100;
        double type_multiplyer = 1;
        double stab = 1;
        
        /*
        //Stab
        if (move type = attacker type)
        {
        stab = 1.5
        }

        else
        {
        stab = 1;
        }
        */
       
        /*
        //Decided type effectivness
        can be 0.5, 1, or 2
        */
        damage = ( ((((2*level)/5)+2) * power * (attack/defense)) / (50) + 2) * type_multiplyer * stab;

        if (damage < 1)
        {
            return 1;
        }

        else
        {
            return (int)damage;
        }
    }
}
