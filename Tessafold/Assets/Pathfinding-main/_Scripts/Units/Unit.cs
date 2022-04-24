using System.Collections;
using UnityEngine;

namespace Morphy_Pathfinding._Scripts.Units {
    public class Unit : MonoBehaviour {

        [SerializeField] private SpriteRenderer _renderer;
        int hitPoints;
        public int hitDamage = 1;

        [SerializeField] bool isEnemy;

        public bool isDead;

        public Vector2 targetPosition;
        public float lerpTime;
        public Vector2 previousPosition;
        public float lerpLenght = 0.2f;


        private void Start()
        {
            targetPosition = transform.position;
            previousPosition = transform.position;
            hitPoints = Random.Range(2, 5);
        }

        private void Update()
        {
            if (lerpTime < lerpLenght)
            {
                // Lerping between tiles
                transform.position = Vector2.Lerp(previousPosition, targetPosition, lerpTime / lerpLenght);
                lerpTime += Time.deltaTime;
            }
        }

        public void Init(Sprite sprite) {
            // Basically rendering a unit sprite
            _renderer.sprite = sprite;
        }

        public void Hit(int damage)
        {
            StartCoroutine(AttackDelay()); // Delay before the attack
            hitPoints -= damage; 
            GetComponent<Animator>().Play("Base Layer.GettingHitAnim", 0, 0.25f); // Playing hit anim

            if (hitPoints <= 0)
            {
                // Dying
                StartCoroutine(Dying());
            }
        }

        IEnumerator Dying()
        {
            GetComponent<Animator>().SetBool("Dying", true);

            isDead = true;

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
        }

        IEnumerator AttackDelay()
        {
            yield return new WaitForSeconds(0.4f);
        }
    }
}
