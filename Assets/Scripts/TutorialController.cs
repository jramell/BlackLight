using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour
{

    public Transform frontOfDoor;

    //Speed per frame of Baroth
    public float speed;

    //--------------------------------------------------------------------------------------------------------------
    //Variables
    //--------------------------------------------------------------------------------------------------------------

    //Which event is the tutorial currently in?
    //Event 0: firstDialog plays
    //Event 1: secondDialog plays. Baroth leaves the room
    //Event 2: The player is now outside. Baroth explains health bar.
    private static int currentEvent;

    //Should Baroth be moving?
    private bool shouldMove;

    //Should Baroth leave the room?
    private bool leave;

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

    void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        firstDialog = audios[0];
        secondDialog = audios[1];
        shouldMove = false;
        StartCoroutine(ManageEvents());
    }

    void Update()
    {
        if (shouldMove)
        {
            transform.position = NPCUtils.MoveTo(transform.position, frontOfDoor.position, speed);
            if (transform.position == frontOfDoor.position)
            {
                LeaveRoom();
            }
        }
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
            shouldMove = true;
            
        }

        if (currentEvent == 2)
        {

        }

        if (currentEvent == 3)
        {

        }
    }

    void LeaveRoom()
    {
        GameObject.Find("Door").GetComponent<AudioSource>().Play();
        Destroy(gameObject, GameObject.Find("Door").GetComponent<AudioSource>().clip.length);
    }
}
