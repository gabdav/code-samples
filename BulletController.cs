using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BulletController : MonoBehaviour
{
    private ObjectPool objPool;
    private Vector3 startPoint;
    private int ammountAlreadySpawned;
    private float chargeUpTimer = 0;
    private Color color;
    private bool chargeSoundisOn =false;
    private bool chargingSoundisOn=false;
    private List<GameObject> spawnedProjectiles = new List<GameObject>();
    private float deathBoxTimer=0.5f;
    private bool miniBoom = false;

    [Header("Projectile Settings")]
    public int numberOfProjectiles;
    public float projectileSpeed;
    public GameObject ProjectilePrefab;
    public float radius = 1f;
    public Transform parentSpawner;
    public GameObject particleExplosion;
    public GameObject miniParticleExplosion;
    public GameObject chargeUp;
    public int ammountOfCharge = 0;
    public bool release = false;
    public float maxCharge = 100;
    public bool timeToBlow = false;
    public GameObject DeathBoxMB;
    public CamerShake cameraShake;
    public int charges;

    public TextMeshProUGUI textCounter;
     void Start()
    {
        objPool = ObjectPool.Instance;
        
    }

    void FixedUpdate() {
        MoveProjectiles(spawnedProjectiles);
    }

    void Update()
    {
        textCounter.text = charges.ToString();
        if (Time.timeScale != 0 && charges>0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                release = false;
                if (ammountAlreadySpawned <= ammountOfCharge)
                {
                    color = color * Time.deltaTime;
                    GameObject vfxCharge = Instantiate(chargeUp, transform.position, Quaternion.identity);
                    vfxCharge.transform.SetParent(parentSpawner);
                    Destroy(vfxCharge, vfxCharge.GetComponent<ParticleSystem>().main.duration);
                    ammountAlreadySpawned++;
                    if(!chargingSoundisOn){
                    FindObjectOfType<AudioManager>().Play("Charging");  
                    chargingSoundisOn = true;
                    }                  
                }
                chargeUpTimer += Time.deltaTime;
                if(chargeUpTimer>= maxCharge && !chargeSoundisOn){

                    FindObjectOfType<AudioManager>().Play("ChargedUp");
                    chargeSoundisOn =true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                ammountAlreadySpawned = 0;
                release = true;
                 FindObjectOfType<AudioManager>().Stop("ChargedUp");
                 FindObjectOfType<AudioManager>().Stop("Charging");                   
                 chargeSoundisOn = false;
                 chargingSoundisOn = false;
                if (chargeUpTimer >= maxCharge)
                {
                    timeToBlow = true;
                    FindObjectOfType<AudioManager>().Play("Boom");
                    StartCoroutine(cameraShake.Shake(.15f,.1f));
                    startPoint = transform.position;
                    spawnedProjectiles = SpawnProjectile(numberOfProjectiles);
                    GameObject vfx = Instantiate(particleExplosion, transform.position, Quaternion.identity);
                    vfx.transform.SetParent(parentSpawner);
                    Destroy(vfx, vfx.GetComponent<ParticleSystem>().main.duration);
                    charges--;
                }
                else
                {
                    timeToBlow = false;
                }
                chargeUpTimer = 0;
            }
            else
            {
                timeToBlow = false;

            }
            if(Input.GetKeyDown(KeyCode.E) && !Input.GetKey(KeyCode.Space)){
                miniBoom = true;
                deathBoxTimer = 0.5f;
                DeathBoxMB.SetActive(true);
                startPoint = transform.position;
                FindObjectOfType<AudioManager>().Play("MiniBoom");
                spawnedProjectiles = SpawnProjectile(numberOfProjectiles);
                GameObject vfx = Instantiate(miniParticleExplosion, transform.position, Quaternion.identity);
                vfx.GetComponent<ParticleSystem>().main.duration.Equals(vfx.GetComponent<ParticleSystem>().main.duration/2);
                vfx.transform.SetParent(parentSpawner);
                 Destroy(vfx, vfx.GetComponent<ParticleSystem>().main.duration);
                charges--;
            }
                
        }
         if(miniBoom && DeathBoxMB.activeSelf){
                    deathBoxTimer-= Time.deltaTime;
                    if(deathBoxTimer <= 0){
                    DeathBoxMB.SetActive(false);
                    miniBoom = false;
             }
         }
    }



    private List<GameObject> SpawnProjectile(int _numberOfProjectiles)
    {   
        List<GameObject> projectiles = new List<GameObject>();
        for(int i = 0; i<= _numberOfProjectiles - 1; i++)
        {
            GameObject tmpObj = objPool.SpaqnFromPool("Projectile", startPoint, Quaternion.identity);
            tmpObj.transform.SetParent(parentSpawner);
            tmpObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projectiles.Add(tmpObj);
        }
        return projectiles;
    }

    private void MoveProjectiles(List<GameObject> projectiles){
        float angleStep = 360f / projectiles.Count;
         float angle = 0f;
         foreach(GameObject tmpObj in projectiles){
            float projectileDirXPos = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
            float projectileDirYPos = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;
            Vector3 projectileVector = new Vector3(projectileDirXPos, projectileDirYPos, 0);
            Vector3 projectileMoveDir = (((projectileVector - startPoint).normalized * projectileSpeed) * Time.fixedDeltaTime);
            tmpObj.GetComponent<Rigidbody>().velocity = new Vector3(projectileMoveDir.x, 0, projectileMoveDir.y);
            angle += angleStep;
         }

    }
}
