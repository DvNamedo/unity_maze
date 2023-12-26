using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{


    void Start()
    {
        Centers.instance.currentHP = Centers.instance.maxHP;

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("kill"))
        {
            TakeDamage(1);
        }

        if (collision.gameObject.CompareTag("spike"))
        {
            if (!collision.gameObject.GetComponent<SpikeTrapDemo>().isSafe)
            {
                TakeDamage(2);
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {





        if (collision.gameObject.CompareTag("bonus"))
        {

            Destroy(collision.gameObject);
            getBonus(10);
        }

        if (collision.gameObject.CompareTag("end"))
        {
            Centers.instance.isGameEnd = true;
        }


    }

    void getBonus(int bonus)
    {
        Centers.instance.score += bonus;
        Debug.Log($"bonus! : {Centers.instance.score}");
    }

    void TakeDamage(int damage)
    {
        Debug.Log("damaged!");
        Centers.instance.currentHP -= damage;

        if (Centers.instance.currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Centers.instance.isDead = true;
        Centers.instance.isGameEnd = true;
    }
}
