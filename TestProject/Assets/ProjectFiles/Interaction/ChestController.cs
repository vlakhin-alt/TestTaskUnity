using UserInteraction;
using UnityEngine;
public class ChestController:MonoBehaviour,IInteractable
{
    [Header("Settings")]
    [SerializeField] private Transform lid;
    [SerializeField] private float openAngle=120f;
    [SerializeField] private float openSpeed=2f;

    private bool isOpen=false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

     private void Start()
    {
        if (lid != null)
        {
            closedRotation = lid.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openAngle, 0f, 0f); 
        }
    }

    private void Update()
    {
        if (lid == null) return;

        Quaternion target = isOpen ? openRotation : closedRotation;
        lid.localRotation = Quaternion.Slerp(lid.localRotation, target, Time.deltaTime * openSpeed);
    }

    public string GetInteractionPrompt()
    {
        if (isOpen)
            return string.Empty;
        else
            return "E - открыть сундук";
    }

    public bool CanInteract(GameObject player)
    {
        if (isOpen) return false;

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null) return false;


        Item held = pc.GetHeldItem();
        return held != null && held.gameObject.name.Contains("Key"); 
    }

    public void OnInteractStart()
    {
        if (isOpen) return;

        PlayerController pc = FindObjectOfType<PlayerController>();
        Item held = pc.GetHeldItem();

        if (held != null && held.gameObject.name.Contains("Key"))
        {
            OpenChest(pc);
        }
    }

    public void OnInteractHold() { }
    public void OnInteractEnd() { }

    private void OpenChest(PlayerController pc)
    {
        isOpen = true;
       
        if (pc.GetHeldItem() != null)
        {
            Destroy(pc.GetHeldItem().gameObject);
            pc.ClearHeldItem();
        }
        Debug.Log("Сундук был открыт!Ура!Ура!Ура!");
    }

    
    public void ForceOpen()
    {
        isOpen = true;
    }
}

