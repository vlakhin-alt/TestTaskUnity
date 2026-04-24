using UnityEngine;

[CreateAssetMenu(fileName="PlayerConfig",menuName="Configs/PlayerConfig")]
public class PlayerConfig:ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float lookSensitivity=2f;

    [Header("Interaction")]
    public float interactionRange=10f;
}
