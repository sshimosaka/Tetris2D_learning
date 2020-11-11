using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawntetrismino : MonoBehaviour
{
    //アタッチされたゲームオブジェクトにInspecterビューに表記される
    public GameObject[] Tetrominoes;

    // Start is called before the first frame update
    void Start()
    {
        //最初に呼び出す処理
        NewTetromino();
    }

    public void NewTetromino()
    {
        //ゲームオブジェクトを生成
        //Inspecterビューに各minoをアタッチするとブロックが出現する
        //また、spawnの現在地(transform)から回転処理をしないで生成するという意味になる
        Instantiate(Tetrominoes[Random.Range(0, Tetrominoes.Length)], transform.position, Quaternion.identity);
    }
}
