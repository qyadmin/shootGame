using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameFinishUI : MonoBehaviour
{
    public Text shoottime_total;
    public Text shootnum_total;
    public Text shootnum_miss;
    public Text shootnum_error;
    //public Text shootnum_getshoot;
    //public Text shootnum_though;

    public Text steelnum_total;
    public Text steelnum_shoot;
    public Text steelnum_miss;
    public Text steelnum_error ;

    public Text papernum_total;
    public Text papernum_shoot;
    public Text papernum_miss ;
    public Text papernum_error;

    public Text paper_A ;
    public Text paper_C ;
    public Text paper_D ;

    [SerializeField]
    Button Restart, Exit;
    public Button View;
    // Start is called before the first frame update
    void Awake()
    {
        Restart.onClick.AddListener(delegate () {
            Restart_Application();
        });


        Exit.onClick.AddListener(delegate () {
            EditorUI._Instance.worningUI.Type = worningType.handle;
            EditorUI._Instance.worningUI.tital.text = LocalizationManager.GetInstance.GetValue("20011");
            EditorUI._Instance.worningUI.msg.text = LocalizationManager.GetInstance.GetValue("20012");
            EditorUI._Instance.worningUI.cancel.onClick.AddListener(delegate () {
                EditorUI._Instance.worningUI.gameObject.SetActive(false);
            });
            EditorUI._Instance.worningUI.determine.onClick.AddListener(delegate () {
                //SocketClient.socketClient.isRun = false;
                Application.Quit();
            });
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
        });
    }

    private void OnEnable()
    {
        if (ShootGameEditor._Instance.getExistXml)
        {
            Exit.gameObject.SetActive(true);
            Restart.gameObject.SetActive(true);
            View.gameObject.SetActive(false);
        }
        else
        {
            Exit.gameObject.SetActive(false);
            Restart.gameObject.SetActive(false);
            View.gameObject.SetActive(true);
        }
    }

    private void Restart_Application()
    {
        //SocketClient.socketClient.EndClient();
        SceneManager.LoadSceneAsync("TimeTrial");
    }
}
