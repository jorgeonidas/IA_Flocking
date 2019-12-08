using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public FlockManager myManager;
    float speed;
    bool turning = false;
    Animator anim;

	void Start () {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
        anim = GetComponent<Animator>();
        //velocidad y ciclo de la animacion
        //estas variables estan ligados a el parametro speedMultiplier y offset respectivamente
        anim.SetFloat("swimSpeed", speed);
        anim.SetFloat("swimSpeed", Random.Range(0, 1.0f));
	}
	

	void Update () {

        //Determina limites en forma de una caja rectangular 
        Bounds b = new Bounds(myManager.transform.position, myManager.swimLimits * 2);

        //si esta furea de los limites, empieza a dar la vuelta
        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;

        //si sale de los limites
        if (!b.Contains(transform.position))
        {
            turning = true;
            //Le doy direccion opuesta para que se devuelva
            direction = myManager.transform.position - transform.position;
        }//si esta cerca de un obstaculo
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            //calculo vector reflejo
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
            //Debug.DrawRay(this.transform.position, this.transform.forward * 50, Color.red);
        }
        else
        {
            turning = false;
        }
        //si giro
        if (turning)
        {
   
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * myManager.rotationSpeed);
        }
        else//caso contrario aplico las reglas
        {
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if (Random.Range(0, 100) < 20)
                AppyRules();

            anim.SetFloat("swimSpeed", speed);
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
	}

    void AppyRules()
    {
        GameObject[] gos;
        gos = myManager.allFish;
        //sumatoria de los centros
        Vector3 vCentre = Vector3.zero;
        //sumatoria del vector evasion
        Vector3 vAvoid = Vector3.zero;
        //sumatoria de las velocidades
        float gSpeed = 0.01f;
        //distancia entre este pez y el vecino
        float nDistance;
        int groupSize = 0; //cantidad de vecinos dentro del el radio indicado por flockManager.neighbourdDistance

        //por cada pez en el FlockManager
        foreach (GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                //si estoy muy cerca de otro pez sumo su centro y aumento el tamaño del grupo
                if (nDistance <= myManager.neighbourdDistance)
                {
                    vCentre += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f)//si esta muy cerca de su vecino actual, acumulo vector evasion
                    {                       //Vector evasion entre este pez y su vecino
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;//acumulo las velocidades
                }
            }
        }

        //si el tamaño del grupo es de al menos dos integrantes ( este pez y su vecino ) 
        //entonces cambio las reglas
        if(groupSize > 0)
        {
            vCentre = vCentre / groupSize + (myManager.goalPosition - this.transform.position); //centro pomedio del grupo
            speed = gSpeed / groupSize;     //velocidad promedio del grupo

            Vector3 direction = (vCentre + vAvoid) - transform.position; //nueva direcion

            //si esta nueva direccion no da cero, entonces el pez rota
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                        Quaternion.LookRotation(direction), 
                                                        myManager.rotationSpeed * Time.deltaTime);
        }
    }
}
