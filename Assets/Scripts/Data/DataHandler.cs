using UnityEngine;
using UnityEngine.UI;
using Unity.XR.PXR;
using UnityEngine.XR;
using System.IO;
using System;


public class DataHandler : MonoBehaviour
{
    public Camera cam;
    public EyeTrackingManager eyeTrackingManager;
    public RoomCapture RoomCaptureManager;
    public Canvas canva;

    PlayerData data;
    int PlayerNum = 1;
    PlayerDataPerFrame playerDataPerFrame;
    DateTime startTime;
    DateTime endTime;

    bool recording = false;

    Renderer ET_Point_Renderer;

    // 保存PlayerData对象到本地
    private void Start()
    {
        data = new PlayerData();

        ET_Point_Renderer = eyeTrackingManager.Greenpoint.GetComponent<Renderer>();
    }
    private void Update()
    {
        if (recording)
        {
            ET_Point_Renderer.enabled = false;
        }
        else
        {
            ET_Point_Renderer.enabled = true;
        }

    }

    private void FixedUpdate()
    {
        playerDataPerFrame = new PlayerDataPerFrame();

        Vector3 HeadPosition;
        Vector3 HeadRotation;
        Vector3 EyePosition;
        Vector3 EyeRotation;

        float GazeLength = 3;
        Vector3 GazePosition;
        Vector3 ScreenPositionOfGazePosition; 

        InputDevice OperateDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool isBButtonPressed = false;
        bool isAButtonPressed = false;


        // 获取各个键值状态
        OperateDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAButtonPressed);
        OperateDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isBButtonPressed);

        // 按A开始录制
        if (isAButtonPressed && !recording)
        {
            StartRecording();
        }

        // 按B结束录制
        if (isBButtonPressed && recording)
        {
            StopRecording();
        }

        // 录入数据
        if (recording)
        {
            GazePosition = eyeTrackingManager.combineEyeGazeOriginInWorldSpace + GazeLength * eyeTrackingManager.combineEyeGazeVectorInWorldSpace;

            // 计算ET点的屏幕位置
            ScreenPositionOfGazePosition = cam.WorldToScreenPoint(GazePosition);

            // 计算相对坐标和旋转
            Transform Anchor = RoomCaptureManager.anchorObject.transform;
            Vector3 WorldHeadPosition = cam.transform.position;
            Vector3 WorldHeadRotation = cam.transform.forward;
            GetRelativeTransform(WorldHeadPosition, WorldHeadRotation, Anchor, out HeadPosition, out HeadRotation);
            Vector3 WorldEyePosition = eyeTrackingManager.combineEyeGazeOriginInWorldSpace;
            Vector3 WorldEyeRotation = eyeTrackingManager.combineEyeGazeVectorInWorldSpace;
            GetRelativeTransform(WorldEyePosition, WorldEyeRotation, Anchor, out EyePosition, out EyeRotation);

            // 获取眼动信息
            EyePupilInfo eyePupilInfo = eyeTrackingManager.eyePupilInfo;
            float leftEyeOpenness = eyeTrackingManager.leftEyeOpenness;
            float rightEyeOpenness = eyeTrackingManager.rightEyeOpenness;
            playerDataPerFrame.LoadData(
                HeadPosition,
                HeadRotation,
                EyePosition,
                EyeRotation,
                ScreenPositionOfGazePosition,
                eyePupilInfo,
                leftEyeOpenness,
                rightEyeOpenness
                );

            data.PlayerDatas.Add(playerDataPerFrame);
        }
}

/// Functions
public void SavePlayerData(PlayerData playerData)
    {
        data.SubjectID = PlayerNum.ToString();

        string baseFileName = "Player" + PlayerNum.ToString() + "Data.json";
        string savePath = Path.Combine(Application.persistentDataPath, baseFileName);

        // 检查文件是否存在，如果存在则递增 PlayerNum 直到找到一个不存在的文件名
        while (File.Exists(savePath))
        {
            PlayerNum += 1;
            savePath = Path.Combine(Application.persistentDataPath, "Player" + PlayerNum.ToString() + "Data.json");
        }

        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, jsonData);

        // 显示保存成功
        string text = "Player " + PlayerNum.ToString() + " saved at " + endTime.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 3f));

        // 玩家编号+1
        PlayerNum += 1;
    }

    void GetRelativeTransform(Vector3 position, Vector3 rotation, Transform Anchor, out Vector3 relativePosition, out Vector3 relativeRotation)
    {
        // 计算相对位置
        relativePosition = Anchor.InverseTransformPoint(position);
        
        // 计算相对旋转    
        Matrix4x4 matrix = Anchor.worldToLocalMatrix;
        relativeRotation = matrix.MultiplyVector(rotation);
    }

    void StartRecording()
    {
        recording = true;

        string baseFileName = "Player" + PlayerNum.ToString() + "Data.json";
        string savePath = Path.Combine(Application.persistentDataPath, baseFileName);

        // 检查文件是否存在，如果存在则递增 PlayerNum 直到找到一个不存在的文件名
        while (File.Exists(savePath))
        {
            PlayerNum += 1;
            savePath = Path.Combine(Application.persistentDataPath, "Player" + PlayerNum.ToString() + "Data.json");
        }

        //获取开始录制的时间
        startTime = DateTime.Now;

        string text = "Player " + PlayerNum.ToString() + " Start at " + startTime.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 2f));
    }

    void StopRecording()
    {
        recording = false;

        // 获取结束时刻
        endTime = DateTime.Now;

        // 保存起止时间
        data.startTime = startTime.ToString();
        data.endTime = endTime.ToString();

        // 保存被试数据
        SavePlayerData(data);

        // 数据清空
        data.PlayerDatas.Clear();
    }
}
