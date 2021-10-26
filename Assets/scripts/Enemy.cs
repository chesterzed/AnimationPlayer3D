using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public bool isDead;
    private float _timer;

    private void Update()
    {
        if (isDead)
            Dead();
    }

    public void Dead()
    {
        _timer += Time.deltaTime;
        if (_timer > 5)
        {
            GetComponent<Animator>().enabled = true;
            transform.position = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
            transform.rotation = Quaternion.identity;
             _timer = 0;
            isDead = false;
        }
    }

}
