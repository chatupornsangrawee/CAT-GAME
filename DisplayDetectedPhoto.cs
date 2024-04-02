using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using VInspector;
using SimpleJSON;

[System.Serializable]
public class DetectionItem
{
    public string label;
    public float confidence;
    public float x_min, x_max, y_min, y_max;
}

[System.Serializable]
public class DetectionResult
{
    public string status;
    public List<DetectionItem> items;
}

public class DisplayDetectedPhoto : MonoBehaviour
{
    [SerializeField] Color[] _boxColor;
    public RawImage rawImageDetected; // Assign in Inspector
    public GameObject boundingBoxPrefab; // Assign in Inspector
    private string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiYzYxYTJkYWQtMmIzYi00MjIzLWJjMzctZGQyMGRkZmU0YzUyIiwidHlwZSI6ImFwaV90b2tlbiJ9.bPsDmmZli_P4YHWnLyanm2a-_hb1eE9grtKJm8v2rg0";
    private string apiUrl = "https://api.edenai.run/v2/image/object_detection";


    [SerializeField] DetectionResult _fakeTestData;
    [Button] void TestDisplayBox() => DisplayBoundingBoxes(_fakeTestData);

    public void ProcessImageAndDrawBox() => StartCoroutine(ProcessLatestImageAndDetect(PathUtil.GetSavePath()));

    internal IEnumerator ProcessLatestImageAndDetect(string directoryPath)
    {
        string imagePath = GetLatestImagePath(directoryPath);
        if (!string.IsNullOrEmpty(imagePath))
        {
            yield return StartCoroutine(LoadImage(imagePath, texture => StartCoroutine(SendImageToEdenAI(texture))));
        }
        else
        {
            Debug.LogWarning("No image found in directory: " + directoryPath);
        }
    }

