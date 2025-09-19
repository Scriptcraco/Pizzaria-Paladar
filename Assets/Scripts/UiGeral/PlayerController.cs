using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    private InputSystem_Actions inputActions;

    [Header("Parâmetros de Movimento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravityScale = 1.0f;

    public int dinheiro = 0;
    public float estoqueMaximoBebida = 10f;
    public float estoqueAtualBebida = 0f;
    public float velocidadeGeladeira = 1f;
    public float velocidadedeprodução = 1f;
    public float estoqueMaximo = 10f;
    public float estoqueAtual = 0f;

    public float estoqueAtualEmpacotado = 0f;
    public float estoqueMaximoEmpacotado = 10f;



   

    [Header("Estados")]
    private bool isGrounded = false;
    public bool gameOver = false;
    private Vector2 moveInput;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Efeitos e Sons")]
    public AudioClip jumpSound;
    public AudioClip deathSound;
    public ParticleSystem movementParticles;
    public ParticleSystem deathParticles;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Jump.performed += ctx => OnJumpInput();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
public void SalvarDados()
{
    PlayerPrefs.SetInt("dinheiro", dinheiro);
    PlayerPrefs.SetFloat("estoqueBebida", estoqueAtualBebida);
    PlayerPrefs.SetFloat("estoqueProduto", estoqueAtual);
    PlayerPrefs.SetFloat("estoqueEmpacotado", estoqueAtualEmpacotado);
    PlayerPrefs.Save();
    Debug.Log("💾 Dados salvos!");
}

public void CarregarDados()
{
    dinheiro = PlayerPrefs.GetInt("dinheiro", 0);
    estoqueAtualBebida = PlayerPrefs.GetFloat("estoqueBebida", 0);
    estoqueAtual = PlayerPrefs.GetFloat("estoqueProduto", 0);
    estoqueAtualEmpacotado = PlayerPrefs.GetFloat("estoqueEmpacotado", 0);
    Debug.Log("📂 Dados carregados!");
}

private void OnApplicationQuit()
{
    SalvarDados();
}

private void Start()
{
    rb = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();

    if (groundCheck == null)
        Debug.LogWarning("⚠️ 'groundCheck' não está atribuído no Inspector!");

    Physics.gravity *= gravityScale;

    CarregarDados();
}

    private void Update()
    {
        if (gameOver) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        if (gameOver) return;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        float speedMultiplier = isRunning ? 2f : 1f;

        if (moveDirection.sqrMagnitude > 0.01f && isGrounded)
        {
            moveDirection.Normalize();

            Vector3 currentVelocity = rb.linearVelocity;

            Vector3 newVelocity = new Vector3(
                moveDirection.x * moveSpeed * speedMultiplier,
                currentVelocity.y,
                moveDirection.z * moveSpeed * speedMultiplier
            );

            rb.linearVelocity = newVelocity;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            animator.SetFloat("Speed_f", isRunning ? 1f : 0.5f);

            // Partículas somente ao correr (null-safe)
            if (movementParticles != null)
            {
                if (isRunning)
                {
                    if (!movementParticles.isPlaying)
                        movementParticles.Play();
                }
                else
                {
                    if (movementParticles.isPlaying)
                        movementParticles.Stop();
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            animator.SetFloat("Speed_f", 0f);

            if (movementParticles != null && movementParticles.isPlaying)
                movementParticles.Stop();
        }
    }

    private void OnJumpInput()
    {
        if (gameOver || !isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (movementParticles.isPlaying)
            movementParticles.Stop();

        audioSource.PlayOneShot(jumpSound);
        animator.SetTrigger("Jump_trig");
    }



    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    public int GetEstoqueAtualBebida() => (int)estoqueAtualBebida;
    public int GetEstoqueMaximoBebida() => (int)estoqueMaximoBebida;
    public int GetEstoqueAtual() => (int)estoqueAtual;
    public int GetEstoqueMaximo() => (int)estoqueMaximo;

    public int GetEstoqueAtualEmpacotado() => (int)estoqueAtualEmpacotado;
    public int GetEstoqueMaximoEmpacotado() => (int)estoqueMaximoEmpacotado;
       

    public void AdicionarBebida(int quantidade)
    {
        estoqueAtualBebida = Mathf.Min(estoqueAtualBebida + quantidade, estoqueMaximoBebida);
        Debug.Log($"Player: bebida adicionada. Estoque atual = {estoqueAtualBebida}");
    }

    public void AdicionarProduto(int quantidade)
    {
        estoqueAtual = Mathf.Min(estoqueAtual + quantidade, estoqueMaximo);
        Debug.Log($"Player: produto adicionado. Estoque atual = {estoqueAtual}");
    }

    public void AdicionarItem(int quantidade)
    {
        estoqueAtualEmpacotado = Mathf.Min(estoqueAtualEmpacotado + quantidade, estoqueMaximoEmpacotado);
        Debug.Log($"Player: item adicionado. Estoque atual = {estoqueAtualEmpacotado}");
    }

    public void RemoverProduto(int quantidade)
    {
        estoqueAtualEmpacotado = Mathf.Max(0, estoqueAtualEmpacotado - quantidade);
        Debug.Log($"Player: produto removido. Estoque atual = {estoqueAtualEmpacotado}");
    }

    public void RemoverBebida(int quantidade)
    {
        estoqueAtualBebida = Mathf.Max(0, estoqueAtualBebida - quantidade);
        Debug.Log($"Player: bebida removida. Estoque atual = {estoqueAtualBebida}");
    }
 public int GetDinheiro() => dinheiro;

    public void RemoverItem(int quantidade)
    {
        estoqueAtual = Mathf.Max(0, estoqueAtual - quantidade);
        Debug.Log($"Player: item removido. Estoque atual = {estoqueAtual}");
    }
    public void AdicionarDinheiro(int valor)
    {
        dinheiro += valor;
        Debug.Log($"💰 Dinheiro recebido: {valor}. Total atual: {dinheiro}");
    }
    public bool GastarDinheiro(int valor)
    {
        if (dinheiro < valor)
        {
            Debug.Log("❌ Dinheiro insuficiente!");
            return false;
        }

        dinheiro -= valor;
        Debug.Log($"💸 Dinheiro gasto: {valor}. Restante: {dinheiro}");
        return true;
    }


}

