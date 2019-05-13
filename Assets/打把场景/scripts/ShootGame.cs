using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGame : MonoBehaviour
{
   

    public bool isEditor;

    public GameObject Player;

    public GameObject ThirdCam;

    public GameObject EditorCam;

    public delegate void SwitchAwake();  
  
    public static event SwitchAwake m_SwitchAwake;


    public void Start()
    {
        if (isEditor)
        {
            Player.SetActive(false);
            ThirdCam.SetActive(false);
            EditorCam.SetActive(true);
            GameObject.Find("ScreenHUD").SetActive(false);
        }
        else
        {
            Player.SetActive(true);
            ThirdCam.SetActive(true);
            EditorCam.SetActive(false);
            GameObject.Find("ScreenHUD").SetActive(true);
            m_SwitchAwake();
        }
    }


    
}
