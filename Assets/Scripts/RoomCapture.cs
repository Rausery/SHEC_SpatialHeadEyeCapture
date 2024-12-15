using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using UnityEngine;


public class RoomCapture : MonoBehaviour
{
    public GameObject anchorPrefab;
    [HideInInspector]
    public GameObject anchorObject;
    [HideInInspector]
    public Vector3 OriginLocation = new Vector3();
    [HideInInspector] 
    public Quaternion OriginRotation = new Quaternion();
    [HideInInspector]
    public ulong OriginHandle = 0;

    Dictionary<ulong, PxrEventAnchorEntityLoaded> anchorEntityResults = new Dictionary<ulong, PxrEventAnchorEntityLoaded>();
    ulong taskId;

    PxrResult AnchorLoadResult = PxrResult.TIMEOUT_EXPIRED;

    private void Start()
    {
        // 加载房间数据
        LoadRoomData();

        // 加载锚点
        StartCoroutine(LoadOriginAnchor());
    }

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool isPrimaryButton;
        bool isSencondaryButton;

        // 实时更新原点姿态
        if (OriginHandle != 0)
        {
            PXR_MixedReality.GetAnchorPose(OriginHandle, out OriginRotation, out OriginLocation);
        }

        // 走丢了重新标定
        PxrEventSpatialTrackingStateUpdate pxrEventSpatialTrackingStateUpdate;
        pxrEventSpatialTrackingStateUpdate.message =  
            PxrSpatialTrackingStateMessage.Located;
        pxrEventSpatialTrackingStateUpdate.state = PxrSpatialTrackingState.Valid;
        SpatialTrackingStateUpdate(pxrEventSpatialTrackingStateUpdate);

        // X键按压
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPrimaryButton) && isPrimaryButton)
        {
        }

        // Y键按压
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out isSencondaryButton) && isSencondaryButton)
        {
        }
    }



    /// <summary>
    /// functions
    /// </summary>
    private void LoadRoomData()
    {
        PxrSpatialSceneDataTypeFlags[] flags = { PxrSpatialSceneDataTypeFlags.Ceiling, PxrSpatialSceneDataTypeFlags.Door, PxrSpatialSceneDataTypeFlags.Floor, PxrSpatialSceneDataTypeFlags.Opening,
        PxrSpatialSceneDataTypeFlags.Window,PxrSpatialSceneDataTypeFlags.Wall,PxrSpatialSceneDataTypeFlags.Object };
        AnchorLoadResult = PXR_MixedReality.LoadAnchorEntityBySceneFilter(flags, out taskId);
    }

    private void SpatialTrackingStateUpdate(PxrEventSpatialTrackingStateUpdate stateInfo)
    {
        if (stateInfo.state == PxrSpatialTrackingState.Invalid || stateInfo.state == PxrSpatialTrackingState.Limited)
        {
            // 空间无效
            PXR_MixedReality.StartSpatialSceneCapture(out var taskId);
        }
    }
    
    //只加载桌子的anchor作为原点
    private PxrResult AnchorEntityLoaded(PxrEventAnchorEntityLoaded LoadInfo)
    {
        bool AnchorLoaded = false;
        PxrResult LoadResult = PxrResult.ERROR_ANCHOR_ENTITY_NOT_FOUND;

        if (LoadInfo.result == PxrResult.SUCCESS && LoadInfo.count != 0 && !AnchorLoaded)
        {
            PXR_MixedReality.GetAnchorEntityLoadResults(LoadInfo.taskId, LoadInfo.count, out var loadedAnchors);
            foreach (var key in loadedAnchors.Keys)
            {
                PXR_MixedReality.GetAnchorSceneLabel(key, out var label);

                if (label == PxrSceneLabel.Table)
                {
                    // 实例化锚点预制件
                    anchorObject = Instantiate(anchorPrefab);

                    // 读取锚点变换
                    PXR_MixedReality.GetAnchorPose(key, out var orientation, out var position);
                    // 读取桌子体积信息
                    PXR_MixedReality.GetAnchorVolumeInfo(key, out var center, out var extent);

                    anchorObject.transform.position = position;
                    anchorObject.transform.rotation = orientation;

                    // 把锚点放在绘制起点
                    Vector3 centerToStart = new Vector3(-extent.x / 2, extent.y / 2, -extent.z);
                    anchorObject.transform.position = anchorObject.transform.TransformPoint(centerToStart);

                    // 旋转锚点
                    Vector3 anchorRotate = new Vector3(90f, 0f, 0f);
                    anchorObject.transform.rotation *= Quaternion.Euler(anchorRotate);

                    // 读取锚点数据
                    OriginLocation = anchorObject.transform.position;
                    OriginRotation = anchorObject.transform.rotation;
                    OriginHandle = key;

                    // 本游戏只加载一个锚点
                    AnchorLoaded = true;
                    
                    LoadResult = PxrResult.SUCCESS;
                }
            }
        }

        return LoadResult;
    }

    /// <summary>
    /// 协程
    /// </summary>
    IEnumerator LoadOriginAnchor()
    {
        // 在这里等待一段时间
        yield return new WaitForSeconds(1f);

        // 在等待之后显示原点锚点
        PxrEventAnchorEntityLoaded pxrEventAnchorEntityLoaded = new PxrEventAnchorEntityLoaded();
        pxrEventAnchorEntityLoaded.count = 7;
        pxrEventAnchorEntityLoaded.result = AnchorLoadResult;
        pxrEventAnchorEntityLoaded.taskId = taskId;
        pxrEventAnchorEntityLoaded.location = PxrPersistLocation.Local;
        PxrResult result = AnchorEntityLoaded(pxrEventAnchorEntityLoaded);

        // 如果没有标定桌子，要求标定
        if (result != PxrResult.SUCCESS)
        {
            // 显示错误信息
            StartCoroutine(DebugText.ShowDebugLogsForAWhile("You need to calibrate a table \r\n as a spatial anchor.", -1f));
        }

        // 显示标定信息
        string text = "Successfully Loaded!" + "\r\n" + "handle" + OriginHandle.ToString() + "\r\n" + "location:" + OriginLocation.ToString() + "\r\n" + "rotation:" + OriginRotation.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 3f));
    }
}