using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static void LoadDemo()
    {
        SceneManager.LoadScene("Demo");
    }

    public static void LoadOpenTutorial()
    {
        //Not implemented
    }
}
