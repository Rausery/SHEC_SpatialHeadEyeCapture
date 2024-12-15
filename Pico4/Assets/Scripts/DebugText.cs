using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DebugText : MonoBehaviour
{
    public static DebugText instance; // 单例实例
    private TextMeshProUGUI debugText;
    private void Awake()
    {
        // 确保只有一个实例存在
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

    //显示一定时间的提示语句
    public static IEnumerator ShowDebugLogsForAWhile(string debugLog, float time)
    {
        instance.debugText.text = debugLog;

        if (instance.debugText != null) // 确保 textMeshPro 不为 null
        {
            // 当显示时间小于等于零的时候，持久显示
            if (time < 0.0001f)
            {
                instance.debugText.text = debugLog;
            }

            // 当显示时间大于零的时候，按照设定时间显示
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

        // 将输入转换为富文本格式
        string formattedInput = "<color=#FF0000FF>" + input + "</color>\n";

        // 先获取当前文本
        string currentText = instance.debugText.text;

        // 检查当前文本的行数
        int currentLineCount = instance.debugText.textInfo.lineCount;

        // 如果当前行数超过20，删除超出的部分
        while (currentLineCount > 20)
        {
            // 移除最后一行文本
            int lastLineIndex = instance.debugText.textInfo.lineInfo[currentLineCount - 1].lastCharacterIndex;
            currentText = currentText.Substring(0, lastLineIndex);

            // 更新行数计数
            currentLineCount = Mathf.Max(1, currentLineCount - 1); // 至少保留1行
        }

        // 将新文本添加到最前方
        instance.debugText.text = formattedInput + currentText;
    }
}
