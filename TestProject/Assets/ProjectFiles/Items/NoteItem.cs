using UnityEngine;
using TMPro;
using System.Collections;

public class NoteItem : Item
{
    [Header("Note Settings")]
    [SerializeField] private string noteText = "Внимание! Спасибо за внимание!";
    [SerializeField] private GameObject rightPage;    
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private float animationDuration = 0.5f;

    private Coroutine animationCoroutine;

    protected override void Start()
    {
        base.Start();
    }

    public override void StartInspection(PlayerController pc, Transform cam)
    {
        base.StartInspection(pc, cam);
        if (descriptionText != null)
        {
            descriptionText.text = noteText;
            descriptionText.gameObject.SetActive(true);
        }
        transform.localRotation = Quaternion.Euler(0, -90, -270); 
    
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimatePage(true));
    }

    public override void ResetToWorld()
    {
        base.ResetToWorld();
        if (descriptionText != null)
            descriptionText.gameObject.SetActive(false);
       
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimatePage(false));
    }

    public override void PickUp()
    {
        base.PickUp();
        if (descriptionText != null)
            descriptionText.gameObject.SetActive(false);
    }

    
    public override void UpdateInspection() { }

    private IEnumerator AnimatePage(bool open)
    {
        if (rightPage == null) yield break;

        Quaternion startRot = rightPage.transform.localRotation;
        Quaternion endRot = open ? Quaternion.Euler(-90, 0, 0) : Quaternion.identity;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            rightPage.transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rightPage.transform.localRotation = endRot;
        animationCoroutine = null;
    }
}