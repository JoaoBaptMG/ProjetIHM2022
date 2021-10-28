using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    // The submenus
    public GameObject mainSubmenu, guideSubmenu, settingsSubmenu;

    // The preselected buttons for each submenu
    public GameObject mainSubmenuPreselectedButton, guideSubmenuPreselectedButton, settingsSubmenuPreselectedButton;

    private void Start()
    {
        DisplayMainMenu();
    }

    public void Play()
    {
        Game.LoadLevel();
    }

    public void Quit()
    {
        Debug.Log("Quitting the game!");
        Application.Quit();
    }

    public void DisplayMainMenu()
    {
        guideSubmenu.SetActive(false);
        settingsSubmenu.SetActive(false);
        mainSubmenu.SetActive(true);

        // Clearing the game object that is considered selected by the event system
        EventSystem.current.SetSelectedGameObject(null);

        // Asigning the main menu preselected button as the game object currently considered selected by the event system
        EventSystem.current.SetSelectedGameObject(mainSubmenuPreselectedButton);
    }

    public void DisplaySettingsMenu()
    {
        mainSubmenu.SetActive(false);
        settingsSubmenu.SetActive(true);

        // Clearing the game object that is considered selected by the event system
        EventSystem.current.SetSelectedGameObject(null);

        // Asigning the main menu preselected button as the game object currently considered selected by the event system
        EventSystem.current.SetSelectedGameObject(settingsSubmenuPreselectedButton);
    }

    public void DisplayGuideMenu()
    {
        mainSubmenu.SetActive(false);
        guideSubmenu.SetActive(true);

        // Clearing the game object that is considered selected by the event system
        EventSystem.current.SetSelectedGameObject(null);

        // Asigning the main menu preselected button as the game object currently considered selected by the event system
        EventSystem.current.SetSelectedGameObject(guideSubmenuPreselectedButton);
    }
}
