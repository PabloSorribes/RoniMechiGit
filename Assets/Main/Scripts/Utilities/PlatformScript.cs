using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

    private Transform childTransform;
    private BoxCollider boxCollider;
    
    public enum Movement { idle, left, right, up, down};
    public Movement movement;
    public float speedMultiplier = 2f;

    // Use this for initialization
    void Start () {
        //Set the Tag in script
        this.tag = _Tags.ground;

        //Add a boxcollider to the Main gameObject
        boxCollider = gameObject.AddComponent<BoxCollider>();
        

        //Get ref to the child's transform
        childTransform = transform.GetChild(0);

        //Change the size of the Main Gameobject based on the Child's Transform
        boxCollider.size = new Vector3(childTransform.transform.localScale.x, childTransform.transform.localScale.y, childTransform.transform.localScale.z);
    }

    private void Update() {
        TestMovementBehaviour();
    }

    void TestMovementBehaviour() {
        Vector3 _movementDirection = new Vector3();

        //Decide direction
        switch (movement) {
            case Movement.left:
                _movementDirection = Vector3.left;
                break;

            case Movement.right:
                _movementDirection = Vector3.right;
                break;

            case Movement.up:
                _movementDirection = Vector3.up;
                break;

            case Movement.down:
                _movementDirection = Vector3.down;
                break;
        }

        //Move in the specified direction.
        transform.Translate(_movementDirection * Time.deltaTime * speedMultiplier);
    }
}
