using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryManager : MonoBehaviour
{
    public GameObject imagePrefab; // GameObject ของภาพที่จะใช้สร้าง
    public Transform contentPanel; // Parent ของภาพ

    void Start()
    {
        // โหลดไฟล์ภาพทั้งหมดจากโฟลเดอร์
        string[] imageFiles = Directory.GetFiles(PathUtil.GetSavePath(), "*.png");

        // สร้างกล่องภาพสำหรับแต่ละภาพ
        foreach (string filePath in imageFiles)
        {
            Texture2D texture = LoadTextureFromFile(filePath);
            CreateImageBox(texture);
        }
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return texture;
    }

    void CreateImageBox(Texture2D texture)
    {
        GameObject imageGO = Instantiate(imagePrefab, contentPanel);
        RawImage rawImage = imageGO.GetComponent<RawImage>();
        rawImage.texture = texture;
    }
}
