using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManagerContent : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public GameObject photoButtonPrefab;

    private string folderPath => PathUtil.GetSavePath(); // กำหนดพาทตามที่ต้องการ

    private void Start()
    {
        if (gridLayoutGroup == null || photoButtonPrefab == null)
        {
            Debug.LogError("gridLayoutGroup or photoButtonPrefab is not assigned.");
            return;
        }

        LoadImages();
    }

    private void LoadImages()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Gallery folder not found.");
            return;
        }

        string[] imagePaths = Directory.GetFiles(folderPath, "*.png");

        foreach (string path in imagePaths)
        {
            Texture2D texture = LoadTextureFromFile(path);
            if (texture != null)
            {
                CreatePhotoButton(texture);
            }
        }
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // auto-resize based on fileData
        return texture;
    }

    private void CreatePhotoButton(Texture2D texture)
    {
        GameObject buttonGO = Instantiate(photoButtonPrefab, gridLayoutGroup.transform);
        Button button = buttonGO.GetComponent<Button>();
        button.onClick.AddListener(() => ShowFullImage(texture));

        // Set image on the button
        Image image = buttonGO.GetComponent<Image>();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        image.sprite = sprite;
    }

    private void ShowFullImage(Texture2D texture)
    {
        // แสดงรูปภาพขนาดเต็ม
        // อย่าลืมปิดหน้าต่าง Gallery หรือทำให้มีการเลือกรูปอื่น ๆ เมื่อคลิกที่รูป
        Debug.Log("Show full image");
        // เช่น โชว์ใน Image เต็มขนาดหรือใช้โมดัลใน Unity ตามที่ต้องการ
    }
}
