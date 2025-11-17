using UnityEngine;

public class Wallnut : MonoBehaviour
{
    private Plant plant;
    public Animator animator;

    [Header("Phase Thresholds")]
    public float phase2Threshold = 0.66f;
    public float phase3Threshold = 0.33f;

    private static readonly int Fase = Animator.StringToHash("Fase");
    private int currentPhase = 1;
    public int maxHealth = 55;

    private void Start()
    {
        plant = GetComponent<Plant>();
        if (plant != null)
        {
            plant.health = maxHealth;
        }
        SetPhase(1);
    }

    // Este método debe ser llamado por Plant.Hit(int damage)
    public void OnPlantHit()
    {
        UpdatePhase();
    }

    private void UpdatePhase()
    {
        if (plant == null) return;

        float healthPercent = (float)plant.health / maxHealth;
        int newPhase = 1;

        if (healthPercent <= phase3Threshold)
            newPhase = 3;
        else if (healthPercent <= phase2Threshold)
            newPhase = 2;

        if (newPhase != currentPhase)
        {
            SetPhase(newPhase);
        }
    }

    private void SetPhase(int phase)
    {
        currentPhase = phase;
        if (animator != null)
        {
            animator.SetInteger(Fase, phase);
        }
    }
}