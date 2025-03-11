using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TypeInfo
{
    public List<string> attack = new List<MoveInfo> {};
    public List<string> defense = new List<MoveInfo> {};
    // List of current moves 

    public TypeInfo(string name, string type1, string type2, int level, Stats stats)
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
