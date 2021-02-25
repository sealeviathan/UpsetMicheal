using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    
    public bool gameOver = false;
    public enum playerState {Running, Dead};
    public enum gameState {Playing, Ended};
    
    playerState curState_pl;
    gameState curState_game;
    public playerState CurState
    {
        get {return curState_pl;}
        set {curState_pl = value;}
    }
    public gameState GameState
    {
        get {return curState_game;}
        set {curState_game = value;}
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        gameOver = false;
        curState_game = gameState.Playing;
        curState_pl = playerState.Running;
    }

    // Update is called once per frame
    void Update()
    {
        if(curState_pl == playerState.Running)
        {

        }
        else if (curState_pl == playerState.Dead)
        {
            curState_game = gameState.Ended;
        }
        //All gameover logic. Shut stuff down n stuff.
        if(curState_game == gameState.Ended)
        {
            gameOver = true;
            MapBuilder.instance.slowing = true;
            Debug.Log("GameOver");
            GameObject.FindWithTag("MainCar").GetComponent<Vehicle_Controller>().StopAllCoroutines();
        }
    }
}
