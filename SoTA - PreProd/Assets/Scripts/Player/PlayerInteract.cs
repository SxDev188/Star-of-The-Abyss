using TMPro;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Author: Karin, Sixten
/// 
/// Modified by: Gabbriel
/// 
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactionRange = 0.5f;

    private Collider currentHighlighted;

    public float InteractionRange 
    { 
        get
        {
            return interactionRange;
        } 
    }

    private void Update()
    {
        HighlightNearestInteractable();
    }

    private void HighlightNearestInteractable()
    {
        Collider nearest = InsideInteractRange();

        if (currentHighlighted != null && currentHighlighted != nearest)
        {
            HighlighterScript oldHighlighter = currentHighlighted.GetComponent<HighlighterScript>();
            if (oldHighlighter != null)
            {
                oldHighlighter.DisableHighlight();
            }
        }

        if (nearest != null)
        {
            HighlighterScript highlighter = nearest.GetComponent<HighlighterScript>();
            if (highlighter != null)
            {
                highlighter.EnableHighlight();
            }
        }

        currentHighlighted = nearest;
    }

    private Collider InsideInteractRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Collider closestCollider = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        return closestCollider;
    }

    private void OnInteract()
    {
        Collider closestCollider = InsideInteractRange();
        if (closestCollider != null)
        {
            closestCollider.GetComponent<IInteractable>().Interact();
        }
    }

    // Visualizes the interaction range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}