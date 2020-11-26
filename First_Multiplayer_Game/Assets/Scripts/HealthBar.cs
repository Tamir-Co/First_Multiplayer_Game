using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image health_fill;

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    public void set_MaxHealth(int max_health)
    {
        slider.maxValue = max_health;
        slider.value = max_health;
        health_fill.color = gradient.Evaluate(1);
    }

    public void set_Health(int health)
    {
        slider.value = health;
        health_fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}