using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupCanvasController : MonoBehaviour
{
    // public RawImage popupImage;
    // public GameObject popupCanvas;

    // // ฟังก์ชันนี้จะถูกเรียกเพื่อตั้งค่ารูปภาพที่ต้องการแสดงใน Popup
    // public void ShowPopupUI(string imageUrl)
    // {
    //     // ดึงรูปภาพจาก URL หรือข้อมูลที่คุณได้รับมา
    //     // และตั้งค่าให้กับ RawImage ใน Popup
    //     StartCoroutine(LoadImage(imageUrl));

    //     // แสดง Popup Canvas
    //     popupCanvas.SetActive(true);
    // }

    // // ฟังก์ชันสำหรับโหลดรูปภาพจาก URL และแสดงใน RawImage
    // IEnumerator LoadImage(string url)
    // {
    //     using (WWW www = new WWW(url))
    //     {
    //         yield return www;

    //         if (string.IsNullOrEmpty(www.error))
    //         {
    //             // ถ้าโหลดรูปภาพสำเร็จ ให้กำหนด Texture ให้กับ RawImage
    //             popupImage.texture = www.texture;
    //         }
    //         else
    //         {
    //             Debug.LogError("Error loading image: " + www.error);
    //         }
    //     }
    // }

    // // ฟังก์ชันสำหรับปิด Popup และลบรูปภาพ
    // public void ClosePopup()
    // {
    //     // ปิด Popup Canvas
    //     popupCanvas.SetActive(false);

    //     // ลบรูปภาพที่แสดงใน RawImage
    //     Destroy(popupImage.texture);
    // }
}
