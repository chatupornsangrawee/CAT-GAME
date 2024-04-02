/*using UnityEngine;

public class PopupController : MonoBehaviour
{
    private string imageUrl;  // URL หรือข้อมูลรูปภาพที่จะถูกประมวลผลหรือลบ

    // สร้างเมท็อดสำหรับการเรียกเมื่อปุ่ม "Learn" ถูกกด
    public void OnLearnButtonClicked()
    {

    }

    // สร้างเมท็อดสำหรับการเรียกเมื่อปุ่ม "Discard" ถูกกด
    public void OnDiscardButtonClicked()
    {
        // ลบรูปภาพจาก Firebase Storage
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference storageRef = storage.GetReferenceFromUrl(imageUrl);

        storageRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to delete image from Firebase Storage: " + task.Exception);
            }
            else
            {
                Debug.Log("Image deleted from Firebase Storage.");
            }
        });

        // TODO: อื่น ๆ ที่คุณต้องการทำหลังจากลบรูปภาพ
    }

    // เพิ่มเมธอด 'SetImageData' ที่ CameraController สามารถเรียกใช้ได้
    public void SetImageData(string imageUrl)
    {
        this.imageUrl = imageUrl;
    }
}*/