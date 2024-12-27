using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ControlsScene : MonoBehaviour
{
    public CanvasGroup[] Groups; // CanvasGroups for fade effect
    public Button button;

    void Start()
    {
        // Start the fade-in effect when the scene starts
        StartCoroutine(FadeInGroups());
    }

    void Update()
    {
        // Check if the Return or A key is pressed
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.A))
        {
            // Call StartGame method when either key is pressed
            StartGame();
        }
    }

    // Function to start the game (fade-out and load the next scene)
    public void StartGame()
    {
        StartCoroutine(FadeOutGroupsAndLoadScene("NightLoader")); // Replace "GameScene" with the name of your game scene
        button.interactable = false;
    }

    // Coroutine to fade in CanvasGroups
    private IEnumerator FadeInGroups()
    {
        float duration = 2.0f; // Fade-in duration in seconds
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);

            foreach (CanvasGroup group in Groups)
            {
                group.alpha = alpha;
            }

            yield return null;
        }
    }

    // Coroutine to fade out CanvasGroups and load the next scene
    private IEnumerator FadeOutGroupsAndLoadScene(string sceneName)
    {
        float duration = 2.0f; // Fade-out duration in seconds
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsed / duration);

            foreach (CanvasGroup group in Groups)
            {
                group.alpha = alpha;
            }

            yield return null;
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
        SceneManager.LoadScene(sceneName); // Load the next scene after fade-out
    }
}
