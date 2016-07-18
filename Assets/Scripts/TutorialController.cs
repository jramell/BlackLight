using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

    //Which event is the tutorial currently in?
    private static int currentEvent;

    //--------------------------------------------------------------------------------------------------------------
    //Audio components
    //--------------------------------------------------------------------------------------------------------------

    //The very first dialog Baroth speaks about how the player must be confused. He asks if the player can speak.
    //currentEvent == 0
    private AudioSource firstDialog;

    //The second dialog Baroth speaks about how temporal loss of sensorial capacity is common in newcomers. Explains
    //the world you are going into is really hostile and dangerous, so he wants to help you. He tells him to go outside
    //and goes outside himself
    //currentEvent == 1
    private AudioSource secondDialog;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start ()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        firstDialog = audios[0];
        secondDialog = audios[1];
        StartCoroutine(ManageEvents());
	}

    IEnumerator ManageEvents()
    {
        yield return new WaitForSeconds(2);
        if (currentEvent == 0)
        {
            firstDialog.Play();
            yield return new WaitForSeconds(firstDialog.clip.length);
            //Baroth waits for answer
            yield return new WaitForSeconds(2);
            //Next dialog
            currentEvent++;
        }

        if (currentEvent == 1)
        {
            secondDialog.Play();
            yield return new WaitForSeconds(secondDialog.clip.length);
        }

        if (currentEvent == 2)
        {

        }

        if (currentEvent == 3)
        {

        }
    }
}
