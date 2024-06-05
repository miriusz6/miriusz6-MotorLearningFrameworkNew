using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Experiment : MonoBehaviour
{
    public Canvas MsgDisplay;
    
    public DefaultExperimentSetting defaultExperimentSetting;
    public List<ExperimentSetting> experimentSettings;

    public float  startPointFromEdgeOffset;
    public float  startToEndDistance;
    public float coneToTargetReqProximity;

    public string toDisplayAtFinish;



    private OVRInput.Button nextButton = OVRInput.Button.One;
    private OVRInput.Button calibrateButton = OVRInput.Button.Two;

    private Setting[] settings;

    private State activeState = State.Calibration;

    private bool isFinished = false;
    

    public GameObject Arm;
    public GameObject Room;
    public Transform RightHandAnchor;

    // defined in calibration ?
    private Vector3 endPointPos;
    private Vector3 startPointPos;

    private Setting currentSetting;


    private Setting defaultSetting;

    public Material checkPointMaterial;
    public Material optimalPathMaterial;
    public float checkPointTransparency;
    private GameObject startPoint;
    private GameObject endPoint;
    private GameObject optimalPath;

    private bool table_calibrated = false;

    


    // private Text display;


    enum State
    {
        Calibration,
        BeforeTrial,
        Trail,
        AfterTrial,
        Comeback,
        Finished
    }


    void Start()
    {

        Debug.Log("STARTED!: ");
        createDefaultSetting();
        createCheckpoints();
        InitCalibration();
        optimalPath = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //if ( settings.Length < 1)
        //{
        //    Debug.Log("There must be at least 1 valid setting ! Shutting down.");
        //}

    }


    void createCheckpoints()
    {
        startPoint = GameObject.Instantiate(defaultSetting.Arm.transform.Find("Cone").gameObject, this.transform);
        startPoint.name = "StartPoint";
        Renderer rend = startPoint.GetComponent<Renderer>();
        rend.material = checkPointMaterial;
        Color col = rend.material.color;
        col.a = checkPointTransparency;
        rend.material.color = col;
        endPoint = GameObject.Instantiate(startPoint, this.transform);
        endPoint.name = "EndPoint";

        startPoint.SetActive(false);
        endPoint.SetActive(false);
    }

    void ShowEnviroment() { SetActiveEnviroment(true); }
    void HideEnviroment() { SetActiveEnviroment(false); }

    void SetActiveEnviroment(bool active)
    {
        currentSetting.Arm.SetActive(active);
        currentSetting.Room.SetActive(active);
        startPoint.SetActive(active);
        endPoint.SetActive(active);
        optimalPath.SetActive(active);
    }

  

    // Update is called once per frame
    void Update()
    {
        trySwitchState();
        
    }

    void Calibrate()
    {
        //Vector3 conePos = defaultSetting.Arm.transform.Find("Cone").position;

        // initial table position
        
        // offset from tracking pivot to middle of table edge from players side
        Vector3 offsetPivotToSurfaceEdge = new Vector3(0f, 0.5f, -0.5f);
        Vector3 offsetArmToConeBottom = new Vector3(0f, 0.096f, 0.0356f);

        Vector3 hand_pos = RightHandAnchor.position;

        Transform defaultTableTrans = defaultSetting.Room.transform.Find("Table");
        Debug.Log("def table pos:"+ defaultTableTrans.position.ToString());
        Transform table_edge_center = defaultSetting.Table.transform.Find("player_edge_center");
        Transform cone_bottom_edge_center = defaultSetting.Arm.transform.Find("cone_bottom_player_edge");
        //cone_bottom_edge_center.localPosition ODEJMIJ DWIE POS OD SIEBIE ZEBY MIEC LOKAL ALBO CUS
        // MISSING!
        defaultSetting.Table.transform.position = hand_pos - table_edge_center.localPosition + cone_bottom_edge_center.localPosition;
        

        // move startpoint to: table pos - table surface center offser + user defined offset
        startPoint.transform.position =  defaultSetting.Cone.transform.position + new Vector3(0, 0, startPointFromEdgeOffset);
        // move endpoint to startpoint pos + user defined movement distance
        endPoint.transform.position = startPoint.transform.position + new Vector3(0, 0, startToEndDistance);

        // create optimal path from start point to end point
        
        
        optimalPath.transform.localScale = new Vector3(0.01f, 0.001f, startToEndDistance);
        optimalPath.transform.position = table_edge_center.position + new Vector3(0, 0, startPointFromEdgeOffset + startToEndDistance / 2);
        // set material for optimal path
        Renderer rend = optimalPath.GetComponent<Renderer>();
        rend.material = optimalPathMaterial;
        Color col = rend.material.color;
        col.a = checkPointTransparency;
        rend.material.color = col;

        // assign
        startPointPos = startPoint.transform.position;
        endPointPos = endPoint.transform.position;

        

        Debug.Log("Calibrated");
    }


    void trySwitchState()
    {
        switch (activeState)
        {
            case State.Calibration:
                if (table_calibrated && IsPressed(nextButton) && IsConeInPosition(startPointPos)) 
                {
                    createCustomSettings();
                    activeState = State.Comeback;
                }
                else if (table_calibrated && IsPressed(calibrateButton))
                {
                    Calibrate();
                }
                else if (!table_calibrated && IsPressed(calibrateButton)) // MISSING
                {
                    Calibrate();    
                    defaultSetting.SetActive(true);
                    startPoint.SetActive(true);
                    optimalPath.SetActive(false);
                    table_calibrated = true;
                    
                }
                break;
            case State.BeforeTrial:
                if (IsPressed(nextButton))
                {
                    Debug.Log("button pressed, starting trial");
                    InitTrial();
                    activeState = State.Trail;
                }
                break;
            case State.Trail:
                if (IsConeInPosition(endPointPos)) //|| IsPressedMockup())
                {
                    InitAfterTrial();
                    activeState = State.AfterTrial;
                }
                break;
            case State.AfterTrial:
                if (IsPressed(nextButton))
                {
                    InitComeback();
                    activeState = State.Comeback;
                }
                break;
            case State.Comeback:
                if (isFinished)
                {
                    InitFinished();
                    activeState = State.Finished;
                }
                else if (IsConeInPosition(startPointPos))// || IsPressedMockup())
                {
                    TryDrawNextSetting();
                    InitBeforeTrial();
                    activeState = State.BeforeTrial;
                }
                break;
            case State.Finished:
                break;
        }

    }



    void InitBeforeTrial()
    {
        // show BeforeTrial msg
        Debug.Log("Initiating BeforeTrial");
        // CHANGE 
        DisplayMsg(currentSetting.TextBeforeTrial);
        HideEnviroment();
        Debug.Log("BeforeTrial Initiated");
    }
    void InitTrial()
    {
        Debug.Log("Initiating Trial");
        HideDisplay();

        // apply new body
        // start logging
        StartLogging();
        ShowEnviroment();
        endPoint.SetActive(true);
        startPoint.SetActive(false);
        Debug.Log("Trial Initiated");
    }

    void InitAfterTrial()
    {
        Debug.Log("Initiating AfterTrial");
        // show AfterTrial msg
        DisplayMsg(currentSetting.TextAfterTrial);
        // stop Logging
        StopLogging();
        HideEnviroment();
        Debug.Log("AfterTrial Initiated");
    }

    void InitComeback()
    {
        Debug.Log("Initiating Comeback");
        HideDisplay();
        ShowEnviroment();
        // switch to default body
        endPoint.SetActive(false);
        optimalPath.SetActive(false);
        currentSetting.SetActive(false);
        startPoint.SetActive(true);
        defaultSetting.ArmMapping.AlignWith(currentSetting.Arm.transform.position);
        defaultSetting.SetActive(true);
        Debug.Log("Comeback Initiated");
    }

    void InitFinished()
    {
        DisplayMsg(toDisplayAtFinish);
        
    }

    void InitCalibration() 
    {
        // use to get offset from edgde ! GetComponent<Renderer>().bounds.size
    }


   

    void StartLogging() { }
    void StopLogging() { }

    
    void PopCurrentSetting()
    {
        var buff = new List<Setting>(settings);
        buff.Remove(currentSetting);
        settings = buff.ToArray();
    }


    int DrawSettingID()
    {
        return new System.Random().Next(0, settings.Length);
    }

    // Draws next Exper. setting randomly
    // Consider 0 trials!
    void TryDrawNextSetting()
    {
        if (currentSetting is null)
        {
            defaultSetting.SetActive(false);
            currentSetting = settings[DrawSettingID()];
            currentSetting.SetActive(true);
            return;
        }

        if (defaultSetting.IsActive)
        {
            currentSetting.ArmMapping.AlignWith(defaultSetting.Arm.transform.position);
        }

        defaultSetting.SetActive(false);

        // some trials left
        Debug.Log("Active ID: " + currentSetting.id.ToString() + "trials left: " + currentSetting.TrialsLeft.ToString());
        if ( currentSetting.TrialsLeft > 1) {
            currentSetting.TrialsLeft--;
            return;
        }
        
        // no trials left
        Debug.Log("Setting popped");
        PopCurrentSetting();

        // no more settings left
        if (settings.Length < 1) 
        { 
            isFinished = true; return; 
        }

        currentSetting.SetActive(false);
        Setting newSetting = settings[DrawSettingID()];
        newSetting.ArmMapping.AlignWith(currentSetting.Arm.transform.position);
        currentSetting = newSetting;
        currentSetting.SetActive(true);
    }





    // Displays Msg on canvas
    void DisplayMsg(string msg)
    {
        MsgDisplay.gameObject.SetActive(true);
        //MsgDisplay.gameObject.GetComponent<Text>().text = "(" + activeState.ToString() + ")" + msg;

        MsgDisplay.gameObject.GetComponent<Text>().text = msg;


    }

    void HideDisplay()
    {
        MsgDisplay.gameObject.SetActive(false);
    }

    bool IsPressedMockup()
    {
        return Input.GetKeyUp(KeyCode.Space);
    }

    bool IsPressed(OVRInput.Button button)
    {
        //Debug.Log("Awaiting button");
        return OVRInput.GetUp(button) || IsPressedMockup();
    }

    bool IsConeInPosition(Vector3 targetPos)
    {
        Vector3 conePos;
         if (defaultSetting.IsActive)
        {
            conePos = defaultSetting.Cone.transform.position;
        }
        else
        {
            conePos = currentSetting.Cone.transform.position;
        }
        
        float dist = Vector3.Distance(targetPos, conePos);
        //return dist < coneToTargetReqProximity;
        return dist < coneToTargetReqProximity;
        //return true;
    }

  
    GameObject createDefaultArm()
    {
        // make the template objects of arm inactive and all its future clones
        Arm.gameObject.SetActive(false);
        // create copy of arm template
        // create copy of arm template
        GameObject defaultArm = GameObject.Instantiate(Arm, Arm.transform.parent);
        defaultArm.name = "Default Arm";
        // find all elements of the arm
        GameObject hand = defaultArm.transform.Find("Hand").gameObject;
        GameObject cone = defaultArm.transform.Find("Cone").gameObject;
        GameObject sleeve = defaultArm.transform.Find("Sleeve").gameObject;
        // if default materials provided apply them to default arm
        if (defaultExperimentSetting.HandMaterial != null) {
            hand.GetComponent<Renderer>().material = defaultExperimentSetting.HandMaterial; }
        if (defaultExperimentSetting.ConeMaterial != null) {
            cone.GetComponent<Renderer>().material = defaultExperimentSetting.ConeMaterial; }
        if (defaultExperimentSetting.SleeveMaterial != null) {
            sleeve.GetComponent<Renderer>().material = defaultExperimentSetting.SleeveMaterial; }
        
        ArmMapping mapping = defaultArm.AddComponent<ArmMapping>();
        attachMapping(mapping, defaultExperimentSetting);
        return defaultArm;
    }

    void attachMapping(ArmMapping mapping, DefaultExperimentSetting setting)
    {
        (bool, bool, bool) rotationFreeze =
            (setting.freezeRotationX,
            setting.freezeRotationY,
            setting.freezeRotationZ);
        (bool, bool, bool) translationFreeze =
            (setting.freezeTranslationAxisX,
            setting.freezeTranslationAxisY,
            setting.freezeTranslationAxisZ);
        mapping.Configure(RightHandAnchor, rotationFreeze, translationFreeze,
            setting.translationDegrees, setting.translationScale, setting.translationOffset);
        
    }

    GameObject createDefaultRoom()
    {
        // make the template objects of room inactive and all its future clones
        Room.gameObject.SetActive(false);
        // create copy of room template
        GameObject defaultRoom = GameObject.Instantiate(Room);
        defaultRoom.name = "Default Room";
        // find all elements of the room
        GameObject ceiling = defaultRoom.transform.Find("Ceiling").gameObject;
        GameObject floor = defaultRoom.transform.Find("Floor").gameObject;
        GameObject wall1 = defaultRoom.transform.Find("Wall1").gameObject;
        GameObject wall2 = defaultRoom.transform.Find("Wall2").gameObject;
        GameObject wall3 = defaultRoom.transform.Find("Wall3").gameObject;
        GameObject wall4 = defaultRoom.transform.Find("Wall4").gameObject;
        // if defualt materials provided apply them to defualt room
        if (defaultExperimentSetting.CeilingMaterial != null) {
            ceiling.GetComponent<Renderer>().material = defaultExperimentSetting.CeilingMaterial; }
        if (defaultExperimentSetting.FloorMaterial != null) 
        { floor.GetComponent<Renderer>().material = defaultExperimentSetting.FloorMaterial; }
        if (defaultExperimentSetting.WallMaterial!= null) { 
            wall1.GetComponent<Renderer>().material = defaultExperimentSetting.WallMaterial;
            wall1.GetComponent<Renderer>().material = defaultExperimentSetting.WallMaterial;
            wall1.GetComponent<Renderer>().material = defaultExperimentSetting.WallMaterial;
            wall1.GetComponent<Renderer>().material = defaultExperimentSetting.WallMaterial;
        }
        return defaultRoom;
    }

    GameObject createCustomArm(ExperimentSetting experSetting)
    {
        // copy the default arm
        GameObject customArm = GameObject.Instantiate(defaultSetting.Arm, defaultSetting.Arm.transform.parent);
        customArm.name = "Custom Arm "+experSetting.name;
        // find all elements of the arm
        GameObject hand = customArm.transform.Find("Hand").gameObject;
        GameObject cone = customArm.transform.Find("Cone").gameObject;
        GameObject sleeve = customArm.transform.Find("Sleeve").gameObject;
        Material customHandMat = experSetting.HandMaterial;
        Material customConeMat = experSetting.ConeMaterial;
        Material customSleeveMat = experSetting.SleeveMaterial;
        
        // if custom materials provided apply them to custom arm
        if (customHandMat != null) { hand.GetComponent<Renderer>().material = customHandMat; }
        if (customConeMat != null) { cone.GetComponent<Renderer>().material = customConeMat; }
        if (customSleeveMat != null) { sleeve.GetComponent<Renderer>().material = customSleeveMat; }
        attachMapping(customArm.GetComponent<ArmMapping>(), experSetting);
        return customArm;
    }

    GameObject createCustomRoom(ExperimentSetting exprSetting)
    {
        // copy the default room
        GameObject customRoom = GameObject.Instantiate(defaultSetting.Room);
        customRoom.name = "Custom Room " + exprSetting.name;
        // find all elements of the room
        GameObject table = customRoom.transform.Find("Table").gameObject;
        GameObject ceiling = customRoom.transform.Find("Ceiling").gameObject;
        GameObject floor = customRoom.transform.Find("Floor").gameObject;
        GameObject wall1 = customRoom.transform.Find("Wall1").gameObject;
        GameObject wall2 = customRoom.transform.Find("Wall2").gameObject;
        GameObject wall3 = customRoom.transform.Find("Wall3").gameObject;
        GameObject wall4 = customRoom.transform.Find("Wall4").gameObject;
        // if custom materials provided apply them to custom room
        if (exprSetting.TableMaterial!= null) { table.GetComponent<Renderer>().material = exprSetting.TableMaterial; }
        if (exprSetting.CeilingMaterial != null) { ceiling.GetComponent<Renderer>().material = exprSetting.CeilingMaterial; }
        if (exprSetting.FloorMaterial != null) { floor.GetComponent<Renderer>().material = exprSetting.FloorMaterial; }
        if (exprSetting.WallMaterial != null) { wall1.GetComponent<Renderer>().material = exprSetting.WallMaterial; }
        if (exprSetting.WallMaterial != null) { wall2.GetComponent<Renderer>().material = exprSetting.WallMaterial; }
        if (exprSetting.WallMaterial  != null) { wall3.GetComponent<Renderer>().material = exprSetting.WallMaterial; }
        if (exprSetting.WallMaterial != null) { wall4.GetComponent<Renderer>().material = exprSetting.WallMaterial; }
        return customRoom;
    }


    void createDefaultSetting()
    {
        // create default setting
        GameObject defArm = createDefaultArm();
        GameObject defHand = defArm.transform.Find("Hand").gameObject;
        GameObject defCone = defArm.transform.Find("Cone").gameObject;
        GameObject defSleeve = defArm.transform.Find("Sleeve").gameObject;
        GameObject defRoom = createDefaultRoom();
        GameObject defTable = defRoom.transform.Find("Table").gameObject;
        defaultSetting = new Setting(
            name: "DefaultSetting",
            id: 0, 
            trialCnt: 0,
            textBefore: defaultExperimentSetting.toDisplayBeforeEachTrial,
            textAfter: defaultExperimentSetting.toDisplayAfterEachTrial,
            arm: defArm, 
            hand: defHand,
            sleeve: defSleeve,
            cone: defCone,
            room: defRoom,
            table: defTable
            );
        defaultSetting.ArmMapping.AlignWith(RightHandAnchor.transform.position);
    }

    /// <summary>
    // creates Setting objects from ExperimentSetting
    /// </summary>
    /// 
    void createCustomSettings()
    {
        // create custom settings
        var buff = new List<Setting>();
        foreach (ExperimentSetting exprSett in experimentSettings)
        {
            // skip if trial count < 1
            if (exprSett.amountOfTrials < 1) { 
                Debug.Log("Setting \"" + exprSett.name + "\" must have at least 1 trialcount !"); 
                continue; }

            GameObject customArm = createCustomArm(exprSett);
            GameObject customHand = customArm.transform.Find("Hand").gameObject;
            GameObject customCone = customArm.transform.Find("Cone").gameObject;
            GameObject customSleeve = customArm.transform.Find("Sleeve").gameObject;
            GameObject customRoom = createCustomRoom(exprSett);
            GameObject customTable = customRoom.transform.Find("Table").gameObject;
            
            String txtBeforeTrial = exprSett.toDisplayBeforeEachTrial;
            String txtAfterTrial = exprSett.toDisplayAfterEachTrial;
            if (txtBeforeTrial.Length == 0) { txtBeforeTrial = defaultExperimentSetting.toDisplayBeforeEachTrial; }
            if (txtAfterTrial.Length == 0) { txtAfterTrial = defaultExperimentSetting.toDisplayAfterEachTrial; }
            int id = exprSett.settingID;
            if( id <1) { id = generateUniqueID(buff); }
            //Setting new_setting = new Setting(exprSett.name, id, exprSett.amountOfTrials,
            //    customArm,customRoom, txtBeforeTrial,txtAfterTrial);
            Setting new_setting = new Setting(
                name: exprSett.name,
                id: id,
                trialCnt: exprSett.amountOfTrials,
                textBefore: txtBeforeTrial,
                textAfter: txtAfterTrial,
                arm: customArm,
                hand: customHand,
                sleeve: customSleeve,
                cone: customCone,
                room: customRoom,
                table: customTable
                );
            buff.Add(new_setting);
        }
        settings = buff.ToArray();
    }

    // defensive programming
    // if a bad id provided the method will genereate a valid one
    int generateUniqueID(List<Setting> seenSettings)
    {
        var seenIDs = new List<int>();
        foreach (Setting setting in seenSettings)
        {
            seenIDs.Add(setting.id);
        }

        int candidate_id = 1;
        while (true)
        {
            if (!seenIDs.Contains(candidate_id)) { break; }
            candidate_id++;
        }
        return candidate_id;
    }


    // simpler and more ssecure version of ExperimentSetting
    // carrying only the necessary info for the rest of the app lifetime 
    public class Setting
    {
        public string name { get; private set; }
        public int id { get; private set; }
        public int TrialsLeft { get; set;}
        public GameObject Arm { get; private set; }
        public GameObject Hand { get; private set; }
        public GameObject Sleeve { get; private set; }
        public GameObject Cone { get; private set; }
        public GameObject Room { get; private set; }
        public GameObject Table { get; private set; }

        public bool IsActive { get; private set; }

        public string TextBeforeTrial { get; private set; }
        public string TextAfterTrial { get; private set; }

        public ArmMapping ArmMapping { get; private set; }

        public Setting(string name, 
            int id, 
            int trialCnt,
            string textBefore,
            string textAfter,
            GameObject arm,
            GameObject hand,
            GameObject sleeve,
            GameObject cone, 
            GameObject room,
            GameObject table)
        {
            this.name = name;
            this.id = id;
            this.TrialsLeft = trialCnt;
            this.TextBeforeTrial = textBefore;
            this.TextAfterTrial = textAfter;
            this.Arm = arm;
            this.Hand = hand;
            this.Sleeve = sleeve;
            this.Cone = cone;
            this.Room = room;
            this.Table = table;
            this.IsActive = false;
            this.ArmMapping = Arm.GetComponent<ArmMapping>();
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            Arm.SetActive(active);
            Room.SetActive(active);
            IsActive = active;
            //if (active)
            //{
            //    Arm.GetComponent<ArmMapping>().AlignWith();
            //}
        }
    }


}
 