using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
public class ShootGame : MonoBehaviour
{

    public static ShootGame _Instance;
    public bool isEditor;

    public GameObject Player;

    public GameObject ThirdCam;

    public GameObject EditorCam;

    public delegate void SwitchAwake();

    public event SwitchAwake m_SwitchAwake;

    public event SwitchAwake m_SwitchStop;

    public bool timeTrial;

    GameObject ScreenHUD;

    TimeTrialManager timer;
    private void Awake()
    {
        _Instance = this;
        m_SwitchAwake += OnStart;
        m_SwitchStop += ViewStop;
        ScreenHUD = GameObject.Find("ScreenHUD");
        timer = GameObject.Find("Timer").GetComponent<TimeTrialManager>();
        timer.Finish += Getresults;
    }
    public void Start()
    {
        if (isEditor)
        {
            Player.SetActive(false);
            ThirdCam.SetActive(false);
            EditorCam.SetActive(true);
            ScreenHUD.SetActive(false);
            m_SwitchStop();
        }
        else
        {
            Player.SetActive(true);
            ResetPerson();
            ThirdCam.SetActive(true);
            EditorCam.SetActive(false);
            ScreenHUD.SetActive(true);
            m_SwitchAwake();
        }
    }

    private void OnDestroy()
    {
        m_SwitchAwake -= OnStart;
        m_SwitchStop -= ViewStop;
        timer.Finish -= Getresults;
        Timer.Instance.CancelTimer(timerid);
    }


    private void Update()
    {
        Timer.Instance.DoUpdate();
    }


    public void ViewStop()
    {
        timeTrial = false;
        Player.GetComponent<CapsuleCollider>().enabled = false;
        ResetPerson();
        ResetSwitch();
        ResetTime();
        ResetWeapon();
    }

    private void StartGame()
    {
        timeTrial = true;

        Player.GetComponent<CapsuleCollider>().enabled = true;

    }

    int timerid = -1;

    IEnumerator delate()
    {
        yield return new WaitForSeconds(5);
        StartGame();
    }
    void OnStart()
    {       
        timerid = Timer.Instance.AddDeltaTimer(5, 1, 5, StartGame);
        EditorUI._Instance.timerUI.Open();
    }


    void ResetPerson()
    {
        if (ShootGameEditor._Instance.Arealist == null || ShootGameEditor._Instance.Arealist.Count == 0)
            return;
        Quaternion rotation = ShootGameEditor._Instance.Arealist[0].m_ShootingItem[0].Prefab.transform.rotation;
        Vector3 positon = ShootGameEditor._Instance.Arealist[0].m_ShootingItem[0].Prefab.transform.position;

        Player.transform.rotation = rotation;
        Player.transform.position = positon;
        Player.transform.Translate(Vector3.forward * -10);
    }

    void ResetWeapon()
    {
        Player.GetComponent<ShootBehaviour>().Weapon.ResetBullets();
        Player.GetComponent<ShootBehaviour>().ResetBulletHole();
    }

    void ResetTime()
    {
        timer.suspended();
        Timer.Instance.CancelTimer(timerid);
        EditorUI._Instance.timerUI.Close();
    }

