using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class AnalyticsUtils : MonoBehaviour {

	// Use this for initialization
	public static void InformFirstEnemyDeath()
    {
        if (PlayerController.ANALYTICS_ACTIVE)
        {
            Analytics.CustomEvent(PlayerController.FIRST_ENEMY_KILL_EVENT_NAME,
            new Dictionary<string, object> {
                { "Time since game start", Time.time},
                { "Amount of health lost", (GameObject.Find("Player").GetComponent<PlayerController>().GetMaxHealthPoints() - GameObject.Find("Player").GetComponent<PlayerController>().GetHealthPoints()) },
                { "Times died", PlayerController.GetAmountOfTimesDied() }
            });
        }
    }

    public static void RegisterDummyEvent()
    {
        if (PlayerController.ANALYTICS_ACTIVE)
        {
            Analytics.CustomEvent(PlayerController.PUNCH_DUMMY_EVENT_NAME,
            new Dictionary<string, object> {
                { "Time since game start", Time.time},
                { "Amount of times punched", GameObject.Find("Player").GetComponent<PlayerController>().GetTimesDummyWasPunched() }
            });
        }
    }

    public static void RegisterQuitEvent()
    {
        if (PlayerController.ANALYTICS_ACTIVE)
        {
            Analytics.CustomEvent(PlayerController.QUIT_EVENT_NAME,
            new Dictionary<string, object> {
                { "Time since game start", Time.time},
                { "currentEvent at quit", TutorialController.GetCurrentEvent() }
            });
        }
    }
}
