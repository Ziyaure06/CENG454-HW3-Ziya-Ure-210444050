using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class ColorFeedback : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color deadColor = Color.red;

    private Renderer _renderer;
    private IDamageable _damageable;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
     
        _damageable = GetComponent<IDamageable>();
    }

    private void OnEnable()
    {
        if (_damageable != null)
        {
            
            _damageable.OnHealthPercentChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (_damageable != null)
        {
          
            _damageable.OnHealthPercentChanged -= HandleHealthChanged;
        }
    }

    
    private void HandleHealthChanged(float percent)
    {
       
        _renderer.material.color = Color.Lerp(deadColor, healthyColor, percent);
    }
}