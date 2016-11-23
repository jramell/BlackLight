using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour {

    public GameObject[] credits;

    private int current;

    private int maxLength;
	// Update is called once per frame

    void Awake()
    {
        maxLength = credits.Length;
    }

	void Update () {
	    if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1))
        {
            credits[current].SetActive(false);
            current++;
            if(current < maxLength)
            {
            credits[current].SetActive(true);
            }
            else
            {
                SceneLoader.LoadMainMenu();
            }
        }
	}
}
