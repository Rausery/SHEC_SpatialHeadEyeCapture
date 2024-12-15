using UnityEngine;
using UnityEngine.UI;
using Unity.XR.PXR;
using UnityEngine.XR;
using System.IO;
using System;
using System.Text;
using Unity.VisualScripting;


public class DataHandler : MonoBehaviour
{
    GameObject XRroot;

    private string Overall_filePath;

    public Camera cam;
    public EyeTrackingManager eyeTrackingManager;
    public RoomCapture RoomCaptureManager;
    public Canvas canva;

    int PlayerNum = 1;
    [HideInInspector]
    public PlayerDataPerFrame CurrentPlayerData = new PlayerDataPerFrame();
    PlayerDataPerFrame LastPlayerData = new PlayerDataPerFrame();
    DateTime startTime;
    DateTime endTime;

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
    public float Roll = 0;
    [HideInInspector]
    public float EyeSpeed = 0;

    // 键值状态
    bool isBButtonPressed = false;
    bool isAButtonPressed = false;
    bool isMenuButtonPressed = false;
    bool recording = false;

    Renderer ET_Point_Renderer;

    // 保存PlayerData对象到本地
    private void Start()
    {
        XRroot = GameObject.Find("XR_Root");
        Overall_getAvalableFilePath();

        ET_Point_Renderer = eyeTrackingManager.Greenpoint.GetComponent<Renderer>();
    }

    float cooldownTime = 1f;
    float lastTriggerTime = 0f;
    private void Update()
    {
        InputDevice SubjectDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        SubjectDevice.TryGetFeatureValue(CommonUsages.menuButton, out isMenuButtonPressed);

        if (recording)
        {
            ET_Point_Renderer.enabled = false;
        }
        else
        {
            ET_Point_Renderer.enabled = true;
        }

        // 按左手菜单键改变可见性
        if (isMenuButtonPressed)
        {
            // 检查冷却时间
            if (Time.time - lastTriggerTime >= cooldownTime)
            {
                ChangeVisibility();

                // 更新最后一次触发时间
                lastTriggerTime = Time.time;
            }
        }
    }

    private void FixedUpdate()//48fps frame per second
    {
        Vector3 HeadPosition;
        Vector3 HeadRotation;
        Vector3 EyePosition;
        Vector3 EyeRotation;

        float GazeLength = 3;
        Vector3 GazePosition;
        Vector3 ScreenPositionOfGazePosition; 

        InputDevice OperateDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        isBButtonPressed = false;
        isAButtonPressed = false;

        // 获取各个键值状态
        OperateDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAButtonPressed);
        OperateDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isBButtonPressed);

        // 按A开始录制
        if (isAButtonPressed && !recording)
        {
            recording = true;
        }

        // 按B结束录制
        if (isBButtonPressed && recording)
        {
            recording = false;
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

            // 计算特征值
            Speed = CalculateSpeed();
            EyeSpeed = CalculateEyeSpeed();
            //Yaw = CalculateAngle();
            //Pitch = CalculatePitch();
            New_CalculateRotation(out Yaw, out Pitch, out Roll);

            // 更新前后帧
            LastPlayerData.Position = CurrentPlayerData.Position;
            LastPlayerData.Rotation = CurrentPlayerData.Rotation;
            LastPlayerData.ET_Orientation = CurrentPlayerData.ET_Orientation;

            CurrentPlayerData.LoadData
            (
                HeadPosition,
                HeadRotation,
                EyePosition,
                EyeRotation,
                ScreenPositionOfGazePosition,
                eyePupilInfo,
                leftEyeOpenness,
                rightEyeOpenness,
                Speed,
                EyeSpeed,
                Yaw,
                Pitch,
                Roll
            );

            AppendToCSV(Overall_filePath, CurrentPlayerData.ToString());
        }
    }

