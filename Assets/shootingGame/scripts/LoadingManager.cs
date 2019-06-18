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
    Text worning;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        StartCoroutine(BeginLoading());
    }

    // Update is called once per frame
    void Update()
    {
        if(asyn != null)
        slider.value = asyn.progress;
        loadinglabel.text = "加载进度：" + (slider.value * 100) + "%";
        worning.text = Xml_ShootingItem._Instance.path + "             " + Xml_ShootingItem._Instance.existXml;
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
        
        if (Xml_ShootingItem._Instance.existXml)
            asyn = SceneManager.LoadSceneAsync("TimeTrial");
        else
            asyn = SceneManager.LoadSceneAsync("ChoiceScene");
        yield return asyn;
    }
}
