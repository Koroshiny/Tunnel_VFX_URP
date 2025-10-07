using UnityEngine;

public class VFXCameraController : MonoBehaviour
{
    public Transform targetPosition; 
    private bool isFollowing = false;

    void Update()
    {
        if (isFollowing && targetPosition != null)
        {
            transform.position = targetPosition.position;
            transform.rotation = targetPosition.rotation;
        }
    }

    public void MoveToTarget()
    {
        isFollowing = true;
    }

    public void StopMoving()
    {
        isFollowing = false;
    }
}