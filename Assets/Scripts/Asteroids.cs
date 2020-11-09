using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{

    [Header("Основные настройки")]
        [Tooltip("Максимальное количество жизней игрока")]
        [Range(1,10)]
        public int MaxLifes = 5;
        [Tooltip("Продолжительность неуязвимости")]
        [Range(1,10)]
        public float GodModeTimer = 3;
        [Tooltip("Скорость поворота корабля (градусы в секунду)")]
        [Range(30,360)]
        public float MaxRotationSpeed = 90;
        [Tooltip("Максимальная скорость корабля")]
        [Range(50,3000)]
        public float MaxSpeed = 800;
        [Tooltip("Ускорение корабля (метры в секунду")]
        [Range(10,1000)]
        public float Acceleration = 100;
        [Tooltip("Минимальное время до появлениця UFO (секунды)")]
        [Range(8,30)]
        public float ufoMinTimeout= 20;
        [Tooltip("Максимальный время до появления UFO (секунды)")]
        [Range(10,100)]
        public float ufoMaxTimeout = 40;
        [Tooltip("Будут ли снаряды переноситься на противоположную сторону экрана")]
        public bool BulletTeleport = true;
        [Tooltip("Будет ли UFO переноситься на противоположную сторону экрана")]
        public bool UFOTeleport = true;
        [Tooltip("Размер большого астероида")]
        [Range(10,300)]
        public float LargeAsteroidSize = 100;
        [Tooltip("Размер среднего астероида")]
        [Range(10,300)]
        public float MediumAsteroidSize = 70;
        [Tooltip("Размер малого астероида")]
        [Range(10,300)]
        public float SmallAsteroidSize = 50;
        [Tooltip("Максимальная скорость астероида")]
        [Range(50,3000)]
        public float AsteroidMaxSpeed = 300;
        [Tooltip("Минимальная скорость астероида")]
        [Range(0,1000)]
        public float AsteroidMinSpeed = 10;

    [Header("Стрельба")]
        [Tooltip("Стрелять только по клику (как в указано в ТЗ)")]
        public bool ShotOnlyOnClick = true;
        [Tooltip("Скорость снарядов")]
        [Range(300,2000)]
        public float BulletSpeed =500;
        [Tooltip("Скорость стрельбы игрока (выстрелы в секунду)")]
        [Range(1,6)]
        public float PlayerShotSpeed = 3;
        [Tooltip("Минимальный промежуток стрельбы НЛО (секунды)")]
        [Range(1,3)]
        public float ufoMinShotTimeout= 2;
        [Tooltip("Максимальный промежуток стрельбы НЛО (секунды)")]
        [Range(4,10)]
        public float ufoMaxShotTimeout = 5;

    [Header("Пул объектов")]
        [Tooltip("Пул астероидов")]
        [Range(1,100)]
        public int AsteroidsCount = 30;
        [Tooltip("Пул боеприпасов")]
        [Range(1,100)]
        public int AmmoCount = 5;
        [Tooltip("Пул взрывов")]
        [Range(1,100)]
        public int ExplosionsCount = 5;

    [Header("НЕ МЕНЯТЬ")]
        public Text ScoreText;               // Текст для отображения очков.
        public GameObject LifeIndicator;     // Prefab для отображиния жизней
        public GameObject PlayerSpaceShip;   // Корабль.
        public GameObject Bullet;            // Снаряд.
        public GameObject UFO;
        public GameObject ExplosionObj;
        public GameObject AsteroidObj;
        public AudioClip LExplosion;
        public AudioClip MExplosion;
        public AudioClip SExplosion;
        
    [HideInInspector]
        public static int score = 0;
        public static int stage = 1;              //стадия игры
        public static bool ControllerType = true; //тип контроллера true - клавиатура+мышь
        List<GameObject> LifesIndicators;         //диндикаторы жизней
        List<GameObject> Ammo, Explosions, Astds; //пулы объектов
        public static bool GodMode = false;
        public Coroutine ufoCor;                  //корутина вызова UFO
        AudioSource audioSource;
        float respawnTimer;                  


    //////////////// СОБЫТИЯ

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        // Создаем индикаторы жизней
        LifesIndicators = new List<GameObject>();
        for (int i = 0; i < MaxLifes; i++)
        {
            GameObject lifeIndicator = (GameObject)Instantiate(LifeIndicator);
            lifeIndicator.transform.position = new Vector3(-500f+i*30,450f,-600f);
            LifesIndicators.Add(lifeIndicator);
        }
        // Создаем боеприпасы
        Ammo = new List<GameObject>();
        for (int i = 0; i < AmmoCount; i++)
        {
            GameObject ammo = (GameObject)Instantiate(Bullet);
            ammo.SetActive(false);
            Ammo.Add(ammo);
        }
        // Создаем взрывы
        Explosions = new List<GameObject>();
        for (int i = 0; i < ExplosionsCount; i++)
        {
            GameObject explosion = (GameObject)Instantiate(ExplosionObj);
            explosion.SetActive(false);
            Explosions.Add(explosion);
        }
        // Создаем астероиды
        Astds = new List<GameObject>();
        for (int i = 0; i < AsteroidsCount; i++)
        {
            GameObject astd = (GameObject)Instantiate(AsteroidObj);
            astd.SetActive(false);
            Astds.Add(astd);
        }
        PrepareNewGame();
    }

    void Update()
    {
        // Проверяем наличие астероидов
        bool isAsteroidExists = false;
        foreach (GameObject a in Astds)
            if (a.activeInHierarchy)
            {
                isAsteroidExists = true;
                break;
            }
        // Если не нашли активный астероид - создаем новые в зависимости от стадии игры
        if (!isAsteroidExists && (stage > 0) && !IsInvoking("NextStage")) Invoke("NextStage", 2f);
        if (!isAsteroidExists && (stage == 0)) NextStage();
    }

    ////////////////////////////////////////////////////////////////////////////////////

    // Старт новой игры
    public void StartNewGame()
    {
        PrepareNewGame();
        GetNewLife();
        StartUFO();
    }

    // Подготовка к старту новой игры
    void PrepareNewGame()
    {
        score = 0;
        stage = 0;
        // Активировать все индикаторы жизни
        foreach (GameObject a in LifesIndicators) a.SetActive(true);
        // Деактивируем все снаряды
        foreach (GameObject a in Ammo) a.SetActive(false);
        // Деактивируем все взрывы
        foreach (GameObject a in Explosions) a.SetActive(false);
        // Деактивируем все астероиды
        foreach (GameObject a in Astds) a.SetActive(false);
        // Деактивируем UFO
        UFO.SetActive(false);
    }
    
    // Новая стадия игры, создаем астероиды
    public void NextStage()
    {
        stage++;
        for (int i = 0; i < stage + 1; i++)
        {
            float halfScreenWidth = - Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x;
            float halfScreenHeight = - Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).y;
            GameObject a = GetAsteroid();
            a.gameObject.tag = "1";
            a.transform.position = new Vector3(Random.Range(-halfScreenWidth,halfScreenWidth),
                                               Random.Range(-halfScreenHeight,halfScreenHeight),0);
            a.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0,360));
            a.transform.localScale = new Vector3(LargeAsteroidSize,LargeAsteroidSize,LargeAsteroidSize);
            a.gameObject.GetComponent<Asteroid>().speed = Random.Range(AsteroidMinSpeed, AsteroidMaxSpeed);
            a.SetActive(true);
        }
    }

    // Создание двух астероидов при попадании
    public void Get2Asteroids(GameObject target)
    {
        float newSpeed = Random.Range(AsteroidMinSpeed, AsteroidMaxSpeed);
        for (int i = 0; i < 2; i++)
        {
            GameObject a = GetAsteroid();
            a.transform.rotation = target.transform.rotation * Quaternion.Euler(0,0,i*90-45);
            if (target.tag == "1") a.tag = "2";
            if (target.tag == "2") a.tag = "3";
            a.transform.position = target.transform.position;
            if (a.tag == "2") a.transform.localScale = new Vector3(MediumAsteroidSize,MediumAsteroidSize,MediumAsteroidSize);
            if (a.tag == "3") a.transform.localScale = new Vector3(SmallAsteroidSize,SmallAsteroidSize,SmallAsteroidSize);
            a.gameObject.GetComponent<Asteroid>().speed = newSpeed;
            a.SetActive(true);
        }      
    }

    // Добавить очки к счетчику
    public void AddScore(int s)
    {
        score += s;
        ScoreText.text = "Score: " + score;
    }

    // Запуск UFO корутиной
    public void StartUFO()
    {
        if (!(ufoCor == null)) StopCoroutine(ufoCor);
        ufoCor = StartCoroutine(getUFO());
    }

    // Взорвать объект
    public void Explosion(GameObject target)
    {
        if (target.tag == "UFO")
        {
            audioSource.PlayOneShot(LExplosion);
            GameObject explosion = GetExplosion();
            explosion.transform.position = target.transform.position;
            explosion.transform.localScale = new Vector3(2f, 2f, 2f);
            explosion.SetActive(true);
        }
        if ((target.tag == "1") || (target.tag == "2") || (target.tag == "3"))
        {
            GameObject explosion = GetExplosion();
            explosion.transform.position = target.transform.position;
            explosion.transform.localScale = target.transform.localScale / 50;
            explosion.SetActive(true);
            if (target.tag == "1") audioSource.PlayOneShot(LExplosion);
            if (target.tag == "2") audioSource.PlayOneShot(MExplosion);
            if (target.tag == "3") audioSource.PlayOneShot(SExplosion);
        }
    }

    // Перенос объекта на противоположную сторону экрана
    public void Teleport(GameObject s)
    {
        float halfScreenWidth = - Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x;
        float halfScreenHeight = - Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).y;
        // Грубо телепортируем объект
        s.transform.position = new Vector3(s.transform.position.x, s.transform.position.y, 500);
        if (s.transform.position.x < - halfScreenWidth) s.transform.position =
                        new Vector3(s.transform.position.x + halfScreenWidth * 2, s.transform.position.y, 500);
        if (s.transform.position.x > halfScreenWidth) s.transform.position =
                        new Vector3(s.transform.position.x - halfScreenWidth * 2, s.transform.position.y, 500);
        if (s.transform.position.y < - halfScreenHeight) s.transform.position =
                        new Vector3(s.transform.position.x, s.transform.position.y + halfScreenHeight * 2, 500);
        if (s.transform.position.y > halfScreenHeight) s.transform.position =
                        new Vector3(s.transform.position.x, s.transform.position.y - halfScreenHeight * 2, 500);
        s.transform.position = new Vector3(s.transform.position.x, s.transform.position.y, 0);        
    }

    //////////////// РАБОТА С ПУЛАМИ

    // Поучить астероид
    private GameObject GetAsteroid()
    {
        foreach (GameObject ast in Astds)
        {
            if (!ast.activeInHierarchy) return ast;
        }
        GameObject astd = (GameObject)Instantiate(AsteroidObj);
        Astds.Add(astd);
        astd.SetActive(false);
        return astd;
    }

    // Получить снаряд
    public GameObject GetAmmo(GameObject gun)
    {
        foreach (GameObject am in Ammo)
        {
            if (!am.activeInHierarchy)
            {
                PrepareBullet(am, gun);
                return am;
            }
        }
        GameObject ammo = (GameObject)Instantiate(Bullet);
        Ammo.Add(ammo);
        ammo.SetActive(false); // нужно выключить для срабатывания OnEnable() в дальнейшем
        PrepareBullet(ammo, gun);
        return ammo;
    }

    // Поучить взрыв
    private GameObject GetExplosion()
    {
        foreach (GameObject expl in Explosions)
        {
            if (!expl.activeInHierarchy) return expl;
        }
        GameObject explosion = (GameObject)Instantiate(ExplosionObj);
        Explosions.Add(explosion);
        explosion.SetActive(false);
        return explosion;
    }

    // Вспомогательная функция получения снаряда
    void PrepareBullet(GameObject ammo, GameObject gun)
    {
        ammo.tag = gun.tag;
        ammo.transform.rotation = gun.transform.rotation;
        ammo.transform.position = gun.transform.position;
        ammo.SetActive(true);
    }

    // Получить новую жизнь
    public void GetNewLife()
    {
        bool gotLife = false;

        for (int i = MaxLifes - 1; i > -1; i--)
        {
            if (LifesIndicators[i].activeInHierarchy)   
            {
                LifesIndicators[i].SetActive(false);    //Деактивируем первый встретившийся активный объект.
                gotLife = true;                         //Жизнь получена
                break;
            }
        }
        if (gotLife) // Если получили новую жизнь. Установим неуязвимость.
        {
            GodMode = true;
            StartCoroutine(GodModeAnimation());
            gameObject.GetComponent<AudioSource>().Play(0);
        }
        else GameObject.Find("GameHUD").GetComponent<HUD>().SendMessage("GameOver");
    }


    //////////// КОРУТИНЫ    

    IEnumerator GodModeAnimation()
    {
        for (float g = GodModeTimer; g >= 0; g -= 0.25f)
        {
            PlayerSpaceShip.SetActive(!PlayerSpaceShip.activeInHierarchy);
            yield return new WaitForSeconds(0.25f);
        }
        PlayerSpaceShip.SetActive(true);
        GodMode = false;
    }

    IEnumerator getUFO()
    {
        yield return new WaitForSeconds(Random.Range(ufoMinTimeout, ufoMaxTimeout));
        float yDist = Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).y / 5 * 3;
        float ufoX = Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x;
        float ufoY = Random.Range(yDist, -yDist);
        if (Random.Range(-1f,1f) > 0)
        {
            ufoX *= -1;
            UFO.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
            UFO.transform.rotation = Quaternion.Euler(0, 0, -90);
        UFO.transform.position = new Vector3(ufoX, ufoY, 0);
        UFO.SetActive(true);
    }
}
