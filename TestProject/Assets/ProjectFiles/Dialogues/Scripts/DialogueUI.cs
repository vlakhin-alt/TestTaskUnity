using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private GameObject optionButtonPrefab;

    private DialogueManager manager;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        manager = GetComponent<DialogueManager>();
    }

    public void ShowDialogue(string npcName)
    {
        npcNameText.text = npcName;
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void DisplayNode(DialogueNode node)
    {
        npcText.text = node.npcText;
        ClearOptions();

        foreach (var option in node.options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = option.optionText;

            btn.onClick.AddListener(() => manager.SelectOption(option));
        }
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
    }
}