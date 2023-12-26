using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public int maxHP = 100; // 최대 HP
    public int currentHP;   // 라이브 HP

    void Start()
    {
        currentHP = maxHP;

    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("kill"))
        {
            TakeDamage(20);
        }

        if (collision.gameObject.CompareTag("bonus"))
        {
            Destroy(collision.gameObject);
            getBonus(10);
        }

    }

    void getBonus(int bonus)
    {
        Centers.instance.score += bonus;
    }

    void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
