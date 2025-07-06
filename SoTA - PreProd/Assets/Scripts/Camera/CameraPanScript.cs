using System.Collections;
using UnityEngine;
/// <summary>
/// Author:Karin
/// 
/// Modified by: Linus
/// 
/// </summary>
public class CameraPanScript : MonoBehaviour
{
    public static CameraPanScript Instance;

    public float panSpeed = 5f;
    private Vector3 targetPosition;

    public Vector3 TargetPosition 
    { 
        get 
        { 
            return targetPosition;
        }
        set
        {
            targetPosition = value;
            transform.position = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        targetPosition = transform.position;
    }

    public void PanCamera(Vector2 direction)
    {
        targetPosition = transform.position + new Vector3(direction.x * 11f, 0, direction.y * 11f);

        StopAllCoroutines(); 
        StartCoroutine(SmoothPan());
    }

    private IEnumerator SmoothPan()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; 
    }
}
