using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idou : MonoBehaviour
{
    [SerializeField] float m_dushSpeed = 7f;
    /// <summary>動く速さ</summary>
    [SerializeField] float m_movingSpeed = 5f;
    /// <summary>ターンの速さ</summary>
    [SerializeField] float m_turnSpeed = 3f;
    /// <summary>ジャンプ力</summary>
    [SerializeField] float m_jumpPower = 5f;
    /// <summary>接地判定の際、中心 (Pivot) からどれくらいの距離を「接地している」と判定するかの長さ</summary>
    [SerializeField] float m_isGroundedLength = 0.1f;
    CharacterController m_charaCon;
    Rigidbody m_rb;
    Animator m_anim;
    bool m_runFlag = false;
    float speed = 0f;

    void Start()
    {
        m_charaCon = GetComponent<CharacterController>();
        m_rb = GetComponent<Rigidbody>();
        m_anim = GetComponent<Animator>();
        //マウスカーソルの位置を固定する
        Cursor.lockState = CursorLockMode.Locked;
        //マウスカーソルの表示の有無
        Cursor.visible = false;
    }

    void Update()
    {
        // 方向の入力を取得し、方向を求める
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        // 入力方向のベクトルを組み立てる
        Vector3 dir = Vector3.forward * v + Vector3.right * h;

        // カメラを基準に入力が上下=奥/手前, 左右=左右にキャラクターを向ける
        dir = Camera.main.transform.TransformDirection(dir);    // メインカメラを基準に入力方向のベクトルを変換する
        dir.y = 0;  // y 軸方向はゼロにして水平方向のベクトルにする
        
        if (Input.GetButtonDown("Dush")) //Left shift
        {
            //入力した方向へ走る
            Vector3 runVelo = dir.normalized * m_dushSpeed;
            //走っている状態でジャンプしたときのy軸を保持する
            runVelo.y = m_rb.velocity.y;
            m_rb.velocity = runVelo;

            m_anim.SetTrigger("Run");
        }
        else if (Input.GetButtonUp("Dush"))
        {
            m_anim.SetTrigger("Walk");
        }
        
        if (IsGrounded())
        {
            if (dir == Vector3.zero)
            {
                // 方向の入力がニュートラルの時は、y 軸方向の速度を保持するだけ
                m_rb.velocity = new Vector3(0f, m_rb.velocity.y, 0f);
            }
            else
            {
                // 入力方向に滑らかに回転させる
                transform.rotation = Quaternion.LookRotation(dir);
                //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * m_turnSpeed);  // Slerp を使うのがポイント

                Vector3 velo = dir.normalized * m_movingSpeed; // 入力した方向に移動する
                velo.y = m_rb.velocity.y;   // ジャンプした時の y 軸方向の速度を保持する
                m_rb.velocity = velo;   // 計算した速度ベクトルをセットする
                
            }

            if (m_anim)
            {
                if (IsGrounded())
                {
                    Vector3 velo = m_rb.velocity;
                    velo.y = 0;
                    m_anim.SetFloat("Speed", velo.magnitude);
                }
                else
                {
                    m_anim.SetFloat("Speed", 0f);
                }
            }

            // ジャンプの入力を取得し、接地している時に押されていたらジャンプする
            if (Input.GetButtonDown("Jump"))
            {
                m_rb.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
                if (m_anim)
                {
                    m_anim.SetTrigger("Jump");
                }
            }
            Debug.Log(m_rb.velocity.magnitude);
        }

         

       



        /// <summary>
        /// 地面に接触しているか判定する
        /// </summary>
        /// <returns></returns>
        bool IsGrounded()
        {
            // Physics.Linecast() を使って足元から線を張り、そこに何かが衝突していたら true とする
            Vector3 start = this.transform.position;   // start: オブジェクトの中心
            Vector3 end = start + Vector3.down * m_isGroundedLength;  // end: start から真下の地点
            Debug.DrawLine(start, end); // 動作確認用に Scene ウィンドウ上で線を表示する
            bool isGrounded = Physics.Linecast(start, end); // 引いたラインに何かがぶつかっていたら true とする
            return isGrounded;
        }


    }
}
