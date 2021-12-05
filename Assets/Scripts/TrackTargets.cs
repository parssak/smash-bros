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

  void Awake()
  {
    camera = GetComponent<Camera>();
  }

  void LateUpdate()
  {
    Rect boundingBox = CalculateTargetsBoundingBox();
    transform.position = CalculateCameraPosition(boundingBox);
    Debug.Log(boundingBox);
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
      
      if (player && player.isDead) {
        position = Vector3.zero;
      }
      
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
    float zAxis = transform.position.z;
    Vector3 cameraTarget = new Vector3(boundingBoxCenter.x, boundingBoxCenter.y, zAxis);
    return Vector3.Lerp(GetComponent<Camera>().transform.position, cameraTarget, Time.deltaTime * zoomSpeed);
  }

  
    
}