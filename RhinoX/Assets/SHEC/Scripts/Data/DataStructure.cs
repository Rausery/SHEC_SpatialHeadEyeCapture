using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class PlayerDataPerFrame
{
    public Vector3 Position;
    public Vector3 Rotation;

    // ÌØÕ÷Öµ
    public float Speed;
    public float Yaw;
    public float Pitch;

    public PlayerDataPerFrame() { }

    public void LoadData(Vector3 position, Vector3 direction, float speed, float yaw, float pitch)
    {
        Position = position;
        Rotation = direction;
        Speed = speed;
        Yaw = yaw;
        Pitch = pitch;
    }

    public override string ToString()
    {
        return Position.x + "," + Position.y + "," + Position.z + ","
            + Rotation.x + "," + Rotation.y + "," + Rotation.z + ","
            + Speed + "," + Yaw + "," + Pitch;
    }
}

