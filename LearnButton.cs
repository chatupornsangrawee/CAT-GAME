using UnityEngine;

public class LearnButton : MonoBehaviour
{
    public DisplayDetectedPhoto displayDetectedPhoto;

    public void OnButtonClick()
    {
        StartCoroutine(displayDetectedPhoto.ProcessLatestImageAndDetect(PathUtil.GetSavePath()));
    }
}