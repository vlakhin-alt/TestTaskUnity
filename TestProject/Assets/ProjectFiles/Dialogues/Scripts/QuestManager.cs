using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private Toggle questToggle;

    private string currentQuestItem;

    private void Awake()
    {
        Debug.Log($"[QuestManager] Awake on {gameObject.name}");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Поиск объектов
        if (questPanel == null)
        {
            questPanel = GameObject.Find("QuestPanel");
            Debug.Log($"[QuestManager] questPanel found: {questPanel != null}");
        }
        if (questText == null)
        {
            var textObj = GameObject.Find("QuestText");
            if (textObj != null) questText = textObj.GetComponent<TextMeshProUGUI>();
            Debug.Log($"[QuestManager] questText found: {questText != null}");
        }
        if (questToggle == null)
        {
            var toggleObj = GameObject.Find("QuestToggle");
            if (toggleObj != null) questToggle = toggleObj.GetComponent<Toggle>();
            Debug.Log($"[QuestManager] questToggle found: {questToggle != null}");
        }

        if (questPanel != null)
            questPanel.SetActive(false);
        else
            Debug.LogError("[QuestManager] QuestPanel not found!");
    }

    public void ActivateQuest(string itemType)
    {
        Debug.Log($"[QuestManager] ActivateQuest called with: {itemType}");
        currentQuestItem = itemType;
        if (questText != null) questText.text = $"Принесите: {itemType}";
        if (questPanel != null)
        {
            questPanel.SetActive(true);
            Debug.Log($"[QuestManager] Panel active: {questPanel.activeSelf}");
        }
        else
        {
            Debug.LogError("[QuestManager] questPanel is null!");
        }
        if (questToggle != null) questToggle.isOn = false;
    }

    public bool IsQuestActive() => !string.IsNullOrEmpty(currentQuestItem);

    public bool TryCompleteQuest(Item heldItem)
    {
        if (!IsQuestActive() || heldItem == null) return false;
        if (heldItem.name.Contains(currentQuestItem))
        {
            CompleteQuest();
            return true;
        }
        return false;
    }

    private void CompleteQuest()
    {
        Debug.Log("[QuestManager] Quest completed!");
        if (questToggle != null) questToggle.isOn = true;
        currentQuestItem = null;
    }

    public void HideQuestPanel()
    {
        if (questPanel != null) questPanel.SetActive(false);
    }
}