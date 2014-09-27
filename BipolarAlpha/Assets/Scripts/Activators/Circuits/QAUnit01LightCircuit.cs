using UnityEngine;
using System.Collections;

public class QAUnit01LightCircuit : Circuit
{

  MeshRenderer _mesh;
  public void Start()
  {
    _mesh = GetComponent<MeshRenderer>();

    Deactivate();
  }

  #region Circuit Methods

    /// <summary>
    /// Method used to infer circuit output by looking at input
    /// This method is overriden to infer using the logical operation AND
    /// <param name="inputsArray">Binary input for the circuit</param>
    /// </summary>
   protected override bool LogicOperation(bool[] inputsArray)
   {
     bool isOpen = true; ;

     foreach (bool input in inputsArray)
     {
       isOpen = isOpen && input;
     }

     if (isOpen)
       Activate();
     else
       Deactivate();

     return isOpen;
   }

   public override void Activate()
   {
     if(_mesh)
      _mesh.material.SetColor("_Color", new Color(0, 0, 255));
   }

   public override void Deactivate()
   {
     if(_mesh)
     _mesh.material.SetColor("_Color", new Color(255, 0, 0));
   }


   /// <summary>
   /// Method that returns each circuit Name, used for debug
   /// </summary>
   public override string CircuitName()
   {
     return "QAUnit01LightCircuit";
   }
   #endregion
}
