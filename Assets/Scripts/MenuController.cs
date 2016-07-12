using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    //Main related variables
    public GameObject mainButtons;

    //Option related variables
    public Text sensitivityText;

    public GameObject optionsUI;

    void Start()
    {

    }

    //--------------------------------------------------------------------------------------------------------------
    //Option related functions
    //--------------------------------------------------------------------------------------------------------------



    //is executed when the button options is clicked
    public void OnOptionsClick()
    {
        mainButtons.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void IncreaseSensitivity()
    {
        GameObject.Find("Player").SendMessage("IncreaseSensitivity");
    }

    public void DecreaseSensitivity()
    {
        GameObject.Find("Player").SendMessage("DecreaseSensitivity");
    }

    void UpdateSensitivityText(string update)
    {
        sensitivityText.text = update;
    }
}
