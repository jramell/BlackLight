using UnityEngine;
using System.Collections;

public class NPCUtils : MonoBehaviour {

    //Moves the start position in a straight line to the target position with a maximum delta of speed
    public static Vector3 MoveTo(Vector3 startPosition, Vector3 targetPosition, float speed)
    {
        return Vector3.MoveTowards(startPosition, targetPosition, speed * Time.deltaTime);
    }
}
