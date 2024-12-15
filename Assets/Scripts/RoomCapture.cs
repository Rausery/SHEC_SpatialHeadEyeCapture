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
        // ���ط�������
        LoadRoomData();

        // ����ê��
        StartCoroutine(LoadOriginAnchor());
    }

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool isPrimaryButton;
        bool isSencondaryButton;

        // ʵʱ����ԭ����̬
        if (OriginHandle != 0)
        {
            PXR_MixedReality.GetAnchorPose(OriginHandle, out OriginRotation, out OriginLocation);
        }

        // �߶������±궨
        PxrEventSpatialTrackingStateUpdate pxrEventSpatialTrackingStateUpdate;
        pxrEventSpatialTrackingStateUpdate.message =  
            PxrSpatialTrackingStateMessage.Located;
        pxrEventSpatialTrackingStateUpdate.state = PxrSpatialTrackingState.Valid;
        SpatialTrackingStateUpdate(pxrEventSpatialTrackingStateUpdate);

        // X����ѹ
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPrimaryButton) && isPrimaryButton)
        {
        }

        // Y����ѹ
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
            // �ռ���Ч
            PXR_MixedReality.StartSpatialSceneCapture(out var taskId);
        }
    }
    
    //ֻ�������ӵ�anchor��Ϊԭ��
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
                    // ʵ����ê��Ԥ�Ƽ�
                    anchorObject = Instantiate(anchorPrefab);

                    // ��ȡê��任
                    PXR_MixedReality.GetAnchorPose(key, out var orientation, out var position);
                    // ��ȡ���������Ϣ
                    PXR_MixedReality.GetAnchorVolumeInfo(key, out var center, out var extent);

                    anchorObject.transform.position = position;
                    anchorObject.transform.rotation = orientation;

                    // ��ê����ڻ������
                    Vector3 centerToStart = new Vector3(-extent.x / 2, extent.y / 2, -extent.z);
                    anchorObject.transform.position = anchorObject.transform.TransformPoint(centerToStart);

                    // ��תê��
                    Vector3 anchorRotate = new Vector3(90f, 0f, 0f);
                    anchorObject.transform.rotation *= Quaternion.Euler(anchorRotate);

                    // ��ȡê������
                    OriginLocation = anchorObject.transform.position;
                    OriginRotation = anchorObject.transform.rotation;
                    OriginHandle = key;

                    // ����Ϸֻ����һ��ê��
                    AnchorLoaded = true;
                    
                    LoadResult = PxrResult.SUCCESS;
                }
            }
        }

        return LoadResult;
    }

    /// <summary>
    /// Э��
    /// </summary>
    IEnumerator LoadOriginAnchor()
    {
        // ������ȴ�һ��ʱ��
        yield return new WaitForSeconds(1f);

        // �ڵȴ�֮����ʾԭ��ê��
        PxrEventAnchorEntityLoaded pxrEventAnchorEntityLoaded = new PxrEventAnchorEntityLoaded();
        pxrEventAnchorEntityLoaded.count = 7;
        pxrEventAnchorEntityLoaded.result = AnchorLoadResult;
        pxrEventAnchorEntityLoaded.taskId = taskId;
        pxrEventAnchorEntityLoaded.location = PxrPersistLocation.Local;
        PxrResult result = AnchorEntityLoaded(pxrEventAnchorEntityLoaded);

        // ���û�б궨���ӣ�Ҫ��궨
        if (result != PxrResult.SUCCESS)
        {
            // ��ʾ������Ϣ
            StartCoroutine(DebugText.ShowDebugLogsForAWhile("You need to calibrate a table \r\n as a spatial anchor.", -1f));
        }

        // ��ʾ�궨��Ϣ
        string text = "Successfully Loaded!" + "\r\n" + "handle" + OriginHandle.ToString() + "\r\n" + "location:" + OriginLocation.ToString() + "\r\n" + "rotation:" + OriginRotation.ToString();
        StartCoroutine(DebugText.ShowDebugLogsForAWhile(text, 3f));
    }
}