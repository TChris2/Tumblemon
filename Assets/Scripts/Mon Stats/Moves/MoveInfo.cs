using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveInfo
{
    public string name;
    public string type;
    public string attackType;
    public int power;
    public int accuracy;
    public int total_pp;
    public int pp;
    public string status;
    public int statusOdds;
    public int priority;

    public MoveInfo (string name, string type, string attackType, int power, int accuracy, int pp, string status, int statusOdds)
    {
        this.name = name;
        this.type = type;
        this.attackType = attackType;
        this.power = power;
        this.accuracy = accuracy;
        this.total_pp = pp;
        this.pp = total_pp;
        this.status = status;
        this.statusOdds = statusOdds;
        this.priority = 0;
    }
}