/// Functions

    void GetRelativeTransform(Vector3 position, Vector3 rotation, Transform Anchor, out Vector3 relativePosition, out Vector3 relativeRotation)
    {
        // 计算相对位置
        relativePosition = Anchor.InverseTransformPoint(position);
        
        // 计算相对旋转    
        Matrix4x4 matrix = Anchor.worldToLocalMatrix;
        relativeRotation = matrix.MultiplyVector(rotation);
    }

    void ChangeVisibility()
    {
        if (XRroot != null)
        {
            XRroot.SetActive(!XRroot.activeSelf);
        }
    }

    public void Overall_getAvalableFilePath()
    {
        // 设置 CSV 文件的基础文件名
        string baseFileName = "Player" + PlayerNum + "Data.csv";
        Overall_filePath = Path.Combine(UnityEngine.Application.persistentDataPath, baseFileName);

        // 检查文件是否存在，如果存在则递增 PlayerNum 直到找到一个不存在的文件名
        while (File.Exists(Overall_filePath))
        {
            PlayerNum++;
            baseFileName = "Player" + PlayerNum + "Data.csv";
            Overall_filePath = Path.Combine(UnityEngine.Application.persistentDataPath, baseFileName);
        }

        // 记录metadata
        string metadata = "PlayerNum:" + PlayerNum + " Date:" + DateTime.Now.ToString();
        AppendToCSV(Overall_filePath, metadata);

        // 创建表头
        string header = "Time,HeadPositionX,HeadPositionY,HeadPositionZ," +
            "HeadRotationX,HeadRotationY,HeadRotationZ," +
            "EyePositionX,EyePositionY,EyePositionZ," +
            "EyeRotationX,EyeRotationY,EyeRotationZ," +
            "ScreenPositionX,ScreenPositionY,ScreenPositionZ," +
            "LeftEyePupilDiameter,RightEyePupilDiameter," +
            "LeftEyeOpenness,RightEyeOpenness," +
            "Speed,EyeSpeed,Yaw,Pitch,Roll";
        AppendToCSV(Overall_filePath, header);
    }

    public void AppendToCSV(string filepath, string newLine)
    {
        // 确保追加模式，如果文件不存在，将会创建文件
        using (StreamWriter writer = new StreamWriter(filepath, true, Encoding.UTF8))
        {
            writer.WriteLine(newLine);
        }
    }

    float CalculateSpeed()
    {
        if ((CurrentPlayerData != null) && (LastPlayerData != null))
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

            // 计算两个向量的点积
            float dotProduct = Vector3.Dot(SpeedDir.normalized, HeadDir.normalized);

            // 使用反余弦函数计算夹角（弧度）
            float angleInRadians = Mathf.Acos(dotProduct);

            // 将弧度转换为角度
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

    void New_CalculateRotation(out float yaw, out float pitch, out float roll)
    {
        float yawAngle = cam.transform.rotation.eulerAngles.y;
        float pitchAngle = cam.transform.rotation.eulerAngles.x;
        float rollAngle = cam.transform.rotation.eulerAngles.z;

        yaw = yawAngle;
        pitch = pitchAngle;
        roll = rollAngle;
    }

    float CalculateEyeSpeed()
    {
        if ((CurrentPlayerData != null) && (LastPlayerData != null))
        {
            Vector3 vec1 = CurrentPlayerData.ET_Orientation;
            Vector3 vec2 = LastPlayerData.ET_Orientation;
            //计算旋转角度，用反余弦
            float angleInRadians = Mathf.Acos(Vector3.Dot(vec1.normalized, vec2.normalized));
            float angleInDegrees = Mathf.Rad2Deg * angleInRadians;

            // 计算旋转速度
            float eyeSpeed = Mathf.Abs(angleInDegrees * 48);
            if (eyeSpeed == float.NaN)
            {
                return 0f;
            }
            else
            {
                return eyeSpeed;
            }
        }
        else
        {
            return 0f;
        }

    }
}
