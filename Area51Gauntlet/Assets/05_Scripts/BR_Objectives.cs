using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BR_Objectives : MonoBehaviour {
    private BR_ObjectivePanel ObjPanel;
    public enum ObjectiveType {NONE, REACH, KILL, PICKUP, PROTECT, TUTORIAL, HACK, THRUSTER, RETURN, CARRY, ACTIVATE ,FIND, ATTACK, POWERPANEL};
    public enum ObjectiveState {Started, Complete};
    public ObjectiveType objType;
    public ObjectiveState objState = ObjectiveState.Started;
    public int objNumber;
    [SerializeField]
    private string killText;
    [SerializeField]
    private string reachText;
    [SerializeField]
    private string pickupText;
    [SerializeField]
    private string protectText;
    [SerializeField]
    private string tutorialText;
    [SerializeField]
    private string hackText;
    [SerializeField]
    private string returnText;
    [SerializeField]
    private string carryText;
    [SerializeField]
    private string activateText;
    [SerializeField]
    private string findText;
    [SerializeField]
    private string attackText;
    [SerializeField]
    private Text selfText;
    private float timer = 3f;
    // Use this for initialization
    void Start () {
        ObjPanel = FindObjectOfType<BR_ObjectivePanel>();

	}
	
	// Update is called once per frame
    public void Update()
    {
        Transform Header = ObjPanel.objectiveHeader.transform;
        gameObject.GetComponent<RectTransform>().position = new Vector3(Header.position.x, Header.position.y - ((1 + objNumber) * Screen.height / 21), Header.position.z);
        if (objState == ObjectiveState.Complete)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                ObjPanel.RemoveObjective(objType);
        }
    }

    public void SetupObjective(ObjectiveType _objType, int _objNum, string _target)
    {
        objType = _objType;
        objNumber = _objNum;
        switch (objType)
        {
            case ObjectiveType.KILL:
                selfText.text = killText + " " + _target;
                return;

            case ObjectiveType.PICKUP:
                selfText.text = pickupText + " " + _target;
                return;

            case ObjectiveType.PROTECT:
                selfText.text = protectText + " " + _target;
                return;

            case ObjectiveType.REACH:
                selfText.text = reachText + " " + _target;
                return;

            case ObjectiveType.TUTORIAL:
                selfText.text = tutorialText + " " + _target;
                return;

            case ObjectiveType.HACK:
                selfText.text = hackText + " " + _target;
                return;

            case ObjectiveType.THRUSTER:
                selfText.text = pickupText + " " + _target;
                return;

            case ObjectiveType.RETURN:
                selfText.text = returnText + " " + _target;
                return;

            case ObjectiveType.CARRY:
                selfText.text = string.Format(carryText, _target);
                return;

            case ObjectiveType.ACTIVATE:
                selfText.text = activateText + " " + _target;
                return;

            case ObjectiveType.FIND:
                selfText.text = findText + " " + _target;
                return;

            case ObjectiveType.ATTACK:
                selfText.text = string.Format(attackText, _target);
                return;

            case ObjectiveType.POWERPANEL:
                selfText.text = killText + " " + _target;
                return;

            case ObjectiveType.NONE:
                Debug.LogError("WARNING: no Objective type selected");
                return;
        }
    }
    public void CompleteObjective()
    {
        gameObject.GetComponent<Text>().color = Color.green;
        objState = ObjectiveState.Complete;
    }

    public void FailObjective()
    {
        gameObject.GetComponent<Text>().color = Color.red;
        objState = ObjectiveState.Complete;
    }
}
