using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelFailedMenu : MonoBehaviour
{
    public GameObject preselectedButton;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(preselectedButton);
    }

    public void ReloadLevel()
    {
        Game.LoadLevel();
    }

    public void DisplayStartMenu()
    {
        Game.DisplayStartMenu();
    }
}
