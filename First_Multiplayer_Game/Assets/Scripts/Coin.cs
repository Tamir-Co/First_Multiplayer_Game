using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D coin_rb;
    private float initial_Y_pos;
    private SpriteRenderer sprite_renderer;

    // Start is called before the first frame update
    void Start()
    {
        coin_rb = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();

        initial_Y_pos = transform.position.y;
        coin_rb.velocity = new Vector2(Random.Range(-2.3f, 2.3f), 18f);

        StartCoroutine(FadeOut());  // Fedes out after 10 sec if not collected
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < initial_Y_pos)
        {
            coin_rb.gravityScale = 0;
            coin_rb.velocity = new Vector2();
        }
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(10);
        Color tmpcolor = sprite_renderer.color;
        while (tmpcolor.a > 0)  // Slowly fades out
        {
            tmpcolor.a -= Time.deltaTime;
            sprite_renderer.color = tmpcolor;
            yield return null;
        }
        Destroy(gameObject);
    }
}