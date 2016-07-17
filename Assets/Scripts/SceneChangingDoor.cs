using UnityEngine;
using System.Collections;
using System;

public class SceneChangingDoor : MonoBehaviour, IInteractable
{
    //Name of the scene this door opens
    public string sceneToLoad;

    //Description of the action this door will take. Appears in the GUI
    public string actionDescription;

    public void DoAction()
    {
        GetComponent<AudioSource>().Play();

        StartCoroutine(WaitForAudioToStop());
    }

    public void UpdateInteractionText()
    {
        GameObject.Find("Player").SendMessage("UpdateInteractionText", actionDescription);
    }

    IEnumerator WaitForAudioToStop()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);

        SceneLoader.LoadSceneWithName(sceneToLoad);
    }
}
