using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonParty : MonoBehaviour
{
    // Mon Party
    public List<MonInfo> tumblemonParty = new List<MonInfo>();
    // Database of all mons
    private MonDatabase monDatabase;
    private MoveDatabase moveDatabase;

    private void Start()
    {
        monDatabase = FindObjectOfType<MonDatabase>();
        moveDatabase = FindObjectOfType<MoveDatabase>();
    }

    public void SelectMon(string monName)
    {
        // If the database is empty
        if (monDatabase == null) return;

        MonInfo selectedMon = monDatabase.GetMonByName(monName);

        if (selectedMon != null & tumblemonParty.Count < 6)
        {
            // Create a new instance to avoid reference issues
            MonInfo newMon = new MonInfo(selectedMon.name, selectedMon.type1, selectedMon.type2, selectedMon.level, selectedMon.stats);
            
            tumblemonParty.Add(newMon);
            Debug.Log(monName + " added to party!");
        }
        else
        {
            Debug.LogWarning("Mon not found: " + monName);
        }
    }

    public void GetMoveInfo(string moveName)
    {

    }
}