using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public event SwitchAwake m_SwitchStop;

    public bool timeTrial;

    GameObject ScreenHUD;

    TimeTrialManager timer;
    private void Awake()
    {
        _Instance = this;
        m_SwitchAwake += OnStart;
        m_SwitchStop += ViewStop;
        ScreenHUD = GameObject.Find("ScreenHUD");
        timer = GameObject.Find("Timer").GetComponent<TimeTrialManager>();
    }
    public void Start()
    {
        if (isEditor)
        {
            Player.SetActive(false);
            ThirdCam.SetActive(false);
            EditorCam.SetActive(true);
            ScreenHUD.SetActive(false);
            m_SwitchStop();
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


    public void ViewStop()
    {
        timeTrial = false;
        Player.GetComponent<CapsuleCollider>().enabled = false;
        ResetPerson();
        ResetSwitch();
        ResetTime();
        ResetWeapon();
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


    void ResetPerson()
    {
        Player.transform.position = new Vector3(0, 0, 25);
    }

    void ResetWeapon()
    {
        Player.GetComponent<ShootBehaviour>().Weapon.ResetBullets();
    }

    void ResetTime()
    {
        timer.suspended();
    }

    void ResetSwitch()
    {
        ShootGameEditor._Instance.Arealist.ForEach(item =>
        {
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().Isnow = false;
        });
    }
}
