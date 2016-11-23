using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    public Text title;
    public Image startButton;
    public Text startButtonText;

    //Time it will for the title to appear completely in seconds
    public float timeOfTitleFade;

    //Time it will for the start button to appear completely in seconds
    public float timeOfStartButtonFade;

    //Time in seconds the script waits from the moment if finishes fading in the title to start the fade-in of the start button
    public float timeBetweenFades;

    private AudioSource backgroundMusic;

    public Image[] mainMenuButtons;

    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        backgroundMusic.Play();
        yield return new WaitForSeconds(1.5f);
        Color tempColorTitle = title.color;
        float rateOfFade = 1 / timeOfTitleFade * 0.01f;
        while (title.color.a < 1)
        {
            tempColorTitle.a += rateOfFade;
            title.color = tempColorTitle;
            yield return new WaitForSeconds(rateOfFade);
        }
        yield return new WaitForSeconds(timeBetweenFades);

        //Color tempStartButtonColor = startButton.color;
        //Color tempStartTextColor = startButtonText.color;

        //rateOfFade = 1 / timeOfStartButtonFade * 0.01f;
        //while (startButton.color.a < 1)
        //{
        //    tempStartButtonColor.a += rateOfFade;
        //    tempStartTextColor.a += rateOfFade;
        //    startButton.color = tempStartButtonColor;
        //    startButtonText.color = tempStartTextColor;
        //    yield return new WaitForSeconds(rateOfFade);
        //}
        int length = mainMenuButtons.Length;
        for (int i = 0; i < length; i++)
        {
            Text currentButtonText = mainMenuButtons[i].transform.Find("Text").gameObject.GetComponent<Text>();
            Color tempStartButtonColor = mainMenuButtons[i].color;
            Color tempStartTextColor = currentButtonText.color;

            rateOfFade = 1 / timeOfStartButtonFade * 0.01f;
            while (mainMenuButtons[i].color.a < 1)
            {
                tempStartButtonColor.a += rateOfFade;
                tempStartTextColor.a += rateOfFade;
                mainMenuButtons[i].color = tempStartButtonColor;
                currentButtonText.color = tempStartTextColor;
                yield return new WaitForSeconds(rateOfFade);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    //private IEnumerator Loop()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(backgroundMusic.clip.length + 3);
    //        backgroundMusic.Play();
    //    }
    //}

    public void LoadDemo()
    {
        SceneManager.LoadScene("Room");
    }

    public void LoadOpenTutorial()
    {
        SceneManager.LoadScene("Terrain");
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public static void LoadSceneWithName(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnCreditsClick()
    {
        SceneManager.LoadScene("Credits");
    }
}
