using UnityEngine;
using UnityEngine.InputSystem;
using UserInteraction;
using TMPro;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.XR.Haptics;
using Unity.VisualScripting;
public enum PlayerState
{
    FreeMovement,
    InspectingItem,
    Dialogue
}
public class PlayerController:MonoBehaviour
{
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Interaction")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private LayerMask interactionMask=~0;
    private IInteractable currentInteractable;
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;
    private IInteractable heldInteractable;
    [Header("Item Inspection")]
    [SerializeField] private float inspectionDistance=1.5f;

    private Item currentInspectingItem;
    private Item heldItem;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed=5f;
    [SerializeField] private float lookSensivity=2f;
    
    private bool isHolding;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalLookRotation;
    
    private PlayerState currentState=PlayerState.FreeMovement;
    private PlayerInputActions inputActions;
    
    private void Awake()
    {
        inputActions=new PlayerInputActions();

        if(characterController==null)
        characterController=GetComponent<CharacterController>();

        if(cameraTransform==null)
        cameraTransform=Camera.main.transform;
        
        if (config == null)
    {
        config = ScriptableObject.CreateInstance<PlayerConfig>();
        config.moveSpeed = 5f;
        config.lookSensitivity = 2f;
        config.interactionRange = 3f;
        Debug.LogWarning("Проблема снова вылезла с конфигом игрока!!");
    }
    }

    private void OnEnable()
    {

        inputActions.Player.Enable();

        inputActions.Player.Move.performed+=OnMove;
        inputActions.Player.Move.canceled+=OnMove;

        inputActions.Player.Look.performed+=OnLook;
        inputActions.Player.Look.canceled+=OnLook;

        inputActions.Player.Interact.performed+=OnInteractPerformed;
        inputActions.Player.Interact.canceled += OnInteractCanceled;

    }
    private void OnDisable()
    {
        inputActions.Player.Move.performed-=OnMove;
        inputActions.Player.Move.canceled-=OnMove;
        inputActions.Player.Look.performed-=OnLook;
        inputActions.Player.Look.canceled-=OnLook;

         inputActions.Player.Interact.performed -= OnInteractPerformed;
         inputActions.Player.Interact.canceled  -= OnInteractCanceled;
  

        inputActions.Player.Disable();

    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput=ctx.ReadValue<Vector2>();

    }
    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput= ctx.ReadValue<Vector2>();
    }
    

    private void Update()
    {
        switch (currentState)
        {
        case PlayerState.FreeMovement:
        HandleMovement();
        HandleLook();
        DetectInteractable();

        if (isHolding && heldInteractable != null) 
        heldInteractable.OnInteractHold();
        break;

        case PlayerState.InspectingItem:
        HandleItemInspection();
        break;

        case PlayerState.Dialogue:
        break;
    }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.right*moveInput.x+transform.forward*moveInput.y;
        characterController.SimpleMove(moveDirection*moveSpeed);

    }
    private void HandleLook()
    {
        float mouseX=lookInput.x*lookSensivity;
        transform.Rotate(Vector3.up*mouseX);

        float mouseY=lookInput.y*lookSensivity;
        verticalLookRotation-=mouseY;
        verticalLookRotation=Mathf.Clamp(verticalLookRotation,-90f,90f);
        cameraTransform.localRotation=Quaternion.Euler(verticalLookRotation,0f,0f);
        
    }
    private void DetectInteractable()
    {
        Ray ray=new Ray(cameraTransform.position,cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,config.interactionRange, interactionMask))
        {
            IInteractable interactable =hit.collider.GetComponent<IInteractable>();
            if(interactable !=null && interactable.CanInteract(gameObject))
            {

                if(currentInteractable!=interactable)
                {
                    currentInteractable=interactable;
                    UpdateInteractionPrompt(interactable.GetInteractionPrompt());

                }
                return;
            } 
        }
        if(currentInteractable!=null)
        {
            currentInteractable=null;
            UpdateInteractionPrompt(string.Empty);
        }
    }
  private void OnInteractPerformed(InputAction.CallbackContext ctx)
{
    switch (currentState)
    {
        case PlayerState.FreeMovement:
            if (ctx.interaction is TapInteraction)
            {
                currentInteractable?.OnInteractStart();
            }
            else if (ctx.interaction is HoldInteraction)
            {
                isHolding = true;
                heldInteractable = currentInteractable;
            }
            break;

        case PlayerState.InspectingItem:
            if (ctx.interaction is TapInteraction)
            {
                ExitInspection(true);   // второе нажатие E — подобрать
            }
            break;

        case PlayerState.Dialogue:
            // ввод обрабатывается кнопками UI
            break;
    }
}

private void OnInteractCanceled(InputAction.CallbackContext ctx)
{
    if (currentState != PlayerState.FreeMovement) return;

    if (isHolding && heldInteractable != null)
    {
        heldInteractable.OnInteractEnd();
        isHolding = false;
        heldInteractable = null;
    }
}
    private  void  UpdateInteractionPrompt(string prompt)
    {
        if (promptText==null)
        return;

        promptText.text=prompt;
        promptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));

    }
    public void EnterInspection(Item item)
    {
        if (currentState != PlayerState.FreeMovement) return;
        currentInspectingItem =item;
        SetState(PlayerState.InspectingItem);
        currentInspectingItem.StartInspection(this,cameraTransform);
    }
    public void SetState(PlayerState newState)
{
    currentState = newState;
    
    
    if (newState == PlayerState.FreeMovement)
        UpdateInteractionPrompt(string.Empty);
    else if (newState == PlayerState.InspectingItem)
        UpdateInteractionPrompt(string.Empty);
}

    private void ExitInspection(bool pickUp)
    {
        if(currentInspectingItem==null) return;
        if(pickUp)
        {
            currentInspectingItem.PickUp();
            heldItem=currentInspectingItem;
        }
        else
        {
            currentInspectingItem.ResetToWorld();
        }
        currentInspectingItem=null;
        SetState(PlayerState.FreeMovement);
    }
    private void HandleItemInspection()
    {
        if(currentInspectingItem!=null)
        {
            currentInspectingItem.UpdateInspection();
        }
    }
    public Item GetHeldItem()
    {return heldItem;}

    public void  ClearHeldItem()
    {
        heldItem=null;
    }
    public void PickUpItemDirectly(Item item)
    {
        heldItem=item;
        item.StartInspection(this,cameraTransform);
        item.PickUp();
    }

}
