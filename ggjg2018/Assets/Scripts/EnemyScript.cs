using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int scoreValue = 1;
    private float speedMult = 60f;
    private float waitTime = 1f;
    private bool isMoving = false;

    private Animator mAnim;

    int attackCount = 0;

    // Use this for initialization
    void Start ()
    {
        mAnim = gameObject.GetComponent<Animator>();
    }
    private void OnMouseDown()
    {

        Destroy(gameObject);
        

    }
    // Update is called once per frame
    void Update ()
    {
		if(this.transform.position.y <= -2.95f)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime * speedMult, Space.World);
        }
		else
		{
		    StartCoroutine(Wait());
		}

        if (isMoving)
        {
            this.transform.Translate((BoardManager.GetForeGroundSpeed()/2) * Time.deltaTime);
        }
	}

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f); //time to wait until actually attacking the player
        
        
        if (attackCount < 1)
        {
            mAnim.SetBool("attack", true);
            if (mAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Animation")) //checking to see if the animation has cycled once
            {
                Debug.Log("attacking");
                GameManager.DecrementHealth(); //attacks the player 
                mAnim.SetBool("attack",false);
                attackCount++;
            }
                
        }
        
        yield return new WaitForSeconds(waitTime); //time to wait before moving across the screen
        isMoving = true;
    }

    private void OnDestroy()
    {
        GameManager.IncrementScore(scoreValue);
    }
}
