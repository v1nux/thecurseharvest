//using UnityEngine;

//public class toggleMenu : MonoBehaviour
//{
//    public GameObject canvasGroup; // Assign your Menu Canvas or Panel here in the Inspector
//    private bool isPaused = false;

//    void Update()
//    {
//        // Use Input.GetKeyDown to ensure it only triggers once per press
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (isPaused)
//            {
//                Resume();
//            }
//            else
//            {
//                Pause();
//            }
//        }
//    }

//    public void Resume()
//    {
//        canvasGroup.SetActive(false);
//        Time.timeScale = 1f; // Resume game time
//        isPaused = false;

//        // Optional: Lock cursor back to game
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

//    void Pause()
//    {
//        canvasGroup.SetActive(true);
//        Time.timeScale = 0f; // Freeze game time
//        isPaused = true;

//        // Optional: Show cursor for menu navigation
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//    }
//}
