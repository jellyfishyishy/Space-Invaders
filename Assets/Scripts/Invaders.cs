using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class Invaders : MonoBehaviour
{
    /// <summary>
    /// Prefab for invader
    /// </summary>
    public Invader[] prefabs;

    /// <summary>
    /// missile projectile
    /// </summary>
    public Projectile missilePrefab;

    /// <summary>
    /// # of rows
    /// </summary>
    public int rows = 5;

    /// <summary>
    /// # of columns
    /// </summary>
    public int columns = 11;

    /// <summary>
    /// Direction moving in
    /// </summary>
    private Vector3 dir = Vector2.right;

    private float missileAttackRate = 1f;

    /// <summary>
    /// speed of invaders
    /// </summary>
    public AnimationCurve speed;

    /// <summary>
    /// sound byte
    /// </summary>
    public AudioSource pew;

    public int amountKilled
    {
        get; private set;
    }

    public int amountAlive => this.totalInvaders - this.amountKilled;

    public int totalInvaders => this.rows * this.columns;

    public float percentKilled => (float) this.amountKilled / (float) this.totalInvaders;

    // Start is called before the first frame update
    void Start()
    {
        pew = GetComponent<AudioSource>();
        pew.playOnAwake = false;
        InvokeRepeating(nameof(MissileAttack), missileAttackRate, missileAttackRate);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * this.speed.Evaluate(this.percentKilled) * Time.deltaTime;

        Vector3 lEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (dir == Vector3.right && invader.position.x >= (rEdge.x-1))
            {
                NextRow();
            }
            else if (dir == Vector3.left && invader.position.x <= (lEdge.x+1))
            {
                NextRow();
            }
        }
    }

    ///
    /// changes direction
    /// 
    private void NextRow()
    {
        dir.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float width = 2.0f * (this.columns - 1);
            float height = 2.0f * (this.rows - 1);
            Vector2 centering = new Vector2(-width/2, -height/2);
            Vector3 rowPos = new Vector3(centering.x, centering.y + (row * 2.0f), 0.0f);
            
            for (int col = 0; col < this.columns; col++)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform);
                invader.killed += InvaderKilled;
                Vector3 pos = rowPos;
                pos.x += col * 2.0f;
                invader.transform.localPosition = pos;
            }

        }
    }

    private void InvaderKilled()
    {
        pew.PlayOneShot(pew.clip);
        pew.Play();
        this.amountKilled++;

        if (this.amountKilled >= this.totalInvaders)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void MissileAttack()
    {
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1f/(float)this.amountAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }
}
