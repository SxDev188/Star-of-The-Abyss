using System.Collections;
using UnityEngine;
using FMOD.Studio;
/// <summary>
/// Author:Gabbriel, Sixten, Linus
/// 
/// Modified by: 
/// 
/// </summary>
public class BoulderStarPushScript : MonoBehaviour
{

    //this script handles moving the boulder when the star is thrown at it

    [field: Header("Star Push Parameters")]
    [SerializeField] float starPushSpeed = 20f;
    [SerializeField] float starPushDistance = 1f;
    [SerializeField] float starPushCooldown = 0.2f;

    Rigidbody boulderRigidbody;
    BoulderController boulderController;
    BoulderPushController pushController;

    IEnumerator StarPushCoroutine;

    private bool isBeingStarPushed = false;
    public bool IsBeingStarPushed { get { return isBeingStarPushed; } }
    public bool IsCurrentlyMoving { get; private set; } = false;


    [SerializeField] BoulderSideHitbox[] boulderSideHitboxes = new BoulderSideHitbox[4];

    void Start()
    {
        boulderRigidbody = GetComponent<Rigidbody>();
        boulderController = GetComponent<BoulderController>();
        pushController = GetComponent<BoulderPushController>();

    }

    public void CheckSideHitboxes()
    {
        foreach (BoulderSideHitbox sideHitBox in boulderSideHitboxes)
        {
            if(sideHitBox.WasHitByStar)
            {
                StarPushInDirection(sideHitBox.PushDirection, starPushDistance);
                break;
            }
        }

        foreach (BoulderSideHitbox sideHitBox in boulderSideHitboxes)
        {
            sideHitBox.Reset();
        }
    }

    public void StarPushInDirection(Vector3 direction, float distance)
    {
        if (pushController.IsBeingPushed)
        {
            return;
        }


        if (pushController.CheckForValidPushDestination(direction, distance))
        {
            StarPushCoroutine = StarPushInDirection_IEnumerator(direction, distance);
            StartCoroutine(StarPushCoroutine);
        }
    }

    IEnumerator StarPushInDirection_IEnumerator(Vector3 direction, float distance)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.BoulderSFX);

        Vector3 targetDestination = transform.position + direction * distance;

        isBeingStarPushed = true;
        
        //Waits for two fixed updates here. When isKinematic is changed in the line above, that will cause a TriggerExit and then TriggerEnter
        //in the pressure plate. The IsCurrentlyMoving flag is used to make the pressure plate ignore this false exit/enter. 
        //Waiting for two fixed updates makes sure that the isKinematic change is properly ignored BEFORE setting IsCurrentlyMoving to true.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        IsCurrentlyMoving = true; //While this is true, pressure plate will accept the collision with the boulder

        while (Vector3.Distance(transform.position, targetDestination) > pushController.PushDestinationAcceptanceRadius)
        {
            //sets velocity to zero as the starthere could sometimes be a downward force (that was not gravity)
            //still unclear where it came from but setting velocity to 0 seems to fix it!

            if (!boulderRigidbody.isKinematic) //to avoid warning that sometimes would appear in editor
            {
                boulderRigidbody.velocity = new Vector3(0, 0, 0);
            }

            Vector3 tempDirection = targetDestination - transform.position;

            transform.position += tempDirection * starPushSpeed * Time.deltaTime;

            yield return null;
        }

        IsCurrentlyMoving = false; //From this point, the pressure plate will ignore collision with the boulder.
        //Without this, SnapToFloor() below would trigger a false OnTriggerExit and immediate OnTriggerEnter messing up both SFX and particle effects

        boulderController.SnapToFloor(); //looks weird if this snap happens AFTER the cooldown
        yield return new WaitForSeconds(starPushCooldown);

        StopStarPush();
    }

    public void StopStarPush()
    {
        if (StarPushCoroutine != null)
        {
            StopCoroutine(StarPushCoroutine);
        }

        isBeingStarPushed = false;
    }
}
