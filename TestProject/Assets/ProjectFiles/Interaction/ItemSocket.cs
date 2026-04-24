using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UserInteraction;


public class ItemSocket:MonoBehaviour,IInteractable
{
[SerializeField] private Item expectedItem;
[SerializeField] private Transform itemPlaceholder;


private Item currentItem;
private PlayerController playerController;

public string GetInteractionPrompt()
    {
        if (currentItem!=null) return "Е -взять";
        else return "E - положить";
    }
public bool CanInteract(GameObject player)
    {
        playerController=player.GetComponent<PlayerController>();
        Item held=playerController.GetHeldItem();
        if (currentItem==null) return held!=null &&(expectedItem==null || held == expectedItem);
        else return held==null;
    }
public void OnInteractStart()
    {
        Item held=playerController.GetHeldItem();
        if (currentItem== null &&held!= null)
        PlaceItem(held);
        else if (currentItem!=null && held==null)
        TakeItem();
    }
public void OnInteractHold(){}
public void OnInteractEnd(){}
public void PlaceItem(Item item)
{
    currentItem = item;
    

    Transform parent = itemPlaceholder != null ? itemPlaceholder : transform;
    item.transform.SetParent(parent);
    

    item.transform.localPosition = Vector3.zero;
    item.transform.localRotation = Quaternion.identity;
    item.transform.localScale = Vector3.one;
    

    Collider col = item.GetComponent<Collider>();
    if (col != null) col.enabled = true;
    
    MeshRenderer renderer = item.GetComponent<MeshRenderer>();
    if (renderer != null) renderer.enabled = true;
    
    item.SetSocket(this);
    playerController.ClearHeldItem();
}
public void ReturnItem(Item item)
    {
        PlaceItem(item);
    }
private void TakeItem()
    {
        if (currentItem!=null) 
        {playerController.PickUpItemDirectly(currentItem);
        currentItem=null;
        }
    }

}
