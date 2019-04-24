using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour
{
    public static SettingUIManager _Instance;

    [SerializeField]
    Button Restart, Exit;

    // Start is called before the first frame update
    void Awake()
    {
        _Instance = this;
        Restart.onClick.AddListener(delegate() {
            Restart_Application();
        });

        Exit.onClick.AddListener(delegate () {
            Exit_Application();
        });
    }

    public void OnReset()
    {
        Restart.gameObject.SetActive(false);
    }

    public void CanRestart()
    {
        Restart.gameObject.SetActive(true);
    }
    
    private void Exit_Application()
    {
        Application.Quit();
    }

    private void Restart_Application()
    {
        SceneManager.LoadSceneAsync("Loading");
    }
    
}
