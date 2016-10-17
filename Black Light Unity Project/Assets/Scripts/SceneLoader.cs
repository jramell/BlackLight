using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

	public void LoadDemo()
    {
        SceneManager.LoadScene("Room");
    }

    public void LoadOpenTutorial()
    {
        SceneManager.LoadScene("Terrain");
    }

    public static void LoadSceneWithName(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
