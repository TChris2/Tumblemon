using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainerInfo
{
    public string name;
    public int attack_priority;
    public int heal_threshold;
    public int heal_priority;
    public int swap_priority;
    public string spriteName;
    public string winText;
    public string loseText;
    public string introSFX;

    public TrainerInfo(string name, int attack_priority, int heal_threshold, int heal_priority, int swap_priority, string spriteName, 
        string winningText, string lossingText, string introSFX)
    {
        this.name = name;
        this.attack_priority = attack_priority;
        this.heal_threshold = heal_threshold;
        this.heal_priority = heal_priority;
        this.swap_priority = swap_priority;
        this.spriteName = spriteName;
        this.winText = winningText;
        this.loseText = lossingText;
        this.introSFX = introSFX;
    }

    public TrainerInfo Clone()
    {
        return new TrainerInfo(
            name,
            attack_priority,
            heal_threshold,
            heal_priority,
            swap_priority,
            spriteName,
            winText,
            loseText,
            introSFX
        );
    }
}
