using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class move : MonoBehaviour
{
    public double i = 0.0;
    public int rot = 1;
    [SerializeField] private MouseLook m_MouseLook;
    // Start is called before the first frame update
    void Start()
    {
        m_MouseLook.Init(transform, Camera.main.transform);
        //transform.position = new Vector3(0, 0, 0);
    }

    Vector3 moveKitty(bool x, float step)
    {

        return x ? new Vector3(transform.position.x + step, transform.position.y, transform.position.z) : new Vector3(transform.position.x, transform.position.y , transform.position.z + step);

    }
    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, Camera.main.transform);
    }
    // Update is called once per frame
    void Update()
    {
        RotateView();
        m_MouseLook.UpdateCursorLock();
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector3 temppos = transform.localPosition;

            temppos.y += 1;
            transform.localPosition = temppos;
        }
        else
if (Input.GetKeyDown(KeyCode.J))
        {
            Vector3 temppos = transform.localPosition;

            temppos.y -= 1;
           transform.localPosition = temppos;
        }
        //else
        //    if (Input.GetKey (KeyCode.UpArrow))
        //    transform.position += Vector3.up * Time.deltaTime; 
        //else
        //    if (Input.GetKey (KeyCode.DownArrow))
        //    transform.position += Vector3.down * Time.deltaTime; 
    }
}
