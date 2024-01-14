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

    //��ʾһ��ʱ�����ʾ���
    public static IEnumerator ShowDebugLogsForAWhile(string debugLog, float time)
    {
        instance.textMeshPro.text = debugLog;

        if (instance.textMeshPro != null) // ȷ�� textMeshPro ��Ϊ null
        {
            // ����ʾʱ��С�ڵ������ʱ�򣬳־���ʾ
            if (time < 0.0001f)
            {
                instance.textMeshPro.text = debugLog;
            }

            // ����ʾʱ��������ʱ�򣬰����趨ʱ����ʾ
            else
            {
                instance.textMeshPro.text = debugLog;
                yield return new WaitForSeconds(time);
                instance.textMeshPro.text = "";
            }
        }
    }
}
