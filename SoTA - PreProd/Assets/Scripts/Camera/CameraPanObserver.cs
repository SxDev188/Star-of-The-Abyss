using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Author:Linus
/// 
/// Modified by:
/// 
/// </summary>
public class CameraPanObserver : MonoBehaviour
{
    private PlayerSegment player;
    [SerializeField] private float panSpeed = 5f;
    private Vector3 cameraStartPosition;
    private Vector3 cameraSavePosition;
    private void OnSegmentChanged()
    {
        Vector3 segmentPosition = player.GetCurrentSegmentPosition();
        Vector3 targetPosition = cameraStartPosition + segmentPosition;
        StopAllCoroutines();
        SaveStateManager.Instance.Save();
        StartCoroutine(SmoothPan(targetPosition));
    }
    private void Awake()
    {
        cameraStartPosition = transform.position;
        cameraSavePosition = cameraStartPosition;
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<PlayerSegment>();
        }
        if (player != null)
        {
            player.SegmentChanged += OnSegmentChanged;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.SegmentChanged -= OnSegmentChanged;
        }
    }
    public Vector3 GetCameraSavePosition()
    {
        return cameraSavePosition;
    }
    public void SetCameraPosition(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothPan(position));
    }
    private IEnumerator SmoothPan(Vector3 targetPosition)
    {
        cameraSavePosition = targetPosition;
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
