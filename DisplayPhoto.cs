using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using VInspector;

public class DisplayPhoto : MonoBehaviour
{
    public RawImage rawImage;

    void OnEnable()
    {
        ActionMain.OnCaptureImage += DisplayImage;
    }
    void OnDisable()
    {
        ActionMain.OnCaptureImage -= DisplayImage;
    }

    [Button]
    public void DisplayImage()
    {
        if (Directory.Exists(PathUtil.GetSavePath()))
        {
            string[] imageFiles = Directory.GetFiles(PathUtil.GetSavePath(), "*.png")
                .OrderByDescending(file => new FileInfo(file).LastWriteTime)
                .ToArray();

            if (imageFiles.Length > 0)
            {
                byte[] imageData = File.ReadAllBytes(imageFiles[0]);
                Texture2D texture = new(2, 2);
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
