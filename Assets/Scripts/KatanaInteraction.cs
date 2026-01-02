using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required if using TextMeshPro

public class KatanaInteraction : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject winScreenUI;
    [SerializeField] private GameObject interactionPrompt; // The "Press F" text/image

    private bool _isPlayerInRange = false;

    void Start()
    {
        // Ensure both UI elements are hidden at the start
        if (winScreenUI != null) winScreenUI.SetActive(false);
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }

    void Update()
    {
        // Check if player is in range and presses 'F'
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        if (winScreenUI != null)
        {
            winScreenUI.SetActive(true);
            if (interactionPrompt != null) interactionPrompt.SetActive(false);

            // Pause the game and unlock cursor
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // --- Button Functions for the Win Screen ---
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // --- Proximity Detection ---
    private void OnTriggerEnter(Collider other)
    {
        // Verify the object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }
}