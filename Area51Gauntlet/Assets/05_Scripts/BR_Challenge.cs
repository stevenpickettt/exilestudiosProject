using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BR_Challenge : MonoBehaviour {
    private BR_ObjectivePanel ObjPanel;
    private BR_GameController gameController;
    public enum ChallengeType { NONE, RANDOM, BLASTER, AUTO, SCATTER, MELEE, KILL, COMBO, DRONEEXPLD, HEALTH, SHIELD, SLOMO, BOSSTIME, TURRET};
    public enum ChallengeReward {BRONZE = 0, SILVER = 1, GOLD = 2};
    public enum ChallengeStatus { STARTED, COMPLETE, FAILED};
    [SerializeField]
    private int[] score; 
    public ChallengeType challengeType;
    public ChallengeReward challengeReward;
    public ChallengeStatus challengeStatus;
    public int challengeNumber;
    [SerializeField]
    private string killText;
    [SerializeField]
    private string comboText;
    [SerializeField]
    private string meleeText;
    [SerializeField]
    private string blasterText;
    [SerializeField]
    private string autoText;
    [SerializeField]
    private string scatterText;
    [SerializeField]
    private string droneExplodeText;
    [SerializeField]
    private string healthText;
    [SerializeField]
    private string shieldText;
    [SerializeField]
    private string slomoText;
    [SerializeField]
    private string bossTimeText;
    [SerializeField]
    private string turretText;
    [SerializeField]
    private Text selfText;
    private string formattedString;
    public int countGoal;
    public int curCount;

    AudioSource audiosource;
    AudioController audioController;



    float timer;
    // Use this for initialization
    void Start()
    {
        ObjPanel = FindObjectOfType<BR_ObjectivePanel>();
        gameController = FindObjectOfType<BR_GameController>();

        audioController = FindObjectOfType<AudioController>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform Header = ObjPanel.challengeHeader.transform;
        gameObject.GetComponent<RectTransform>().position = new Vector3(Header.position.x, Header.position.y - ((1 + challengeNumber) * Screen.height / 18), Header.position.z);
        if (challengeType == ChallengeType.BOSSTIME)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                challengeStatus = ChallengeStatus.FAILED;
                selfText.color = Color.red;
                audioController.Failed_ObjectiveSFX();
            }
        }
       
    }

    public void SetupChallenge(ChallengeType _challengeType, int _challengeNum, int _count,ChallengeReward _reward)
    {
        challengeType = _challengeType;
        challengeNumber = _challengeNum;
        countGoal = _count;
        challengeReward = _reward;
        switch (challengeType)
        {
            case ChallengeType.COMBO:
                formattedString = string.Format(comboText, _count);
                break;

            case ChallengeType.KILL:
                formattedString = string.Format(killText, _count);
                break;

            case ChallengeType.BLASTER:
                formattedString = string.Format(blasterText, _count);
                break;

            case ChallengeType.AUTO:
                formattedString = string.Format(autoText, _count);
                break;

            case ChallengeType.SCATTER:
                formattedString = string.Format(scatterText, _count);
                break;

            case ChallengeType.HEALTH:
                formattedString = healthText;
                break;

            case ChallengeType.SHIELD:
                formattedString = shieldText;
                break;

            case ChallengeType.MELEE:
                formattedString = string.Format(meleeText, _count);
                break;

            case ChallengeType.DRONEEXPLD:
                formattedString = string.Format(droneExplodeText, _count);
                break;

            case ChallengeType.SLOMO:
                formattedString = string.Format(slomoText, _count);
                break;

            case ChallengeType.TURRET:
                formattedString = string.Format(turretText, _count);
                break;

            case ChallengeType.BOSSTIME:
                formattedString = string.Format(bossTimeText, _count);
                timer = _count * 60;
                break;

            case ChallengeType.NONE:
                Debug.LogError("WARNING: None Challenge type selected");
                break;

            case ChallengeType.RANDOM:
                Debug.LogError("WARNING: Random Challenge type selected");
                break;
        }
        selfText.text = formattedString;
        switch (_reward)
        {
            case ChallengeReward.BRONZE:
                selfText.color = new Color32(205, 127, 50, 255);
                return;

            case ChallengeReward.SILVER:
                selfText.color = new Color32(192, 192, 192, 255);
                return;

            case ChallengeReward.GOLD:
                selfText.color = new Color32(255, 215, 0, 255);
                return;
        }
    }

    public void ChallengeTrack(int _count)
    {
        string trackingString = "{0}\n{1} out of {2}";

        switch (challengeType)
        {
            default:
                curCount += _count;
                selfText.text = string.Format(trackingString, formattedString, curCount, countGoal);
                if (curCount == countGoal)
                    DoReward();
                break;

            case ChallengeType.COMBO:
                if (_count >= countGoal)
                    DoReward();
                break;
        }
    }
    public void DoReward()
    {
        challengeStatus = ChallengeStatus.COMPLETE;
        selfText.color = Color.green;
        switch (challengeReward)
        {
            case ChallengeReward.BRONZE:
                selfText.text = selfText.text + " +" + score[0]; 
                gameController.ModHighScoreChallenges(score[0],challengeReward);
                break;
            case ChallengeReward.SILVER:
                selfText.text = selfText.text + " +" + score[1];
                gameController.ModHighScoreChallenges(score[1], challengeReward);
                break;
            case ChallengeReward.GOLD:
                selfText.text = selfText.text + " +" + score[2];
                gameController.ModHighScoreChallenges(score[2], challengeReward);
                break;
        }
    }

}