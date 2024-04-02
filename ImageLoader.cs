using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class ImageLoader : MonoBehaviour
{
    public Image[] targetImages; // ลิตเตอร์ที่ต้องการใส่ภาพ (ต้องกำหนดใน Inspector)

    void Start()
    {
        StartCoroutine(LoadImagesFromFolder());
    }

    IEnumerator LoadImagesFromFolder()
    {
        string folderPath = "CroppedPicture"; // ระบุชื่อโฟลเดอร์ที่ต้องการโหลดภาพ
        string fullPath = Path.Combine(Application.streamingAssetsPath, folderPath);
        
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError("Folder not found: " + fullPath);
            yield break;
        }

        string[] imagePaths = Directory.GetFiles(fullPath, "*.png");
        for (int i = 0; i < Mathf.Min(imagePaths.Length, targetImages.Length); i++)
        {
            string filePath = "file://" + imagePaths[i];
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImages[i].sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }
}
