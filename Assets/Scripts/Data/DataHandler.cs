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

    // ����PlayerData���󵽱���
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


        // ��ȡ������ֵ״̬
        OperateDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAButtonPressed);
        OperateDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isBButtonPressed);

        // ��A��ʼ¼��
        if (isAButtonPressed && !recording)
        {
            StartRecording();
        }

        // ��B����¼��
        if (isBButtonPressed && recording)
        {
            StopRecording();
        }

        // ¼������
        if (recording)
        {
            GazePosition = eyeTrackingManager.combineEyeGazeOriginInWorldSpace + GazeLength * eyeTrackingManager.combineEyeGazeVectorInWorldSpace;

            // ����ET�����Ļλ��
            ScreenPositionOfGazePosition = cam.WorldToScreenPoint(GazePosition);

            // ��������������ת
            Transform Anchor = RoomCaptureManager.anchorObject.transform;
            Vector3 WorldHeadPosition = cam.transform.position;
            Vector3 WorldHeadRotation = cam.transform.forward;
            GetRelativeTransform(WorldHeadPosition, WorldHeadRotation, Anchor, out HeadPosition, out HeadRotation);
            Vector3 WorldEyePosition = eyeTrackingManager.combineEyeGazeOriginInWorldSpace;
            Vector3 WorldEyeRotation = eyeTrackingManager.combineEyeGazeVectorInWorldSpace;
            GetRelativeTransform(WorldEyePosition, WorldEyeRotation, Anchor, out EyePosition, out EyeRotation);

            // ��ȡ�۶���Ϣ
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

        // ����ļ��Ƿ���ڣ������������� PlayerNum ֱ���ҵ�һ�������ڵ��ļ���
        while (File.Exists(savePath))
        {
            PlayerNum += 1;
            savePath = Path.Combine(Application.persistentDataPath, "Player" + PlayerNum.ToString() + "Data.json");
        }

        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, jsonData);

        // ��ʾ����ɹ�
        string text = "Player " + PlayerNum.ToString() + " saved at " + endTime.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 3f));

        // ��ұ��+1
        PlayerNum += 1;
    }

    void GetRelativeTransform(Vector3 position, Vector3 rotation, Transform Anchor, out Vector3 relativePosition, out Vector3 relativeRotation)
    {
        // �������λ��
        relativePosition = Anchor.InverseTransformPoint(position);
        
        // ���������ת    
        Matrix4x4 matrix = Anchor.worldToLocalMatrix;
        relativeRotation = matrix.MultiplyVector(rotation);
    }

    void StartRecording()
    {
        recording = true;

        string baseFileName = "Player" + PlayerNum.ToString() + "Data.json";
        string savePath = Path.Combine(Application.persistentDataPath, baseFileName);

        // ����ļ��Ƿ���ڣ������������� PlayerNum ֱ���ҵ�һ�������ڵ��ļ���
        while (File.Exists(savePath))
        {
            PlayerNum += 1;
            savePath = Path.Combine(Application.persistentDataPath, "Player" + PlayerNum.ToString() + "Data.json");
        }

        //��ȡ��ʼ¼�Ƶ�ʱ��
        startTime = DateTime.Now;

        string text = "Player " + PlayerNum.ToString() + " Start at " + startTime.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 2f));
    }

    void StopRecording()
    {
        recording = false;

        // ��ȡ����ʱ��
        endTime = DateTime.Now;

        // ������ֹʱ��
        data.startTime = startTime.ToString();
        data.endTime = endTime.ToString();

        // ���汻������
        SavePlayerData(data);

        // �������
        data.PlayerDatas.Clear();
    }
}
