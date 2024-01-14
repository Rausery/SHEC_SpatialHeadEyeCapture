using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    // ����͸��
    void Awake()
    {
        PXR_MixedReality.EnableVideoSeeThrough(true);
    }

    // Ӧ�ûָ����ٴο���͸��
    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_MixedReality.EnableVideoSeeThrough(true);
        }
    }

}
