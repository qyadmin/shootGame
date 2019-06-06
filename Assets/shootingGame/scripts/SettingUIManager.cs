using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour
{
    [SerializeField]
    Button Restart, Exit;

    // Start is called before the first frame update
    void Awake()
    {
        Restart.onClick.AddListener(delegate() {
            Restart_Application();
        });


        Exit.onClick.AddListener(delegate () {
            EditorUI._Instance.worningUI.Type = worningType.handle;
            EditorUI._Instance.worningUI.tital.text = "退出程序";
            EditorUI._Instance.worningUI.msg.text = "确认是否退出程序";
            EditorUI._Instance.worningUI.cancel.onClick.AddListener(delegate () {
                EditorUI._Instance.worningUI.gameObject.SetActive(false);
            });
            EditorUI._Instance.worningUI.determine.onClick.AddListener(delegate () {
                Application.Quit();
            });
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
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
