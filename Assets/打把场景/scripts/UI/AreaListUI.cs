using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaListUI : MonoBehaviour
{
    [SerializeField]
    public GameObject AreaPerfabUI;
    [SerializeField]
    public GameObject AreaUICount;
    [SerializeField]
    public Button Add, Subtract;
    public Transform LockUI;
    public Sprite ordinary, active;
    private void Start()
    {
        AreaPerfabUI = Resources.Load<GameObject>("UI/ShootAreaUIPerfab");
    }
}
