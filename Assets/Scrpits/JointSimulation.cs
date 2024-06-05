using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointSimulation : MonoBehaviour
{
    //public Transform HandT;
    public Transform WristT;
    public Transform TrackingT;

    public Transform Shoulder; // z 70
    public Transform Elbow; // z -80

    private Vector3 WristMinBound = new Vector3(-90, -90, -50);
    private Vector3 WristMaxBound = new Vector3(90, 90, 45);

    private Vector3 ShoulderMinBound = new Vector3(-10, -45, -45);
    private Vector3 ShoulderMaxBound = new Vector3(90, 90, 90);

    private Vector3 ElbowMinBound = new Vector3(-90, 0, -90);
    private Vector3 ElbowMaxBound = new Vector3(90, 0, 45);


    private Vector3 WristMinRot; // 90
    private Vector3 WristMaxRot; // 270
    private Vector3 ElbowMinRot; 
    private Vector3 ElbowMaxRot;
    private Vector3 ShoulderMaxRot;
    private Vector3 ShoulderMinRot;

    private Quaternion prevRot = new Quaternion();

    Vector3 initShoulderWristDir;
    Vector3 initElbowWristDir;



    Vector3 prevElbowWristDir;

    private Vector3 prevWristPos;
    private Vector3 prevTrackDist;

    private float initReach;
    private float maxReach;
    private float minReach;
    private float prevReach;
    private float shoulderReachStep;
    private float elbowReachStep;
    private float elbowShoulderWeight = 1.2f;//1.4f;

    private float wristInitHeight;

    bool RespectsLimits(Vector3 trackR, Vector3 MinBound, Vector3 MaxBound )
    {
        bool X = trackR.x <= MinBound.x || trackR.x >= MaxBound.x;
        bool Y = trackR.y <= MinBound.y || trackR.y >= MaxBound.y;
        bool Z = trackR.z <= MinBound.z || trackR.z >= MaxBound.z;
        return X && Y && Z;
    }


    // Start is called before the first frame update
    void Start()
    {
        Vector3 wristInitRot = WristT.localRotation.eulerAngles;
        WristMaxRot = new Vector3(
            360 + WristMinBound.x,
            360 + WristMinBound.y,
            360 + WristMinBound.z);
        WristMinRot = new Vector3(
            WristMaxBound.x,
            WristMaxBound.y,
            WristMaxBound.z);

        ElbowMaxRot = new Vector3(
            360 + ElbowMinBound.x,
            360 + ElbowMinBound.y,
            360 + ElbowMinBound.z);
        ElbowMinRot = new Vector3(
            ElbowMaxBound.x,
            ElbowMaxBound.y,
            ElbowMaxBound.z);

        ShoulderMaxRot = new Vector3(
            360 + ShoulderMinBound.x,
            360 + ShoulderMinBound.y,
            360 + ShoulderMinBound.z);
        ShoulderMinRot = new Vector3(
            ShoulderMaxBound.x,
            ShoulderMaxBound.y,
            ShoulderMaxBound.z);



        wristInitHeight = WristT.position.y;


        initShoulderWristDir = (WristT.position - Shoulder.position).normalized;
        initElbowWristDir = (WristT.position - Elbow.position).normalized;

        prevWristPos = TrackingT.position;

        prevElbowWristDir = (WristT.position - Elbow.position).normalized;
        
        prevReach = Mathf.Abs((WristT.position - Shoulder.position).z);
        maxReach =  prevReach + (prevReach*1.23f);
        minReach = prevReach * 0.7f;
        initReach = prevReach;
        shoulderReachStep = (ShoulderMaxBound.z / prevReach);
        elbowReachStep = (ElbowMinBound.z / prevReach);
    }

    // Update is called once per frame
    void Update()
    {
        
        Quaternion trackRot = TrackingT.localRotation;
        if (prevRot != trackRot) 
        {
            if (RespectsLimits(trackRot.eulerAngles, WristMinRot, WristMaxRot))
            {
                WristT.localRotation = RotationInLocalSystem(WristT, trackRot);
            }
            prevRot = trackRot;
        }


        // X axis: only shoulder rotation from Elbow -> new Wrist pos
        

        if( prevWristPos != TrackingT.position)
        {

            //// X axis (left,right)
            //Vector3 Elbowdir = (TrackingT.position - Elbow.position).normalized;
            //Vector3 a = new Vector3(prevElbowWristDir.x, 0, prevElbowWristDir.z);
            //Vector3 b = new Vector3(Elbowdir.x, 0, Elbowdir.z);
            //var ShoulderAngle = Vector3.Angle(a.normalized,b.normalized);
            //Quaternion ShoulderRot;
            //if(a.x > b.x) { ShoulderRot = Quaternion.Euler(0, -ShoulderAngle, 0); }
            //else { ShoulderRot = Quaternion.Euler(0, ShoulderAngle, 0); }
            //Shoulder.localRotation = Shoulder.localRotation * ShoulderRot;
            //prevElbowWristDir = Elbowdir;

            // SH 42.03501X - 1.03611

            // EL 59.89059X - 2.94967
            


            // Z axis (forward,back)

            float currReach = Mathf.Abs((TrackingT.position - Shoulder.position).z);
            if ( prevReach != currReach && (currReach < maxReach && currReach > minReach))
            {
                float ratio = initReach / currReach;
                float regElRot = (59.89059f * ratio) - 2.94967f;
                Elbow.localRotation = Elbow.transform.localRotation * Quaternion.Euler(0, 0, -regElRot);

                float regShRot = (59.89059f * ratio) - 2.94967f;
                Shoulder.localRotation = Shoulder.transform.localRotation * Quaternion.Euler(0, 0, -regShRot);

                //float reachDiff = currReach - prevReach;

                //// divide reach
                //float elbowReach = reachDiff * elbowShoulderWeight;
                //float shoulderReach = reachDiff / elbowShoulderWeight;

                //float toRotateElbowDeg = elbowReach * elbowReachStep;
                //float rotElbowDir = Mathf.Sign(toRotateElbowDeg);

                //float elbowOverLimit = 0;
                //////Elbow rot
                //Quaternion candidateElbowRot = Elbow.transform.localRotation * Quaternion.Euler(0, 0, toRotateElbowDeg);
                //bool inElbowLimit = RespectsLimits(candidateElbowRot.eulerAngles, ElbowMinRot, ElbowMaxRot);
                //if (!inElbowLimit)
                //{
                //    float overLimit;
                //    if (rotElbowDir < 0)
                //    {
                //        overLimit = Mathf.Abs(ElbowMaxRot.z - candidateElbowRot.eulerAngles.z);
                //    }
                //    else
                //    {
                //        overLimit = Mathf.Abs(ElbowMinRot.z - candidateElbowRot.eulerAngles.z);
                //    }
                //    float untilLimit = (Mathf.Abs(toRotateElbowDeg) - overLimit) * rotElbowDir;


                //    candidateElbowRot = Elbow.transform.localRotation * Quaternion.Euler(0, 0, untilLimit);
                //    elbowOverLimit = overLimit;
                //    Elbow.localRotation = candidateElbowRot;
                //}
                //else
                //{
                //    Elbow.localRotation = candidateElbowRot;
                //}



                //float toRotateShoulderDeg = shoulderReach * shoulderReachStep;
                //float rotShoulderDir = Mathf.Sign(toRotateShoulderDeg);
                ////Shoulder rot
                //Quaternion candidateShoulderRot = Shoulder.transform.localRotation * Quaternion.Euler(0, 0, toRotateShoulderDeg);
                //bool inShoulderLimit = RespectsLimits(candidateShoulderRot.eulerAngles, ShoulderMinRot, ShoulderMaxRot);
                //if (!inShoulderLimit)
                //{
                //    float overLimit;
                //    if (rotShoulderDir < 0)
                //    {
                //        overLimit = Mathf.Abs(ShoulderMaxRot.z - candidateShoulderRot.eulerAngles.z);
                //    }
                //    else
                //    {
                //        overLimit = Mathf.Abs(ShoulderMinRot.z - candidateShoulderRot.eulerAngles.z);
                //    }
                //    float untilLimit = (Mathf.Abs(toRotateShoulderDeg) - overLimit) * rotShoulderDir;


                //    candidateShoulderRot = Shoulder.transform.localRotation * Quaternion.Euler(0, 0, untilLimit);
                //    Shoulder.localRotation = candidateShoulderRot;
                //}
                //else
                //{
                //    Shoulder.localRotation = candidateShoulderRot;
                //}




















                //Elbow.localRotation = candidateRot;

                //if ( toRotateDeg + Elbow.transform.localEulerAngles.z > ElbowMinBound)
                //Quaternion ElbowReachRot;
                //ElbowReachRot = Quaternion.Euler(0, 0, reachDiff * (elbowReachStep));
                //Elbow.localRotation = Elbow.localRotation * ElbowReachRot;



                //Quaternion ShoulderReachRot;

                //ShoulderReachRot = Quaternion.Euler(0, 0, reachDiff * shoulderReachStep);

                //Shoulder.localRotation = Shoulder.localRotation * ShoulderReachRot;


                prevReach = currReach;
            }
            




            //Vector3 Elbowdir = TrackingT.position - Elbow.position;
            //var ElbowRot = Quaternion.FromToRotation(initElbowWristDir, Elbowdir.normalized);
            //Elbow.localRotation = RotationInLocalSystem(Elbow, ElbowRot);

            //Vector3 dir = TrackingT.position - Shoulder.position;
            //var shoulderRot = Quaternion.FromToRotation(initShoulderWristDir, dir.normalized);
            //Shoulder.localRotation = RotationInLocalSystem(Shoulder, shoulderRot);




            prevWristPos = TrackingT.position;
        }

        
        
    }

    Quaternion RotationInLocalSystem(Transform toRotate, Quaternion worldRotation)
    {
        return Quaternion.Inverse(toRotate.parent.rotation) * worldRotation * toRotate.parent.rotation;
    }
}




