    void ResetSwitch()
    {
        ShootGameEditor._Instance.Arealist.ForEach(item =>
        {
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().Isnow = false;
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().IsMove = false;
        });
        
    }
    void Getresults()
    {
        float shoottime_total = 0;
        int shootnum_total = 0;
        int shootnum_miss = 0;
        int shootnum_error = 0;
        int shootnum_getshoot = 0;
        int shootnum_though = 0;

        int steelnum_total = 0;
        int steelnum_shoot = 0;
        int steelnum_miss = 0;
        int steelnum_error = 0;

        int papernum_total = 0;
        int papernum_shoot = 0;
        int papernum_miss = 0;
        int papernum_error = 0;

        int paper_A = 0;
        int paper_C = 0;
        int paper_D = 0;
        ShootGameEditor._Instance.Arealist.ForEach(area =>
        {
            shootnum_total += area.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().LevelShootNum;
            area.m_ShootingItem.ForEach(item =>
            {
                if (item.Prefab.GetComponent<TargetHealth>())
                {
                    TargetHealth targetHealth = item.Prefab.GetComponent<TargetHealth>();
                    switch (targetHealth.getTargetType)
                    {
                        case TargetType.SliceSteel:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    steelnum_shoot++;
                                }
                                steelnum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    steelnum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.Other;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.Other;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.Rotate_double:
                            steelnum_total += 8;

                            if (targetHealth.IsGetshoot)
                            {
                                targetHealth.get_target_resoult.ForEach(target =>
                                {
                                    steelnum_shoot += target.Other;
                                    shootnum_getshoot += target.Other;
                                    shootnum_though += target.Though;
                                });
                            }
                            break;
                        case TargetType.Rotate_single:
                            steelnum_total += 6;

                            if (targetHealth.IsGetshoot)
                            {
                                targetHealth.get_target_resoult.ForEach(target =>
                                {
                                    steelnum_shoot += target.Other;
                                    shootnum_getshoot += target.Other;
                                    shootnum_though += target.Though;
                                });
                            }
                            break;
                        case TargetType.BarSteel:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    steelnum_shoot++;
                                }
                                steelnum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    steelnum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.Other;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.Other;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.IDPA:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.IPSC:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.LRMove:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.UDMove:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.Slide_single:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                        case TargetType.Slide_double:
                            if (!targetHealth.ProhibitShooting && !targetHealth.InvalidItem)
                            {
                                if (targetHealth.IsDead)
                                {
                                    papernum_shoot++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        paper_A += target.A;
                                        paper_C += target.C;
                                        paper_D += target.D;
                                    });
                                }
                                papernum_total++;
                            }

                            if (targetHealth.ProhibitShooting)
                                if (targetHealth.IsGetshoot)
                                {
                                    papernum_error++;
                                    targetHealth.get_target_resoult.ForEach(target =>
                                    {
                                        shootnum_error += target.A + target.C + target.D;
                                    });
                                }
                            targetHealth.get_target_resoult.ForEach(target =>
                            {
                                shootnum_getshoot += target.A + target.C + target.D;
                                shootnum_though += target.Though;
                            });
                            break;
                    }
                }
            });
        });
        shoottime_total = timer.GetTime;
        shootnum_miss = shootnum_total - shootnum_getshoot + shootnum_though;
        steelnum_miss = steelnum_total - steelnum_shoot;
        papernum_miss = papernum_total - papernum_shoot;
        Debug.Log(string.Format("总时间：{0}, 总射击：{1},漏枪：{2},错枪：{3}", shoottime_total, shootnum_total, shootnum_miss, shootnum_error));
        Debug.Log(string.Format("钢靶数量：{0},完成：{1},漏射：{2},错射：{3}", steelnum_total, steelnum_shoot, steelnum_miss, steelnum_error));
        Debug.Log(string.Format("纸靶数量：{0},完成：{1},漏射：{2},错射：{3}", papernum_total, papernum_shoot, papernum_miss, papernum_error));
        Debug.Log(string.Format("A：{0},C：{1},D：{2}", paper_A, paper_C, paper_D));

        EditorUI._Instance.gameFinishUI.shoottime_total.text = shoottime_total.ToString("f2")+"s";
        EditorUI._Instance.gameFinishUI.shootnum_total.text = shootnum_total.ToString();
        EditorUI._Instance.gameFinishUI.shootnum_miss.text = shootnum_miss.ToString();
        EditorUI._Instance.gameFinishUI.shootnum_error.text = shootnum_error.ToString();

        EditorUI._Instance.gameFinishUI.steelnum_total.text = steelnum_total.ToString();
        EditorUI._Instance.gameFinishUI.steelnum_shoot.text = steelnum_shoot.ToString();
        EditorUI._Instance.gameFinishUI.steelnum_miss.text = steelnum_miss.ToString();
        EditorUI._Instance.gameFinishUI.steelnum_error.text = steelnum_error.ToString();

        EditorUI._Instance.gameFinishUI.papernum_total.text = papernum_total.ToString();
        EditorUI._Instance.gameFinishUI.papernum_shoot.text = papernum_shoot.ToString();
        EditorUI._Instance.gameFinishUI.papernum_miss.text = papernum_miss.ToString();
        EditorUI._Instance.gameFinishUI.papernum_error.text = papernum_error.ToString();

        EditorUI._Instance.gameFinishUI.paper_A.text = paper_A.ToString();
        EditorUI._Instance.gameFinishUI.paper_C.text = paper_C.ToString();
        EditorUI._Instance.gameFinishUI.paper_D.text = paper_D.ToString();

        EditorUI._Instance.gameFinishUI.gameObject.SetActive(true);
    }
}
