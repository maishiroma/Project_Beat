using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers {

    public class PlayerAttack : MonoBehaviour {

        public Animator playerAnimator;

        public Transform attackPoint;
        public float attackRange;

        public LayerMask attackLayers;

        private Collider2D[] collisions = new Collider2D[3];

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

            int collisionCount = Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, collisions, attackLayers);

            print(collisionCount);

        }
	}
}