using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIAnimationsController : MonoBehaviour
{
    [SerializeField] private Animator MenuUIAnimator;

    public void ShowTitle()
    {
        MenuUIAnimator.Play("TitleAppears");
    }

    public void MoveTitle()
    {
        MenuUIAnimator.Play("TitleTranslation");
    }

    public void ShowNMT()
    {
        MenuUIAnimator.Play("NMTCameo");
    }
}
