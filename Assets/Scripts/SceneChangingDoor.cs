using UnityEngine;
using System.Collections;
using System;

public class SceneChangingDoor : Interactive
{
    //Name of the scene this door opens
    public string sceneToLoad;

    bool used;

    public override void DoAction()
    {
        if (!used)
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(WaitForAudioToStop());
            used = true;
        }
    }

    IEnumerator WaitForAudioToStop()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        SceneLoader.LoadSceneWithName(sceneToLoad);
    }
}
