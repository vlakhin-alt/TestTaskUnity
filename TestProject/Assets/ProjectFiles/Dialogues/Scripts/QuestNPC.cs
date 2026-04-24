using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UserInteraction;

public class QuestNPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private string npcName = "Квестодатель";
    [SerializeField] private DialogueData questDialogue; // 

    [Header("Quest UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private Toggle questToggle;

    [Header("Quest Settings")]
    [SerializeField] private string[] possibleItems = { "Морковка", "Записка" };

    private string requiredItem;
    private bool questActive;

    private void Start()
    {
        if (questPanel != null) questPanel.SetActive(false);
    }

    public string NPCName => npcName;
    public string GetInteractionPrompt() => "E - поговорить";
    public bool CanInteract(GameObject player) => true;

    public void OnInteractStart()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        Item held = pc?.GetHeldItem();


        if (questActive && held != null && held.ItemName.Contains(requiredItem))
        {
            CompleteQuest();

            DialogueManager.Instance.StartDialogue(questDialogue, npcName);
            return;
        }

   
        if (!questActive)
        {
            ActivateRandomQuest();
        }

  
        DialogueManager.Instance.StartDialogue(questDialogue, npcName);
    }

    public void OnInteractHold() { }
    public void OnInteractEnd() { }

    private void ActivateRandomQuest()
    {
        if (possibleItems.Length == 0) return;
        requiredItem = possibleItems[Random.Range(0, possibleItems.Length)];
        questActive = true;

        if (questPanel != null)
        {
            questPanel.SetActive(true);
            questText.text = $"Принесите: {requiredItem}";
            questToggle.isOn = false;
        }
    }

    private void CompleteQuest()
    {
        questActive = false;
        if (questToggle != null) questToggle.isOn = true;
      
    }
}