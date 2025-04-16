using System.Collections.Generic;
using UnityEngine.UI;
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

    public Image HealthBar;
    public TMPro.TMP_Text NameText;
    public TMPro.TMP_Text LevelText;
    public TMPro.TMP_Text HealthText;
    public TMPro.TMP_Text TotalHealthText;
    public Animator EffectAnimator;
    public Animator AttackAnimator;
    public Animator MonAnimator;
    public CanvasGroup TextBoxCanvas;

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
