using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; 
public class UIController : MonoBehaviour
{
    public GameObject pauseMenu;
    public string mainMenuSceneName = "Main Menu";
    private bool isPaused = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze the game
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
    }

    public void OnResumeButtonPress()
    {
        isPaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
    }

    public void OnQuitButtonPress()
    {
        StartCoroutine(DisconnectAndLoad());
    }
    

    IEnumerator DisconnectAndLoad()
    {
        Time.timeScale = 1f; // Reset time scale

        // Stop the timer explicitly before leaving
        RoomManager.instance.isTimerRunning = false;

        // Leave the room and wait until the client has left
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.CurrentRoom != null)
            {
                yield return null; // Wait for the client to leave the room
            }
        }

        // Proceed to disconnect if needed
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                yield return null; // Wait for the client to disconnect
            }
        }

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
