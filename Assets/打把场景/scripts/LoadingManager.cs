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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginLoading());
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = asyn.progress;
        loadinglabel.text = "加载进度：" + (slider.value * 100) + "%";
    }
    AsyncOperation asyn;
    IEnumerator BeginLoading()
    {
        asyn = SceneManager.LoadSceneAsync("TimeTrial");
        yield return asyn;
    }
}
