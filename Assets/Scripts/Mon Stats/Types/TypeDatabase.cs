using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TypeDatabase : MonoBehaviour
{
    public List<TypeInfo> TypeList = new List<TypeInfo>();

    private void Awake()
    {
        LoadTypeData();
    }

    private void LoadTypeData()
    {
        // Load the text file from Resources
        TextAsset textAsset = Resources.Load<TextAsset>("Types");

        if (textAsset == null)
        {
            Debug.LogError("Types.txt not found in Resources folder.");
            return;
        }

        string[] lines = textAsset.text.Split('\n');
        TypeInfo currentType = null;

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
                    if (currentType != null) 
                    {
                        TypeInfo newType = currentType;
                        TypeList.Add(newType);
                    }
                    currentType = new TypeInfo();
                    currentType.name = value;
                    break;
                case "Normal":
                    if (currentType != null) 
                    {
                        string[] types = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string type in types)
                        {
                            currentType.normal.Add((type.Trim()));
                        }
                    }
                    break;
                case "Effective":
                    if (currentType != null) 
                    {
                        string[] types = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string type in types)
                        {
                            currentType.effective.Add((type.Trim()));
                        }
                    }
                    break;
                case "Weak":
                    if (currentType != null) 
                    {
                        string[] types = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string type in types)
                        {
                            currentType.weak.Add((type.Trim()));
                        }
                    }
                    break;
                case "No Effect":
                    if (currentType != null) 
                    {
                        string[] types = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        foreach (string type in types)
                        {
                            currentType.no_effect.Add((type.Trim()));
                        }
                    }
                    break;
            }
        }

        if (currentType != null) 
        {
            TypeList.Add(currentType);
        }
    }

    public TypeInfo GetTypeByName(string typeName)
    {
        return TypeList.Find(type => type.name == typeName);
    }
}
