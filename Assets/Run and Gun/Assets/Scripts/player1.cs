using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : MonoBehaviour
{
    private Animator animator;
    public float speed = 5f; 
    public float jumpForce = 600; 
    public AudioManager audioManager;
    public AudioClip sonidoSalto;
    private Rigidbody2D rb2d; 
    private bool facingRight = true;
    private bool jump;
    private bool onGround = false;
    private bool canJump = true; 
    private Transform groundCheck;
    private float hForce = 0;
    private bool isDead = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        groundCheck = gameObject.transform.Find("GroundCheck");
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (!isDead)
        {
            // Verificar si el personaje está en el suelo
            onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
            
            // Solo permitir saltar cuando esté en el suelo
            if (onGround)
            {
                canJump = true;  // Permitimos que salte nuevamente
            }

            // Detectar la flecha hacia arriba para saltar
            if (Input.GetKeyDown(KeyCode.UpArrow) && canJump && onGround)
            {
                jump = true;
                // Asegurarse de que AudioManager.Instance esté inicializado
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirSonido(sonidoSalto);
                }
                canJump = false;  // Bloquear el salto hasta que vuelva a tocar el suelo
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (rb2d.velocity.y > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                }
            }
        }
    }

    private void FixedUpdate() // Corrección: Añadir llaves de apertura
    {
        if (!isDead)
        {
            // Detectar las flechas izquierda y derecha en lugar de usar el eje "Horizontal"
            if (Input.GetKey(KeyCode.RightArrow))
            {
                hForce = 1;  // Mover a la derecha
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                hForce = -1;  // Mover a la izquierda
            }
            else
            {
                hForce = 0;  // No moverse si no se presiona ninguna flecha
            }

            rb2d.velocity = new Vector2(hForce * speed, rb2d.velocity.y);

            // Activar la animación de caminar si hay movimiento lateral
            animator.SetBool("isRunning", hForce != 0);

            // Voltear el personaje según la dirección del movimiento
            if (hForce > 0 && !facingRight)
            {
                Flip();
            }
            else if (hForce < 0 && facingRight)
            {
                Flip();
            }

            // Manejar el salto
            if (jump)
            {
                jump = false;
                rb2d.AddForce(Vector2.up * jumpForce);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
