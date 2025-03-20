using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveDatabase : MonoBehaviour
{
    public List<MoveInfo> MoveList = new List<MoveInfo>();

    private void Awake()
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

        string[] lines = textAsset.text.Split('\n');
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
                        SaveMove(currentMove);
                    }
                    currentMove = new MoveInfo(value, "", "", 0, 0, 0, "", 0);
                    break;
                case "Type":
                    if (currentMove != null) currentMove.type = value;
                    break;
                case "Attack Type":
                    if (currentMove != null) currentMove.attackType = value;
                    break;
                case "Power":
                    if (currentMove != null) currentMove.power = int.Parse(value);
                    break;
                case "Accuracy":
                    if (currentMove != null) currentMove.accuracy = int.Parse(value);
                    break;
                case "PP":
                    if (currentMove != null) currentMove.pp = int.Parse(value);
                    break;
                case "Status":
                    if (currentMove != null) currentMove.status = value;
                    break;
                case "Status Odds":
                    if (currentMove != null) currentMove.statusOdds = int.Parse(value);
                    break;
            }
        }

        if (currentMove != null) 
        {
            SaveMove(currentMove);
        }
    }

    private void SaveMove(MoveInfo currentMove)
    {
        MoveInfo newMove = new MoveInfo(currentMove.name, currentMove.type, currentMove.attackType, 
            currentMove.power, currentMove.accuracy, currentMove.pp, currentMove.status, currentMove.statusOdds);
        MoveList.Add(newMove);
    }

    public MoveInfo GetMoveByName(string moveName)
    {
        return MoveList.Find(move => move.name == moveName);
    }
}
