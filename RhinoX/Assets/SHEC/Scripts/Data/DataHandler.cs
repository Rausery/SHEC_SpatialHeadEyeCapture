using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using UnityEngine.XR.ARSubsystems;



public class DataHandler : MonoBehaviour
{
    private string Overall_filePath;

    private Camera cam;
    private GameObject Anchor;

    int PlayerNum = 1;
    public PlayerDataPerFrame CurrentPlayerData = new PlayerDataPerFrame();
    PlayerDataPerFrame LastPlayerData = new PlayerDataPerFrame();

    // Raw Datas
    [HideInInspector]
    public Vector3 HeadPosition;
    [HideInInspector]
    public Vector3 HeadRotation;
    [HideInInspector]
    public Vector3 EyePosition;
    [HideInInspector]
    public Vector3 EyeRotation;

    // Features
    [HideInInspector]
    public float Speed = 0;
    [HideInInspector]
    public float Yaw = 0;
    [HideInInspector]
    public float Pitch = 0;
    [HideInInspector]
    public float EyeSpeed = 0;

    bool isBButtonPressed = false;
    bool isAButtonPressed = false;

    [HideInInspector]
    public bool recording = false;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Anchor = GameObject.Find("Anchor");

        Overall_getAvalableFilePath();
    }

    private void FixedUpdate()
    {
        HandleInput();

        // ¼������
        if (recording)
        {
            // ��������������ת
            Transform AnchorTrans = Anchor.transform;
            HeadPosition = cam.transform.position;
            HeadRotation = cam.transform.forward;
            // GetRelativeTransform(WorldHeadPosition, WorldHeadRotation, AnchorTrans, out HeadPosition, out HeadRotation);

            // ��������ֵ
            Speed = CalculateSpeed();
            Yaw = CalculateAngle();
            Pitch = CalculatePitch();

            // ����ǰ��֡
            LastPlayerData.Position = CurrentPlayerData.Position;
            LastPlayerData.Rotation = CurrentPlayerData.Rotation;

            CurrentPlayerData.LoadData(
                HeadPosition,
                HeadRotation,
                Speed,
                Yaw,
                Pitch
                );

            AppendToCSV(Overall_filePath,CurrentPlayerData.ToString());
        }
}

    /// <summary>
    /// Functions
    /// </summary>
    /// 

    void HandleInput()
    {
        InputDevice OperateDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice SubjectDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // ��ȡ������ֵ״̬
        OperateDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAButtonPressed);
        OperateDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isBButtonPressed);

        if (isAButtonPressed && !recording)
        {
            recording = true;
        }

        if (isBButtonPressed && recording)
        {
            recording = false;
        }

    }

    public void Overall_getAvalableFilePath()
    {
        // ���� CSV �ļ��Ļ����ļ���
        string baseFileName = "Player" + PlayerNum + "Data.csv";
        Overall_filePath = Path.Combine(UnityEngine.Application.persistentDataPath, baseFileName);

        // ����ļ��Ƿ���ڣ������������� PlayerNum ֱ���ҵ�һ�������ڵ��ļ���
        while (File.Exists(Overall_filePath))
        {
            PlayerNum++;
            baseFileName = "Player" + PlayerNum + "Data.csv";
            Overall_filePath = Path.Combine(UnityEngine.Application.persistentDataPath, baseFileName);
        }

        // ��¼metadata
        string metadata = "PlayerNum:" + PlayerNum + " Date:" + DateTime.Now.ToString();
        AppendToCSV(Overall_filePath, metadata); 

        // ������ͷ
        string header = "HeadPositionX,HeadPositionY,HeadPositionZ," +
            "HeadRotationX,HeadRotationY,HeadRotationZ," +
            "Speed,Yaw,Pitch";
        AppendToCSV(Overall_filePath, header);
    }

    public void AppendToCSV(string filepath, string newLine)
    {
        // ȷ��׷��ģʽ������ļ������ڣ����ᴴ���ļ�
        using (StreamWriter writer = new StreamWriter(filepath, true, Encoding.UTF8))
        {
            writer.WriteLine(newLine);
        }
    }

    float CalculateSpeed()
    {
        if((CurrentPlayerData != null) && (LastPlayerData != null))
        {
            float deltaX = CurrentPlayerData.Position.x - LastPlayerData.Position.x;
            float deltaZ = CurrentPlayerData.Position.z - LastPlayerData.Position.z;

            float speed = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ) * 48;

            return speed;
        }
        else
        {
            return 0;
        }
    }
    float CalculateAngle()
    {
        if ((CurrentPlayerData != null) && (LastPlayerData != null))
        {
            Vector3 SpeedDir = CurrentPlayerData.Position - LastPlayerData.Position;
            SpeedDir = new Vector3(SpeedDir.x, 0f, SpeedDir.z);
            Vector3 HeadDir = CurrentPlayerData.Rotation;
            HeadDir = new Vector3(HeadDir.x, 0f, HeadDir.z);

            // �������������ĵ��
            float dotProduct = Vector3.Dot(SpeedDir.normalized, HeadDir.normalized);

            // ʹ�÷����Һ�������нǣ����ȣ�
            float angleInRadians = Mathf.Acos(dotProduct);

            // ������ת��Ϊ�Ƕ�
            float angleInDegrees = Mathf.Rad2Deg * angleInRadians;
            float angle = Mathf.Abs(angleInDegrees);
            return angle;
        }
        else
        {
            return 0f;
        }
    }

    float CalculatePitch()
    {
        if ((CurrentPlayerData != null) && (LastPlayerData != null))
        {
            float y = CurrentPlayerData.Rotation.y;
            float pitch = Mathf.Rad2Deg * Mathf.Asin(y);

            return pitch;
        }
        else
        {
            return 0f;
        }
    }
}
