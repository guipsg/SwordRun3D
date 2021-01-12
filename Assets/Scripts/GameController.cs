using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        }

        if (player.gameObject.transform.position.y < -20f)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
