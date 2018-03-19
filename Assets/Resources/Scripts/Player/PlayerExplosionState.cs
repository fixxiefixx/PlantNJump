using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerExplosionState : PlayerState {

    private float endTimer = 2;

    public PlayerExplosionState(GameObject go) : base(go, "explosion")
    {

    }

    public override void EnterState()
    {
        player.twineLineRenderer.enabled = false;
        player.VisibleObject.SetActive(false);
        player.rigid.bodyType = RigidbodyType2D.Static;
        GameObject.Instantiate(player.PlantExplosionPrefab, player.transform.position, Quaternion.identity);
    }

    public override void FixedUpdate()
    {
        endTimer -= Time.deltaTime;
        if(endTimer<0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
