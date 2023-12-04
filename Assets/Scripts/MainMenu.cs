using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panel de Settings")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Botones")]
    [SerializeField] private GameObject buttonsParent;

    [Header("Titulo")]
    [SerializeField] private GameObject Title;


    //----------------------------------------------------

    public void ManageSettingsPanel()
    {
        //Activamos o desactivamos el Panel de Settings dependiendo de su estado

        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            buttonsParent.SetActive(true);
            Title.SetActive(true);
        }

        else
        {
            settingsPanel.SetActive(true);
            buttonsParent.SetActive(false);
            Title.SetActive(false);
        }
            
    }

    //----------------------------------------------------

    public void StartGame()
    {
        SceneManager.LoadScene("ANTONIOTestRoom");
    }
}
