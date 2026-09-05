using UnityEngine;

// Purely visual falling debris (no physics/collision) used for the earthquake-style effect.
public class DebrisPiece : MonoBehaviour
{
    private float fallSpeed;
    private float rotationSpeed;
    private float lifeTime;
    private float timer;

    public void Init(float _fallSpeed, float _rotationSpeed, float _lifeTime)
    {
        fallSpeed = _fallSpeed;
        rotationSpeed = _rotationSpeed;
        lifeTime = _lifeTime;
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
