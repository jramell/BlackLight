using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

    public const string POWER_UP_SPEED = "Speed";

    /// <summary>
    /// Waits for a number of realtime seconds passed by parameter
    /// </summary>
    /// <param name="timeInSeconds">the number of seconds the function will wait</param>
    /// <returns>finishes when the number of seconds have passed</returns>
    public static IEnumerator WaitForSecondsRealtime(float timeInSeconds)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + timeInSeconds)
        {
            yield return null;
        }
    }
}
