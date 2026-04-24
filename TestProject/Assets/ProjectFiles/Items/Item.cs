using UnityEngine;
using UnityEngine.InputSystem; 
using UserInteraction;

public class Item : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "Предмет";
    [SerializeField] private string description = "Описание предмета";
    [SerializeField] private Vector3 inspectionPostitionOffset = new Vector3(-0.5f, 0f, 1f);
    [SerializeField] private Vector3 holdPositionOffset = new Vector3(0.5f, -0.3f, 1f);
    [SerializeField] private float rotationSpeed = 2f;

    public string ItemName => itemName;

    [Header("Key Settings")]
    [SerializeField] private bool isKey = false;
    public bool IsKey => isKey;

    private Transform cameraTransform;
    private PlayerController playerController;
    private ItemSocket originalSocket;
    private bool isInspecting = false;
    private bool isHeld = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Vector3 lastMousePosition;

    protected virtual void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
    }

    public string GetInteractionPrompt()
    {
        if (isHeld) return "Нажмите E чтобы положить";
        if (isInspecting) return "Е - взять";
        return $"E - осмотреть {itemName}";
    }

    public bool CanInteract(GameObject player) => !isHeld && !isInspecting;

    public void OnInteractStart()
    {
        if (!isInspecting && !isHeld)
        {
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
                if (playerController == null)
                {
                    Debug.LogError("PlayerController not found in scene!");
                    return;
                }
            }
            playerController.EnterInspection(this);
        }
    }

    public void OnInteractHold() { }
    public void OnInteractEnd() { }

    public virtual void StartInspection(PlayerController pc, Transform cam)
    {
        playerController = pc;
        cameraTransform = cam;
        isInspecting = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        transform.SetParent(cam);
        Vector3 worldTargetPos = cam.position 
                               + cam.forward * inspectionPostitionOffset.z
                               + cam.right * inspectionPostitionOffset.x
                               + cam.up * inspectionPostitionOffset.y;
        transform.position = worldTargetPos;
        transform.localRotation = Quaternion.identity;
        lastMousePosition = Mouse.current.position.ReadValue();
    }

   
    public virtual void UpdateInspection()
    {
        if (!isInspecting || cameraTransform == null) return;

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 currentMousePos = Mouse.current.position.ReadValue();
            Vector3 delta = currentMousePos - lastMousePosition;
            float rotationX = delta.y * rotationSpeed;
            float rotationY = -delta.x * rotationSpeed;

            Vector3 localUp = transform.InverseTransformDirection(cameraTransform.up);
            Vector3 localRight = transform.InverseTransformDirection(cameraTransform.right);
            transform.Rotate(localUp, rotationY, Space.Self);
            transform.Rotate(localRight, rotationX, Space.Self);
            lastMousePosition = currentMousePos;
        }
        else
        {
            lastMousePosition = Mouse.current.position.ReadValue();
        }
    }

    public virtual void PickUp()
    {
        isInspecting = false;
        isHeld = true;
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.identity;
    }

    public void ReturnToSocket()
    {
        if (originalSocket != null)
            originalSocket.ReturnItem(this);
        else
            ResetToWorld();
    }

    public virtual void ResetToWorld()
    {
        isHeld = false;
        isInspecting = false;
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }

    public void SetSocket(ItemSocket socket) => originalSocket = socket;
    public bool IsHeld() => isHeld;
    public bool IsInspecting => isInspecting;
}