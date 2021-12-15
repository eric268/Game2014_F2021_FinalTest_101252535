//------------------ShrinkingPlatform.cs------------------
//------------------Author: Eric Galway ------------------
//------------------Student Number: 101252535-------------
//--------Description: shrinking platform controller------
//--------controls the shrinking and expansion of the ----
//--------floating platforms. Also bobs them up and down--
//--------Last Edited: December 14 2021-------------------
//--------Revision History: 1.5 Added vertical bobbing ---


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enumeration for determining shrinking state
public enum ShrinkState
{
    EXPANDING,
    SHRINKING,
}

//Shrinking platform script
public class ShrinkingPlatform : MonoBehaviour
{
    public LayerMask playerLayerMask;
    public Rigidbody2D rigidBody;
    public float startingWidthScale;
    public float expandSpeed;
    public bool isColliding;
    public AudioSource[] audioSources;
    public float floatDistance;
    public float floatSpeed;
    public float direction;
    private float distanceTravelled;

    // Start is called before the first frame update
    void Start()
    {
        //Get the starting scale so that we know the value to return to
        startingWidthScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the platform up and down
        BobPlatform();
    }

    void BobPlatform()
    {
        float amountToMove = transform.position.y + direction * floatSpeed * Time.deltaTime;
        //Only want to travel on y axis
        transform.position = new Vector3(transform.position.x, amountToMove, transform.position.z);
        distanceTravelled += floatSpeed * Time.deltaTime;
        if (distanceTravelled >= floatDistance)
        {
            distanceTravelled = 0;
            direction *= -1;
        }
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
        //Wait briefly for player to fall before expanding again
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(Expand());
    }
    //Reduces the size of the platform up to 0 using a corountien
    IEnumerator Shrink()
    {
        PlaySound(ShrinkState.SHRINKING);
        while (transform.localScale.x > 0)
        {
            //If we stop colliding we want to exit out of this early 
            if (!isColliding)
                yield break;

            float scale = transform.localScale.x - expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);

            yield return null;
        }
        //Ensure if reduce scale to a negative number through subtraction we set it to 0
        transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);

        //This is how we notify that the player isnt colliding with a 0 x scale platform
        //need this cause OnCollisionExit2D will not detect leaving collision when it turns to 0
        if (transform.localScale.x == 0)
        {
            isColliding = false;
            StartCoroutine(Disappeared());
        }
        else
            yield return null;
    }
 
    //Expands the platform using a coroutine
    IEnumerator Expand()
    {
        //Want to wait slightly for player to fall or jump etc 
        yield return new WaitForSecondsRealtime(0.1f);
        PlaySound(ShrinkState.EXPANDING);
        isColliding = false;
        while (transform.localScale.x < startingWidthScale)
        {
            //Want to exit early if we are colliding again
            if (isColliding)
                yield break;

            float scale = transform.localScale.x + expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);

            yield return null;
        }
        //Want to ensure that we get back to exact starting value
        transform.localScale = new Vector3(startingWidthScale, transform.localScale.y, transform.localScale.z);

    }

    //Sometimes collision wasnt being detected by OnEnter so used this instead. Only fires
    //when we aren't already colliding so does the same thing just checks more often
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Only want to shrink platform when we are colliding with the top of it
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

    //Checks if we are no longer colliding with the platform
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
