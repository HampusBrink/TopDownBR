using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] private AnimationCurve zoneCurve;
        
    [SerializeField] private Vector2 mapSize;

    [SerializeField] private float endRadius = 0;

    [SerializeField] private float totalShrinkTime = 3f * 60f;
        
    [SerializeField] private Material mat;
        
    private CircleCollider2D _col;

    private int _zonePhases;
    private float _phaseTime;
        
    private float _startRadius;
    private float _startVisualRadius;
    private Vector2 _startPosition;

    private Vector2 _endPoint;

    private void Awake()
    {
        _col = GetComponent<CircleCollider2D>();
            
        _endPoint = new Vector2(Random.Range(-mapSize.x, mapSize.x), Random.Range(-mapSize.y, mapSize.y));
        _startRadius = _col.radius;
        _startVisualRadius = mat.GetFloat("_Size");
        _startPosition = transform.position;
    }

    private float _visualTime, _physicTime;
    private void FixedUpdate()
    {
        _physicTime += Time.fixedDeltaTime;

        float elapsed01 = Mathf.Clamp01(_physicTime / totalShrinkTime);
        float eval = zoneCurve.Evaluate(elapsed01);
            
        ChangeRadius(Mathf.Lerp(_startRadius,endRadius / 2f,eval));
        ChangePosition(Vector2.Lerp(_startPosition,_endPoint,eval));
    }

    private void Update()
    {
        _visualTime += Time.deltaTime;

        float elapsed01 = (_visualTime / totalShrinkTime);
        float eval = zoneCurve.Evaluate(elapsed01);

        ChangeVisualSize(Mathf.Lerp(_startVisualRadius, endRadius, eval));
            
            
        // Change relative visual size always, as we're nice to people with 50+ FPS.
    }

    private void ChangePosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
    }

    private void ChangeVisualSize(float size)
    {
        mat.SetFloat("_Size", size);
    }

    private void ChangeRadius(float radius)
    {
        _col.radius = radius;
    }
        
    private void OnDisable()
    {
        mat.SetFloat("_Size", _startVisualRadius);
    }
}