////float reachDiff = currReach - prevReach;

////// divide reach
////float elbowReach = reachDiff * elbowShoulderWeight;
////float shoulderReach = reachDiff / elbowShoulderWeight;

////float toRotateElbowDeg = elbowReach * elbowReachStep;
////float rotElbowDir = Mathf.Sign(toRotateElbowDeg);

////float elbowOverLimit = 0;
////////Elbow rot
////Quaternion candidateElbowRot = Elbow.transform.localRotation * Quaternion.Euler(0, 0, toRotateElbowDeg);
////bool inElbowLimit = RespectsLimits(candidateElbowRot.eulerAngles, ElbowMinRot, ElbowMaxRot);
////if (!inElbowLimit)
////{
////    float overLimit;
////    if (rotElbowDir < 0)
////    {
////        overLimit = Mathf.Abs(ElbowMaxRot.z - candidateElbowRot.eulerAngles.z);
////    }
////    else
////    {
////        overLimit = Mathf.Abs(ElbowMinRot.z - candidateElbowRot.eulerAngles.z);
////    }
////    float untilLimit = (Mathf.Abs(toRotateElbowDeg) - overLimit) * rotElbowDir;


////    candidateElbowRot = Elbow.transform.localRotation * Quaternion.Euler(0, 0, untilLimit);
////    elbowOverLimit = overLimit;
////    Elbow.localRotation = candidateElbowRot;
////}
////else
////{
////    Elbow.localRotation = candidateElbowRot;
////}



