using UnityEngine;
using System.Collections;

public class UIFunctions : MonoBehaviour {

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }

}
