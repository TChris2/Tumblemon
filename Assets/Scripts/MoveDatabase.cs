using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveDatabase : MonoBehaviour
{
    public List<MoveInfo> MoveList = new List<MoveInfo>();

    private void Start()
    {
        LoadMoveData();
    }

    private void LoadMoveData()
    {
        // Load the text file from Resources
        TextAsset textAsset = Resources.Load<TextAsset>("Moves");

        if (textAsset == null)
        {
            Debug.LogError("Moves.txt not found in Resources folder.");
            return;
        }

        /*string[] lines = textAsset.text.Split('\n');
        MoveInfo currentMove = null;

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
                    if (currentMove != null) 
                    {
                        MoveInfo newMon = new MoveInfo(currentMove.name, currentMove.type1, currentMove.type2, 
                            currentMove.level, new Stats(currentMove.stats.health, currentMove.stats.attack, currentMove.stats.special_attack, 
                            currentMove.stats.defense, currentMove.stats.special_defense, currentMove.stats.speed, currentMove.level));
                        newMon.moveList = currentMove.moveList;
                        TumblemonList.Add(newMon);
                    }
                    currentMove = new MoveInfo(value, "", "", 1, new Stats(0, 0, 0, 0, 0, 0, 0));
                    break;
                case "Type1":
                    if (currentMove != null) currentMove.type1 = value;
                    break;
                case "Type2":
                    if (currentMove != null) currentMove.type2 = value;
                    break;
                case "Level":
                    if (currentMove != null) currentMove.level = int.Parse(value);
                    break;
                case "Health":
                    if (currentMove != null) currentMove.stats.health = int.Parse(value);
                    break;
                case "Attack":
                    if (currentMove != null) currentMove.stats.attack = int.Parse(value);
                    break;
                case "SpecialAttack":
                    if (currentMove != null) currentMove.stats.special_attack = int.Parse(value);
                    break;
                case "Defense":
                    if (currentMove != null) currentMove.stats.defense = int.Parse(value);
                    break;
                case "SpecialDefense":
                    if (currentMove != null) currentMove.stats.special_defense = int.Parse(value);
                    break;
                case "Speed":
                    if (currentMove != null) currentMove.stats.speed = int.Parse(value);
                    break;
                case "Moves":
                    if (currentMove != null) 
                    {
                        string[] moves = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string move in moves)
                        {
                            currentMove.moveList.Add(moveDatabase.GetMoveByName(move.Trim()));
                        }
                    }
                    break;
            }
        }

        if (currentMove != null) 
        {
            MoveInfo newMon = new MoveInfo(currentMove.name, currentMove.type1, currentMove.type2, 
                currentMove.level, new Stats(currentMove.stats.health, currentMove.stats.attack, currentMove.stats.special_attack, 
                currentMove.stats.defense, currentMove.stats.special_defense, currentMove.stats.speed, currentMove.level));
            newMon.moveList = currentMove.moveList;
            TumblemonList.Add(newMon);
        }*/
    }

    public MoveInfo GetMoveByName(string moveName)
    {
        MoveInfo move = MoveList.Find(move => move.name == moveName);

        if (move == null)
        {
            return new MoveInfo("", "", 0, 0, 0, "", 0);
        }

        return move;
    }
}
