using UnityEngine;

public static class PathUtil
{
    public static string folderName = "CatTopia";
    public static string GetSavePath() => $"{Application.persistentDataPath}/{folderName}";
    public static string GetDirectory() => $"{Application.persistentDataPath}";
}