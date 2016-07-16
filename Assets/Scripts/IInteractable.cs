using UnityEngine;
using System.Collections;

//Interface for interactive objects. Limiting the interaction is their responsability
public interface IInteractable {

    //Show the action in the GUI
    void UpdateInteractionText();

    //Do the action this interactive object does
    void DoAction();

}
