using UnityEngine;

public class Character : Entity
{
    [Header("Progression")]
    public int level = 1;
    public int currentXP = 0;
    public ClassSystem.Classes characterClass;

    [Header("Abilities")]
    public AbilityManager abilityManager;

    [Header("Damage Feedback")]
    [SerializeField] PlayerDamageFeedback damageFeedback;

    bool deathHandled;

    public override void Awake()
    {
        base.Awake();
    }

    public virtual void Start()
    {
        if (CompareTag("Player") && GameManager.Instance != null)
        {
            characterClass = GameManager.Instance.selectedClass;
        }

        ApplyClassToStats();

        if (CompareTag("Player") && EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.CacheBaseStats();
            EquipmentManager.Instance.RecalculateStats();
        }

        if (CompareTag("Player") && damageFeedback == null)
        {
            damageFeedback = FindFirstObjectByType<PlayerDamageFeedback>();
        }
    }

    protected override void OnDamaged(int damageAmount)
    {
        base.OnDamaged(damageAmount);

        if (!CompareTag("Player"))
            return;

        if (damageFeedback == null)
            return;

        if (stats == null || stats.maxHealth <= 0)
            return;

        float intensity = Mathf.Clamp01((float)damageAmount / stats.maxHealth);
        damageFeedback.PlayDamageFlash(intensity);
    }

    public void ApplyClassToStats()
    {
        if (ClassSystem.Instance == null)
        {
            return;
        }

        ClassStats cs = ClassSystem.Instance.GetStats(characterClass);

        stats.level = Mathf.Max(1, stats.level);
        level = stats.level;

        int lvl = stats.level;

        stats.maxHealth = cs.baseHealth + cs.healthPerLevel * (lvl - 1);
        stats.maxMana = cs.baseMana + cs.manaPerLevel * (lvl - 1);
        stats.maxStamina = cs.baseStamina + cs.staminaPerLevel * (lvl - 1);

        stats.strength = cs.baseStrength;
        stats.dexterity = cs.baseDexterity;
        stats.intelligence = cs.baseIntelligence;
        stats.charisma = cs.baseCharisma;

        stats.FillToMax();
        ApplyClassAbilities();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;
        TryLevelUp();
    }

    public int GetXPToNextLevel()
    {
        int currentLevel = Mathf.Max(1, stats.level);
        return 100 + ((currentLevel - 1) * 50);
    }

    void TryLevelUp()
    {
        bool leveledUp = false;

        while (currentXP >= GetXPToNextLevel())
        {
            currentXP -= GetXPToNextLevel();
            stats.level++;
            level = stats.level;
            leveledUp = true;
        }

        if (!leveledUp)
            return;

        ApplyClassToStats();

        if (CompareTag("Player") && EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.RecalculateStats();
        }
    }

    public void ApplyClassAbilities()
    {
        AbilityLoadout chosen = null;

        if (ClassSystem.Instance != null)
            chosen = ClassSystem.Instance.GetLoadout(characterClass);

        abilityManager.SetLoadout(chosen);
    }

    public override void Die()
    {
        if (deathHandled)
            return;

        deathHandled = true;

        if (stats != null)
            stats.health = 0;

        if (CompareTag("Player"))
        {
            if (GameOverManager.Instance != null)
                GameOverManager.Instance.TriggerGameOver();

            return;
        }

        gameObject.SetActive(false);
    }
}