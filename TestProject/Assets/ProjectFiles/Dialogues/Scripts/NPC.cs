using UnityEngine;
using UserInteraction;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "Персонаж";
    [SerializeField] private DialogueData dialogue; //

    public string NPCName => npcName;

    public string GetInteractionPrompt() => "E - поговорить";
    public bool CanInteract(GameObject player) => true;

    public void OnInteractStart()
    {
        DialogueManager.Instance.StartDialogue(dialogue, npcName);
    }

    public void OnInteractHold() { }
    public void OnInteractEnd() { }
}