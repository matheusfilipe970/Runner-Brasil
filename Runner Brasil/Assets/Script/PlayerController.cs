using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;
    public float sideSwipeForce = 5f;
    public float smoothness = 5f; // Fator de suavização

    private Rigidbody playerRigidbody;
    private int currentLane = 1; // 0 = Esquerda, 1 = Centro, 2 = Direita
    private bool isGrounded = false;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Movimentação automática para frente
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Verificar entrada de toque
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Verificar o tipo de toque
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Não faz nada no início do toque
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
                    // Pula se o toque for arrastado para cima, não houver movimento lateral significativo e estiver no chão
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

        // Limita o movimento a três faixas
        currentLane = Mathf.Clamp(currentLane, 0, 2);

        // Calcula a posição alvo suavizada
        Vector3 targetPosition = new Vector3((currentLane - 1) * sideSwipeForce, transform.position.y, transform.position.z);

        // Limita a posição alvo às faixas desejadas
        targetPosition.x = Mathf.Clamp(targetPosition.x, -sideSwipeForce, sideSwipeForce);

        // Aplica suavização apenas à coordenada x da posição alvo
        targetPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * smoothness);

        // Mantém as coordenadas y e z da posição atual
        targetPosition.y = transform.position.y;
        targetPosition.z = transform.position.z;

        // Atualiza a posição do jogador
        transform.position = targetPosition;
    }

    private void Jump()
    {
        // Adiciona uma força vertical para simular um pulo
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Define como false quando pula
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o jogador está no chão
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
