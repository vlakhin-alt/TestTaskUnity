using UnityEngine;
namespace UserInteraction
{
public interface IInteractable
{
string GetInteractionPrompt();
bool CanInteract(GameObject player);
void OnInteractStart();
void OnInteractHold();
void OnInteractEnd();
}
}