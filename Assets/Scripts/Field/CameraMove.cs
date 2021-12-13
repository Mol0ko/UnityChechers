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
        var startRotation = transform.rotation;
        var endRotation = Quaternion.Euler(new Vector3(0, 180, 0)) * startRotation;
        for (float t = 0; t < 0.7f; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / 0.7f);
            yield return null;
        }
        transform.rotation = endRotation;
    }
}