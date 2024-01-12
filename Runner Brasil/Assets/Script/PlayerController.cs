using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;
    public float sideSwipeForce = 5f;
    public float smoothness = 5f; // Fator de suaviza��o

    private Rigidbody playerRigidbody;
    private int currentLane = 1; // 0 = Esquerda, 1 = Centro, 2 = Direita
    private bool isGrounded = false;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Movimenta��o autom�tica para frente
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Verificar entrada de toque
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Verificar o tipo de toque
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // N�o faz nada no in�cio do toque
                    break;

                case TouchPhase.Moved:
                    // Verifica o deslocamento do toque para decidir entre movimento lateral ou pulo
                    float swipeDeltaX = touch.deltaPosition.x / Screen.width;

                    if (Mathf.Abs(swipeDeltaX) > 0.1f)
                    {
                        MoveSideways(swipeDeltaX);
                    }
                    break;

                case TouchPhase.Ended:
                    // Pula se o toque for arrastado para cima, n�o houver movimento lateral significativo e estiver no ch�o
                    if (touch.deltaPosition.y > 0 && Mathf.Abs(touch.deltaPosition.x / Screen.width) <= 0.1f && isGrounded)
                    {
                        Jump();
                    }
                    break;
            }
        }
    }

    private void MoveSideways(float swipeDeltaX)
    {
        // Atualiza a faixa atual com base no deslocamento do toque
        if (swipeDeltaX > 0 && currentLane < 2)
        {
            currentLane++;
        }
        else if (swipeDeltaX < 0 && currentLane > 0)
        {
            currentLane--;
        }

        // Limita o movimento a tr�s faixas
        currentLane = Mathf.Clamp(currentLane, 0, 2);

        // Calcula a posi��o alvo suavizada
        Vector3 targetPosition = new Vector3((currentLane - 1) * sideSwipeForce, transform.position.y, transform.position.z);

        // Limita a posi��o alvo �s faixas desejadas
        targetPosition.x = Mathf.Clamp(targetPosition.x, -sideSwipeForce, sideSwipeForce);

        // Aplica suaviza��o apenas � coordenada x da posi��o alvo
        targetPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * smoothness);

        // Mant�m as coordenadas y e z da posi��o atual
        targetPosition.y = transform.position.y;
        targetPosition.z = transform.position.z;

        // Atualiza a posi��o do jogador
        transform.position = targetPosition;
    }

    private void Jump()
    {
        // Adiciona uma for�a vertical para simular um pulo
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Define como false quando pula
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o jogador est� no ch�o
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
