using System;
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

    [Header("clip Pergamino")]
    [SerializeField] private AudioClip clipPergamine;

    private AudioSource mAudioSource;
    private Animator mAnimator;

    //----------------------------------------------------

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }

    //----------------------------------------------------

    private void Start()
    {
        settingsPanel.SetActive(false);
    }

    //----------------------------------------------------

    public void ShowSettingsPanel()
    {
        //Activamos o desactivamos el Panel de Settings dependiendo de su estado

        settingsPanel.SetActive(true);
        buttonsParent.SetActive(false);
        Title.SetActive(false);  

    }

    //----------------------------------------------------

    public void HideSettingsPanel()
    {
        //Activamos o desactivamos el Panel de Settings dependiendo de su estado
        
        settingsPanel.SetActive(false);
        buttonsParent.SetActive(true);
        Title.SetActive(true);

    }

    //----------------------------------------------------

    public void QuitGame()
    {
        Application.Quit();
    }

    //----------------------------------------------------

    public void PlayOptionSound()
    {
        mAudioSource.PlayOneShot(clipPergamine, 1f);
    }

    //----------------------------------------------------

    public void FadeInStartGame()
    {
        mAnimator.Play("FadeIn");
        Invoke(nameof(StartGame), 2.5f);
    }

    //----------------------------------------------------

    public void StartGame()
    {
        SceneManager.LoadScene("ANTONIOTestRoom");
    }
}
