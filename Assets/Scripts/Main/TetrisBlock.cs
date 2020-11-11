using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    //回転
    public Vector3 rotationPoint;
    //時間の変数
    private float previousTime;
    //落下してる時間
    public float fallTime = 0.8f;
    //テトリスのフィールドサイズ
    public static int height = 20;
    public static int width = 10;
    //この変数を加えることでブロックを積み上げることができる
    //以下は二次元配列を示している
    private static Transform[,] grid = new Transform[width, height];

    //ゲームエフェクト
    public GameObject effectPrefabs;
    public Vector3 effectRotation;

    //効果音
    private AudioSource MoveSound;

    //下キーを押し続けるための処理
    //Dictionaryはキーと値をセットで指定し、キーで検索して値を取得する事が出来ます。
    //<書式>Dictionary<キーの型, 値の型> dictionary = new Dictionary<キーの型, 値の型> ();
    Dictionary<KeyCode, int> keyInputTime = new Dictionary<KeyCode, int>();
    bool GetKeyEx(KeyCode keyCode)
    {
        //Dictionaryに押されたキーが含まれていないことをチェック
        if (!keyInputTime.ContainsKey(keyCode))
        {
            keyInputTime.Add(keyCode, -1);
        }
        //キーを押したとき動く方向に+1を加算していく
        if (Input.GetKey(keyCode))
        {
            keyInputTime[keyCode]++;
        }
        else
        {
            //上ボタンを押し続けると段々早くなるためここで制御する
            //押さなければ-1する
            keyInputTime[keyCode] = -1;
        }
        return (keyInputTime[keyCode] == 0 || keyInputTime[keyCode] >= 10);
    }


    private void Start()
    {
        MoveSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        //ブロックのキー操作
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            MoveSound.Play();
            //ValidMoveメソッドができたらキー操作の処理に差し込む
            if (!ValidMove())
            {
                transform.position -= new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            MoveSound.Play();
            if (!ValidMove())
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //回転する処理
            transform.RotateAround(transform.TransformPoint(rotationPoint),new Vector3(0,0,1), 90);
            MoveSound.Play();
            if (!ValidMove())
            {
                //90にするとTブロックがフィールドサイズを突き抜けてしまうため値に気を付けること
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }
        }
        //ブロックの1マスずつ自動落下する処理
        //これに加え下キーを押したときの処理を追加する
        //この処理の条件は下キーを押されてら?の処理を行い、押されてなければ:の処理を行う(三項演算子)
        if (Time.time - previousTime > (GetKeyEx(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            //このTetrisBlockにアタッチされたゲームオブジェクトに対して以下の処理を行う
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                //移動するたびに有効な位置かどうかを確認する
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckforLines();
                //Spawn処理が終わったらブロックが終わったことを示すための処理がいる
                this.enabled = false;
                //ブロックを生み出すクラスからメソッドを呼び出す
                FindObjectOfType<Spawntetrismino>().NewTetromino();

            }
            previousTime = Time.time;
        }
    }
    //方針:(ブロックの揃い有無→ブロックを消す→消えたブロックの空白を埋める)
    //ラインがあるか?確認
    void CheckforLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }
    //列がそろっているか確認
    bool HasLine(int i)
    {
        for (int j = 0; j< width; j++)
        {
            if(grid[j,i] == null)
            {
                return false;
            }
        }
        FindObjectOfType<GameManagement>().AddScore();
        return true;
    }
    //ラインを消す
    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
            //ブロック消えた後にエフェクトを出す
            Instantiate(effectPrefabs, transform.position, Quaternion.Euler(effectRotation));
        }
    }
    //列を下げる
    void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                //もし消えたブロックより上のブロックを下に下げる
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    //ブロックを積み上げるロジック
    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;

            //height-1=19のところまでブロックが来たら 
            if (roundedY >= height - 1)
            {
                //GameOver()メソッドを呼び出す
                FindObjectOfType<GameManagement>().GameOver();
            }
        }
    }

    //Update文だけだと指定したフィールドサイズを突き抜けてしまうためブロックの動きを制限するための処理を書く必要がある
    //またbool型を指定しているので戻り値が必要
    
    //ブロックの移動範囲の制御
    bool ValidMove()
    {
        //拡張for文を利用し変数transformからchildrenに格納する
        foreach (Transform children in transform)
        {
            //ブロックの子の座標を取得している
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            //フィールドサイズより超えている場合
            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if(grid[roundedX,roundedY] != null)
            {
                return false;
            }
        }
        //フィールドサイズより超えていない場合
        return true;
    }
}
