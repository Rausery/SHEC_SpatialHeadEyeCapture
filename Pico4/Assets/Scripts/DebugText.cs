using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DebugText : MonoBehaviour
{
    public static DebugText instance; // ����ʵ��
    private TextMeshProUGUI debugText;
    private void Awake()
    {
        // ȷ��ֻ��һ��ʵ������
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //��ʾһ��ʱ�����ʾ���
    public static IEnumerator ShowDebugLogsForAWhile(string debugLog, float time)
    {
        instance.debugText.text = debugLog;

        if (instance.debugText != null) // ȷ�� textMeshPro ��Ϊ null
        {
            // ����ʾʱ��С�ڵ������ʱ�򣬳־���ʾ
            if (time < 0.0001f)
            {
                instance.debugText.text = debugLog;
            }

            // ����ʾʱ��������ʱ�򣬰����趨ʱ����ʾ
            else
            {
                UpdateDebugText(debugLog);
                yield return new WaitForSeconds(time);
                instance.debugText.text = "";
            }
        }
    }

    public static void ShowDebugLogs(string debugLog)
    {
        instance.debugText.text = debugLog + "\n" + instance.debugText.text;
    }


    public static void UpdateDebugText(string input)
    {
        if (instance.debugText == null)
        {
            Debug.LogError("DebugText field is not assigned.");
            return;
        }

        // ������ת��Ϊ���ı���ʽ
        string formattedInput = "<color=#FF0000FF>" + input + "</color>\n";

        // �Ȼ�ȡ��ǰ�ı�
        string currentText = instance.debugText.text;

        // ��鵱ǰ�ı�������
        int currentLineCount = instance.debugText.textInfo.lineCount;

        // �����ǰ��������20��ɾ�������Ĳ���
        while (currentLineCount > 20)
        {
            // �Ƴ����һ���ı�
            int lastLineIndex = instance.debugText.textInfo.lineInfo[currentLineCount - 1].lastCharacterIndex;
            currentText = currentText.Substring(0, lastLineIndex);

            // ������������
            currentLineCount = Mathf.Max(1, currentLineCount - 1); // ���ٱ���1��
        }

        // �����ı���ӵ���ǰ��
        instance.debugText.text = formattedInput + currentText;
    }
}
