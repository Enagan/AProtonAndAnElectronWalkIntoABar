//Made by. Ivo Capelo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;




/// <summary>
/// AnimationChildHandler is a class that holds animations for an object
/// The class is meant to be a child in a hiearchy that contains a parent AnimationRootHandler
/// Scripts that call animations on this object should be located at AnimationRootHandlers and use them to call this object's animations
/// For usage reffer to instructions in AnimationRootHandler.cs
/// </summary>
[RequireComponent(typeof(Animation))]
public class AnimationChildHandler : MonoBehaviour
{

  #region private variables

  //_childName is the ID of a child in the hiearchy, every child should have a different name
  //Different hiearchies can have children with the same name
  [SerializeField]
  private string _childName = "";

  //Animation Component of AnimationChildHandler, required
  private Animation _handlerAnim;

  //Dictionary containing quick access to clips in Animation component and their respective names
  private Dictionary<string, AnimationState> _dictionaryData;

  #endregion

  #region public properties

  public string childName
  {
    get { return _childName; }

    set { _childName = value; }
  }

  //property of an AnimationChildHandler that refers to available animation clips 
  public Dictionary<string, AnimationState> animationClips
  {
    //Getter of property initializes _dictionaryData at startup
    get {

      //Get Component if null
      if (_handlerAnim == null)
        _handlerAnim = this.GetComponent<Animation>();

      
      if (_dictionaryData==null && _handlerAnim !=null)
      {
        //Create new dictionary
        _dictionaryData = new Dictionary<string, AnimationState>();

        //Get All animations in Animation component and add links to dictionary
        foreach (AnimationState clip in _handlerAnim)
        {
           _dictionaryData.Add(clip.name, clip);
        }
      }
      return _dictionaryData;
    }

    //Simple setter
    set 
    {
      _dictionaryData = value; 
    }
  }

  #endregion

  #region Monobehavior methods
  void Start () {
    //Force naming on AnimationChildHandler
    if (childName.CompareTo("") == 0)
    {
      throw new BipolarExceptionUnassignedName("Nameless Animation instanced");
    }
    else
    {
      if(_handlerAnim == null)
        _handlerAnim = this.GetComponent<Animation>();
    }
	}
  #endregion

  #region Animation call methods

  //Getter for Animation component
  public Animation getAnimation()
  {
    return _handlerAnim;
  }

  //Plays clip
  public void playAnimation(string clipName)
  {
    _handlerAnim.Play(clipName);
  }
  #endregion


}
