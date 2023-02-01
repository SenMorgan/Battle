using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Animations;

[ExecuteAlways]

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your Plane Marker here")]
    [SerializeField] private GameObject _planeMarkerPrefab;

    private ARRaycastManager _arRaycastManagerScript;

    private Vector2 _touchPosition = default;

    public GameObject objToSpawn;

    [Header("Put scrollView here")]
    public GameObject scrollView;

    public bool chooseObject = false;

    [Header("Put your AR Camera here")]
    [SerializeField] private Camera _ARCamera;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private GameObject _selectedObject;

    public bool rotation;

    private Quaternion _YRotation;

    [Header("Put your object size TextMesh here")]
    public GameObject objectSizeText;

    private TextMeshProUGUI _sizeText;

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManagerScript = GetComponent<ARRaycastManager>();
        Assert.IsNotNull(_arRaycastManagerScript);

        Assert.IsNotNull(objectSizeText);
        _sizeText = objectSizeText.GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_sizeText);

        _planeMarkerPrefab.SetActive(false);
        scrollView.SetActive(false);
    }


    // Print real world size of the object
    void PrintObjectSize(GameObject _obj)
    {
        if (_obj != null)
        {
            // Print all local scale values and all lossyScale values with 2 decimal places
            _sizeText.text = "Local scale: " + _obj.transform.localScale.ToString("F2") +
                            System.Environment.NewLine +
                            "Lossy scale: " + _obj.transform.lossyScale.ToString("F2");

        }
        else
        {
            _sizeText.text = "Object is null";
        }
    }

    // Update is called once per frame
    void Update()
    {
        _arRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), _hits, TrackableType.Planes);

        if (chooseObject)
        {
            SetObject();
        }

        // MoveAndRotateObject();
        // ShowPlaneMarker();

        GameObject _obj = GameObject.FindWithTag("Player");
        if (_obj != null)
        {
            _obj = _obj.transform.Find("Top").gameObject;
            PrintObjectSize(_obj);
        }

        // TODO: remove. This is for debug
        if (rotation)
        {
            rotation = false;
            // Find object with name BoxParent
            GameObject _boxParent = GameObject.Find("BoxParent");
            Assert.IsNotNull(_boxParent, "BoxParent is null");
            // Change Z axis size of the box collider
            _boxParent.GetComponent<BoxCollider>().size =
                new Vector3(_boxParent.GetComponent<BoxCollider>().size.x,
                _boxParent.GetComponent<BoxCollider>().size.y,
                 _boxParent.GetComponent<BoxCollider>().size.z + 0.1f);

        }
    }

    void OnEnable()
    {
        BoxBaseScript.onSizeChanged += onParentSizeChanged;
    }

    void OnDisable()
    {
        BoxBaseScript.onSizeChanged -= onParentSizeChanged;
    }

    // Change the size and position of every part of the box. «Bottom», «Top», «Left», «Right», «Rear» are the names of parts of the box
    // Center and position of the «Bottom» part is fixed
    // Use Vector3 from, Vector3 to arguments to get old and new size of the box
    void onParentSizeChanged(Vector3 from, Vector3 to)
    {
        GameObject _obj = GameObject.FindWithTag("Player");

        // Debug.Log("From: " + from.ToString() + " To: " + to.ToString() + " Object: " + _obj);

        if (_obj == null)
        {
            //log
            Debug.Log("Object is null in onParentSizeChanged");
            return;
        }

        GameObject _bottom = _obj.transform.Find("Bottom").gameObject;
        GameObject _top = _obj.transform.Find("Top").gameObject;
        GameObject _left = _obj.transform.Find("Left").gameObject;
        GameObject _right = _obj.transform.Find("Right").gameObject;
        GameObject _rear = _obj.transform.Find("Rear").gameObject;

        // Delta between old and new size of the box
        Vector3 _size = new Vector3(to.x - from.x, to.y - from.y, to.z - from.z);




        /* If box depth and width were changed, then we need to change X and Z scale of the «Top» and
            «Bottom» parts of the box, because this parts were rotated when box was created.*/
        Vector3 _newTopBottomSize = new Vector3(_top.transform.localScale.x + _size.x,
                                        _top.transform.localScale.y + _size.z,
                                        _top.transform.localScale.z);
        _top.transform.localScale = _newTopBottomSize;
        _bottom.transform.localScale = _newTopBottomSize;

        /* Also if box depth changed, then we need to change Z position of the «Top» and
            «Bottom» parts of the box to ensure that the «Rear» part is still attached to the wall.
            To change this value, we need to change «Position Offset» in «Constraint Settings» in
            «Parent Constraint» component using «SetTranslationOffset» method */
        ParentConstraint _topParentConstraint = _top.GetComponent<ParentConstraint>();
        // Get actual position offset
        Vector3 _topConstrPositionOffset = _topParentConstraint.GetTranslationOffset(0);
        // Change Z position offset
        _topConstrPositionOffset.z = _topConstrPositionOffset.z - _size.z / 2;
        // Set new position offset
        _topParentConstraint.SetTranslationOffset(0, _topConstrPositionOffset);

        ParentConstraint _bottomParentConstraint = _bottom.GetComponent<ParentConstraint>();
        // Get actual position offset
        Vector3 _bottomConstrPositionOffset = _bottomParentConstraint.GetTranslationOffset(0);
        // Change Z position offset
        _bottomConstrPositionOffset.z = _bottomConstrPositionOffset.z - _size.z / 2;
        // Set new position offset
        _bottomParentConstraint.SetTranslationOffset(0, _bottomConstrPositionOffset);

        /* And finally we need to change center of the box collider of the «Constrain source» object
            to ensure that the «Rear» part is still attached to the wall */
        // Get the «Constrain source» object
        GameObject _constrainSource = _topParentConstraint.GetSource(0).sourceTransform.gameObject;
        // Change center of the box collider
        _constrainSource.GetComponent<BoxCollider>().center = new Vector3(_constrainSource.GetComponent<BoxCollider>().center.x,
                                                                        _constrainSource.GetComponent<BoxCollider>().center.y,
                                                                        _constrainSource.GetComponent<BoxCollider>().center.z - _size.z / 2);

    }

    // Show marker
    void ShowPlaneMarker()
    {
        if (_hits.Count > 0)
        {
            _planeMarkerPrefab.transform.position = _hits[0].pose.position;

            if (!_planeMarkerPrefab.activeInHierarchy)
                _planeMarkerPrefab.SetActive(true);
        }
    }


    void SetObject()
    {
        // Set object on marker
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Touched the UI");
                return;
            }
            // Instantiate your object
            Instantiate(objToSpawn, _hits[0].pose.position, objToSpawn.transform.rotation);
            chooseObject = false;
            // _planeMarkerPrefab.SetActive(false);
        }
    }

    void MoveAndRotateObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            _touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = _ARCamera.ScreenPointToRay(_touchPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("Unselected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                    else
                    {
                        Debug.Log("No object found with tag 'Unselected'");
                    }
                }
            }

            _selectedObject = GameObject.FindWithTag("Selected");
            if (_selectedObject == null)
            {
                Debug.Log("No object found with tag 'Selected'");
                return;
            }

            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1)
            {

                if (rotation)
                {
                    _YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f);
                    _selectedObject.transform.rotation = _YRotation * _selectedObject.transform.rotation;
                }
                else
                {
                    _arRaycastManagerScript.Raycast(_touchPosition, _hits, TrackableType.Planes);
                    _selectedObject.transform.position = _hits[0].pose.position;
                }
            }
            else if (touch.phase == TouchPhase.Moved && Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                _selectedObject.transform.localScale += new Vector3(difference * 0.001f, difference * 0.001f, difference * 0.001f);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (_selectedObject.CompareTag("Selected"))
                {
                    _selectedObject.tag = "Unselected";
                }

            }
        }
    }
}
