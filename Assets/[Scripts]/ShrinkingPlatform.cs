using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShrinkState
{
    EXPANDING,
    SHRINKING,
}


public class ShrinkingPlatform : MonoBehaviour
{
    public LayerMask playerLayerMask;
    public float startingWidthScale;
    public float expandSpeed;
    public bool isColliding;
    public AudioSource[] audioSources;

    // Start is called before the first frame update
    void Start()
    {
        startingWidthScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
    }

    void CheckCollision()
    {
        //collider.GetContacts()
    }

    private void ShrinkPlatform()
    {
        StartCoroutine(Shrink());
    }

    private void ExpandPlatform()
    {
        StartCoroutine(Expand());
    } 

    IEnumerator Disappeared()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(Expand());
    }

    IEnumerator Shrink()
    {
        PlaySound(ShrinkState.SHRINKING);
        while (transform.localScale.x > 0)
        {
            if (!isColliding)
                yield break;
            float scale = transform.localScale.x - expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            yield return null;
        }
        transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);

        if (transform.localScale.x == 0)
        {
            isColliding = false;
            StartCoroutine(Disappeared());
        }
        else
            yield return null;
    }
 

    IEnumerator Expand()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        PlaySound(ShrinkState.EXPANDING);
        isColliding = false;
        while (transform.localScale.x < startingWidthScale)
        {
            if (isColliding)
                yield break;
            float scale = transform.localScale.x + expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            yield return null;
        }
        transform.localScale = new Vector3(startingWidthScale, transform.localScale.y, transform.localScale.z);

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if ((layer == layer << playerLayerMask.value) && collision.contacts[0].normal == Vector2.down)
        {
            if (!isColliding)
            {
                isColliding = true;
                ShrinkPlatform();
            }

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

    private void PlaySound(ShrinkState state)
    {
        switch (state)
        {
            case ShrinkState.EXPANDING:
                audioSources[(int)ShrinkState.SHRINKING].Stop();
                if (!audioSources[(int)ShrinkState.EXPANDING].isPlaying)
                {
                    audioSources[(int)ShrinkState.EXPANDING].Play();
                }
                break;
            case ShrinkState.SHRINKING:
                audioSources[(int)ShrinkState.EXPANDING].Stop();
                if (!audioSources[(int)ShrinkState.SHRINKING].isPlaying)
                {
                    audioSources[(int)ShrinkState.SHRINKING].Play();
                }
                break;
        }

    }
}
