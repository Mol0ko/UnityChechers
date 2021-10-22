using System.Collections;
using UnityEngine;

public class ChipStepMove : MonoBehaviour
{
    public void MoveToPosition(int x, int z)
    {
        StopAllCoroutines();
        StartCoroutine(Move(x, z));
    }

    private IEnumerator Move(int x, int z)
    {
        var startingPosition = transform.position;
        var finalPosition = new Vector3(x, transform.position.y, z);
        var moveTime = 1f;
        while (moveTime > 0)
        {
            transform.position = Vector3.Lerp(startingPosition, finalPosition, 1 - moveTime);
            moveTime -= Time.deltaTime;
            yield return null;
        }
    }
}