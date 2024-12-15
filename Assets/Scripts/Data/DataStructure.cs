using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;
using UnityEngine.InputSystem.LowLevel;


[Serializable]
public class PlayerDataPerFrame
{
    public string TimeStamp;

    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 ET_Position;
    public Vector3 ET_Orientation;
    public Vector3 ET_ScreenPosition;

    public float LeftEyePupilDiameter;
    public float RightEyePupilDiameter;
    public float LeftEyeOpenness;
    public float RightEyeOpenness;

    // ����ֵ
    public float Speed;
    public float EyeSpeed;
    public float Yaw;
    public float Pitch;
    public float Roll = 0;

    public PlayerDataPerFrame() { }

    // �ϰ汾Yaw��Pitch���㷽������ע������ƶ�����ĽǶ�
    public void LoadData(Vector3 position, Vector3 direction, Vector3 et_Position, Vector3 et_Orientation, Vector3 et_ScreenPosition,
        EyePupilInfo et_PupilInfo, float leftEyeOpenness, float rightEyeOpenness,
        float speed, float eyeSpeed, float yaw, float pitch)
    {
        Position = position;
        Rotation = direction;
        ET_Position = et_Position;
        ET_Orientation = et_Orientation;
        ET_ScreenPosition = et_ScreenPosition;
        LeftEyePupilDiameter = et_PupilInfo.leftEyePupilDiameter;
        RightEyePupilDiameter = et_PupilInfo.rightEyePupilDiameter;
        LeftEyeOpenness = leftEyeOpenness;
        RightEyeOpenness = rightEyeOpenness;
        Speed = speed;
        EyeSpeed = eyeSpeed;
        Yaw = yaw;
        Pitch = pitch;
    }

    // �°汾Yaw, Pitch��Roll���㷽������ע�������������ϵ�ĽǶ�
    public void LoadData(Vector3 position, Vector3 direction, Vector3 et_Position, Vector3 et_Orientation, Vector3 et_ScreenPosition,
    EyePupilInfo et_PupilInfo, float leftEyeOpenness, float rightEyeOpenness,
    float speed, float eyeSpeed, float yaw, float pitch, float roll)
    {
        // ��¼����ʱ���
        TimeStamp = DateTime.Now.ToString();
        Position = position;
        Rotation = direction;
        ET_Position = et_Position;
        ET_Orientation = et_Orientation;
        ET_ScreenPosition = et_ScreenPosition;
        LeftEyePupilDiameter = et_PupilInfo.leftEyePupilDiameter;
        RightEyePupilDiameter = et_PupilInfo.rightEyePupilDiameter;
        LeftEyeOpenness = leftEyeOpenness;
        RightEyeOpenness = rightEyeOpenness;
        Speed = speed;
        EyeSpeed = eyeSpeed;
        Yaw = yaw;
        Pitch = pitch;
        Roll = roll;
    }

    public override string ToString()
    {
        if (Roll != 0)
        {
            return TimeStamp + "," + Position.x + "," + Position.y + "," + Position.z + ","
                + Rotation.x + "," + Rotation.y + "," + Rotation.z + ","
                + ET_Position.x + "," + ET_Position.y + "," + ET_Position.z + ","
                + ET_Orientation.x + "," + ET_Orientation.y + "," + ET_Orientation.z + ","
                + ET_ScreenPosition.x + "," + ET_ScreenPosition.y + "," + ET_ScreenPosition.z + ","
                + LeftEyePupilDiameter + "," + RightEyePupilDiameter + ","
                + LeftEyeOpenness + "," + RightEyeOpenness + ","
                + Speed + "," + EyeSpeed + "," + Yaw + "," + Pitch + "," + Roll;
        }
        else
        {
            return Position.x + "," + Position.y + "," + Position.z + ","
                + Rotation.x + "," + Rotation.y + "," + Rotation.z + ","
                + ET_Position.x + "," + ET_Position.y + "," + ET_Position.z + ","
                + ET_Orientation.x + "," + ET_Orientation.y + "," + ET_Orientation.z + ","
                + ET_ScreenPosition.x + "," + ET_ScreenPosition.y + "," + ET_ScreenPosition.z + ","
                + LeftEyePupilDiameter + "," + RightEyePupilDiameter + ","
                + LeftEyeOpenness + "," + RightEyeOpenness + ","
                + Speed + "," + EyeSpeed + "," + Yaw + "," + Pitch;
        }
    }
}

public class PlayerData
{
    public string SubjectID;
    public string startTime;
    public string endTime;
    public List<PlayerDataPerFrame> PlayerDatas;

    public PlayerData()
    {
        SubjectID = "null";
        PlayerDatas = new List<PlayerDataPerFrame>();
    }
}