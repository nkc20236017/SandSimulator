using UnityEngine;
using UnityEngine.SceneManagement;

public class Parabola : MonoBehaviour
{
    [Header("Parabola Config")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayerMask;
    
    private LineRenderer _lineRenderer;
    private Camera _camera;
    private Scene _predictionScene;
    private PhysicsScene2D _physicsScene2D;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        
        _camera = Camera.main;
    }
    
    public void CreatePhysicsScene(GameObject blowOutObject, Vector3 origin, Vector3 direction)
    {
        var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _predictionScene = SceneManager.CreateScene("Parabola", parameters);
        _physicsScene2D = _predictionScene.GetPhysicsScene2D();
        
        var simulationObject = Instantiate(blowOutObject, origin, Quaternion.identity);
        simulationObject.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(simulationObject, _predictionScene);
    }

    public void SimulateParabola(Vector3 origin, Vector3 direction)
    {
        var ghost = Instantiate(gameObject, origin, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghost, _predictionScene);
        
        var rigidbody2D = ghost.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = direction;
    }
}
