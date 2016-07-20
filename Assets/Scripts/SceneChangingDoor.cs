using UnityEngine;
using System.Collections;
using System;

public class SceneChangingDoor : Interactive
{
    //Name of the scene this door opens
    public string sceneToLoad;

    public override void DoAction()
    {
        GetComponent<AudioSource>().Play();

        StartCoroutine(WaitForAudioToStop());
    }

    IEnumerator WaitForAudioToStop()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);

        SceneLoader.LoadSceneWithName(sceneToLoad);
    }
}
