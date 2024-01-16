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
    private bool isGrounded = true;

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
                    // Pula somente se o toque for arrastado para cima e estiver no ch�o
                    if (touch.deltaPosition.y > 0 && Mathf.Abs(touch.deltaPosition.x / Screen.width) <= 0.1f && isGrounded)
                    {
                        Jump();
                    }
                    break;
            }
        }

        // Debug para verificar o estado do isGrounded
        Debug.Log("isGrounded: " + isGrounded);
    }

    private void FixedUpdate()
    {
        // Verifica se o jogador est� no ch�o
        isGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        // Ajuste o raio do Raycast conforme necess�rio
        float raycastLength = 0.1f;

        // Cria um raio que vai para baixo a partir da posi��o do jogador
        Ray ray = new Ray(transform.position, Vector3.down);

        // Executa um Raycast e retorna verdadeiro se atingir algo
        return Physics.Raycast(ray, raycastLength);
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

        // Atualiza a posi��o do jogador
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);

        // Limita a posi��o do jogador �s faixas desejadas
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -sideSwipeForce, sideSwipeForce), transform.position.y, transform.position.z);
    }

    private void Jump()
    {
        // Adiciona uma for�a vertical para simular um pulo
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
