using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TutorialController : Interactive
{

    public Transform frontOfDoor;

    //Speed per frame of Baroth
    public float speed;

    public GameObject roomLight;

    public float textWriteDelay;

    //--------------------------------------------------------------------------------------------------------------
    //Variables
    //--------------------------------------------------------------------------------------------------------------

    //Which event is the tutorial currently in?
    //Event 0: Baroth introduces himself and tells the player he is heading towards a dangerous place.
    //Event 1: secondDialog plays. Baroth leaves the room
    //Event 2: The player is now outside. Baroth explains health bar.
    private static int currentEvent;

    //Should Baroth be moving?
    private bool shouldMove;

    //Should Baroth leave the room?
    private bool leave;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
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
        Debug.Log("started ManageEvents");
        yield return new WaitForSeconds(2);
        if (currentEvent == 0)
        {
           yield return GameObject.Find("Player").GetComponent<PlayerController>().IntroduceText("test text test text test text",
               GetComponent<AudioSource>(), 1);
        }

        if (currentEvent == 1)
        {

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

        //Allows the player to interact with the door
        GameObject.Find("Player").SendMessage("EnableInteractivity");
    }

    //Should be moved to Player Controller later

    public override void DoAction()
    {
        throw new NotImplementedException();
    }
}
