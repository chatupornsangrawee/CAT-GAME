using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class TextTranslation : MonoBehaviour
{
    private string apiKey = "AIzaSyA3FDbR-FF9PzE_IMjkTGzv4f5A7l4rSXA"; // แทน YOUR_API_KEY ด้วย API Key ของคุณ
    private string translationEndpoint = "https://translation.googleapis.com/language/translate/v2?key={0}&q={1}&target=th";

    void Start()
    {
        // อ่านข้อความจากไฟล์ labels.txt ใน StreamingAssets
        string[] lines = ReadLinesFromFile("CroppedPicture/labels.txt");

        // เรียกใช้งาน Coroutine เพื่อแปลข้อความทีละบรรทัด
        StartCoroutine(TranslateLines(lines));
    }

    string[] ReadLinesFromFile(string fileName)
    {
        // อ่านข้อมูลจากไฟล์ใน StreamingAssets และแยกบรรทัดออกเป็นอาร์เรย์ของข้อความ
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] lines = File.ReadAllLines(filePath);
        return lines;
    }

    IEnumerator TranslateLines(string[] lines)
    {
        foreach (string line in lines)
        {
            // เรียกใช้งาน Coroutine เพื่อแปลข้อความแต่ละบรรทัด
            yield return StartCoroutine(TranslateText(line));
        }
    }

    IEnumerator TranslateText(string textToTranslate)
    {
        // สร้าง URL สำหรับการเรียกใช้งาน Google Translate API
        string url = string.Format(translationEndpoint, apiKey, UnityWebRequest.EscapeURL(textToTranslate));

        // ส่งคำขอ GET ไปยัง Google Translate API
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // รอรับข้อมูลจากเซิร์ฟเวอร์
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // ดึงข้อมูล JSON จากการตอบกลับของ API
                string jsonResponse = webRequest.downloadHandler.text;

                // แปลง JSON เป็น object โดยใช้ JSONUtility
                TranslationResponse response = JsonUtility.FromJson<TranslationResponse>(jsonResponse);

                if (response != null && response.data != null && response.data.translations != null && response.data.translations.Length > 0)
                {
                    // ดึงข้อความที่แปลมาจาก response
                    string translatedText = response.data.translations[0].translatedText;

                    // แสดงข้อความที่แปลในกล่องข้อความ
                    Debug.Log(translatedText);

                    // เขียนข้อความที่แปลลงในไฟล์ labels2.txt
                    WriteToFile("CroppedPicture/labels2.txt", translatedText);
                }
                else
                {
                    Debug.LogError("Failed to parse translation response.");
                }
            }
            else
            {
                Debug.LogError("Translation request failed. Error: " + webRequest.error);
            }
        }
    }

    void WriteToFile(string fileName, string textToWrite)
    {
        // กำหนดที่อยู่และชื่อไฟล์ที่ต้องการเขียน
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // เปิดไฟล์เพื่อเขียนข้อมูล
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // เขียนข้อความลงไฟล์
            writer.WriteLine(textToWrite);
        }
    }




}

[System.Serializable]
public class TranslationResponse
{
    public TranslationData data;
}

[System.Serializable]
public class TranslationData
{
    public Translation[] translations;
}

[System.Serializable]
public class Translation
{
    public string translatedText;
}
