using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
    public bool startVisible;
    public InteractiveSwitch nextStage;
    public bool levelEnd;
    public AudioClip StartSound,EndSound;

    private GameObject player;
    private TargetHealth boss;
    private int minionsDead = 0;
    private State currentState;

    private bool isnow = false;

    private TimeTrialManager timer;

    private ThirdPersonOrbitCam orbitcam;

    public int effectiveShooting;

    public int shootingTime;

    private Text bullet_num;


    private enum State
    {
        DISABLED,
        MINIONS,
        END
    }

    private void Awake()
    {
        ShootGame._Instance.m_SwitchAwake += OnAwake;
    }

    private void OnStart()
    {
        timer.LevelTimerEvent += mandatory_nextStage;

        if (startVisible)
        {
            StopAllCoroutines();
            Moving();
        }
            
    }

    void OnAwake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //effectiveShooting = targets.Count * 2;

        this.ToggleState(false, startVisible);
        timer = GameObject.Find("Timer").GetComponent<TimeTrialManager>();
        orbitcam = GameObject.Find("Main Camera").GetComponent<ThirdPersonOrbitCam>();
        bullet_num = GameObject.Find("bullet_num").GetComponent<Text>();

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
                            AudioSource.PlayClipAtPoint(EndSound, transform.position + Vector3.up);
                        }
                    }

                    bullet_num.text = string.Format("剩余{0}次射击机会", effectiveShooting - player.GetComponent<ShootBehaviour>().Level_shoot_num);
                }               
                break;
        }





        if (!isnow)
            return;
        bullet_num.text = string.Format("剩余{0}次射击机会", effectiveShooting - player.GetComponent<ShootBehaviour>().Level_shoot_num);

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
        }
    }

    public void Moving()
    {
        StartCoroutine(move_to_point());
    }

    IEnumerator move_to_point()
    {
        while (Vector3.Distance(player.transform.position, this.transform.position) > 0.1f)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, 1 * Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, this.transform.rotation, 1 * Time.deltaTime);
            yield return null;
        }
    }

}

