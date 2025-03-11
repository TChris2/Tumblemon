using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TypeInfo
{
    public string name;
    public List<string> normal = new List<string> {};
    public List<string> effective = new List<string> {};
    public List<string> weak = new List<string> {};
    public List<string> no_effect = new List<string> {};
}
