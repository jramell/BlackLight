using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public void LoadDemo()
    {
        SceneManager.LoadScene("Demo");
    }
}
