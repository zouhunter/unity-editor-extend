using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[System.Serializable] tells unity to serialize this class if 
//it's used in a public array or as a public variable in a component 
[System.Serializable]
public class LevelBlockData {
    public string Name;
    public GameObject Prefab;
} //[CreateAssetMenu] creates an entry in the default Create menu of the ProjectView so you can easily create an instance of this ScriptableObject 
[CreateAssetMenu]
public class LevelBlocks : ScriptableObject
{
    //This ScriptableObject simply stores a list of blocks. It kind of acts like a database in that it stores rows of data 
    public List<LevelBlockData> Blocks = new List<LevelBlockData>();
}