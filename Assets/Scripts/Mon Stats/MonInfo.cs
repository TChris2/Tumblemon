using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonInfo
{
    public string name;
    public string type1;
    public string type2;
    public int level;
    public Stats stats;
    // List of all moves
    public List<MoveInfo> moveList = new List<MoveInfo>();
    // List of current moves 
    public List<MoveInfo> moves = new List<MoveInfo>(); 
    // Battle modifiers buffs & debuffs
    public StageMultiplier attack_mult;
    public StageMultiplier special_attack_mult;
    public StageMultiplier special_defense_mult;
    public StageMultiplier defense_mult;
    public StageMultiplier speed_mult;
    public StageMultiplier accuracy_mult; 
    public StageMultiplier evasion_mult; 
    public string status;
    public string spriteName;

    public MonInfo (string name, int level, Stats stats, string spriteName)
    {
        this.name = name;
        this.level = level;
        this.stats = stats;
        this.attack_mult = new StageMultiplier(2,2);
        this.special_attack_mult = new StageMultiplier(2,2);
        this.special_defense_mult = new StageMultiplier(2,2);
        this.defense_mult = new StageMultiplier(2,2);
        this.speed_mult = new StageMultiplier(2,2);
        this.accuracy_mult = new StageMultiplier(3,3); 
        this.evasion_mult = new StageMultiplier(3,3);
        this.status = "None";
        this.spriteName = spriteName;
    }

    public MonInfo Clone()
    {
        MonInfo clone = new MonInfo(name, level, stats.Clone(), spriteName)
        {
            type1 = type1,
            type2 = type2,
            moveList = new List<MoveInfo>(moveList), // assuming these are reference-only templates
            moves = new List<MoveInfo>(),
            attack_mult = new StageMultiplier(attack_mult.numerator, attack_mult.denominator),
            special_attack_mult = new StageMultiplier(special_attack_mult.numerator, special_attack_mult.denominator),
            special_defense_mult = new StageMultiplier(special_defense_mult.numerator, special_defense_mult.denominator),
            defense_mult = new StageMultiplier(defense_mult.numerator, defense_mult.denominator),
            speed_mult = new StageMultiplier(speed_mult.numerator, speed_mult.denominator),
            accuracy_mult = new StageMultiplier(accuracy_mult.numerator, accuracy_mult.denominator),
            evasion_mult = new StageMultiplier(evasion_mult.numerator, evasion_mult.denominator),
            status = status,
        };

        foreach (MoveInfo move in moves)
        {
            clone.moves.Add(move.Clone());
        }

        return clone;
    }
}
