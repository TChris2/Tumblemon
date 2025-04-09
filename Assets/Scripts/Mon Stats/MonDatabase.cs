using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class MonDatabase : MonoBehaviour
{
    public List<MonInfo> TumblemonList = new List<MonInfo>();
    private MoveDatabase moveDatabase;
    private TypeDatabase typeDatabase;
    public bool isMonDatabaseLoaded;

    private void Start()
    {
        isMonDatabaseLoaded = false;

        moveDatabase = FindObjectOfType<MoveDatabase>();
        typeDatabase = FindObjectOfType<TypeDatabase>();
        LoadTumblemonData();
    }

    private void LoadTumblemonData()
    {
        // Load the text file from Resources
        TextAsset textAsset = Resources.Load<TextAsset>("Tumblemon");

        if (textAsset == null)
        {
            Debug.LogError("Tumblemon.txt not found in Resources folder.");
            return;
        }

        string[] lines = textAsset.text.Split('\n');
        MonInfo currentMon = null;

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
                    if (currentMon != null) 
                    {
                        SaveMon(currentMon);
                    }
                    currentMon = new MonInfo(value, 1, new Stats(0, 0, 0, 0, 0, 0, 0), "");
                    break;
                case "Type1":
                    if (currentMon != null)
                    {
                        currentMon.type1 = typeDatabase.GetTypeByName(value);
                    } 
                    break;
                case "Type2":
                    if (currentMon != null)
                    {
                        currentMon.type2 = typeDatabase.GetTypeByName(value);
                    }  
                    break;
                case "Level":
                    if (currentMon != null) currentMon.level = int.Parse(value);
                    break;
                case "Health":
                    if (currentMon != null) currentMon.stats.health = int.Parse(value);
                    break;
                case "Attack":
                    if (currentMon != null) currentMon.stats.attack = int.Parse(value);
                    break;
                case "Special Attack":
                    if (currentMon != null) currentMon.stats.special_attack = int.Parse(value);
                    break;
                case "Defense":
                    if (currentMon != null) currentMon.stats.defense = int.Parse(value);
                    break;
                case "Special Defense":
                    if (currentMon != null) currentMon.stats.special_defense = int.Parse(value);
                    break;
                case "Speed":
                    if (currentMon != null) currentMon.stats.speed = int.Parse(value);
                    break;
                case "Moves":
                    if (currentMon != null) 
                    {
                        string[] moves = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string move in moves)
                        {
                            MoveInfo monMove = moveDatabase.GetMoveByName(move.Trim());
                            if (monMove.name != "")
                                currentMon.moveList.Add(monMove);
                        }
                    }
                    break;
                case "Sprite Name":
                    if (currentMon != null) currentMon.spriteName = value;
                    break;
            }
        }

        if (currentMon != null) 
        {
            SaveMon(currentMon);
        }

        isMonDatabaseLoaded = true;
    }
    
    private void SaveMon(MonInfo currentMon)
    {
        MonInfo newMon = new MonInfo(currentMon.name, currentMon.level, 
        new Stats(currentMon.stats.health, currentMon.stats.attack, currentMon.stats.special_attack, 
            currentMon.stats.defense, currentMon.stats.special_defense, currentMon.stats.speed, currentMon.level), currentMon.spriteName);
        newMon.moveList = currentMon.moveList;
        newMon.type1 = currentMon.type1;
        newMon.type2 = currentMon.type2;
        TumblemonList.Add(newMon);
    }

    public MonInfo GetMonByName(string monName)
    {
        return TumblemonList.Find(mon => mon.name == monName);
    }
}
