using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    public static EditorUI _Instance;

    public RuntimeToolUI runtimeToolUI;

    public AreaListUI areaListUI;

    public ItemListUI itemListUI;

    public AreaItemListUI areaItemListUI;

    public AreaEditorUI areaEditorUI;

    public ItemEditorUI itemEditorUI;

    public SettingUIManager settingUIManager;

    public WorningUI worningUI;

    public PublishTypeUI publishTypeUI;

    public GameFinishUI gameFinishUI;

    public TimerUI timerUI;
    private void Awake()
    {
        _Instance = this;
        worningUI.gameObject.SetActive(false);
    }

    public void editorModle()
    {
        runtimeToolUI.toolsGroup.gameObject.SetActive(true);
        areaListUI.gameObject.SetActive(true);
        itemListUI.gameObject.SetActive(true);
        areaItemListUI.gameObject.SetActive(true);
        areaEditorUI.gameObject.SetActive(false);
        runtimeToolUI.Save_Button.interactable = true;
        settingUIManager.gameObject.SetActive(false);
    }

    public void playingModle()
    {
        runtimeToolUI.toolsGroup.gameObject.SetActive(false);
        areaListUI.gameObject.SetActive(false);
        itemListUI.gameObject.SetActive(false);
        areaItemListUI.gameObject.SetActive(false);
        areaEditorUI.gameObject.SetActive(false);
        runtimeToolUI.Save_Button.interactable = false;
        settingUIManager.gameObject.SetActive(false);
    }

    public void GameModle()
    {
        runtimeToolUI.toolsGroup.gameObject.SetActive(false);
        areaListUI.gameObject.SetActive(false);
        itemListUI.gameObject.SetActive(false);
        areaItemListUI.gameObject.SetActive(false);
        areaEditorUI.gameObject.SetActive(false);
        runtimeToolUI.gameObject.SetActive(false);
        settingUIManager.gameObject.SetActive(true);
    }
}
