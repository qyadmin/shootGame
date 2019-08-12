using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneNameUI : MonoBehaviour
{
    [SerializeField]
    public Image BackGround;
    [SerializeField]
    public Text SceneName;


    IEnumerator Animation(Action action)
    { 
        SceneName.color = new Color(SceneName.color.r,SceneName.color.g,SceneName.color.b, 1);
        BackGround.color = new Color(BackGround.color.r, BackGround.color.b, BackGround.color.g, 0.5f);
        if (Xml_ShootingItem._Instance.existXmlfuntion() && Xml_ShootingItem._Instance.GetSceneName() != null)
            EditorUI._Instance.sceneNameUI.SceneName.text = Xml_ShootingItem._Instance.GetSceneName();
        else
            EditorUI._Instance.sceneNameUI.SceneName.text = "SceneName";
        float color_a = 1;
        yield return new WaitForSeconds(2);
        SceneName.text = "Shooter ,Are you ready ?";
        yield return new WaitForSeconds(2);
        SceneName.text = "StandBy !";
        yield return new WaitForSeconds(UnityEngine.Random.Range(3,6));      
        action.Invoke();

        while (color_a > 0)
        {
            color_a -= Time.deltaTime;
            SceneName.color = new Color(SceneName.color.r, SceneName.color.g, SceneName.color.b, color_a);
            BackGround.color = new Color(BackGround.color.r, BackGround.color.b, BackGround.color.g, color_a/ 2);
            yield return null;
        }
        
    }
    IEnumerator StandBy;
    public void OnStart(Action start)
    {
        StandBy = Animation(start);
        StartCoroutine(StandBy);
    }
    public void OnEnd()
    {
        if (StandBy != null)
            StopCoroutine(StandBy);
        SceneName.color = new Color(SceneName.color.r, SceneName.color.g, SceneName.color.b, 0);
        BackGround.color = new Color(BackGround.color.r, BackGround.color.b, BackGround.color.g, 0);
    }

    private void OnDisable()
    {
        if(StandBy != null)
            StopCoroutine(StandBy);
        SceneName.color = new Color(SceneName.color.r, SceneName.color.g, SceneName.color.b, 0);
        BackGround.color = new Color(BackGround.color.r, BackGround.color.b, BackGround.color.g, 0);
    }
}
