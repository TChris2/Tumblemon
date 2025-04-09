using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonParty : MonoBehaviour
{
    public TrainerInfo Trainer;
    // Mon Party
    public List<MonInfo> MonTeam = new List<MonInfo>();
    // Database of all mons
    private MonDatabase monDatabase;
    private MoveDatabase moveDatabase;
    private TrainerDatabase trainerDatabase;
    // Keeps Track of the amt of potions the user has
    public int potionCount = 3;
    // Keeps track of which mon is currently on the field
    public int currentMon = 0;
    // Keeps track of which move the current mon will use in battle
    public MoveInfo moveSelected;
    // Action trainer will take that turn
    public string action;
    // For preset teams with testpartyselect
    public int select;
    // Keeps track of damage amounts for counter moves
    public int currentDmg;
    public TMPro.TMP_Text HealthBarText;
    public TMPro.TMP_Text ThoughtText1;
    public TMPro.TMP_Text ThoughtText2;

    private void Start()
    {
        monDatabase = FindObjectOfType<MonDatabase>();
        moveDatabase = FindObjectOfType<MoveDatabase>();
        trainerDatabase = FindObjectOfType<TrainerDatabase>();

        StartCoroutine(WaitForDatabaseLoad());
    }

    private IEnumerator WaitForDatabaseLoad()
    {
        // Wait until MonDatabase is populated
        while (!monDatabase.isMonDatabaseLoaded)
        {
            yield return null;
        }

        if (select == 1)
            TestPartySelect1();
        if (select == 2)
            TestPartySelect2();
    }

    private void TestPartySelect1()
    {   
        AddTrainer("Trainer 1");
        SelectMon("Dewott");
        AddMove(0, "Razor Shell");
        SelectMon("Garchomp");
        AddMove(1, "Dragon Breath");
    }

    private void TestPartySelect2()
    {
        AddTrainer("Trainer 2");
        SelectMon("Garchomp");
        AddMove(0, "Dragon Breath");
        SelectMon("Dewott");
        AddMove(1, "Razor Shell");
    }

    public void SelectMon(string monName)
    {
        // If the database is empty
        if (monDatabase == null) return;

        MonInfo selectedMon = monDatabase.GetMonByName(monName);
        if (selectedMon != null & MonTeam.Count <= 6)
        {
            MonTeam.Add(selectedMon.Clone());
            Debug.Log(monName + " added to party!");
        }
        else
        {
            Debug.LogWarning("Mon not found: " + monName);
        }
    }

    public void AddMove(int partyMon, string moveName)
    {
        // If the database is empty
        if (moveDatabase == null) return;

        // Checks if mon already has move
        if (MonTeam[partyMon].moves.Exists(move => move.name == moveName))
        {
            Debug.LogWarning(MonTeam[partyMon].name + " already knows " + moveName + "!");
            return;
        }

        MoveInfo selectedMove = moveDatabase.GetMoveByName(moveName);

        if (selectedMove != null & MonTeam[partyMon].moves.Count <= 4)
        {       
            MonTeam[partyMon].moves.Add(selectedMove.Clone());
            Debug.Log(moveName + " added to " + MonTeam[partyMon].name + "!");
        }
        else
        {
            Debug.LogWarning("Move not found: " + moveName);
        }
    }

    public void AddTrainer(string trainerName)
    {
        // If the database is empty
        if (trainerDatabase == null) 
        {
            Debug.LogWarning("Trainer Database Null");
            return;
        }

        TrainerInfo selectedTrainer = trainerDatabase.GetTrainerByName(trainerName);

        if (selectedTrainer != null)
        {       
            Trainer = selectedTrainer.Clone();
            Debug.Log(trainerName + " assigned! ");
        }
        else
        {
            Debug.LogWarning("Trainer not found: " + trainerName);
        }
    }
}