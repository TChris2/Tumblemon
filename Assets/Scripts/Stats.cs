using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public int total_health;
    public int health;
    public int attack;
    public int special_attack;
    public int defense;
    public int special_defense;
    public int speed;

    public Stats(int health, int attack, int special_attack, int defense, int special_defense, int speed, int level)
    {
        this.total_health = health + (health/50)*level;
        this.health = total_health;
        this.attack = attack + (attack/50)*level;
        this.special_attack = special_attack + (special_attack/50)*level;
        this.defense = defense + (defense/50)*level;
        this.special_defense = special_defense + (special_defense/50)*level;
        this.speed = speed + (speed/50)*level;
    }
}
