using UnityEngine;
using System.Collections;
using System;

public class Door : MonoBehaviour, IInteractable
{
    string action;

    void Start()
    {
        action = "Go outside";
    }
    public void DoAction()
    {
        Debug.Log("DID ACTION");
        //SceneLoader.LoadOpenTutorial();
    }

    public void UpdateInteractionText()
    {
        GameObject.Find("Player").SendMessage("UpdateInteractionText", action);
    }
}
