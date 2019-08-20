using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text loadinglabel;

    [SerializeField]
    Transform worningPlan;
    [SerializeField]
    Text msg;
    bool loadfinish = false;

    [SerializeField]
    Transform language;
    [SerializeField]
    Transform Client;
    [SerializeField]
    Button click;
    [SerializeField]
    Dropdown languagelist;
    [SerializeField]
    Dropdown versionlist;
    [SerializeField]
    Text clientmsg;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1366, 768, true);
        StartCoroutine(BeginLoading());
        SocketClient.socketClient.ClientStartCallBack += Client_bengin;
        SocketClient.socketClient.ClientSucCallBack += Client_suc;
        SocketClient.socketClient.ClientFailCallBack += Client_fail;
        click.onClick.AddListener(delegate () {
            loadScene();
        });
        if (Xml_ShootingItem._Instance.existXml)
        {
            versionlist.gameObject.SetActive(true);
            versionlist.onValueChanged.AddListener(delegate (int i)
            {
                if (i == 1)
                    clientmsg.text = string.Format("ip:{0}  port:{1}", SocketClient.socketClient.GetIp, SocketClient.socketClient.GetPort);
                if (i == 0)
                    clientmsg.text = string.Empty;
            });
        }
        else
        {
            versionlist.gameObject.SetActive(false);
        }
        
    }

    void loadScene()
    {

        if (versionlist.value == 0)
        {
            //LocalizationManager.GetInstance.CleanDic();
            if (languagelist.value == 0)
                LocalizationManager.GetInstance.setlanguage("Chinese");
            if (languagelist.value == 1)
                LocalizationManager.GetInstance.setlanguage("English");
            asyn.allowSceneActivation = true;
        }
            
        if (versionlist.value == 1)
        {          
            SocketClient.socketClient.StartClient();
        }

    }

    void Client_bengin(string msg)
    {
        Client.gameObject.SetActive(true);
    }
    void Client_fail(string msg)
    {
        Client.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        Client.Find("Button").GetComponent<Button>().onClick.AddListener(delegate() {
            Client.gameObject.SetActive(false);
        });
        Client.Find("Button/Text").GetComponent<Text>().text = msg;

    }
    void Client_suc(string msg)
    {
        Client.Find("Button/Text").GetComponent<Text>().text = msg;
        if (languagelist.value == 0)
            LocalizationManager.GetInstance.setlanguage("Chinese");
        if (languagelist.value == 1)
            LocalizationManager.GetInstance.setlanguage("English");
        asyn.allowSceneActivation = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (asyn != null && !loadfinish)
        {
            slider.value = asyn.progress;
            loadinglabel.text = "加载进度：" + (slider.value * 100) + "%";
        }
        if (asyn != null && asyn.progress == 0.9f)
        {
            language.gameObject.SetActive(true);
        }
    }
    AsyncOperation asyn;
    IEnumerator BeginLoading()
    {
#if UNITY_ANDROID
        StartCoroutine(Xml_ShootingItem._Instance.DowLoad());
#else
        Xml_ShootingItem._Instance.OnStart();
#endif
        yield return new WaitForSeconds(1);

#if UNITY_ANDROID
        if (Xml_ShootingItem._Instance.existXml)
            asyn = SceneManager.LoadSceneAsync("TimeTrial");
        else
        {
            worningPlan.gameObject.SetActive(true);
            msg.text = "无法找到配置文件，请导入配置文件后再试";
        }
#else
        if (Xml_ShootingItem._Instance.existXml)
        {
            asyn = SceneManager.LoadSceneAsync("TimeTrial");
            asyn.allowSceneActivation = false;
        }

        else
        {
            asyn = SceneManager.LoadSceneAsync("ChoiceScene");
            asyn.allowSceneActivation = false;
        }
            
#endif

        yield return asyn;
    }
}
