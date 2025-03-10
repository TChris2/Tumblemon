using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveInfo
{
    public string name;
    public string type;
    public int power;
    public int accuracy;
    public int total_pp;
    public int pp;
    public string status;
    public int statusOdds;

    public MoveInfo (string name, string type, int power, int accuracy, int pp, string status, int statusOdds)
    {
        this.name = name;
        this.type = type;
        this.power = power;
        this.accuracy = accuracy;
        this.total_pp = pp;
        this.pp = total_pp;
        this.status = status;
        this.statusOdds = statusOdds;
    }
}
