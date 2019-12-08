using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour {

    public GameObject fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;
    //limites del espacio
    public Vector3 swimLimits = new Vector3(5, 5, 5);

    public Vector3 goalPosition;

    [Header("Fish Settings")] //Muestra la informacion de cabecera en el inspector
    [Range(0.0f, 5.0f)]       //Permite manipular la variable con un slider del rango indicado  
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourdDistance; //distancia para considerar a otro elemento como su vecino
    [Range(0.0f, 5.0f)]
    public float rotationSpeed;


    void Start () {
        //crear el array en memoria
        allFish = new GameObject[numFish];
        for(int i = 0; i < numFish; i++)
        {
            //posicion inicial asignada al pez
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                                            Random.Range(-swimLimits.y, swimLimits.y),
                                                            Random.Range(-swimLimits.z, swimLimits.z));
            //instancio y guardo el pez 
            allFish[i] = (GameObject)Instantiate(fishPrefab, pos, Quaternion.identity);
            
            //Seteando el manager
            allFish[i].GetComponent<Flock>().myManager = this;
        }

        goalPosition = this.transform.position;
	}
	
	void Update () {
       if(Random.Range(0,100)<10)
            goalPosition = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                                            Random.Range(-swimLimits.y, swimLimits.y),
                                                            Random.Range(-swimLimits.z, swimLimits.z));
                                                            
    }
}
