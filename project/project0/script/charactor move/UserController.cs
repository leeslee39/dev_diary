using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public enum charactor_state
{
    speed,
    concentrate
}

namespace ClearSky
{
    public class UserController : MonoBehaviour
    {
        public float movePower = 3f;
        public float movePower_c = 1f;
        public float jumpPower = 4f; //Set Gravity Scale in Rigidbody2D Component to 5

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        public int direction = 1;
        [SerializeField]
        bool isJumping = false;
        private bool alive = true;
        public int  atkDmg = 1;
        [SerializeField]
        float attackcooltime;
        [SerializeField]
        float cooltime = 0;
        bool attackmode;
        public int health = 100;
        float last_h;
        float airtime;
        [SerializeField]
        bool Debugmod;
        public int damage;
        bool nextattack;
        bool iswarp = true;
        [SerializeField]
        charactor_state cs = charactor_state.speed;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (alive && !Debugmod)
            {
                if(cs == charactor_state.speed)
                {
                    Attack();
                    Jump();
                    Run();
                    Onsky();
                }
                else if(cs == charactor_state.concentrate)
                {
                    c_Attack();
                    c_Jump();
                    c_Run();
                    c_Onsky();
                }
                changemode();
            }
        }

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;

            if (!isJumping)
            {
                airtime = 1;
                last_h = Input.GetAxisRaw("Horizontal");
                if (last_h < 0)
                {
                    direction = -1;
                    moveVelocity = new Vector2(last_h, 0);

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);

                }
                if (last_h > 0)
                {
                    direction = 1;
                    moveVelocity = new Vector2(last_h, 0);

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);


                }
                if (last_h == 0)
                {
                    direction = 0;
                    syncani("charactor_runattack1", 8, "charactor_attack1", 6, "charactor_run");
                    syncani("charactor_runattack2", 8, "charactor_attack2", 6, "charactor_run");
                    anim.SetBool("isrun", false);

                }
                transform.position += moveVelocity * movePower * Time.deltaTime;
            }
            else
            {
                if (last_h < 0)
                {
                    direction = -1;
                    moveVelocity = Vector3.left;
                    
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);

                }
                if (last_h > 0)
                {
                    direction = 1;
                    moveVelocity = Vector3.right;
                    
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);


                }
                if (last_h == 0)
                {
                    direction = 0;
                    syncani("charactor_runattack1", 8, "charactor_attack1", 6, "charactor_run");
                    syncani("charactor_runattack2", 8, "charactor_attack2", 6, "charactor_run");
                    anim.SetBool("isrun", false);

                }
                if (airtime >= 0)
                    airtime -= Time.deltaTime / 10;
                else
                    airtime = 0;
                if(Mathf.Abs(last_h) < 1)
                {
                    last_h += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 1.8f;
                }
                else if(last_h * Input.GetAxisRaw("Horizontal") < 0)
                {
                    last_h += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 1.8f;
                }
                transform.position += new Vector3(last_h, 0, 0) * airtime * movePower * Time.deltaTime;
            }
            
            
            
        }
        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            && !anim.GetBool("jumpup") && !isJumping)
            {
                isJumping = true;
                anim.SetBool("jumpup", true);
                rb.velocity = Vector2.zero;
                syncani("charactor_attack1", 6, "charactor_jumpattack1", 6);
                syncani("charactor_attack2", 6, "charactor_jumpattack2", 6);
                syncani("charactor_runattack1", 6, "charactor_jumpattack1", 6);
                syncani("charactor_runattack2", 6, "charactor_jumpattack2", 6);

                Vector2 jumpVelocity = new Vector2(0, jumpPower);
                rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
            }
            if(isJumping && rb.velocity.y < 1f)
            {
                anim.SetBool("falldown", true);
                anim.SetBool("jumpup", false);
                syncani("charactor_jumpattack1", 6, "charactor_fallattack1", 6);
                syncani("charactor_jumpattack2", 6, "charactor_fallattack2", 6);
            }
        }
        void Attack()
        {
            
            if (Input.GetButtonDown("Fire1") && cooltime <= 0 && !nextattack) // 근접공격
            {
                if (!isJumping)
                {
                    anim.SetTrigger("attack1");
                    attackmode = true;
                    cooltime = attackcooltime;
                }
                else
                {
                    if (anim.GetBool("falldown"))
                    {
                        anim.Play("charactor_fallattack1", 0, 0);
                        attackmode = true;
                        cooltime = attackcooltime;
                    }
                    else
                    {
                        anim.Play("charactor_jumpattack1", 0, 0);
                        attackmode = true;
                        cooltime = attackcooltime;
                    }
                }
            }
            else if (Input.GetButtonDown("Fire1") && cooltime >= 0 && cooltime <= 0.2f && attackmode)
            {
                nextattack = true;
                attackmode = false;
            }
            if (nextattack && cooltime <= 0)
            {
                if (!isJumping)
                {
                    nextattack = false;
                    cooltime = attackcooltime;
                    anim.SetTrigger("attack2");
                }
                else
                {
                    if (anim.GetBool("falldown"))
                    {
                        nextattack = false;
                        anim.Play("charactor_fallattack2", 0, 0);
                        cooltime = attackcooltime;
                    }
                    else
                    {
                        nextattack = false;
                        anim.Play("charactor_jumpattack2", 0, 0);
                        cooltime = attackcooltime;
                    }
                }
            }
            

            if (cooltime > 0)
                cooltime -= Time.deltaTime;

            

        }
        void Onsky()
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, LayerMask.GetMask("ground"));
            if ((ray.collider != null||Mathf.Abs(rb.velocity.y) < 0.01f) && isJumping)
            {
                isJumping = false;
                anim.SetBool("falldown", false);
            }
            else if(ray.collider == null && (Mathf.Abs(rb.velocity.y) > 0.1f))
            {
                isJumping = true;
                anim.SetBool("falldown", true);
            }
            else if (ray.collider != null || (Mathf.Abs(rb.velocity.y) < 0.01f))
            {
                if (isJumping)
                {
                    isJumping = false;
                    anim.SetBool("falldown", false);
                }
                if(direction != 0)
                {
                    syncani("charactor_fallattack1", 6, "charactor_runattack1", 6);
                    syncani("charactor_fallattack2", 6, "charactor_runattack2", 6);
                }
                else
                {
                    syncani("charactor_fallattack1", 6, "charactor_attack1", 6);
                    syncani("charactor_fallattack2", 6, "charactor_attack2", 6);
                }
            }
        }
        void changemode()
        {
            if (Input.GetButtonDown("modeshift") && (anim.GetCurrentAnimatorStateInfo(0).IsName("charctor_idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("charactor_idle_const") || anim.GetCurrentAnimatorStateInfo(0).IsName("charactor_run_const") || anim.GetCurrentAnimatorStateInfo(0).IsName("charactor_run")))
            {
                switch (cs)
                {
                    case charactor_state.concentrate:
                        cs = charactor_state.speed;
                        anim.SetInteger("mode", 0);
                        syncani("charactor_idle_const", 8, "charctor_idle", 8);
                        syncani("charactor_run_const", 4, "charactor_run", 8);
                        break;
                    case charactor_state.speed:
                        anim.SetInteger("mode", 1);
                        cs = charactor_state.concentrate;
                        syncani("charctor_idle", 8, "charactor_idle_const", 8);
                        syncani("charactor_run", 8, "charactor_run_const", 4);
                        break;
                }
            }
        }
        bool canattack(string ani_name, int frame, int min, int max)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(ani_name))
            {
                float time = stateInfo.normalizedTime % 1;
                int currentFrame = (int)(time * frame);
                if(currentFrame >= min && currentFrame <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
        void c_Run()
        {
            Vector3 moveVelocity = Vector3.zero;

            if (!isJumping)
            {
                airtime = 1;
                last_h = Input.GetAxisRaw("Horizontal");
                if (last_h < 0)
                {
                    direction = -1;
                    moveVelocity = new Vector2(last_h, 0);

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);

                }
                if (last_h > 0)
                {
                    direction = 1;
                    moveVelocity = new Vector2(last_h, 0);

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);


                }
                if (last_h == 0)
                {
                    direction = 0;
                    syncani("charactor_runattack1", 8, "charactor_attack1", 6);
                    syncani("charactor_runattack2", 8, "charactor_attack2", 6);
                    anim.SetBool("isrun", false);

                }
                transform.position += moveVelocity * movePower_c * Time.deltaTime;
            }
            else
            {
                if (last_h < 0)
                {
                    direction = -1;
                    moveVelocity = Vector3.left;

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);

                }
                if (last_h > 0)
                {
                    direction = 1;
                    moveVelocity = Vector3.right;

                    transform.localScale = new Vector3(direction * 1, 1, 1);
                    if (!anim.GetBool("jumpup"))
                    {
                        anim.SetBool("isrun", true);
                    }
                    syncani("charactor_attack1", 6, "charactor_runattack1", 8);
                    syncani("charactor_attack2", 6, "charactor_runattack2", 8);


                }
                if (last_h == 0)
                {
                    direction = 0;
                    syncani("charactor_runattack1", 8, "charactor_attack1", 6);
                    syncani("charactor_runattack2", 8, "charactor_attack2", 6);
                    anim.SetBool("isrun", false);

                }
                if (airtime >= 0)
                    airtime -= Time.deltaTime / 10;
                else
                    airtime = 0;
                if (moveVelocity.x * Input.GetAxisRaw("Horizontal") <= 0)
                    transform.position += new Vector3(last_h + Input.GetAxisRaw("Horizontal") * 1.3f, 0, 0) * airtime * movePower_c * Time.deltaTime;
                else
                    transform.position += new Vector3(last_h, 0, 0) * movePower_c * Time.deltaTime * airtime;
            }



        }
        void c_Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            && !anim.GetBool("jumpup") && !isJumping)
            {
                isJumping = true;
                anim.SetBool("jumpup", true);
                rb.velocity = Vector2.zero;
                syncani("charactor_attack1", 6, "charactor_jumpattack1", 6);
                syncani("charactor_attack2", 6, "charactor_jumpattack2", 6);
                syncani("charactor_runattack1", 6, "charactor_jumpattack1", 6);
                syncani("charactor_runattack2", 6, "charactor_jumpattack2", 6);

                Vector2 jumpVelocity = new Vector2(0, jumpPower);
                rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
            }
            if (isJumping && rb.velocity.y < 1f)
            {
                anim.SetBool("falldown", true);
                anim.SetBool("jumpup", false);
                syncani("charactor_jumpattack1", 6, "charactor_fallattack1", 6);
                syncani("charactor_jumpattack2", 6, "charactor_fallattack2", 6);
            }
        }
        void c_Attack()
        {

            if (Input.GetButtonDown("Fire1") && cooltime <= 0 && !nextattack) // 근접공격
            {
                if (!isJumping)
                {
                    anim.SetTrigger("attack1");
                    attackmode = true;
                    cooltime = attackcooltime;
                }
                else
                {
                    if (anim.GetBool("falldown"))
                    {
                        anim.Play("charactor_fallattack1", 0, 0);
                        attackmode = true;
                        cooltime = attackcooltime;
                    }
                    else
                    {
                        anim.Play("charactor_jumpattack1", 0, 0);
                        attackmode = true;
                        cooltime = attackcooltime;
                    }
                }
                
            }
            else if (Input.GetButtonDown("Fire1") && cooltime >= 0 && cooltime <= 0.2f && attackmode)
            {
                nextattack = true;
                attackmode = false;
            }
            if (Input.GetKeyDown("e") && iswarp)
            {
                iswarp = false;
                anim.Play("New Animation 0");
            }
            if (nextattack && cooltime <= 0)
            {
                if (!isJumping)
                {
                    nextattack = false;
                    cooltime = attackcooltime;
                    anim.SetTrigger("attack2");
                }
                else
                {
                    if (anim.GetBool("falldown"))
                    {
                        nextattack = false;
                        anim.Play("charactor_fallattack2", 0, 0);
                        cooltime = attackcooltime;
                    }
                    else
                    {
                        nextattack = false;
                        anim.Play("charactor_jumpattack2", 0, 0);
                        cooltime = attackcooltime;
                    }
                }
            }


            if (cooltime > 0)
                cooltime -= Time.deltaTime;

            if(canattack("New Animation 0", 6, 4, 4) && !iswarp)
            {
                anim.Play("New Animation2");
                iswarp = true;
                RaycastHit2D ray = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 2f, LayerMask.GetMask("ground"));
                if (ray.collider != null)
                {
                    ray.collider.gameObject.GetComponent<Rigidbody2D>().AddForce((Vector2)(ray.collider.gameObject.transform.position - transform.position).normalized * 100 + new Vector2(0, 100));
                    isJumping = false;
                    anim.SetBool("falldown", false);
                }
                transform.Translate(new Vector2(2 * transform.localScale.x,0));
            }
            else
            {

            }

        }
        void c_Onsky()
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, LayerMask.GetMask("ground"));
            if ((ray.collider != null || Mathf.Abs(rb.velocity.y) < 0.01f) && isJumping)
            {
                isJumping = false;
                anim.SetBool("falldown", false);
            }
            else if (ray.collider == null && (Mathf.Abs(rb.velocity.y) > 0.1f))
            {
                isJumping = true;
                anim.SetBool("falldown", true);
            }
            else if (ray.collider != null || (Mathf.Abs(rb.velocity.y) < 0.01f))
            {
                if (isJumping)
                {
                    isJumping = false;
                    anim.SetBool("falldown", false);
                }
                if (direction != 0)
                {
                    syncani("charactor_fallattack1", 6, "charactor_runattack1", 6);
                    syncani("charactor_fallattack2", 6, "charactor_runattack2", 6);
                }
                else
                {
                    syncani("charactor_fallattack1", 6, "charactor_attack1", 6);
                    syncani("charactor_fallattack2", 6, "charactor_attack2", 6);
                }
            }
        }

        void syncani(string before, int frame_b,  string after, int frame_a , string over = "norm")
        {
            if(over == "norm")
            {
                over = after;
            }
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(before))
            {
                float time = stateInfo.normalizedTime % 1;
                int currentFrame = (int)(time * frame_b);
                if (currentFrame < frame_a)
                    anim.Play(after, 0, (float)currentFrame / frame_a);
                else
                    anim.Play(over, 0, 0);
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            //attack Effect
            if (canattack("charactor_attack1", 6, 4, 5) ||
                canattack("charactor_attack2", 6, 4, 5) ||
                canattack("charactor_jumpattack1", 6, 4, 5) ||
                canattack("charactor_jumpattack2", 6, 4, 5) ||
                canattack("charactor_fallattack1", 6, 4, 5) ||
                canattack("charactor_fallattack2", 6, 4, 5) ||
                canattack("charactor_runattack1", 8, 4, 5) ||
                canattack("charactor_runattack2", 8, 4, 5))
            {
                if(collision.gameObject.TryGetComponent(out EnemyInfo info))
                {
                    info.HP -= damage;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce((Vector2)(collision.gameObject.transform.position - transform.position).normalized*10 + new Vector2(0,10));
            }
        }

    }
    

}