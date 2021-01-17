using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OwnGradient : MonoBehaviour
{
    //ref to mesh
    public meshGenerator mesh;

    //gradient
    public Gradient myGradient = new Gradient();

    #region Start/update

    void Start()
    {
        //create the gradient
        CreateGradient();
    }

    #endregion

    #region Functions

    public void CreateGradient()
    {
        GradientPicker.Create(myGradient, "", SetGradient, GradientFinished);
    }

    private void SetGradient(Gradient currentGradient)
    {
        mesh.SetNewGradient(currentGradient);
    }

    private void GradientFinished(Gradient finishedGradient)
    {
      
    }

    #endregion
}
