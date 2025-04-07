using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainerDatabase : MonoBehaviour
{
    public List<TrainerInfo> TrainerList = new List<TrainerInfo>();

    private void Awake()
    {
        LoadTrainerData();
    }

    private void LoadTrainerData()
    {
        // Load the text file from Resources
        TextAsset textAsset = Resources.Load<TextAsset>("Trainers");

        if (textAsset == null)
        {
            Debug.LogError("Moves.txt not found in Resources folder.");
            return;
        }

        string[] lines = textAsset.text.Split('\n');
        TrainerInfo currentTrainer = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;

            string[] parts = trimmedLine.Split(':');
            if (parts.Length < 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "Name":
                    if (currentTrainer != null) 
                    {
                        SaveTrainer(currentTrainer);
                    }
                    currentTrainer = new TrainerInfo(value, 0, 0, 0, 0, "");
                    break;
                case "Attack Priority":
                    if (currentTrainer != null) currentTrainer.attack_priority = int.Parse(value);
                    break;
                case "Heal Threshold":
                    if (currentTrainer != null) currentTrainer.heal_threshold = int.Parse(value);
                    break;
                case "Heal Priority":
                    if (currentTrainer != null) currentTrainer.heal_priority = int.Parse(value);
                    break;
                case "Swap Priority":
                    if (currentTrainer != null) currentTrainer.swap_priority = int.Parse(value);
                    break;
                case "Sprite Name":
                    if (currentTrainer != null) currentTrainer.spriteName = value;
                    break;
            }
        }

        if (currentTrainer != null) 
        {
            SaveTrainer(currentTrainer);
        }
    }

    private void SaveTrainer(TrainerInfo currentTrainer)
    {
        TrainerInfo newTrainer = new TrainerInfo(currentTrainer.name, currentTrainer.attack_priority, 
            currentTrainer.heal_threshold, currentTrainer.heal_priority, currentTrainer.swap_priority, currentTrainer.spriteName);
        TrainerList.Add(newTrainer);
    }

    public TrainerInfo GetTrainerByName(string trainerName)
    {
        return TrainerList.Find(trainer => trainer.name == trainerName);
    }
}
