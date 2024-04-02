using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class DisplayCapture : MonoBehaviour
{
    public RawImage rawImage;

    void Start()
    {
        // กำหนด path ที่ต้องการ
        string customPath = @"C:/Users/Nitro5/AppData/Local/Temp/KKUCompany/Catopia\catopia";

        // โค้ดตรวจสอบว่า path นี้มีอยู่หรือไม่
        if (Directory.Exists(customPath))
        {
            // ดึงไฟล์รูปภาพจาก path โดยเรียงลำดับตามเวลาสร้างหรือแก้ไข
            string[] imageFiles = Directory.GetFiles(customPath, "*.png")
                .OrderByDescending(file => new FileInfo(file).LastWriteTime)
                .ToArray();

            if (imageFiles.Length > 0)
            {
                // นำรูปภาพมาแสดงใน RawImage
                byte[] imageData = File.ReadAllBytes(imageFiles[0]);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);

                rawImage.texture = texture;

            }
            else
            {
                Debug.LogWarning("No image files found in the specified path.");
            }
        }
        else
        {
            Debug.LogWarning("The specified path does not exist.");
        }
    }
}