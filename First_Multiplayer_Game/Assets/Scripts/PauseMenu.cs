using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerMovement playerMovement;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update() { }

    public void btn_Resume()
    {
        Time.timeScale = 1;
        playerMovement.set_PauseGame(false);
    }

    public void btn_Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}