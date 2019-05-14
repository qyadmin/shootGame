using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGame : MonoBehaviour
{

    public static ShootGame _Instance;
    public bool isEditor;

    public GameObject Player;

    public GameObject ThirdCam;

    public GameObject EditorCam;

    public delegate void SwitchAwake();  
  
    public event SwitchAwake m_SwitchAwake;

    public bool timeTrial;

    GameObject ScreenHUD;
    private void Awake()
    {
        _Instance = this;
        m_SwitchAwake += OnStart;
        ScreenHUD = GameObject.Find("ScreenHUD");
    }
    public void Start()
    {
        if (isEditor)
        {
            Player.SetActive(false);
            ThirdCam.SetActive(false);
            EditorCam.SetActive(true);
            ScreenHUD.SetActive(false);
        }
        else
        {
            Player.SetActive(true);
            ThirdCam.SetActive(true);
            EditorCam.SetActive(false);
            ScreenHUD.SetActive(true);
            m_SwitchAwake();
        }
    }

    private void StartGame()
    {
        timeTrial = true;
        Player.GetComponent<CapsuleCollider>().enabled = true;
    }

    IEnumerator delate()
    {
        yield return new WaitForSeconds(5);
        StartGame();
    }
    void OnStart()
    {
        StartCoroutine(delate());
    }

}
