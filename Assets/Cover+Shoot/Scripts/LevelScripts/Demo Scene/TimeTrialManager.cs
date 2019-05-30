using UnityEngine;
using UnityEngine.UI;

// This class is created for the example scene. There is no support for this script.
public class TimeTrialManager : MonoBehaviour
{
	public Vector3 playerPosition;
	private float bestTime, totalTime, startTime = 0;
	private Text bestTimeLabel, currentTimeLabel,levelTimeLabel;
	private bool isTimerRunning = false;

	private GameObject player;

    [SerializeField]
    private float level_time;

    private float level_runtime;
    private bool level_time_start = false;

    public delegate void timeEvent();

    public timeEvent LevelTimerEvent;

    public bool IsRunning { get { return isTimerRunning; } }

	void Awake()
	{
		currentTimeLabel = this.transform.Find("Current").GetComponent<Text>();
		bestTimeLabel = this.transform.Find("Best").GetComponent<Text>();
        levelTimeLabel = this.transform.Find("LevelTime").GetComponent<Text>();

        if (PlayerPrefs.HasKey("bestTime"))
		{
			bestTime = PlayerPrefs.GetFloat("bestTime");
            bestTimeLabel.text = string.Format("最佳记录:{0}", bestTime.ToString("n2"));
		}
		else
		{
			bestTimeLabel.text = "";
		}
		currentTimeLabel.text = "0.00";
		currentTimeLabel.gameObject.SetActive(false);
		bestTimeLabel.gameObject.SetActive(false);
	}

    private void Start()
    {
        //SettingUIManager._Instance.OnReset();
    }

    private void Update()
	{
		if(isTimerRunning)
		{
            currentTimeLabel.text = string.Format("当局所用时间:{0}", (Time.time - startTime).ToString("n2"));
		}
        if (level_time_start)
        {
            level_runtime -= Time.deltaTime;
            levelTimeLabel.text = string.Format("打靶剩余时间:{0}", level_runtime.ToString("n2")); 



            if (level_runtime <= 0)
            {
                LevelTimerEvent();
            }
        }
	}

    public void StartLevelTimer(int value)
    {
        level_time_start = true;
        level_runtime = value;
        levelTimeLabel.gameObject.SetActive(true);
    }

    public void EndLevelTimer()
    {
        level_time_start = false;
        levelTimeLabel.gameObject.SetActive(false);
    }



    public void StartTimer()
	{
		isTimerRunning = true;
		startTime = Time.time;
		currentTimeLabel.gameObject.SetActive(true);
		bestTimeLabel.gameObject.SetActive(true);
        
	}

    public void suspended()
    {
        isTimerRunning = false;
    }


    public void EndTimer()
	{
		totalTime = Time.time - startTime;
		isTimerRunning = false;
		startTime = 0;

		if (bestTime == 0 || (bestTime > 0 && totalTime < bestTime))
		{
			bestTime = totalTime;
			currentTimeLabel.text = bestTimeLabel.text = bestTime.ToString("n2");
			PlayerPrefs.SetFloat("bestTime", bestTime);
		}
        EditorUI._Instance.settingUIManager.CanRestart();
	}


}
