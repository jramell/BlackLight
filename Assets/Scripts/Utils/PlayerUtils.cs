using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUtils : MonoBehaviour
{
    public static Color normalTutorialColor = new Color(242f/255f, 183f/255f, 94f/255f, 255f/255f);

    public static Color warningColor = new Color(244f/255f, 94f/255f, 97f/255f, 1);

    /// <summary>
    /// Adds in order to the dialogText every character passed as a parameter with 'textWriteDelay' seconds between each
    /// character addition. Also, plays the writingSoundEffect passed as a parameter the maximum between the amount
    /// of characters in the 'text' string and numberOfPlays.
    /// If numberOfPlays is zero or writingSoundEffect is null, no sound effect will be played.
    /// </summary>
    /// <param name="text">the text to be added to the dialog</param>
    /// <param name="dialogText">the dialog to which the text will be added</param>
    /// <param name="textWriteDelay">the time between character additions to the dialog</param>
    /// <param name="writingSoundEffect">the sound effect that can be played each time a character is written</param>
    /// <param name="numberOfPlays">the number of times the sound effect passed as a parameter will be played. A maximum of once per character is possible.</param>
    /// <returns></returns>
    public static IEnumerator IntroduceText(string text, Text dialogText, float textWriteDelay, AudioSource writingSoundEffect, int numberOfPlays)
    {
        char[] textInChar = text.ToCharArray();
        bool canPlay = writingSoundEffect != null && numberOfPlays > 0;

        for (int i = 0; i < textInChar.Length; i++)
        {
            if (canPlay)
            {
                writingSoundEffect.Play();
                numberOfPlays -= 1;
                canPlay = numberOfPlays > 0;
            }
            dialogText.text += textInChar[i];
            yield return new WaitForSeconds(textWriteDelay);
        }
    }

}
