using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseClasses;

namespace Controllers {

    public class PlayerAttack : MonoBehaviour {

        public Animator playerAnimator;

        public Transform attackPoint;
        public float attackRange;
        public float attackPower;

        public LayerMask attackLayers;

		private void Update()
		{
            if(Input.GetKeyDown(KeyCode.R))
            {
                Attack();
            }
		}

		private void OnDrawGizmosSelected()
		{
            if(attackPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            }
		}

		private void Attack()
        {
            playerAnimator.SetBool("Attack", true);
            Collider2D[] hitEntities = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackLayers);

            for(int index = 0; index < hitEntities.Length; ++index)
            {
                hitEntities[index].gameObject.GetComponent<Entity>().CurrentHealth -= attackPower;
            }
        }
	}
}