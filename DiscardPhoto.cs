using UnityEngine;
using System.IO;

public class DiscardButtonScript : MonoBehaviour
{
    // ฟังก์ชันที่เรียกเมื่อปุ่มถูกคลิก
    public void OnDiscardButtonClicked()
    {
        // ลบไฟล์
        DeleteImageAtPath(PathUtil.GetSavePath());
    }

    // ฟังก์ชันที่ใช้ในการลบไฟล์และลบอ็อบเจ็กท์
    private void DeleteImageAtPath(string path)
    {
        try
        {
            // ตรวจสอบว่าไฟล์มีอยู่จริงหรือไม่ก่อนลบ
            if (File.Exists(path))
            {
                // ลบไฟล์
                File.Delete(path);
                Debug.Log("Deleted: " + path);

                // ลบอ็อบเจ็กท์รูปภาพออกจาก Scene
                GameObject imageObject = GameObject.Find("ImageObjectName"); // แทน "ImageObjectName" ด้วยชื่ออ็อบเจ็กท์รูปภาพ
                if (imageObject != null)
                {
                    Destroy(imageObject);
                    Debug.Log("Destroyed Image GameObject");
                }
            }
            else
            {
                Debug.LogWarning("File does not exist: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error deleting file: " + e.Message);
        }
    }
}
