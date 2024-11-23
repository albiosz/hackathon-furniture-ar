using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.XR.CoreUtils; // For XROrigin
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UnityEngine.InputSystem; // New Input System namespace
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject SpawnableFurniture;
    public XROrigin xrOrigin; // Ensure XROrigin is assigned
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    private void Update()
    {
        EnablePlanes(); 
        // Check for pointer input using the New Input System
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            // Ensure the press action just began
            if (Pointer.current.press.wasPressedThisFrame)
            {
                Vector2 pointerPosition = Pointer.current.position.ReadValue();
                bool collision = raycastManager.Raycast(pointerPosition, raycastHits, TrackableType.PlaneWithinPolygon);

                if (collision && IsButtonPressed() == false)
                {
                    // Instantiate the furniture object at the raycast hit position and rotation
                    GameObject _object = Instantiate(SpawnableFurniture);
                    _object.transform.position = raycastHits[0].pose.position;
                    _object.transform.rotation = raycastHits[0].pose.rotation;

                    Debug.Log("Object placed at: " + raycastHits[0].pose.position);

                    // OPTIONAL: Disable planes after placing the object
                    //DisablePlanes();
                }
            }
        }
    }

    // Separate function to disable planes safely
    private void DisablePlanes()
    {
        if (planeManager.enabled)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false); // Hide individual planes
            }
            planeManager.enabled = false; // Disable plane manager
        }
    }

    private void EnablePlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true); // Hide individual planes
        }
        planeManager.enabled = true; // Disable plane manager
    }
    public bool IsButtonPressed()
    {
        // Check if a UI button is currently being pressed
        if (EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SwitchFurniture(GameObject furniture)
    {
        Debug.Log("Furniture switched successfully!");
        SpawnableFurniture = furniture;
    }
}
