using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public void MoveToAnotherSide()
    {
        StopAllCoroutines();
        StartCoroutine(RotateAroundCenter());
    }

    private IEnumerator RotateAroundCenter()
    {
        var fieldCenter = new Vector3(4, 0, -4);
        var rotationTime = 1f;
        while (rotationTime > 0)
        {
            transform.RotateAround(fieldCenter, Vector3.up, 180 * Time.deltaTime);
            rotationTime -= Time.deltaTime;
            yield return null;
        }
    }
}