using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    

    private void Awake()
    {
        instance = this;
    }

    public GameObject PlayerObj;
    private GameObject player;
    public GameObject deathEffect;
    public float timeToRespawn = 4f;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint(PhotonNetwork.LocalPlayer.ActorNumber -1);
        player = PhotonNetwork.Instantiate(PlayerObj.name, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"{PhotonNetwork.NickName} - {spawnPoint}");

        
    }

    public void Die(string shooter)
    {

        UIController.instance.deathText.text = $"Killed by {shooter}";

        if(player != null)
        {
            StartCoroutine(DieCoroutine());
        }
        
       
    }

    public IEnumerator DieCoroutine()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);


        UIController.instance.deathPanel.SetActive(true);

        yield return new WaitForSeconds(timeToRespawn);

        UIController.instance.deathPanel.SetActive(false);
        SpawnPlayer();
    }

}

   
