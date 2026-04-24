using UnityEngine;

public class DoorController:MonoBehaviour
{
   [Header("Settings")]
   [SerializeField] private float openHeight=4f;
   [SerializeField] private Vector3 closedPosition;


private void Start()
    {
        closedPosition=transform.position;
    }

public void SetOpenAmount(float amount)
    {
        amount =Mathf.Clamp01(amount);
        Vector3 targetPos=closedPosition+Vector3.up*(openHeight*amount);
        transform.position=targetPos;
    }
}
