using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;


[Serializable]
public class PlayerDataPerFrame
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 ET_Position;
    public Vector3 ET_Orientation;
    public Vector3 ET_ScreenPosition;

    public float LeftEyePupilDiameter;
    public float RightEyePupilDiameter;
    public float LeftEyeOpenness;
    public float RightEyeOpenness;

    public PlayerDataPerFrame()
    {

    }

    public void LoadData(Vector3 position, Vector3 direction, Vector3 et_Position, Vector3 et_Orientation, Vector3 et_ScreenPosition, EyePupilInfo et_PupilInfo, float leftEyeOpenness, float rightEyeOpenness)
    {
        Position = position;
        Direction = direction;
        ET_Position = et_Position;
        ET_Orientation = et_Orientation;
        ET_ScreenPosition = et_ScreenPosition;
        LeftEyePupilDiameter = et_PupilInfo.leftEyePupilDiameter;
        RightEyePupilDiameter = et_PupilInfo.rightEyePupilDiameter;
        LeftEyeOpenness = leftEyeOpenness;
        RightEyeOpenness = rightEyeOpenness;
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