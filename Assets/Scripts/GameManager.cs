using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay")]
    public int score = 0;
    public float timeLeft = 90f;

    [Header("Collectible Tracking")]
    public int collectedCount = 0;
    public int totalCollectibles = 30;   // set to 30 in Inspector or auto-detected in Start


    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text targetColorText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public GameObject startPanel;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sfxCorrect;
    public AudioClip sfxWrong;
    public AudioClip bgMusic;
    public AudioClip menuMusic;

    public Collectible.ColorType targetColor;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Show start menu first
        if (startPanel != null)
            startPanel.SetActive(true);

        Time.timeScale = 0f; // Pause game

        // PLAY MENU MUSIC 
        if (audioSource != null && menuMusic != null)
        {
            audioSource.Stop(); // Stop any current playback
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log(" Menu music started!");
        }


    }

    private IEnumerator CountCollectiblesAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // wait 0.2 seconds
        totalCollectibles = FindObjectsOfType<Collectible>().Length;
        Debug.Log($"Total collectibles found: {totalCollectibles}");
    }


    void Update()
    {
        if (isGameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
        }

        UpdateUI();
    }

    //  Updates all UI elements
    public void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(timeLeft)}";

        if (targetColorText != null)
        {
            targetColorText.text = $"Target: {targetColor}";
            targetColorText.color = ColorForType(targetColor); //  match text color to target
        }
    }

    // Called when the player collects a collectible
    public void Collect(Collectible c)
    {
        if (isGameOver) return;

        //  Correct color = +1 point and +5 seconds
        if (c.colorType == targetColor)
        {
            score += 1;
            timeLeft += 5f;
            if (timeLeft > 120f) timeLeft = 120f;

            //  Play correct sound
            if (audioSource != null && sfxCorrect != null)
            {
                Debug.Log(" Correct color collected!");
                audioSource.PlayOneShot(sfxCorrect, 10f);
            }
        }
        else
        {
            //  Wrong color = -1 point and -3 seconds
            score -= 1;
            timeLeft -= 3f;
            if (timeLeft < 0f) timeLeft = 0f;

            //  Play wrong sound 
            if (audioSource != null && sfxWrong != null)
            {
                audioSource.PlayOneShot(sfxWrong, 1f);
                Debug.Log(" Wrong color collected — wrong sound played!");
            }
        }

        collectedCount++; //  Move this outside the if/else so it always increments

        //  Check win condition
        //  Check win condition
        if (collectedCount >= totalCollectibles)
        {
            isGameOver = true;
            Time.timeScale = 0f;

            //  STOP BACKGROUND MUSIC when player wins
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);

                if (finalScoreText != null)
                {
                    if (score > 0)
                    {
                        finalScoreText.text = $"YOU WIN!\nFinal Score: {score}";
                        finalScoreText.color = Color.green;
                    }
                    else
                    {
                        finalScoreText.text = $"YOU LOSE!\nFinal Score: {score}";
                        finalScoreText.color = Color.red;
                    }
                }
            }

            Debug.Log("All collectibles collected!");
            return;
        }


        //  Update UI
        UpdateUI();
    }
    public void RegisterCollectible()
    {
        totalCollectibles++;
    }

    // Randomly selects which color the player should target
    void ChooseTargetColor()
    {
        int r = Random.Range(0, 3);
        targetColor = (Collectible.ColorType)r;

        // Optional: change target color automatically every few seconds
        CancelInvoke(nameof(ChooseTargetColor));
        Invoke(nameof(ChooseTargetColor), Random.Range(8f, 14f));

        UpdateUI();
    }

    //  Converts collectible color type to Unity Color
    Color ColorForType(Collectible.ColorType t)
    {
        switch (t)
        {
            case Collectible.ColorType.Red: return Color.red;
            case Collectible.ColorType.Green: return Color.green;
            case Collectible.ColorType.Blue: return Color.blue;
            default: return Color.white;
        }
    }

    //  Handles when time runs out
    void EndGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        // 🎵 Stop background music only
        if (audioSource != null && audioSource.clip == bgMusic)
        {
            audioSource.Stop();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";

        }

        Debug.Log($" Time’s up! Final Score = {score}");
    }
    // Restart the game
    public void Retry()
    {
        Time.timeScale = 1f; // Ensure game is unpaused
        isGameOver = false; // Reset game over state

        // Destroy the current GameManager instance
        if (Instance == this)
        {
            Instance = null;
        }
        Destroy(gameObject);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //  NEW: Called when the enemy catches the player
    public void PlayerCaught()
    {
        if (isGameOver) return;

        Debug.Log(" Player caught by enemy!");

        isGameOver = true;
        Time.timeScale = 0f;

        // 🎵 Stop background music only
        if (audioSource != null && audioSource.clip == bgMusic)
        {
            audioSource.Stop();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";


        }

    }
    //  ADD THIS ONE METHOD:

    public void StartGame()
    {
        // Hide start panel and start the game
        if (startPanel != null)
            startPanel.SetActive(false);

        Time.timeScale = 1f; // Unpause game

        // 🎵 SWITCH TO GAME MUSIC
        if (audioSource != null && bgMusic != null)
        {
            audioSource.Stop(); // Stop menu music
            audioSource.clip = bgMusic;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log(" Game music started!");
        }

        // Your existing game initialization
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        StartCoroutine(CountCollectiblesAfterDelay());
        ChooseTargetColor();
        UpdateUI();

        Debug.Log(" Game started!");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed — exiting game.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}