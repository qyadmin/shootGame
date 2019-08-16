using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Button determine, cancel;

    [SerializeField]
    Button inside, outside;

    [SerializeField]
    Sprite hight, general;
    [SerializeField]
    Sprite insidebig, outsidebig;
    [SerializeField]
    Image backgroud;

    bool istart = false;
    private void Start()
    {
        determine.image.sprite = Resources.Load<Sprite>("UI/Image/"+LocalizationManager.GetInstance.GetValue("30004"));
        cancel.image.sprite = Resources.Load<Sprite>("UI/Image/" + LocalizationManager.GetInstance.GetValue("30003"));
        determine.onClick.AddListener(delegate(){
            Static.Instance.sceneType = Static.Instance.sceneType;
            slider.gameObject.SetActive(true);
            loadinglabel.gameObject.SetActive(true);
            StartCoroutine(BeginLoading());
        });
        cancel.onClick.AddListener(delegate() {
            Application.Quit();
        });
        inside.onClick.AddListener(delegate () {
            outside.transform.Find("Image").GetComponent<Image>().sprite = general;
            inside.transform.Find("Image").GetComponent<Image>().sprite = hight;
            Static.Instance.sceneType = SceneType.InSide;
            backgroud.sprite = insidebig;
        });
        outside.onClick.AddListener(delegate () {
            outside.transform.Find("Image").GetComponent<Image>().sprite = hight;
            inside.transform.Find("Image").GetComponent<Image>().sprite = general;
            Static.Instance.sceneType = SceneType.OutSide;
            backgroud.sprite = outsidebig;
        });

        outside.onClick.Invoke();
    }

    [SerializeField]
    Slider slider;
    [SerializeField]
    Text loadinglabel;

    // Update is called once per frame
    void Update()
    {
        if (!istart)
            return;
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
