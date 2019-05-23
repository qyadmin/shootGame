using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeToolUI : MonoBehaviour
{

    public Button Move_Button, Rotate_Button, Scale_Button,Lock_Button,View_Button;

    public Transform toolsGroup;
    private void OnEnable()
    {
        Move_Button.interactable = false;
        Rotate_Button.interactable = false;
        Scale_Button.interactable = false;
        Lock_Button.interactable = false;
    }
}
