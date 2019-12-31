using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseClasses {

    public abstract class Entity : MonoBehaviour {

        [Header("General Variables")]
        private float currentHealth;
        [SerializeField]
        private float maxHealth;

        public float CurrentHealth {
            get { return currentHealth; }
            set {
                if(value < 0)
                {
                    currentHealth = 0;
                }
                else if(value > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                else
                {
                    currentHealth = value;
                }
            }
        }

		private void OnValidate()
		{
            currentHealth = maxHealth;
		}


		public bool isAlive()
        {
            return currentHealth > 0;
        }
    }

}