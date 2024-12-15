using Pico.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TurnOffLight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice OperateDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool isBButtonPressed = false;
        bool isAButtonPressed = false;

        // 获取各个键值状态
        OperateDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAButtonPressed);
        OperateDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isBButtonPressed);

        // 按X开始录制
        if (isAButtonPressed & GetComponent<Light>().enabled == true)
        {     
            GetComponent<Light>().enabled = false;
        }

        // 按Y结束录制
        if (isBButtonPressed & GetComponent<Light>().enabled == false)
        {
            GetComponent<Light>().enabled = true;
        }
    }
}
