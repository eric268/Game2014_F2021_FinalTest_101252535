using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingPlatform : MonoBehaviour
{
    public LayerMask playerLayerMask;
    public float startingWidthScale;
    public float expandSpeed;
    // Start is called before the first frame update
    void Start()
    {
        startingWidthScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShrinkPlatform()
    {
        StartCoroutine(Shrink());
    }

    private void ExpandPlatform()
    {
        StartCoroutine(Expand());
    } 

    IEnumerator Shrink()
    {
        while (transform.localScale.x > 0)
        {
            float scale = transform.localScale.x - expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            yield return null;
        }
        transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
        yield return null;
    }
 

    IEnumerator Expand()
    {
        while (transform.localScale.x < startingWidthScale)
        {
            float scale = transform.localScale.x + expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            yield return null;
        }
        transform.localScale = new Vector3(startingWidthScale, transform.localScale.y, transform.localScale.z);
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if ((layer == layer << playerLayerMask.value) &&  collision.contacts[0].normal == Vector2.down)
        {
            ShrinkPlatform();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if ((layer == layer << playerLayerMask.value))
        { 
            ExpandPlatform();
        }
    }
}
