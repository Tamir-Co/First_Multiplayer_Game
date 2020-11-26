using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Enter_MP_Menu : NetworkBehaviour
{
    [SerializeField] private GameObject create_Menu;
    [SerializeField] private GameObject join_Menu;

    private Button btn_create;
    private Button btn_join;
    private Button btn_back;


    // Start is called before the first frame update
    void Start()
    {
        btn_create = GameObject.Find("btn_CreateGame").GetComponent<Button>();
        btn_join = GameObject.Find("btn_JoinGame").GetComponent<Button>();
        btn_back = GameObject.Find("btn_Back").GetComponent<Button>();
    }

    public void btn_Create_MP()  // create multiplayer game
    {
        create_Menu.SetActive(true);
        //SceneManager.LoadScene("EnterMultiPlayer_Scene");
    }

    public void btn_Join_MP()  // join multiplayer game
    {
        ChangeMenu(join_Menu);
        //SceneManager.LoadScene("EnterMultiPlayer_Scene");
    }

    public void btn_BackToFirstScene()  // Go Back to First_Scene
    {
        SceneManager.LoadScene("First_Scene");
    }

    private void ChangeMenu(GameObject new_menu)
    {
        new_menu.SetActive(true);
        btn_create.GetComponent<Button>().interactable = false;
        btn_join.GetComponent<Button>().interactable = false;
        btn_back.GetComponent<Button>().interactable = false;
        GameObject.Find("btn_CreateGame/txt_CREATE GAME").GetComponent<TextMeshPro>().alpha = 0.4f;
        GameObject.Find("btn_JoinGame/txt_JOIN GAME").GetComponent<TextMeshPro>().alpha = 0.4f;
        GameObject.Find("btn_Back/txt_BACK").GetComponent<TextMeshPro>().alpha = 0.4f;
    }
}