using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject lightningPrefab;
    
    [Header("Spawn Point and Camera Transform")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float aimDistance = 100f; // Dystans do punktu celowania od kamery
    
    [Header("Cooldowns")]
    [SerializeField] private float fireballCooldown = 2.0f;
    [SerializeField] private float lightningCooldown = 1.0f;
    
    [Header("Crosshair")]
    [SerializeField] private CrosshairController crosshair;
    [SerializeField] private float crosshairScaleDuration = 0.1f;

    private bool castPrimaryAttack;
    private bool castSecondaryAttack;
    
    private float lastFireballCastTime = -Mathf.Infinity;
    private float lastLightningCastTime = -Mathf.Infinity;
    private void Update()
    {
        // Przykład: Rzuć zaklęcie po kliknięciu lewego przycisku myszy (sterowane przez InputManager)
        if (castPrimaryAttack)
        {
            CastFireball();
            castPrimaryAttack = false;
        }
        else if (castSecondaryAttack)
        {
            CastLightning();
            castSecondaryAttack = false;
        }
    }

    private void CastFireball()
    {
        if (!fireballPrefab || !spawnPoint || !playerCameraTransform)
        {
            Debug.LogError("Fireball Prefab, Spawn Point, or Player Camera Transform not set on " + gameObject.name);
            return;
        }

        var castDirection = GetTargetDirection();
        
        // Utwórz instancję prefabrykatu kuli ognia w miejscu spawnPoint
        var fireballGo = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);

        // Pobierz komponent Fireball ze stworzonego obiektu
        var fireballScript = fireballGo.GetComponent<Projectile>();

        if (fireballScript)
        {
            // Ustaw obliczony kierunek dla kuli ognia
            fireballScript.SetDirection(castDirection);
        }
        else
        {
            Debug.LogError("Prefab assigned to fireballPrefab does not have a FireballProjectile script.");
        }
        
        lastFireballCastTime = Time.time;
        crosshair.SetScale(CrosshairScale.Cast, 0.2f);
    }
    
    private void CastLightning()
    {
        if (!lightningPrefab || !spawnPoint || !playerCameraTransform)
        {
            Debug.LogError("Lightning Prefab, Spawn Point, or Player Camera Transform not set on " + gameObject.name);
            return;
        }
        
        var castDirection = GetTargetDirection();
        
        // Utwórz instancję prefabrykatu kuli ognia w miejscu spawnPoint
        var lightningGo = Instantiate(lightningPrefab, spawnPoint.position, Quaternion.identity);

        // Pobierz komponent Fireball ze stworzonego obiektu
        var lightningScript = lightningGo.GetComponent<Projectile>();

        if (lightningScript)
        {
            // Ustaw obliczony kierunek dla kuli ognia
            lightningScript.SetDirection(castDirection);
        }
        else
        {
            Debug.LogError("Prefab assigned to fireballPrefab does not have a FireballProjectile script.");
        }
        
        lastLightningCastTime = Time.time;
        crosshair.SetScale(CrosshairScale.Cast, crosshairScaleDuration);
    }

    private Vector3 GetTargetDirection()
    {
        // 1. Określ "punkt celowania" w świecie 3D.
        // Robimy to, rzutując Ray z kamery przez środek ekranu (lub po prostu używając kierunku kamery)
        // i biorąc punkt daleko w tym kierunku.
        Vector3 targetPoint;
        // Możesz też użyć Raycasta z kamery, żeby trafić w rzeczywisty obiekt pod celownikiem
        
        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        // Sprawdź warstwy, w które może trafiać Raycast celowania, np. te same co pocisk + Default
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Entity", "Environment", "Ground", "Default"))) // Użyj odpowiednich warstw
        {
            // Jeśli raycast trafił, punktem celowania jest miejsce trafienia
            targetPoint = hitInfo.point;
        }
        else
        {
            // Najprostsza metoda to wzięcie punktu daleko od kamery w jej kierunku 'forward':
            targetPoint = playerCameraTransform.position + playerCameraTransform.forward * aimDistance;
        }
        // Jeśli raycast nie trafił, używamy punktu daleko w przód kamery (jak wyżej)
        // Logika powyżej już to obsługuje jako domyślny targetPoint.
        
        
        // 2. Oblicz kierunek od spawnPoint do targetPoint
        return (targetPoint - spawnPoint.position).normalized;
    }

    // Metoda wywoływana przez InputManager
    public void OnPrimarySpellCast()
    {
        if (Time.time < lastFireballCastTime + fireballCooldown)
        {
            // Cooldown jeszcze trwa, nie można użyć fireball.
            Debug.Log("Fireball is on cooldown.");
            return;
        }
        castPrimaryAttack = true;
    }

    public void OnSecondarySpellCast()
    {
        if (Time.time < lastLightningCastTime + lightningCooldown)
        {
            // Cooldown jeszcze trwa, nie można użyć lightning.
            Debug.Log("Lightning is on cooldown.");
            return;
        }
        castSecondaryAttack = true;
    }
}