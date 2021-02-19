using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ///<summary>走ったときの速さ</summary>
    [SerializeField] float m_dushSpeed = 10f;
    ///<summary>動く速さ</summary>
    [SerializeField] float m_movingSpeed = 7f;
    /// <summary>ジャンプ力</summary>
    [SerializeField] float m_jumpPower = 5f;
    /// <summary>接地判定の際、中心 (Pivot) からどれくらいの距離を「接地している」と判定するかの長さ</summary>
    [SerializeField] float m_isGroundedLength;
    Rigidbody m_rb;
    Animator m_anim;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_anim = GetComponent<Animator>();
        //マウスカーソルの位置の固定
        Cursor.lockState = CursorLockMode.Locked;
        //マウスカーソルの非表示
        Cursor.visible = false;
    }

    
    void Update()
    {
        //方向の入力を取得し、方向を求める
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        //入力方向のベクトルを組み立てる
        Vector3 dir = Vector3.forward * v + Vector3.right * h;

        ////カメラを基準に入力したベクトルを変化させる
        //dir = Camera.main.transform.TransformDirection(dir);
        //dir.y = 0;

        if (IsGrounded())
        {
            if (dir == Vector3.zero)
            {
                m_rb.velocity = new Vector3(0f, m_rb.velocity.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(dir);

                Vector3 velo = dir.normalized * m_movingSpeed;
                velo.y = m_rb.velocity.y;
                m_rb.velocity = velo;
                if (Input.GetButtonDown("Run"))
                {
                    Vector3 runVelo = dir.normalized * m_dushSpeed;
                    runVelo.y = m_rb.velocity.y;
                    m_rb.velocity = runVelo;
                }
            }

            if (m_anim)
            {
                if (IsGrounded())
                {
                    Vector3 velo = m_rb.velocity;
                    velo.y = 0;
                    m_anim.SetFloat("Speed", 0f);
                }
            }

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
    }

    /// <summary>
    /// 地面に接触しているか判定する
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        Vector3 start = this.transform.position;
        Vector3 end = start + Vector3.down * m_isGroundedLength;
        Debug.DrawLine(start, end);
        bool isGrounded = Physics.Linecast(start, end);
        return isGrounded;
    }
}
