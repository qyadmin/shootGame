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

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        StartCoroutine(BeginLoading());
    }

    // Update is called once per frame
    void Update()
    {
        if(asyn != null)
        slider.value = asyn.progress;
        loadinglabel.text = "加载进度：" + (slider.value * 100) + "%";
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
            asyn = SceneManager.LoadSceneAsync("TimeTrial");
         else
            asyn = SceneManager.LoadSceneAsync("ChoiceScene");
#endif

        yield return asyn;
    }
}
