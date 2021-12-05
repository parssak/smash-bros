using UnityEngine;

public class TrackTargets : MonoBehaviour {

  [SerializeField]
  Transform[] targets;

  [SerializeField]
  float boundingBoxPadding = 2f;

  [SerializeField]
  float zoomSpeed = 20f;

  Camera camera;

  const float MAX_TRACK_DISTANCE = 30f;

  float minimumOrthographicSize = 10f;
  float maximumOrthographicSize = 9f;
  float currentOrthographicSize = 10f;

  void Awake()
  {
    camera = GetComponent<Camera>();
  }


  void LateUpdate()
  {
    Rect boundingBox = CalculateTargetsBoundingBox();
    transform.position = CalculateCameraPosition(boundingBox);
  }

  Rect CalculateTargetsBoundingBox()
  {
    float minX = Mathf.Infinity;
    float maxX = Mathf.NegativeInfinity;
    float minY = Mathf.Infinity;
    float maxY = Mathf.NegativeInfinity;

    foreach (Transform target in targets)
    {
      Vector3 position = target.position;
      Player player = target.GetComponent<Player>();
      
      
      if (Vector3.Distance(Vector3.zero, position) > MAX_TRACK_DISTANCE)
      {
        position = position.normalized * MAX_TRACK_DISTANCE;
      }

      minX = Mathf.Min(minX, position.x);
      minY = Mathf.Min(minY, position.y);
      maxX = Mathf.Max(maxX, position.x);
      maxY = Mathf.Max(maxY, position.y);
    }

    return Rect.MinMaxRect(minX - boundingBoxPadding, maxY + boundingBoxPadding, maxX + boundingBoxPadding, minY - boundingBoxPadding);
  }

  Vector3 CalculateCameraPosition(Rect boundingBox)
  {
    Vector2 boundingBoxCenter = boundingBox.center;
    float area = Mathf.Abs(boundingBox.width * boundingBox.height);
    float max = 100f;
    float ratio = area / 12f / max;
    float baseZ = -25f;
    float zAxis = Mathf.Clamp(baseZ * ratio, -39f, -9f);
    Debug.Log("Ratio" + ratio);
    Vector3 cameraTarget = new Vector3(boundingBoxCenter.x, boundingBoxCenter.y, zAxis);
    return Vector3.Lerp(GetComponent<Camera>().transform.position, cameraTarget, Time.deltaTime * zoomSpeed);
  }

  float CalculateOrthographicSize(Rect boundingBox)
  {
    float orthographicSize = currentOrthographicSize;
    Vector3 topRight = new Vector3(boundingBox.x + boundingBox.width, boundingBox.y, 0f);
    Vector3 topRightAsViewport = GetComponent<Camera>().WorldToViewportPoint(topRight);

    if (topRightAsViewport.x >= topRightAsViewport.y)
      orthographicSize = Mathf.Abs(boundingBox.width) / GetComponent<Camera>().aspect / 2f;
    else
      orthographicSize = Mathf.Abs(boundingBox.height) / 2f;

    return Mathf.Clamp(Mathf.Lerp(currentOrthographicSize, orthographicSize, Time.deltaTime * zoomSpeed), minimumOrthographicSize, maximumOrthographicSize);
  }

  void OnDrawGizmos()
  {
    if (targets == null || targets.Length == 0)
    {
      return;
    }

    Rect boundingBox = CalculateTargetsBoundingBox();
    Gizmos.color = Color.grey;
    Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);
  }

  
    
}