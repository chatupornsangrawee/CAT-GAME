/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class ImageCropperAndShower : MonoBehaviour
{
    public RawImage rawImage; // RawImage ที่ใช้ในการแสดงภาพที่จะถูกครอป
    public string objectDetectionAPIURL = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiYzYxYTJkYWQtMmIzYi00MjIzLWJjMzctZGQyMGRkZmU0YzUyIiwidHlwZSI6ImFwaV90b2tlbiJ9.bPsDmmZli_P4YHWnLyanm2a-_hb1eE9grtKJm8v2rg0";
    public string directoryPath = @"C:\Users\Nitro5\AppData\Local\Temp\KKUCompany\Catopia"; // โฟลเดอร์ที่ใช้ในการโชว์ภาพ

    // เมื่อกดปุ่ม Save
    public void OnSaveButtonClicked()
    {
        StartCoroutine(GetObjectCoordinatesAndCrop());
    }

    IEnumerator GetObjectCoordinatesAndCrop()
    {
        // ส่งคำขอ GET ไปยัง API
        UnityWebRequest request = UnityWebRequest.Get(objectDetectionAPIURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch object coordinates: " + request.error);
            yield break;
        }

        // แปลงข้อมูล JSON ที่ได้รับมาจาก API
        string jsonResponse = request.downloadHandler.text;
        // ตัวอย่าง: {"items":[{"label":"cat","confidence":0.9,"x_min":100,"x_max":200,"y_min":100,"y_max":200},{"label":"dog","confidence":0.8,"x_min":300,"x_max":400,"y_min":300,"y_max":400}]}
        DetectionResult result = JsonUtility.FromJson<DetectionResult>(jsonResponse);

        // โหลดภาพเพื่อใช้ในการครอป
        string imagePath = GetLatestImagePath(directoryPath);
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("No image found in directory.");
            yield break;
        }

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Texture2D originalTexture = new Texture2D(2, 2);
        originalTexture.LoadImage(imageBytes);

        foreach (var item in result.items)
        {
            // คำนวณพิกัดและครอปภาพตามแต่ละวัตถุ
            int x = item.x_min;
            int y = item.y_min;
            int width = item.x_max - item.x_min;
            int height = item.y_max - item.y_min;

            // สร้าง Texture2D ใหม่สำหรับภาพที่ครอปแล้ว
            Texture2D croppedTexture = new Texture2D(width, height);

            // คัดลอกพิกัดของภาพที่ต้องการครอปไปยัง Texture2D ใหม่
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    croppedTexture.SetPixel(i, j, originalTexture.GetPixel(x + i, y + j));
                }
            }

            // แปลงพิกัดให้ Texture2D ใหม่เหมือนกับภาพต้นฉบับ
            croppedTexture.Apply();

            // บันทึกภาพครอป
            SaveTextureToFile(croppedTexture, "cropped_image_" + item.label + ".png");
        }

        // โชว์ภาพที่ครอปทุกภาพจากโฟลเดอร์
        ShowAllCroppedImages();
    }


    // ฟังก์ชันสำหรับครอปภาพ
    Texture2D CropImage(Coordinates coordinates)
    {
        string imagePath = GetLatestImagePath(directoryPath);
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("No image found in directory.");
            return null;
        }

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Texture2D originalTexture = new Texture2D(2, 2);
        originalTexture.LoadImage(imageBytes);

        int x = coordinates.x;
        int y = coordinates.y;
        int width = coordinates.width;
        int height = coordinates.height;

        // สร้าง Texture2D ใหม่สำหรับภาพที่ครอปแล้ว
        Texture2D croppedTexture = new Texture2D(width, height);

        // คัดลอกพิกัดของภาพที่ต้องการครอปไปยัง Texture2D ใหม่
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                croppedTexture.SetPixel(i, j, originalTexture.GetPixel(x + i, y + j));
            }
        }

        // แปลงพิกัดให้ Texture2D ใหม่เหมือนกับภาพต้นฉบับ
        croppedTexture.Apply();
        return croppedTexture;
    }

    // ฟังก์ชันสำหรับบันทึก Texture2D เป็นไฟล์
    void SaveTextureToFile(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.persistentDataPath + "/" + filename;
        System.IO.File.WriteAllBytes(filePath, bytes);
        Debug.Log("Image saved to: " + filePath);
    }

    // ฟังก์ชันสำหรับโชว์ภาพที่ครอปทุกภาพจากโฟลเดอร์
    void ShowAllCroppedImages()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        foreach (var file in directoryInfo.GetFiles("*.png"))
        {
            byte[] imageBytes = File.ReadAllBytes(file.FullName);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            // ใช้ RawImage หรือสร้าง GameObject แสดงภาพตามที่ต้องการ
            // ตัวอย่าง: Instantiate(rawImagePrefab, transform.position, transform.rotation).GetComponent<RawImage>().texture = texture;
        }
    }

    // ฟังก์ชันสำหรับหาพิกัดของภาพล่าสุดในโฟลเดอร์
    private string GetLatestImagePath(string directoryPath)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        var latestImageFile = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
        return latestImageFile?.FullName;
    }

    // คลาสสำหรับเก็บข้อมูลพิกัด
    [System.Serializable]
    public class Coordinates
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }
}
*/