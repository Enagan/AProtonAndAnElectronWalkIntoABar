//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Circuit abstract parent class
/// This class handles the input, output and state of all circuits 
/// It contains an identifier that is assigned on creation and is unique to all circuits
/// In order to create a subcircuit one must implement the following methods:
/// * protected abstract bool logicOperation(bool[] inputsArray); calculates output logic from an array of inputs
/// * public abstract string circuitName();  returns the subcircuits name as a string
/// </summary>
public abstract class Circuit : MonoBehaviour, Activator
{
  #region private fields
  // Dictionary that associates the Unique identifier of an input (Key) with its boolean state (Value)
  private Dictionary<Circuit, bool> _inputs;
  
  // List that keeps track of available circuit outputs
  // Must be set-up with Unity Editor
  [SerializeField]
  private List<Circuit> _outputs;

  // The circuit unique identifier, used to propagate the state
  private int _identifier;

  // The circuit state that indicates if it is activated or deactivated
  // Best variable for DEBUG of logical operations
  [SerializeField]
  protected bool _state = false;

  [SerializeField]
  protected GameObject _activatedLightsParent = null;
  #endregion

  #region public properties
  // Getter for circuit outputs. Usefull for circuit serialization
  public List<Circuit> outputs
  {
    get
    {
      return _outputs;
    }
  }

  // Getter for circuit inputs. Usefull for circuit serialization
  public List<Circuit> inputs
  {
    get
    {
      List<Circuit> inputs = new List<Circuit>();
      foreach(Circuit c in _inputs.Keys)
      {
        inputs.Add(c);
      }
      return inputs;
    }
  }
  #endregion

  #region private properties
  // counter Variable and Counter property
  // Are used on Circuit creation to acess unique identifiers
  private static int counter = 0;
  private static int IncrCounter {
     get {
        ++counter;
        return counter; 
     }
   }
  #endregion

  #region Circuit Constructor
  public Circuit()
  {
    _inputs = new Dictionary<Circuit, bool>();
    _outputs = new List<Circuit>();
    _identifier = IncrCounter;
  }

  private void Awake()
  {
    PropagateToOutputs();
  }

  private void Start()
  {
    if (_state)
    { 
      TurnOnLights(); 
    }
    else
    {
      TurnOffLights();
    }
  }
  #endregion
  #region Activator interface methods

   /// <summary>
  /// Activates the object. 
  /// Sends true as input to all output circuits.
  /// Method can be overriden by leaf circuits
  /// </summary>
  public virtual void Activate()
  {
    _state = true;
    PropagateToOutputs(true);
    TurnOnLights();
  }


  /// <summary>
  /// Deactivates the object. 
  /// Sends false as input to all output circuits.
  /// Method can be overriden by leaf circuits
  /// </summary>
  public virtual void Deactivate()
  {
    _state = false;
    PropagateToOutputs(false);

    TurnOffLights();
  }


  protected void TurnOnLights()
  {
    if (_activatedLightsParent == null)
    {
      return;
    }

    foreach (Light light in _activatedLightsParent.GetComponentsInChildren<Light>())
    {
      light.enabled = true;
    }
  }

  protected void TurnOffLights()
  {
    if (_activatedLightsParent == null)
    {
      return;
    }
    
    foreach (Light light in _activatedLightsParent.GetComponentsInChildren<Light>())
    {
      light.enabled = false;
    }
  }

  #endregion

  #region Circuit Methods

  /// <summary>
  /// Method called by an input circuit that updates the current state
  /// and progagates changes to outputs of this circuit
  /// <param name="state">Binary state recieved by the circuit</param>
  /// <param name="inputID">Identifier of input</param>
  /// </summary>
  public bool Input(bool state, Circuit inputCircuit)
  {
    //BipolarConsole.IvoLog(CircuitName() + _identifier + " Input:" + state + " sent by " + inputID);
    //Add input to dictionary
    _inputs[inputCircuit] = state;
	 
     //Infer Logic from operation and call Activate and Deactivate
     bool[] inputsArray = new bool[_inputs.Count];
     _inputs.Values.CopyTo(inputsArray, 0);
    
    if (LogicOperation(inputsArray))
     {
       Activate();
     }
     else
     {
       Deactivate();
     }
       return _state;
   }

  /// <summary>
  /// Auxiliary method that progates output
  /// <param name="output">Output to be propagated</param>
  /// </summary>
  void PropagateToOutputs(bool output)
  {
    //Debug.Log(CircuitName() + _identifier + " Output:" + _state);
    foreach (Circuit c in _outputs)
    {
      if (!(c == null))
      {
        c.Input(output, this);
      }
    }
  }
  
  /// <summary>
  /// Auxiliary method that progates state to output 
  /// </summary>
  public void PropagateToOutputs()
  {
    PropagateToOutputs(_state);
  }
  #endregion

  #region Abstract Circuit Methods
  
  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// This method has to be overriden by each child classes of Circuit
  /// <param name="inputsArray">Binary input for the circuit</param>
  /// </summary>
  protected abstract bool LogicOperation(bool[] inputsArray);

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public abstract string CircuitName();
  #endregion
}
   


