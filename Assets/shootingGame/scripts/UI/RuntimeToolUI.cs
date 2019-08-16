using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeToolUI : MonoBehaviour
{

    public Button Move_Button, Rotate_Button, Scale_Button,Lock_Button,View_Button,Save_Button, previousPage;

    public Sprite Lock, UnLock;

    public Transform toolsGroup;
    private void OnEnable()
    {
        Move_Button.interactable = false;
        Rotate_Button.interactable = false;
        Scale_Button.interactable = false;
        Lock_Button.interactable = false;
    }

    private void Awake()
    {
        Lock = Resources.Load<Sprite>("UI/Image/" + LocalizationManager.GetInstance.GetValue("30001"));
        UnLock = Resources.Load<Sprite>("UI/Image/" + LocalizationManager.GetInstance.GetValue("30002"));
    }

    private void Start()
    {
        previousPage.onClick.AddListener(delegate() {
            EditorUI._Instance.worningUI.Type = worningType.handle;
            EditorUI._Instance.worningUI.tital.text = LocalizationManager.GetInstance.GetValue("20009");
            EditorUI._Instance.worningUI.msg.text = LocalizationManager.GetInstance.GetValue("20010");
            EditorUI._Instance.worningUI.cancel.onClick.AddListener(delegate() {
                EditorUI._Instance.worningUI.gameObject.SetActive(false);
            });
            EditorUI._Instance.worningUI.determine.onClick.AddListener(delegate () {
                StartCoroutine(BeginLoading());
            });
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
        });
    }

    AsyncOperation asyn;
    IEnumerator BeginLoading()
    {
        asyn = SceneManager.LoadSceneAsync("ChoiceScene");
        yield return asyn;
    }
}
