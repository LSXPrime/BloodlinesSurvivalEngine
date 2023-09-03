using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ExitNode_NoLoop_toStart : BaseNode{
    //[Input] public int entry;
    [Input] public Connection entry;
    [Output] public Connection exit;
    public override string GetString()
    {
        return "CloseDialogue_ExitNode_NoLoop_toStart";
    }
    public override object GetValue(NodePort port){
		return null;
	}

    
}
