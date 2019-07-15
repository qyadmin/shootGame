﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
// This class is created for the example scene. There is no support for this script.
public class InteractiveSwitch : MonoBehaviour
{
    [System.Serializable]
    class MovePoint
    {
        [SerializeField]
        public Transform point;
        [SerializeField]
        public float move_speed;
        [SerializeField]
        public float rotate_speed;
        [SerializeField]
        public float delate_time;
    }

    public List<TargetHealth> targets;
    public List<OtherHealth> others;
    public bool startVisible;
    public InteractiveSwitch nextStage;
    public bool levelEnd;
    public AudioClip StartSound,EndSound;

    public int LevelShootNum;

    private GameObject player;
    private TargetHealth boss;
    private int minionsDead = 0;
    private State currentState;

    private bool isnow = false;

    public bool Isnow
    {
        get
        {
            return isnow;
        }
        set
        {
            isnow = value;
            StopAllCoroutines();
        }
    }

    private TimeTrialManager timer;

    private ThirdPersonOrbitCam orbitcam;

    public int effectiveShooting;

    public int shootingTime;  


    private enum State
    {
        DISABLED,
        MINIONS,
        END
    }

    private void Awake()
    {
        ShootGame._Instance.m_SwitchAwake += OnAwake;
        ShootGame._Instance.m_SwitchStop += OnStop;
    }

    private void OnDestroy()
    {
        ShootGame._Instance.m_SwitchAwake -= OnAwake;
        ShootGame._Instance.m_SwitchStop -= OnStop;
        if(timer && timer.LevelTimerEvent != null)
        timer.LevelTimerEvent -= mandatory_nextStage;
    }

    private void OnStart()
    {
        timer.LevelTimerEvent += mandatory_nextStage;

        others.ForEach(item => {
            item.TargetResets();
        });
        targets.ForEach(item => {
            item.TargetResets();
        });
        targets.ForEach(item => {
            item.ResetDate();
        });
        if (startVisible)
        {
            StopAllCoroutines();
            Moving();
        }
            
    }
    void OnStop()
    {
        ToggleState(false, true);
        timer.EndLevelTimer();
        StopAllCoroutines();
        foreach (TargetHealth target in targets)
        {
                target.End();
        }
    }
    void OnAwake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //effectiveShooting = targets.Count * 2;

        this.ToggleState(false, startVisible);
        timer = GameObject.Find("Timer").GetComponent<TimeTrialManager>();
        orbitcam = GameObject.Find("Main Camera").GetComponent<ThirdPersonOrbitCam>();
        //bullet_num = GameObject.Find("bullet_num").GetComponent<Text>();

        if (levelEnd)
        {
            currentState = State.END;
        }
        else
            currentState = State.DISABLED;



