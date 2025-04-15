using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class MonParty
{
    public TrainerInfo Trainer;
    public List<MonInfo> MonTeam = new List<MonInfo>();
    public int potionCount;
    public int currentMon;
    public MoveInfo moveSelected;
    public string action;
    public int currentDmg;

    public TMP_Text HealthBarText;
    public TMP_Text ThoughtText1;
    public TMP_Text ThoughtText2;

    public MonParty() 
    {

    }
    
    public MonParty Clone()
    {
        return new MonParty(this);
    }

    private MonParty(MonParty original)
    {
        Trainer = original.Trainer.Clone();
        potionCount = original.potionCount;
        currentMon = original.currentMon;
        currentDmg = original.currentDmg;
        moveSelected = original.moveSelected != null ? original.moveSelected.Clone() : null;
        MonTeam = new List<MonInfo>();
        foreach (var mon in original.MonTeam)
            MonTeam.Add(mon.Clone());
    }
}
