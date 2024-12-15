using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignedWithSpatialAnchor : MonoBehaviour
{
    public RoomCapture anchorPrefab;
    public Vector3 offset;

    private Transform SpatialAnchor;

    // Start is called before the first frame update
    void Start()
    {
        // 加载锚点
        StartCoroutine(AlignedWithOriginAnchor());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AlignedWithOriginAnchor()
    {
        // 在这里等待一段时间
        yield return new WaitForSeconds(2f);

        SpatialAnchor = anchorPrefab.anchorObject.transform;

        transform.rotation = SpatialAnchor.rotation;
        transform.position = SpatialAnchor.TransformPoint(offset);
    }
}
