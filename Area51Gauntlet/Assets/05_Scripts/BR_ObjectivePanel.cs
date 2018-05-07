using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BR_ObjectivePanel : MonoBehaviour {
    public static BR_ObjectivePanel objPanel;
    bool isActive;
    [SerializeField]
    GameObject backGround;
    [SerializeField]
    public GameObject objectiveHeader;
    public GameObject challengeHeader;
    private RectTransform challengeHeader_curPos;
    private Vector3 challengeHeader_defaultPos;
    [SerializeField]
    RectTransform objTxtSpawn;
    [SerializeField]
    RectTransform challengeTxtSpawn;
    public Text challengeScore;

    [SerializeField]
    GameObject ObjTxtPrefab;
    [SerializeField]
    GameObject ChallengeTxtPrefab;
    public GameObject[] curObjectives;
    public GameObject[] curChallenges;
    [SerializeField]
    float activeTime;
    float timer;
    int selection;
    int cScale;
    string header;
    public float h;

    AudioSource audiosource;
    AudioController audioController;


    // Use this for initialization
    void Start () {
        header = objectiveHeader.GetComponent<Text>().text;

        audioController = FindObjectOfType<AudioController>();
        audiosource = GetComponent<AudioSource>();
        challengeHeader.SetActive(false);
    }
   
	
	// Update is called once per frame
	void Update ()
    {
        h = Screen.height/25;
        challengeHeader.transform.position = new Vector3(objectiveHeader.transform.position.x, objectiveHeader.transform.position.y - (((curObjectives.Length)*h) + h) , objectiveHeader.transform.position.z);
		if (isActive)
        {
            backGround.transform.localScale = new Vector3(1f, Mathf.Lerp(0f, yScale(),7.5f*timer), 1f);
            
            if (timer >= .25f)
            {
                if (curChallenges.Length >= 1)
                    challengeHeader.SetActive(true);
                objectiveHeader.SetActive(true);
                if (curObjectives.Length == 0)
                    objectiveHeader.GetComponent<Text>().text = "No Objectives";
                else
                    objectiveHeader.GetComponent<Text>().text = header;
                for (int x = 0; x<curObjectives.Length;x++)
                {
                    if(curObjectives[x] != null)
                    curObjectives[x].GetComponent<Text>().enabled = true;
                }
                for (int x = 0; x < curChallenges.Length; x++)
                {
                    if (curChallenges[x] != null)
                        curChallenges[x].GetComponent<Text>().enabled = true;
                }
            }
            else
                timer += Time.deltaTime;
        }
        else
        {
            backGround.transform.localScale = new Vector3(yScale(), 0, 1f);
            for (int x = 0; x < curObjectives.Length; x++)
            {
                if (curObjectives[x] != null)
                    curObjectives[x].GetComponent<Text>().enabled = false;
            }
            for (int x = 0; x < curChallenges.Length; x++)
            {
                if (curChallenges[x] != null)
                    curChallenges[x].GetComponent<Text>().enabled = false;
            }
            objectiveHeader.SetActive(false);
            challengeHeader.SetActive(false);
            
        }
        if (curChallenges.Length != 0)
            cScale = 1;
        else
            cScale = 0;
	}

    public void SetActive()
    {
        isActive = !isActive;
    }

    public void AddObjective(BR_Objectives.ObjectiveType _objType, string _target)
    {
        
        if (curObjectives.Length < 7)
        {
            bool canAddObj = true;
            for (int x = 0; x < curObjectives.Length;x++)
            {
                if (curObjectives[x].GetComponent<BR_Objectives>().objType == _objType)
                    canAddObj = false;
            }
            if (canAddObj)
            {
                bool stopChecking = false;
                if (curObjectives.Length == 0)
                {
                    selection = 0;
                }
                else for (int x = 0; x < curObjectives.Length && stopChecking == false; x++)
                    {
                        if (curObjectives[x] == null)
                        {
                            selection = x;
                            stopChecking = true;
                        }
                        else
                            selection = x + 1;
                    }
                GameObject[] temp = new GameObject[curObjectives.Length + 1];
                GameObject _objective = Instantiate(ObjTxtPrefab, objectiveHeader.transform);
                temp[selection] = _objective;
                if (curObjectives.Length != 0)
                    curObjectives.CopyTo(temp, 0);
                curObjectives = temp;
                _objective.name = _objective.name + _objType;
                _objective.GetComponent<BR_Objectives>().SetupObjective(_objType, selection, _target);
                //_objective.GetComponent<RectTransform>().position = new Vector3(objectiveHeader.transform.position.x, objectiveHeader.transform.position.y - (selection * 30), objectiveHeader.transform.position.z);
                isActive = true;
            }
            Debug.Log("Objective type " + _objType + " already added");
        }
        else
            Debug.LogError("TOO MANY OBJECTIVES - Max number of Objectives is 7");
    }

    public void CompleteObjective(BR_Objectives.ObjectiveType _objType)
    {
        
        if (curObjectives.Length > 0)
        {
            for (int x = 0; x < curObjectives.Length; x++)
            {
                if (curObjectives[x].GetComponent<BR_Objectives>().objType == _objType)
                {
                    curObjectives[x].GetComponent<BR_Objectives>().CompleteObjective();
                    x = curObjectives.Length;
                }
            }
        }
    }

    public void FailObjective(BR_Objectives.ObjectiveType _objType)
    {

        if (curObjectives.Length > 0)
        {
            for (int x = 0; x < curObjectives.Length; x++)
            {
                if (curObjectives[x].GetComponent<BR_Objectives>().objType == _objType)
                {
                    curObjectives[x].GetComponent<BR_Objectives>().FailObjective();
                    x = curObjectives.Length;
                }
            }
        }
    }

    public void RemoveObjective(BR_Objectives.ObjectiveType _objType)
    {
        if (curObjectives.Length > 1)
        {
            int y = 0;
            GameObject[] temp = new GameObject[curObjectives.Length - 1];
            for (int x = 0; x < curObjectives.Length; x++)
            {
                BR_Objectives check = curObjectives[x].GetComponent<BR_Objectives>();
                if (check.objType == _objType && check.objState == BR_Objectives.ObjectiveState.Complete)
                    Destroy(curObjectives[x]);
                else
                {
                    temp[y] = curObjectives[x];
                    temp[y].GetComponent<BR_Objectives>().objNumber = y;
                    y++;
                }
            }
            curObjectives = temp;
        }
        else if (curObjectives.Length == 1)
        {
            Destroy(curObjectives[0]);
            curObjectives = new GameObject[0];
        }
    }

    public void AddChallenge(BR_Challenge.ChallengeType _challengeType, int _count, BR_Challenge.ChallengeReward _reward)
    {
        if (curChallenges.Length < 7)
        {
            bool stopChecking = false;
            if (curChallenges.Length == 0)
            {
                selection = 0;
            }
            else for (int x = 0; x < curChallenges.Length && stopChecking == false; x++)
            {
                    if (curChallenges[x] == null)
                    {
                        selection = x;
                        stopChecking = true;
                    }
                    else
                        selection = curChallenges.Length;
            }
            GameObject[] temp = new GameObject[curChallenges.Length + 1];
            GameObject _challenge = Instantiate(ChallengeTxtPrefab, challengeHeader.transform);
            temp[selection] = _challenge;
            if (curChallenges.Length != 0)
                curChallenges.CopyTo(temp, 0);
            curChallenges = temp;
            _challenge.name = _challenge.name + _count;
            _challenge.GetComponent<BR_Challenge>().SetupChallenge(_challengeType, selection, _count, _reward);
            //_challenge.GetComponent<RectTransform>().position = new Vector3(challengeHeader.transform.position.x, challengeHeader.transform.position.y - ((1 +selection) * 40), challengeHeader.transform.position.z);
            isActive = true;
        }
        else
            Debug.LogError("TOO MANY CHALLENGES - Max number of Challenges is 7");
    }

    public void CompleteChallenge(BR_Challenge.ChallengeType _challengeType)
    {
        for (int x = 0; x < curChallenges.Length; x++)
        {
            if (curChallenges[x].GetComponent<BR_Challenge>().challengeType == _challengeType)
            {
                curChallenges[x].GetComponent<BR_Challenge>().DoReward();
            }
        }
    }

    public void FailChallenge(BR_Challenge.ChallengeType _challengeType)
    {
        
        for (int x = 0; x < curChallenges.Length; x++)
        {
            if (curChallenges[x].GetComponent<BR_Challenge>().challengeType == _challengeType)
            {
                curChallenges[x].GetComponent<BR_Challenge>().challengeStatus = BR_Challenge.ChallengeStatus.FAILED;
                curChallenges[x].GetComponent<Text>().color = Color.red;
                audioController.Failed_ObjectiveSFX();
            }
        }
    }

    public void RemoveChallenges()
    {
        if (curChallenges.Length > 0)
        {
            GameObject[] temp = new GameObject[0];
            for (int x = 0; x < curChallenges.Length; x++)
            {
                    Destroy(curChallenges[x]);
            }
            curChallenges = temp;
            challengeHeader.SetActive(false);
            cScale = 0;
        }
    }

    public float yScale()
    {
        float scale = Mathf.Clamp(.075f +((.075f * curObjectives.Length) + (.075f * (cScale + curChallenges.Length))),0,1.1f);
        return scale;
    }

    public void ObjectiveTracker(int _select, int _count)
    {
        if (curChallenges[_select].GetComponent<BR_Challenge>().challengeStatus == BR_Challenge.ChallengeStatus.STARTED)
        {
            curChallenges[_select].GetComponent<BR_Challenge>().ChallengeTrack(_count);
        }
    }
    public void ForceRemoveObjectives()
    {
        if (curObjectives.Length > 0)
        {
            int y = 0;
            GameObject[] temp = new GameObject[0];
            for (int x = 0; x < curObjectives.Length; x++)
            {
                if (curObjectives[x].GetComponent<BR_Objectives>().objType == BR_Objectives.ObjectiveType.THRUSTER || curObjectives[x].GetComponent<BR_Objectives>().objType == BR_Objectives.ObjectiveType.CARRY)
                {
                    temp = new GameObject[y+1];
                    temp[y] = curObjectives[x];
                    curObjectives[y].GetComponent<BR_Objectives>().objNumber = y;
                    y++;
                }
                else
                    Destroy(curObjectives[x]);
            }
            curObjectives = temp;
        }
    }
    public void RemoveAllObjectives()
    {
        GameObject[] temp = new GameObject[0];
        for (int x = 0; x < curObjectives.Length; x++)
        {
                Destroy(curObjectives[x]);
        }
        curObjectives = temp;
    }
}
