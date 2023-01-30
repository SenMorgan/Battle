using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your Plane Marker here")]
    [SerializeField] private GameObject _planeMarkerPrefab;

    private ARRaycastManager _arRaycastManagerScript;

    private Vector2 _touchPosition = default;

    public GameObject ObjToSpawn;

    public bool ChooseObject = false;

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManagerScript = GetComponent<ARRaycastManager>();

        _planeMarkerPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if (ChooseObject == true)
        {
            ShowPlaneMarkerAndSetObject();
        }
    }


    // Always positioned at the center of the screen
    void ShowPlaneMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        _arRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // Show marker
        if (hits.Count > 0)
        {
            _planeMarkerPrefab.transform.position = hits[0].pose.position;
            _planeMarkerPrefab.SetActive(true);
        }

        // Set object on marker
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            // Instantiate your object
            Instantiate(ObjToSpawn, hits[0].pose.position, ObjToSpawn.transform.rotation);
        }
    }
}
