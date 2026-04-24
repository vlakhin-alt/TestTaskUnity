using UnityEngine;
using System.Collections;
using UserInteraction;

public class ValveController : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float maxRotationAngle = 360f;
    [SerializeField] private float returnSpeed = 180f;
    [SerializeField] private DoorController connectedDoor;

    [Header("Rotation")]
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    private float currentAngle = 0f;
    private bool isHolding = false;
    private Coroutine returnCoroutine;

    public string GetInteractionPrompt() => "E - крутить вентиль";
    public bool CanInteract(GameObject player) => true;
    public void OnInteractStart() { }

    public void OnInteractHold()
    {
        isHolding = true;
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        float delta = rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Min(currentAngle + delta, maxRotationAngle);
        ApplyRotation();

        if (connectedDoor != null)
            connectedDoor.SetOpenAmount(currentAngle / maxRotationAngle);
    }

    public void OnInteractEnd()
    {
        isHolding = false;
        returnCoroutine = StartCoroutine(ReturnToZero());
    }

    private void ApplyRotation()
    {
        Quaternion target = Quaternion.AngleAxis(currentAngle, rotationAxis);
        if (wheelTransform != null)
            wheelTransform.localRotation = target;
        else
            transform.localRotation = target;
    }

    private IEnumerator ReturnToZero()
    {
        while (currentAngle > 0f && !isHolding)
        {
            float step = returnSpeed * Time.deltaTime;
            currentAngle = Mathf.Max(currentAngle - step, 0f);
            ApplyRotation();

            if (connectedDoor != null)
                connectedDoor.SetOpenAmount(currentAngle / maxRotationAngle);
            yield return null;
        }
        returnCoroutine = null;
    }
}