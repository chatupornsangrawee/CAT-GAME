using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadLabels2 : MonoBehaviour
{
    public Text[] textPrefabs; // สร้าง Text UI ใน Unity แล้วนำมาเชื่อมโยงที่นี่

    void Update()
    {
        StartCoroutine(ReadLabelsFromFile());
    }

    private IEnumerator ReadLabelsFromFile()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "CroppedPicture", "labels2.txt");

        // ตรวจสอบว่าเป็นไฟล์ใน StreamingAssets หรือไม่
        if (filePath.Contains("://"))
        {
            // ถ้าใช่ให้ใช้ UnityWebRequest เพื่ออ่านไฟล์
            var request = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();
            DisplayLabels(request.downloadHandler.text);
        }
        else
        {
            // ใช้ StreamReader เพื่ออ่านไฟล์ที่อยู่ในโฟลเดอร์จริง
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();
            DisplayLabels(content);
        }
    }

    private void DisplayLabels(string content)
    {
        string[] lines = content.Split('\n'); // แยกข้อความด้วย \n (newline)

        for (int i = 0; i < lines.Length && i < textPrefabs.Length; i++)
        {
            string line = lines[i].Trim(); // ตัดช่องว่างด้านหน้าและด้านหลังของข้อความ
            int openParenIndex = line.IndexOf('('); // หาตำแหน่งของวงเล็บเปิด
            if (openParenIndex != -1)
            {
                string itemName = line.Substring(0, openParenIndex).Trim(); // เอาเฉพาะส่วนที่เป็นชื่อของสิ่งของ
                if (!string.IsNullOrEmpty(itemName) && IsThai(itemName)) // เปลี่ยนเป็น IsThai(itemName)
                {
                    textPrefabs[i].text = itemName; // กำหนดข้อความให้กับ Text UI ตามลำดับ
                }
            }
        }
    }

    private bool IsThai(string text)
    {
        foreach (char c in text)
        {
            // ตรวจสอบว่าตัวอักษรอยู่ในช่วง Unicode ของภาษาไทยหรือไม่
            if (c >= '\u0E00' && c <= '\u0E7F')
            {
                return true; // พบตัวอักษรภาษาไทย
            }
        }
        return false; // ไม่พบตัวอักษรภาษาไทย
    }

}