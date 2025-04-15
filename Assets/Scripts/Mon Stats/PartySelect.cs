using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartySelect : MonoBehaviour
{
    // Database of all mons
    private MonDatabase monDatabase;
    private MoveDatabase moveDatabase;
    private TrainerDatabase trainerDatabase;
    
    public MonParty Trainer1Party;
    public MonParty Trainer2Party;

    private void Awake()
    {
        monDatabase = GetComponent<MonDatabase>();
        moveDatabase = GetComponent<MoveDatabase>();
        trainerDatabase = GetComponent<TrainerDatabase>();

        if (SceneManager.GetActiveScene().name == "Team Select")
        {
            Trainer1Party = new MonParty();
            Trainer2Party = new MonParty();

            StartCoroutine(WaitForDatabaseLoad());
        }
    }

    private IEnumerator WaitForDatabaseLoad()
    {
        // Wait until MonDatabase is populated
        while (!monDatabase.isMonDatabaseLoaded)
        {
            yield return null;
        }

        TestPartySelect1();
        TestPartySelect2();
    }

    private void TestPartySelect1()
    {   
        Trainer1Party.potionCount = 3;
        AddTrainer(Trainer1Party, "Trainer 1");
        SelectMon(Trainer1Party, "Dewott");
        AddMove(Trainer1Party, 0, "Razor Shell");
        SelectMon(Trainer1Party, "Garchomp");
        AddMove(Trainer1Party, 1, "Dragon Breath");
    }

    private void TestPartySelect2()
    {
        Trainer2Party.potionCount = 3;
        AddTrainer(Trainer2Party, "Trainer 2");
        SelectMon(Trainer2Party, "Garchomp");
        AddMove(Trainer2Party, 0, "Dragon Breath");
        SelectMon(Trainer2Party, "Dewott");
        AddMove(Trainer2Party, 1, "Razor Shell");
    }

    public void SelectMon(MonParty party, string monName)
    {
        // If the database is empty
        if (monDatabase == null) return;

        MonInfo selectedMon = monDatabase.GetMonByName(monName);
        if (selectedMon != null & party.MonTeam.Count <= 6)
        {
            party.MonTeam.Add(selectedMon.Clone());
            Debug.Log(monName + " added to party!");
        }
        else
        {
            Debug.LogWarning("Mon not found: " + monName);
        }
    }

    public void AddMove(MonParty party, int partyMon, string moveName)
    {
        // If the database is empty
        if (moveDatabase == null) return;

        // Checks if mon already has move
        if (party.MonTeam[partyMon].moves.Exists(move => move.name == moveName))
        {
            Debug.LogWarning(party.MonTeam[partyMon].name + " already knows " + moveName + "!");
            return;
        }

        MoveInfo selectedMove = moveDatabase.GetMoveByName(moveName);

        if (selectedMove != null & party.MonTeam[partyMon].moves.Count <= 4)
        {       
            party.MonTeam[partyMon].moves.Add(selectedMove.Clone());
            Debug.Log(moveName + " added to " + party.MonTeam[partyMon].name + "!");
        }
        else
        {
            Debug.LogWarning("Move not found: " + moveName);
        }
    }

    public void AddTrainer(MonParty party, string trainerName)
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
            party.Trainer = selectedTrainer.Clone();
            Debug.Log(trainerName + " assigned! ");
        }
        else
        {
            Debug.LogWarning("Trainer not found: " + trainerName);
        }
    }
}