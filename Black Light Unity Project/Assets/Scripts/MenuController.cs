using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    //Main related variables
    public GameObject mainButtons;

    //Option related variables
    public Text sensitivityText;

    public GameObject optionsUI;

    public GameObject extraAudioContainer;

    //--------------------------------------------------------------------------------------------------------------
    //Option related functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        StartCoroutine(ExtraSound());
    }

    IEnumerator ExtraSound()
    {
        yield return StartCoroutine(Utils.WaitForSecondsRealtime(1.5f));

        AudioSource[] audios = extraAudioContainer.GetComponents<AudioSource>();

        for (int i = 0; i < audios.Length; i++)
        {
            audios[i].Play();
            yield return StartCoroutine(Utils.WaitForSecondsRealtime(audios[i].clip.length));
        }
    }

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
