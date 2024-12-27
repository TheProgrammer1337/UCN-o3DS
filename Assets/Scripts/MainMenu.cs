using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For loading scenes
using System.Collections;

public class MainMenu : MonoBehaviour
{
    // Variables
    public int[] currentAI = new int[56]; // Array to store AI levels
    public Image[] AIImages; // Array of AI images (50 images)
    public Text[] AITexts; // Array of AI level texts (50 texts)
    public string[] Descriptions; // Array of AI descriptions (50 descriptions)
    public Text Description; // UI Text for displaying the description
    public Image AIImage; // UI Image to display the selected AI image
    public AudioSource AIChangeSound; // Sound to play when AI level is changed (both increase and decrease)

    // New variables
    public Text CurrentPointValue; // Text to display current point value
    public Text Highscore; // Text to display the highscore
    public Text Best5020Time; // Text to display the best 50/20 mode time

    public bool UseApplicationDataPath = true; // Flag to determine if Application.persistentDataPath should be used
    public string PathToUse; // Custom path to use if UseApplicationDataPath is false

    public CanvasGroup[] Groups; // CanvasGroups for fade effect

    private int currentSelected = 0; // Index of currently selected AI
    private int highscore = 0; // Variable to store highscore
    private int best5020Minutes = 0; // Variable to store best 50/20 mode minutes
    private int best5020Seconds = 0; // Variable to store best 50/20 mode seconds
    private int best5020Milliseconds = 0; // Variable to store best 50/20 mode milliseconds

    private string saveKey = "currentAI"; // Key to save/load AI levels
    private string highscoreKey = "Highscore"; // Key to save/load highscore
    private string best5020TimeKey = "Best5020Time"; // Key to save/load best 50/20 mode time

    private bool isHolding = false;
    private Coroutine holdCoroutine; // Coroutine to manage the hold behavior
    private bool DeisHolding = false;
    private Coroutine DeholdCoroutine; // Coroutine to manage the hold behavior

    void Start()
    {
        // Load the AI levels, highscore, and best 50/20 mode time when the script starts
        LoadAILevels();
        LoadHighscore();
        LoadBest5020Time();

        // Initialize the UI with the current AI
        UpdateAllAITexts();
        UpdateAllAIImagesAlpha();
        UpdateAISelection();
        UpdateCurrentPointValue();
        UpdateHighscoreText();

        // Fade in all CanvasGroups
        StartCoroutine(FadeInGroups());
    }

