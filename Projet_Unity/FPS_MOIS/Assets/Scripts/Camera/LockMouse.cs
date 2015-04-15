// by @torahhorse

using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour
{	
	CursorLockMode lockCursorMode;

	void Start()
	{
		Cursor.lockState = lockCursorMode;
		LockCursor(true);
	}

    void Update()
    {
    	// lock when mouse is clicked
    	if( Input.GetMouseButtonDown(0) && Time.timeScale > 0.0f )
    	{
    		LockCursor(true);
    	}
    
    	// unlock when escape is hit
        if  ( Input.GetKeyDown(KeyCode.Escape) )
        {
        	LockCursor(false);
        }
    }
    
    public void LockCursor(bool lockCursor)
    {
		if(lockCursor == true)
			lockCursorMode = CursorLockMode.Locked;
		if(lockCursor == false)
			lockCursorMode = CursorLockMode.None;
		Cursor.visible = lockCursor;
    }
}