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
    private Animator animator;
    
    public MonParty Trainer1Party;
    public MonParty Trainer2Party;
    private AudioSource audiosource;
    [SerializeField]
    private AudioClip battleMusic;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        audiosource = FindObjectOfType<AudioSource>();
        monDatabase = GetComponent<MonDatabase>();
        moveDatabase = GetComponent<MoveDatabase>();
        trainerDatabase = GetComponent<TrainerDatabase>();
        animator = GameObject.Find("Canvas").GetComponent<Animator>();

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
        AddTrainer(Trainer1Party, "Jade");
        SelectMon(Trainer1Party, "Garchomp");
        AddMove(Trainer1Party, 0, "Dragon Breath");
        AddMove(Trainer1Party, 0, "Crunch");
        SelectMon(Trainer1Party, "Crobat");
        AddMove(Trainer1Party, 1, "Aerial Ace");
        AddMove(Trainer1Party, 1, "Cross Posion");
        AddMove(Trainer1Party, 1, "Shadow Ball");
        SelectMon(Trainer1Party, "Empoleon");
        AddMove(Trainer1Party, 2, "Surf");
        AddMove(Trainer1Party, 2, "Rock Slide");
        AddMove(Trainer1Party, 2, "Blizzard");
    }

    private void TestPartySelect2()
    {
        Trainer2Party.potionCount = 0;
        AddTrainer(Trainer2Party, "John Golf");
        SelectMon(Trainer2Party, "Samurai Dan");
        AddMove(Trainer2Party, 0, "Aerial Ace");
        AddMove(Trainer2Party, 0, "Night Slash");
        AddMove(Trainer2Party, 0, "Cross Posion");
        AddMove(Trainer2Party, 0, "Shadow Ball");
        SelectMon(Trainer2Party, "Rumble Toad Game");
        AddMove(Trainer2Party, 1, "Crunch");
        AddMove(Trainer2Party, 1, "Posion Jab");
        AddMove(Trainer2Party, 1, "Cross Posion");
        AddMove(Trainer2Party, 1, "Body Slam");
        SelectMon(Trainer2Party, "Drift Queen");
        AddMove(Trainer2Party, 2, "Aerial Ace");
        AddMove(Trainer2Party, 2, "Rock Slide");
        AddMove(Trainer2Party, 2, "Earthquake");
        AddMove(Trainer2Party, 2, "Blizzard");
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

    public void StartBattle()
    {
        audiosource.clip = battleMusic;
        audiosource.Play();
        animator.Play("Battle Start");
    }
}