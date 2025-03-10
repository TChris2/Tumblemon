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

    public MonInfo (string name, string type1, string type2, int level, Stats stats)
    {
        this.name = name;
        this.type1 = type1;
        this.type2 = type2;
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
    }
}
