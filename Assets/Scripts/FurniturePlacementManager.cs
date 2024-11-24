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
    private List<GameObject> placedFurnitureObjects = new List<GameObject>();  // Track multiple placed furniture objects
    private GameObject selectedFurniture; // To store the currently selected furniture for rotation

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

                if (IsButtonPressed() == false)
                {
                  // Try to select furniture first
                  if (!SelectFurnitureAtPosition(pointerPosition))
                  {
                    // If no furniture selected, try to place new furniture
                    bool collision = raycastManager.Raycast(pointerPosition, raycastHits, TrackableType.Planes);

                    if (collision)
                    {
                      PlaceFurniture();
                    }
                  }
                }
            }
        }
    }

    private void PlaceFurniture()
    {
        // Instantiate the furniture object at the raycast hit position and rotation
        GameObject _object = Instantiate(SpawnableFurniture);
        _object.transform.position = raycastHits[0].pose.position;
        _object.transform.rotation = Quaternion.Euler(-90, 0, 0);

        // Add the placed object to the list
        placedFurnitureObjects.Add(_object);

        Debug.Log("Object placed at: " + raycastHits[0].pose.position);
    }

    private void EnablePlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true); // Show individual planes
        }
        planeManager.enabled = true; // Enable plane manager
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

  // Method to select a furniture object at a given screen position
  private bool SelectFurnitureAtPosition(Vector2 screenPosition)
  {
    Ray ray = Camera.main.ScreenPointToRay(screenPosition);
    RaycastHit hit;

    // Check if the ray hits any collider
    if (Physics.Raycast(ray, out hit))
    {
      GameObject hitObject = hit.collider.gameObject;

      // Check if the hit object is in the list of placed furniture
      if (placedFurnitureObjects.Contains(hitObject))
      {
        selectedFurniture = hitObject;
        Debug.Log("Selected furniture: " + hitObject.name);
        return true;
      }
    }

    // No furniture selected
    selectedFurniture = null;
    Debug.Log("No furniture selected.");
    return false;
  }

  // Method to rotate the selected furniture object
  private void RotateFurniture(float rotationAngle)
  {
    if (selectedFurniture != null)
    {
      selectedFurniture.transform.Rotate(Vector3.forward, rotationAngle);
      Debug.Log($"Rotated {selectedFurniture.name} by {rotationAngle} degrees.");
    }
    else
    {
      Debug.LogWarning("No furniture selected for rotation.");
    }
  }

  public void RotateFurnitureLeft()
  {
    RotateFurniture(-90);
  }

  public void RotateFurnitureRight()
  {
    RotateFurniture(90);
  }

  // New method to clear furniture objects from last to first
  public void ClearFurniture()
    {
        // Loop through the list in reverse to delete the last added objects
        for (int i = placedFurnitureObjects.Count - 1; i >= 0; i--)
        {
            Destroy(placedFurnitureObjects[i]); // Destroy the object
            placedFurnitureObjects.RemoveAt(i); // Remove the object from the list
        }

        Debug.Log("All placed furniture cleared.");
    }
}
