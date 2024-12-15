using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.PXR;
using TMPro;

public class EyeTrackingManager : MonoBehaviour
{
    public Transform Origin;
    public GameObject EyeCoordinates;
    public GameObject Models;
    public Transform Greenpoint;
    public GameObject SpotLight;
    public TMP_Text debugtext;
    public Camera cam;
    public Vector3 combineEyeGazeVectorInWorldSpace;
    public Vector3 combineEyeGazeOriginInWorldSpace;
    public float leftEyeOpenness;
    public float rightEyeOpenness;
    public string leftPupilDiameter;
    public string rightPupilDiameter;
    public EyePupilInfo eyePupilInfo = new EyePupilInfo();

    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;

    EyeTrackingMode eyeTrackingMode = EyeTrackingMode.PXR_ETM_NONE;
    bool supported = false;
    int supportedModesCount = 0;

    void Start()
    {
        TrackingStateCode  trackingState = (TrackingStateCode)PXR_MotionTracking.GetEyeTrackingSupported(ref supported, ref supportedModesCount, ref eyeTrackingMode);

        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        leftEyeOpenness = 0f;
        rightEyeOpenness = 0f;
    }

    void Update()
    {
        if (supported == true)
        {
            getETPositionAndRotation();
            getPupilInfoAndOpenness();
        }

        // 眼动小球
        if (leftEyeOpenness + rightEyeOpenness > 0.001f)
        {
            EyeCoordinates.transform.position = combineEyeGazeOriginInWorldSpace + 3 * combineEyeGazeVectorInWorldSpace;
        }
    }

    public unsafe void getPupilInfoAndOpenness()
    {
        // 瞳孔信息（仅支持pico 4 enterprise）
        int result = PXR_MotionTracking.GetEyePupilInfo(ref eyePupilInfo);
        leftPupilDiameter = eyePupilInfo.leftEyePupilDiameter.ToString();
        rightPupilDiameter = eyePupilInfo.rightEyePupilDiameter.ToString();

        // 双眼开合信息（pico 4 pro仅可获取开闭信息，pico 4 enterprise可获取开合度信息）
        PXR_MotionTracking.GetEyeOpenness(ref leftEyeOpenness, ref rightEyeOpenness);
    }

    public void getETPositionAndRotation()
    {
        // 眼动位置和朝向
        PXR_EyeTracking.GetHeadPosMatrix(out headPoseMatrix);
        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);
        combineEyeGazeOriginInWorldSpace = headPoseMatrix.MultiplyPoint(combineEyeGazeOrigin);
        combineEyeGazeVectorInWorldSpace = headPoseMatrix.MultiplyVector(combineEyeGazeVector);
    }
}