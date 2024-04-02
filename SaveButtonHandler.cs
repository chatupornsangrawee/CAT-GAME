using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class SaveButtonHandler : MonoBehaviour
{
    [SerializeField] private string apiUrl = "https://api.edenai.run/v2/image/object_detection";
    [SerializeField] private string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiYzYxYTJkYWQtMmIzYi00MjIzLWJjMzctZGQyMGRkZmU0YzUyIiwidHlwZSI6ImFwaV90b2tlbiJ9.bPsDmmZli_P4YHWnLyanm2a-_hb1eE9grtKJm8v2rg0";
    private string folderPath => Application.temporaryCachePath + "/Gallery";

    public void OnSaveButtonPressed(Texture2D sourceImage)
    {
        StartCoroutine(SendImageToEdenAI(sourceImage));
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
                EdenAIResponse result = JsonUtility.FromJson<EdenAIResponse>(www.downloadHandler.text);
                if (result.eden_ai != null && result.eden_ai.items != null && result.eden_ai.items.Length > 0)
                {
                    foreach (var item in result.eden_ai.items)
                    {
                        // Calculate the crop dimensions based on the detection bounds
                        int x = (int)(item.x_min * image.width);
                        int y = (int)(item.y_min * image.height);
                        int width = (int)((item.x_max - item.x_min) * image.width);
                        int height = (int)((item.y_max - item.y_min) * image.height);

                        Texture2D croppedImage = CropImage(image, x, y, width, height);
                        SaveCroppedImage(croppedImage);
                    }
                }
                else
                {
                    Debug.LogWarning("No detections found or parsing error.");
                }
            }
            else
            {
                Debug.LogError("Error sending request: " + www.error);
            }
        }
    }

    private Texture2D CropImage(Texture2D source, int x, int y, int width, int height)
    {
        Color[] pix = source.GetPixels(x, y, width, height);
        Texture2D dest = new Texture2D(width, height);
        dest.SetPixels(pix);
        dest.Apply();
        return dest;
    }

    private void SaveCroppedImage(Texture2D croppedImage)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"CroppedImage_{Time.time}.png";
        string filePath = Path.Combine(folderPath, fileName);
        byte[] bytes = croppedImage.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}
