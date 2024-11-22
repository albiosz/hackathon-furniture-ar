using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FurniturePlacementManager : MonoBehaviour
{
  public GameObject SpawnableFurniture;
  
  public XROrigin sessionOrigin;
  public ARRaycastManager raycastManager;
  public ARPlaneManager planeManager;

  private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();


}
