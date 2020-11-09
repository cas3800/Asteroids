using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private GameObject ast;
    float rotationSpeed, acceleration, maxSpeed, speed, shotTimeout, timeFromShot;
    Vector3 inertion;
    ParticleSystem ps;
    bool shotMode;
    AudioSource accsSound;

    void Start()
    {
        ast = GameObject.Find("Asteroids");
        accsSound = gameObject.GetComponent<AudioSource>(); 
        rotationSpeed = ast.GetComponent<Asteroids>().MaxRotationSpeed;
        maxSpeed = ast.GetComponent<Asteroids>().MaxSpeed;
        acceleration = ast.GetComponent<Asteroids>().Acceleration;
        shotMode = ast.GetComponent<Asteroids>().ShotOnlyOnClick;
        ps = GetComponent<ParticleSystem>();
        shotTimeout = 1 / ast.GetComponent<Asteroids>().PlayerShotSpeed;
        VarReset();
    }

    public void VarReset() // Сброс переменных
    {
        speed = 0;
        inertion = new Vector3(0f,0f,0f);
        transform.position = new Vector3(0f,0f,0f);
        transform.rotation = new Quaternion(0f,0f,0f,0f);
        timeFromShot = 100;
    }

    void Update()
    {
        // Выстрел
        timeFromShot += Time.deltaTime;   
        if ((((Input.GetMouseButtonDown(0) && Asteroids.ControllerType) || Input.GetKeyDown("space")) && shotMode) || ((Input.GetAxisRaw("Fire1") == 1) && !shotMode))
        {
            if (timeFromShot > shotTimeout)
            {
                GameObject bullet = ast.GetComponent<Asteroids>().GetAmmo(gameObject);
                timeFromShot = 0;
            }
            else Debug.Log("Перезарядка.");
        }
        
        // Телепортация
        ast.GetComponent<Asteroids>().Teleport(gameObject);
        
        // Обработка поворота и ускорения
        Vector3 moveDirection = inertion * speed;
        if (Asteroids.ControllerType) // если управляем мышью
        {
            Vector3 mousePosition = Input.mousePosition;                        // Получаем позицию мыши
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);      // Приводим к мировым координатам
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, mousePosition-transform.position); // Направление от корабля к курсору
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, //Поворот c фиксированной скоростью
                            Time.deltaTime * rotationSpeed / Quaternion.Angle(transform.rotation, toRotation)); 
            if (Input.GetMouseButton(1))
            {
                moveDirection += transform.up * acceleration * Time.deltaTime; //Ускорение по нажатию правой клавиши
                if (!accsSound.isPlaying) accsSound.Play(0);
            }
            else if (accsSound.isPlaying) accsSound.Stop();
        }
        else
        {
            // Поворот. Настройки управления в Project settings
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), Vector3.forward);
            // Ускорение по нажатию клавиши. Настройки в Project settings. Если добавить негативную клавишу, то будет замедляться.
            moveDirection += transform.up * acceleration * Input.GetAxis("Vertical") * Time.deltaTime;
        }
        inertion = Vector3.Normalize(moveDirection);
        speed = moveDirection.magnitude;
        if (speed > maxSpeed) speed = maxSpeed;
        transform.Translate(inertion*speed*Time.deltaTime, Space.World);
        
        // Анимация неуязвимости
        if (ps.isPlaying && !Asteroids.GodMode) ps.Stop();
        if (!ps.isPlaying && Asteroids.GodMode) ps.Play();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player") LostLife();
    }

    void OnCollisionStay(Collision collision)
    {
        LostLife();
    }

    void LostLife()
    {
        if (!Asteroids.GodMode)
        {    
            VarReset();
            ast.GetComponent<Asteroids>().GetNewLife();
        }
    }
}