    private string GetLatestImagePath(string directoryPath)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        var latestImageFile = directoryInfo.GetFiles().Where(f => f.Extension.Equals(".png") || f.Extension.Equals(".jpg")).OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
        return latestImageFile?.FullName;
    }

    private IEnumerator LoadImage(string imagePath, System.Action<Texture2D> onLoaded)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageBytes))
        {
            rawImageDetected.texture = texture;
            onLoaded?.Invoke(texture);
        }
        else
        {
            Debug.LogWarning("Failed to load image: " + imagePath);
        }
        yield return null;
    }


    public DetectionResult ConvertJSONToUnityObject(string jsonText)
    {
        var jsonObject = JSON.Parse(jsonText);
        var api4ai = jsonObject["api4ai"];
        var status = api4ai["status"];
        var items = new List<DetectionItem>();
        foreach (JSONNode item in api4ai["items"].AsArray)
        {
            items.Add(new DetectionItem
            {
                label = item["label"],
                confidence = item["confidence"].AsFloat,
                x_min = item["x_min"].AsFloat,
                x_max = item["x_max"].AsFloat,
                y_min = item["y_min"].AsFloat,
                y_max = item["y_max"].AsFloat
            });
        }

        return new DetectionResult
        {
            status = status,
            items = items
        };
    }

    private IEnumerator SendImageToEdenAI(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToJPG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.jpg", "image/jpeg");
        form.AddField("providers", "[\"api4ai\"]");

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Received response from API");

                // Parse the JSON response
                DetectionResult result = ConvertJSONToUnityObject(www.downloadHandler.text);
                if (result != null && result.items != null)
                {
                    Debug.Log("DetectionResult");
                    Debug.Log(www.downloadHandler.text);

                    Debug.Log($"{result.items}");
                    Debug.Log($"{result.items.Count}");

                    DisplayBoundingBoxes(result);
                }
                else
                {
                    Debug.LogWarning("Missing or empty detection result.");
                }
            }
            else
            {
                Debug.LogError("Error sending request: " + www.error);
            }
        }
    }

    private void DisplayBoundingBoxes(DetectionResult result)
    {
        // กำหนดที่อยู่ของไฟล์ labels.txt
        string saveFolderPath = Path.Combine(Application.streamingAssetsPath, "CroppedPicture");
        string filePath = Path.Combine(saveFolderPath, "labels.txt");

        // เรียกใช้งาน StreamWriter โดยใช้โหมด Append เพื่อไม่ทับข้อมูลที่มีอยู่
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // ตรวจสอบข้อมูลที่ส่งเข้ามา
            if (rawImageDetected == null || result == null || result.items == null)
            {
                Debug.LogWarning("Missing image or detection result.");
                return;
            }

            // สร้างบล็อกตรวจจับและบันทึกข้อมูลลงในไฟล์ labels.txt
            int colorIndex = 0;
            foreach (DetectionItem item in result.items)
            {
                GameObject box = Instantiate(boundingBoxPrefab, rawImageDetected.transform);
                RectTransform rt = box.GetComponent<RectTransform>();

                float xMin = item.x_min * rawImageDetected.rectTransform.rect.width;
                float xMax = item.x_max * rawImageDetected.rectTransform.rect.width;
                float yMin = (1 - item.y_max) * rawImageDetected.rectTransform.rect.height;
                float yMax = (1 - item.y_min) * rawImageDetected.rectTransform.rect.height;
                float width = xMax - xMin;
                float height = yMax - yMin;

                rt.anchorMin = new Vector2(xMin / rawImageDetected.rectTransform.rect.width, yMin / rawImageDetected.rectTransform.rect.height);
                rt.anchorMax = new Vector2(xMax / rawImageDetected.rectTransform.rect.width, yMax / rawImageDetected.rectTransform.rect.height);
                rt.sizeDelta = new Vector2(width, height);
                rt.anchoredPosition = new Vector2(xMin + width / 2, yMin + height / 2);

                TMP_Text label = box.GetComponentInChildren<TMP_Text>();
                label.SetText($"{item.label} ({item.confidence:P2})");

                // เขียน label ลงในไฟล์ labels.txt แยกบรรทัดแต่ละข้อมูล
                writer.WriteLine($"{item.label} ({item.confidence:P2})");

                colorIndex++;
                Image boxImage = box.GetComponent<Image>();
                boxImage.color = _boxColor[colorIndex % _boxColor.Length];

                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                CropAndSaveImage(rawImageDetected.texture as Texture2D, item.x_min, item.x_max, item.y_min, item.y_max, saveFolderPath, $"cropped_image_{System.DateTime.Now.ToString("yyyyMMddHHmmss")}_{colorIndex}.png");

                Debug.Log($"{item.label} detected with confidence: {item.confidence:P2}");
            }
        }
    }


    private void CropAndSaveImage(Texture2D originalImage, float xMin, float xMax, float yMin, float yMax, string saveFolderPath, string fileName)
    {
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.Log("Save folder does not exist. Creating folder...");
            Directory.CreateDirectory(saveFolderPath);
        }

        int startX = (int)(xMin * originalImage.width);
        int startY = (int)((1 - yMax) * originalImage.height);
        int width = (int)((xMax - xMin) * originalImage.width);
        int height = (int)((yMax - yMin) * originalImage.height);

        Texture2D croppedTexture = new Texture2D(width, height);
        Color[] pixels = originalImage.GetPixels(startX, startY, width, height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        byte[] croppedBytes = croppedTexture.EncodeToPNG();
        string filePath = Path.Combine(saveFolderPath, fileName);
        File.WriteAllBytes(filePath, croppedBytes);

        Debug.Log("Image cropped and saved at: " + filePath);
    }



    public void ExampleCropAndSave()
    {
    string saveFolderPath = Path.Combine(Application.streamingAssetsPath, "CroppedPicture");
    string fileName = "cropped_image_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";


    // ตรวจสอบว่าโฟลเดอร์ "CroppedPicture" ยังไม่มีอยู่ ถ้าไม่มีให้สร้างโฟลเดอร์ใหม่
    if (!Directory.Exists(saveFolderPath))
    {
        Directory.CreateDirectory(saveFolderPath);
    }

    // เรียกใช้ CropAndSaveImage โดยระบุพิกัดที่ต้องการตัด, รูปภาพต้นฉบับ, โฟลเดอร์ที่ต้องการบันทึก, และชื่อไฟล์
    CropAndSaveImage(rawImageDetected.texture as Texture2D, 0.1f, 0.5f, 0.2f, 0.8f, saveFolderPath, fileName);


    }


}