# GameScriptingMastery
使用C#实现 **&lt;GameScriptingMastery>(游戏脚本高级编程)** 书中的XtremeScript Assembler

### Instruction
* Mov
* Add, Sub, Mul, Div
* Jmp, JE, JNE, JG, JL, JGE, JLE
* Call, Ret

### XtremeScript Assembly Language Grammar
Declare local variable
```
var Sum
```

Define script entry function
```
func main {

	var Num
	Mov Num, 1
	
LoopBody:            # Implement a loop

	Add Num, 1
	JL  Num, 3, LoopBody

}
```

Define a function
```
func MyFunction {
	param X          # Declare some parameters
	param Y
	
	var Sum
	
	Mov Sum,      X
	Add Sum,      Y
	Mov _ret_val, Sum   # Save the result to the stack's registers
}
```

Call a function
```
func Function_1 {

}

func main {
    Call Function_1
}
```

### Usage
Code for execute 'xasm' script:
```csharp
var parser = new XASMParser ( File.ReadAllText ( "main.xasm" ) );
			
var vm = new XVM ();
vm.script = parser.Compile ();
vm.Run ();
```

### Note
* Jump Label can only be defined in function
* Defining functions within functions is not supported
* English is not my native language, please understand
