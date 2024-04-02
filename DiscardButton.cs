using UnityEngine;
using System.IO;

public class DiscardButton : MonoBehaviour
{
    private void Start()
    {
        // Create the folder if it doesn't exist
        string folderPath = PathUtil.GetSavePath();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    private void DeleteLastPhoto()
    {
        // Get the folder path
        string folderPath = PathUtil.GetSavePath();

        // Get all files in the folder
        string[] files = Directory.GetFiles(folderPath);

        // Check if there are any files in the folder
        if (files.Length > 0)
        {
            // Find the latest created file
            string lastFile = files[0];
            for (int i = 1; i < files.Length; i++)
            {
                if (File.GetCreationTime(files[i]) > File.GetCreationTime(lastFile))
                {
                    lastFile = files[i];
                }
            }

            // Delete the last file
            File.Delete(lastFile);
            Debug.Log("Deleted the last captured photo: " + lastFile);
        }
        else
        {
            Debug.Log("No captured photos to delete.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Assuming left mouse button for click
        {
            // Raycast to check if the mouse is over the button
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                // If the mouse is over the button, delete the last photo
                DeleteLastPhoto();
            }
            else
            {
                Debug.Log("Raycast hit something other than the button.");
            }
        }
    }

}