    void Update()
    {
        // Handle navigation with keypad input
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            currentSelected = (currentSelected - 10 + 50) % 50; // Move up 10, wrap around
            UpdateAISelection();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentSelected = (currentSelected + 10) % 50; // Move down 10, wrap around
            UpdateAISelection();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            currentSelected = (currentSelected + 1) % 50; // Move right 1, wrap around
            UpdateAISelection();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            currentSelected = (currentSelected - 1 + 50) % 50; // Move left 1, wrap around
            UpdateAISelection();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(FadeOutGroupsAndLoadScene("WinNightLoader"));
        }
    }

    // Method to set isHolding to true (called when the button is pressed down)
    public void StartHold()
    {
        isHolding = true;
        holdCoroutine = StartCoroutine(HoldIncreaseCoroutine());
    }

    // Method to set isHolding to false (called when the button is released)
    public void EndHold()
    {
        isHolding = false;
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
        }
    }

    // Coroutine to handle the hold behavior
    private IEnumerator HoldIncreaseCoroutine()
    {
        float holdTime = 0f;
        float initialWait = 0.5f; // Initial wait time before starting to accelerate
        float interval = 0.1f; // Time interval between increments
        float speedMultiplier = 1f; // Multiplier for speed increase

        while (isHolding)
        {
            // Increase the value if holding
            IncreaseCurrentSelectedAILevel();

            holdTime += interval;

            if (holdTime > initialWait)
            {
                // If holding for more than 2 seconds, increase speed
                interval = Mathf.Max(0.05f, interval - 0.01f); // Reduce interval to increase speed
                speedMultiplier += 0.1f;
            }

            yield return new WaitForSeconds(interval / speedMultiplier);
        }
    }

    // Method to set isHolding to true (called when the button is pressed down)
    public void StartHoldNeg()
    {
        DeisHolding = true;
        DeholdCoroutine = StartCoroutine(HoldDecreaseCoroutine());
    }

    // Method to set isHolding to false (called when the button is released)
    public void EndHoldNeg()
    {
        DeisHolding = false;
        if (DeholdCoroutine != null)
        {
            StopCoroutine(DeholdCoroutine);
        }
    }

    // Coroutine to handle the hold behavior
    private IEnumerator HoldDecreaseCoroutine()
    {
        float holdTime = 0f;
        float initialWait = 0.5f; // Initial wait time before starting to accelerate
        float interval = 0.1f; // Time interval between increments
        float speedMultiplier = 1f; // Multiplier for speed increase

        while (DeisHolding)
        {
            // Increase the value if holding
            DecreaseCurrentSelectedAILevel();

            holdTime += interval;

            if (holdTime > initialWait)
            {
                // If holding for more than 2 seconds, increase speed
                interval = Mathf.Max(0.05f, interval - 0.01f); // Reduce interval to increase speed
                speedMultiplier += 0.1f;
            }

            yield return new WaitForSeconds(interval / speedMultiplier);
        }
    }

    // Function to increase the current selected AI level
    public void IncreaseCurrentSelectedAILevel()
    {
        if (currentAI[currentSelected] < 20) // Limit AI level to 20 maximum
        {
            currentAI[currentSelected]++;
            //SaveAILevels();
            UpdateAILevelText();
            UpdateAIImageAlpha(currentSelected);
            UpdateCurrentPointValue();
            AIChangeSound.Play();
        }
    }

    // Function to decrease the current selected AI level
    public void DecreaseCurrentSelectedAILevel()
    {
        if (currentAI[currentSelected] > 0) // Limit AI level to 0 minimum
        {
            currentAI[currentSelected]--;
            //SaveAILevels();
            UpdateAILevelText();
            UpdateAIImageAlpha(currentSelected);
            UpdateCurrentPointValue();
            AIChangeSound.Play();
        }
    }

    // Function to set all AI levels to 1
    public void AddAll1()
    {
        for (int i = 0; i < 50; i++)
        {
            currentAI[i]++;
            if (currentAI[i] > 20) currentAI[i] = 20; // Ensure max limit of 20
            if (i < 50)
            {
                AITexts[i].text = currentAI[i].ToString();
            }
            if (i < 50)
            UpdateAIImageAlpha(i);
        }
        //SaveAILevels();
        UpdateCurrentPointValue();
    }

    // Function to set all AI levels to 5
    public void SetAll5()
    {
        for (int i = 0; i < 50; i++)
        {
            currentAI[i] = 5;
            if (i < 50)
            {
                AITexts[i].text = currentAI[i].ToString();
            }
            if (i < 50)
            UpdateAIImageAlpha(i);
        }
        //SaveAILevels();
        UpdateCurrentPointValue();
    }

    // Function to set all AI levels to 20
    public void SetAll20()
    {
        for (int i = 0; i < 50; i++)
        {
            currentAI[i] = 20; // Set all AI levels to max 20
            if (i < 50)
            {
                AITexts[i].text = currentAI[i].ToString();
            }
            if (i < 50)
            UpdateAIImageAlpha(i);
        }
        //SaveAILevels();
        UpdateCurrentPointValue();
    }

    // Function to set all AI levels to 0
    public void SetAll0()
    {
        for (int i = 0; i < 50; i++)
        {
            currentAI[i] = 0; // Set all AI levels to 0
            if (i < 50)
            {
                AITexts[i].text = currentAI[i].ToString();
            }
            if (i < 50)
            UpdateAIImageAlpha(i);
        }
        //SaveAILevels();
        UpdateCurrentPointValue();
    }

    // Function to update the AI image, description, and level text for the selected AI
    private void UpdateAISelection()
    {
        AIImage.sprite = AIImages[currentSelected].sprite;
        Description.text = AIImages[currentSelected].name + ": " + Descriptions[currentSelected];
        UpdateAILevelText();
    }

    // Function to update the current AI level text for the selected AI
    private void UpdateAILevelText()
    {
        AITexts[currentSelected].text = currentAI[currentSelected].ToString();
    }

    // Function to update all AI level texts at the start
    private void UpdateAllAITexts()
    {
        for (int i = 0; i < 50; i++)
        {
            AITexts[i].text = currentAI[i].ToString();
        }
    }

    // Function to update all AI images' alpha based on their AI level
    private void UpdateAllAIImagesAlpha()
    {
        for (int i = 0; i < 50; i++)
        {
            UpdateAIImageAlpha(i);
        }
    }

    // Function to update a single AI image's alpha based on its AI level
    private void UpdateAIImageAlpha(int index)
    {
        Color color = AIImages[index].color;
        color.a = currentAI[index] == 0 ? 128f / 255f : 255f / 255f; // Set alpha to 128 (50% opacity) if AI level is 0, otherwise fully opaque
        AIImages[index].color = color;
    }

    // Function to calculate and update the current point value
    private void UpdateCurrentPointValue()
    {
        int totalPoints = 0;
        for (int i = 0; i < 50; i++)
        {
            totalPoints += currentAI[i] * 10; // Each level adds 10 points, so level 1 = 10 points, level 20 = 200 points
        }
        CurrentPointValue.text = totalPoints.ToString();
    }

    // Function to update the highscore text
    private void UpdateHighscoreText()
    {
        Highscore.text = highscore.ToString();
    }

    // Function to save the current AI levels
    private void SaveAILevels()
    {
        string path = UseApplicationDataPath ? Application.persistentDataPath : PathToUse;
        DataManager.SaveValue(saveKey, currentAI, path);
    }

    // Function to load the AI levels
    private void LoadAILevels()
    {
        if (DataManager.ValueExists(saveKey, "data:/"))
        {
            currentAI = DataManager.GetValue<int[]>(saveKey, "data:/");
            
            // Check if the length is exactly 50 and resize if necessary
            if (currentAI.Length == 50)
            {
                int[] resizedAI = new int[56];
                for (int i = 0; i < currentAI.Length; i++)
                {
                    resizedAI[i] = currentAI[i]; // Copy existing values
                }
                for (int i = 50; i < resizedAI.Length; i++)
                {
                    resizedAI[i] = 0; // Set new indices to 0
                }
                currentAI = resizedAI; // Update the reference
            }
            for (int i = 50; i < currentAI.Length; i++)
            {
                currentAI[i] = 0; // Set new indices to 0
            }
        }
        else
        {
            // Initialize all AI levels to 0 if no saved data exists
            currentAI = new int[56]; // Default length to 56
            for (int i = 0; i < currentAI.Length; i++)
            {
                currentAI[i] = 0;
            }
        }
    }

    // Function to load the highscore
    private void LoadHighscore()
    {
        if (DataManager.ValueExists(highscoreKey, Application.persistentDataPath))
        {
            highscore = DataManager.GetValue<int>(highscoreKey, Application.persistentDataPath);
        }
        else
        {
            highscore = 0; // Default highscore to 0 if no saved data exists
        }
    }

    // Function to load the best 50/20 time
    private void LoadBest5020Time()
    {
        if (DataManager.ValueExists(best5020TimeKey, "data:/"))
        {
            float time = DataManager.GetValue<float>(best5020TimeKey, "data:/");

		    int minutes = Mathf.FloorToInt(time / 60f);
    	    int seconds = Mathf.FloorToInt(time % 60f);
    	    int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f / 10);

    	    // Update the Timer UI with the formatted time
    	    Best5020Time.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
        else
        {
            Debug.Log("???");
            best5020Minutes = 0;
            best5020Seconds = 0;
            best5020Milliseconds = 0;
        }
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

    // Coroutine to fade out CanvasGroups
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

        SceneManager.LoadScene(sceneName); // Load the scene after fade-out
    }

    // Function to fade out and load the ControlsScene
    public void GO()
    {
        SaveAILevels();
        StartCoroutine(FadeOutGroupsAndLoadScene("ControlsSceneLoader"));
        int totalPoints = 0;
        for (int i = 0; i < currentAI.Length; i++)
        {
            totalPoints += currentAI[i] * 10; // Each level adds 10 points, so level 1 = 10 points, level 20 = 200 points
        }

        DataManager.SaveValue<int>("currentPoints", totalPoints, "data:/");
    }
}
