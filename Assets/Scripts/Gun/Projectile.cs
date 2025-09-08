using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] private float returnTime = 3f;
    [SerializeField] private float damagrToDo = 25f;
    private float timer;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > returnTime)
        {
            gameObject.SetActive(false);
            timer = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ObstcaleHealth eh = collision.collider.GetComponent<ObstcaleHealth>();
        if (eh != null)
        {
            eh.TakeDamage(damagrToDo);
        }


        gameObject.SetActive(false);
        timer = 0;
    }
}