        OnStart();
    }


    void Update()
    {
        switch (currentState)
        {
            case State.MINIONS:
                minionsDead = 0;
                foreach (TargetHealth target in targets)
                {
                    if (!target.boss && target.IsDead)
                    {
                        minionsDead++;
                    }
                }

                Debug.Log(minionsDead+"  "+ targets.Count);
                if (minionsDead == targets.Count)
                {
                    LevelShootNum = player.GetComponent<ShootBehaviour>().Level_shoot_num;
                    timer.EndLevelTimer();
                    this.ToggleState(false, false);
                    isnow = false;
                    if (nextStage)
                    {
                        nextStage.ToggleState(false, true);
                        StopAllCoroutines();
                        nextStage.Moving();
                    }
                    else
                    {
                        if (levelEnd)
                        {
                            timer.EndTimer();
                            timer.EndLevelTimer();
                            ToggleState(false, false);

                            if (nextStage)
                            {
                                nextStage.ToggleState(false, true);
                            }
                            if (EndSound)
                                AudioSource.PlayClipAtPoint(EndSound, transform.position + Vector3.up);
                        }
                    }
                    
                    //EditorUI._Instance.timerUI.shootNum.text = string.Format("剩余{0}次射击机会", effectiveShooting - LevelShootNum);
                }               
                break;
        }





        if (!isnow)
            return;
        LevelShootNum = player.GetComponent<ShootBehaviour>().Level_shoot_num;
        //EditorUI._Instance.timerUI.shootNum.text = string.Format("剩余{0}次射击机会", effectiveShooting - LevelShootNum);

        if (player.GetComponent<ShootBehaviour>().Level_shoot_num >= effectiveShooting)
        {
            mandatory_nextStage();
        }
    }




    void mandatory_nextStage()
    {
        if (!isnow)
            return;
        timer.EndLevelTimer();
        this.ToggleState(false, false);
        isnow = false;
        if (nextStage)
        {
            nextStage.ToggleState(false, true);
            StopAllCoroutines();
            nextStage.Moving();
        }
        else
        {
            if (levelEnd)
            {
                timer.EndTimer();
                timer.EndLevelTimer();
                ToggleState(false, false);

                if (nextStage)
                {
                    nextStage.ToggleState(false, true);
                }
                if(EndSound)
                AudioSource.PlayClipAtPoint(EndSound, transform.position + Vector3.up);
            }
        }

    }

    public void ToggleState(bool active, bool visible)
    {
        if (active)
            currentState = State.MINIONS;
        else
            currentState = State.DISABLED;
        this.GetComponent<BoxCollider>().enabled = visible;
        //this.GetComponent<MeshRenderer>().enabled = visible;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (ShootGame._Instance.timeTrial && !timer.IsRunning)
            {
                timer.StartTimer();
                if(startVisible)
                AudioSource.PlayClipAtPoint(StartSound, transform.position + Vector3.up);
            }
            ToggleState(true, false);
            timer.StartLevelTimer(shootingTime);
            //foreach (TargetHealth i in targets)
            //{
            //    if (i.transform.parent && i.transform.parent.gameObject.GetComponent<obstacles_event>())
            //        i.transform.parent.gameObject.GetComponent<obstacles_event>().swich = true;
            //}

            player.GetComponent<ShootBehaviour>().Level_shoot_num = 0;
            isnow = true;
            foreach (TargetHealth target in targets)
            {
                if (!target.boss)
                {
                    target.Revive();
                }
                else
                {
                    boss = target;
                    boss.Kill();
                }
            }
            foreach (OtherHealth otherobj in others)
            {
                otherobj.Revive();
            }
        }
    }

    public void Moving()
    {
        StartCoroutine(move_to_point());
        //Debug.Log(player.transform.position+"   "+this.transform.position);
        //if (!IsMove)
        //    StartCoroutine(AutoMove(player.transform, this.transform));
    }

    IEnumerator move_to_point()
    {
        
        NavMeshAgent playerNav = player.gameObject.GetComponent<NavMeshAgent>();
        while (Vector3.Distance(player.transform.position, this.transform.position) > 0.1f)
        {
            
            playerNav.SetDestination(this.transform.position);                 
            yield return null;
        }
       
        while (Quaternion.Angle(player.transform.rotation, this.transform.rotation)>1)
        {         
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, this.transform.rotation, 10 * Time.deltaTime);
            yield return null;
        }
    }
    public float moveSpeed = 10f;//角色前进速度
    private CharacterController cc;//角色控制器
    public bool IsMove = false;//是否正在寻路过程

    IEnumerator AutoMove(Transform starPoint, Transform targetPoint)
    {
        IsMove = true;

        yield return new WaitForFixedUpdate();
        //运用A星算法计算出到起点到目标点的最佳路径
        Vector3[] ways = AStarRun._Instance.AStarFindWay(starPoint.position, targetPoint.position);


        if (ways.Length == 0)
        {
            IsMove = false;
            yield break;
        }

        ////打印显示出寻路线
        //foreach (var v in ways)
        //{
        //    GameObject way = Instantiate<GameObject>(wayLook);

        //    way.transform.parent = waysParent;
        //    way.transform.localPosition = v;
        //    way.transform.rotation = Quaternion.identity;
        //    way.transform.localScale = Vector3.one;
        //}


        //让玩家开始沿着寻路线移动
        int i = 0;
        Vector3 target = new Vector3(ways[i].x, transform.position.y, ways[i].z);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Debug.Log("run run run !!!");

            //cc.SimpleMove(transform.forward * moveSpeed * Time.deltaTime);
            //player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, 1 * Time.deltaTime);
            //player.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            
            if (i > ways.Length * 0.8f)
            {
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetPoint.rotation, 10 * Time.deltaTime);
                player.transform.position = Vector3.Lerp(player.transform.position, targetPoint.position, 10 * Time.deltaTime);
            }
            else
            {
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, getlookatRoation(target), 5 * Time.deltaTime);
                player.transform.position = Vector3.Lerp(player.transform.position, target, 20 * Time.deltaTime);
            }
                
            if (Vector3.Distance(player.transform.position, target) < 0.1f)
            {
               
                Debug.Log("run is ok !!!");
                ++i;
                if (i >= ways.Length)
                    break;
                target = new Vector3(ways[i].x, player.transform.position.y, ways[i].z);               
                //player.transform.LookAt(target);
                //player.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target), 0.3f);
            }
            
        }

        //移动完毕，删除移动路径
        //for (int child = waysParent.childCount - 1; child >= 0; --child)
        //    Destroy(waysParent.GetChild(child).gameObject);

        //等待执行下一次自动寻路
        IsMove = false;

    }

    Quaternion getlookatRoation(Vector3 target)
    {
        Quaternion lookat_rotation = new Quaternion();
        Transform raw_rotation = player.transform;
        raw_rotation.LookAt(target);
        lookat_rotation = raw_rotation.rotation;
        return lookat_rotation;
    }
}

