using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DebugText : MonoBehaviour
{
    public static DebugText instance;
    public TextMeshProUGUI textMeshPro;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //显示一定时间的提示语句
    public static IEnumerator ShowDebugLogsForAWhile(string debugLog, float time)
    {
        instance.textMeshPro.text = debugLog;

        if (instance.textMeshPro != null) // 确保 textMeshPro 不为 null
        {
            // 当显示时间小于等于零的时候，持久显示
            if (time < 0.0001f)
            {
                instance.textMeshPro.text = debugLog;
            }

            // 当显示时间大于零的时候，按照设定时间显示
            else
            {
                instance.textMeshPro.text = debugLog;
                yield return new WaitForSeconds(time);
                instance.textMeshPro.text = "";
            }
        }
    }
}
