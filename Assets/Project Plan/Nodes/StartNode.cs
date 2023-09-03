using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StartNode : BaseNode {

	//[Output] public int exit;
    //[Input] public Connection restart;
    //[Input] public Connection restart;
    [Output] public Connection exit;
    
    public override string GetString(){
        return "Start";
    }

    
}