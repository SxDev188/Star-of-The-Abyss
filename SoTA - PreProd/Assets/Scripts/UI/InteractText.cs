using UnityEngine;

/// <summary>
/// Author: Sixten
/// 
/// Modified by: Gabbriel
/// 
/// </summary>

public class InteractText : MonoBehaviour
{
    //IIRC this whole source file is from the tutorial but with minor (if any) changes
    // Written by myself though

    [SerializeField] private float hideDelay = 0.5f;
    [SerializeField] private GameObject interactObjectText;

    private float playerInteractionRange = 0f;
    private float interactionRangeOffset = 0.21f; //since the interaction range check happens differently here than in the PlayerInteract script (the highlighting logic), we need an offset to make them feel the same
    private float timeSinceLeftRange;
    private bool isShowingText = false;

    public void Start()
    {
        playerInteractionRange = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteract>().InteractionRange;
    }

    private Collider InsideInteractRange()
    {
        // If we find the player in the range (which our object has) we return the player collider.
        
        // Might been a bit more effective to have the player only check for interaction and check if it was the "lore tile" but this system allows
        // us to just drag & drop the script and you have interaction text :)

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerInteractionRange + interactionRangeOffset);

        foreach (Collider collider in hitColliders)
        {
            if(collider.CompareTag("Player"))
                return collider;
        }

        return null;
    }

    public void FixedUpdate() 
    {
        Collider player = InsideInteractRange();

        if (player != null)
        {
            if (!isShowingText)
            {
                ShowInteractText();
            }
            timeSinceLeftRange = 0f;
        }
        else
        {
            if (isShowingText) // A little delay before we hide the text again
            {
                timeSinceLeftRange += Time.fixedDeltaTime;
                if (timeSinceLeftRange >= hideDelay)
                {
                    HideInteractText();
                }
            }
        }
    }

    private void ShowInteractText()
    {
        interactObjectText.gameObject.SetActive(true);
        isShowingText = true;
    }

    private void HideInteractText()
    {
        interactObjectText.gameObject.SetActive(false);
        isShowingText = false;
    }
}