////float toRotateShoulderDeg = shoulderReach * shoulderReachStep;
////float rotShoulderDir = Mathf.Sign(toRotateShoulderDeg);
////toRotateShoulderDeg = (Mathf.Abs(toRotateShoulderDeg) + elbowOverLimit) * rotShoulderDir;
//////Shoulder rot
////Quaternion candidateShoulderRot = Shoulder.transform.localRotation * Quaternion.Euler(0, 0, toRotateShoulderDeg);
////bool inShoulderLimit = RespectsLimits(candidateShoulderRot.eulerAngles, ShoulderMinRot, ShoulderMaxRot);
////if (!inShoulderLimit)
////{
////    float overLimit;
////    if (rotShoulderDir < 0)
////    {
////        overLimit = Mathf.Abs(ShoulderMaxRot.z - candidateShoulderRot.eulerAngles.z);
////    }
////    else
////    {
////        overLimit = Mathf.Abs(ShoulderMinRot.z - candidateShoulderRot.eulerAngles.z);
////    }
////    float untilLimit = (Mathf.Abs(toRotateShoulderDeg) - overLimit) * rotShoulderDir;


////    candidateShoulderRot = Shoulder.transform.localRotation * Quaternion.Euler(0, 0, untilLimit);
////    Shoulder.localRotation = candidateShoulderRot;
////}
////else
////{
////    Shoulder.localRotation = candidateShoulderRot;
////}



// r = 2.686001
// r/2 = 1,3430005
// r/4 = 0,67150025
// 3/4 r = 2,01450075
// 0.8r = 2,1488008



//Sh 42.03501X - 1.03611
// 1 = 0
// 1.25 = 8
// 1.5 = 20
// 1.75 = 30
// 1.8 = 33,5
// 2 = 90

//El 59.89059X - 2.94967
// 1.25 = -9
// 1.5 = -25
// 1.75 = -40
// 1,8 = -49


