using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> nodes = new List<DialogueNode>();
}

[System.Serializable]
public class DialogueNode
{
    public string nodeID;
    [TextArea(3, 5)] public string npcText;
    public List<PlayerOption> options = new List<PlayerOption>();
    public bool isEndNode;
    public string questAction;
    public string questParameter;
}

[System.Serializable]
public class PlayerOption
{
    public string optionText;
    public string nextNodeID;
    public string action;
}