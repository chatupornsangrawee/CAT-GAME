using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using VInspector;

public class TestAIJsonData : MonoBehaviour
{
    [TextArea][SerializeField] string _testJsonData;

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


    [Button]
    public void TestConvert()
    {
        DetectionResult result = ConvertJSONToUnityObject(_testJsonData);
        Debug.Log($"{result.items}");
        Debug.Log($"{result.items.Count}");
    }
}