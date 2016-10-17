using UnityEngine;
using System.Collections;

//Template for NPCs, must finish
public abstract class NPC : Interactive {

    private bool talkingToPlayer;

    private string currentLine;

    void FinishConversation()
    {
        talkingToPlayer = false;
    }

}
