using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI ui;
    [SerializeField] private PlayerController playerController;

    private DialogueData currentDialogue;
    private Dictionary<string, DialogueNode> nodeDictionary;
    private DialogueNode currentNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueData dialogue, string npcName)
{
    if (dialogue == null) { Debug.LogError("DialogueData is null!"); return; }
    if (string.IsNullOrEmpty(npcName)) npcName = "NPC";

    currentDialogue = dialogue;

    nodeDictionary = new Dictionary<string, DialogueNode>();
    foreach (var node in dialogue.nodes)
        nodeDictionary[node.nodeID] = node;

    if (nodeDictionary.ContainsKey("Start"))
    {
        currentNode = nodeDictionary["Start"];
        playerController.SetState(PlayerState.Dialogue);
        ui.ShowDialogue(npcName);
        ui.DisplayNode(currentNode);
    }
    else
    {
        Debug.LogError("No 'Start' node in dialogue!");
    }
}

    public void SelectOption(PlayerOption option)
    {
        if (!string.IsNullOrEmpty(option.action) && option.action == "CloseDialogue")
        {
            EndDialogue();
            return;
        }

        if (!string.IsNullOrEmpty(option.nextNodeID) && nodeDictionary.ContainsKey(option.nextNodeID))
        {
            currentNode = nodeDictionary[option.nextNodeID];
            ui.DisplayNode(currentNode);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        playerController.SetState(PlayerState.FreeMovement);
        ui.HideDialogue();
    }
}