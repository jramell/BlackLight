using UnityEngine;
using System.Collections;

public abstract class Interactive : MonoBehaviour {

    public string actionDescription;

    public void UpdateInteractionText()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().UpdateInteractionText(actionDescription);
    }

    public abstract void DoAction();
}
