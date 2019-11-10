using System;
using System.Collections.Generic;
using System.Net;


public class XVM {
	
	public Script script;

	public void Run () {
	
		ResetScript ();

		if ( !script.isMainFuncPresent ) {
			Console.WriteLine ( "当前脚本没有入口函数: main" );
			return;
		}

		Instr currentInstr = GetCurrentInstr ();

		while ( currentInstr != null ) {
			
			int currentInstrIdx = script.instrStream.currentInstr;
			
			switch ( currentInstr.opCode ) {
				
				case OpCode.Mov:

					Op Mov_dest = GetOp ( 0 );
					Op Mov_source = GetOp ( 1 );
					
					Console.WriteLine ( $"Mov {Mov_dest} {Mov_source}" );

					if ( Mov_dest == Mov_source ) {
						break;
					}
					
					CopyValue ( Mov_dest, Mov_source );
					
					// 使用dest覆盖堆栈中地址的值???
					
					break;
				
				case OpCode.Add:
					
					Op Add_dest = GetOp ( 0 );
					
					if ( Add_dest.type == OpType.INT ) {
						Add_dest.intLiteral += GetOpAsInt ( 1 );
					}
					else if ( Add_dest.type == OpType.FLOAT ) {
						Add_dest.floatLiteral += GetOpAsFloat ( 1 );
					}
					else if ( Add_dest.type == OpType.STRING ) {
						Add_dest.floatLiteral += GetOpAsFloat ( 1 );
					}

					Console.WriteLine ( $"Add {Add_dest} {GetOp ( 1 )}" );
					
					break;
				
				case OpCode.Sub:
					
					Op Sub_dest = GetOp ( 0 );
					
					if ( Sub_dest.type == OpType.INT ) {
						Sub_dest.intLiteral -= GetOpAsInt ( 1 );
					}
					else if ( Sub_dest.type == OpType.FLOAT ) {
						Sub_dest.floatLiteral -= GetOpAsFloat ( 1 );
					}

					Console.WriteLine ( $"Sub {Sub_dest} {GetOp ( 1 )}" );
					
					break;
				
				case OpCode.Mul:
					
					Op Mul_dest = GetOp ( 0 );
					
					if ( Mul_dest.type == OpType.INT ) {
						Mul_dest.intLiteral *= GetOpAsInt ( 1 );
					}
					else if ( Mul_dest.type == OpType.FLOAT ) {
						Mul_dest.floatLiteral *= GetOpAsFloat ( 1 );
					}

					Console.WriteLine ( $"Mul {Mul_dest} {GetOp ( 1 )}" );
					
					break;
				
				case OpCode.Div:
					
					Op Div_dest = GetOp ( 0 );
					
					if ( Div_dest.type == OpType.INT ) {
						Div_dest.intLiteral /= GetOpAsInt ( 1 );
					}
					else if ( Div_dest.type == OpType.FLOAT ) {
						Div_dest.floatLiteral /= GetOpAsFloat ( 1 );
					}

					Console.WriteLine ( $"Div {Div_dest} {GetOp ( 1 )}" );
					
					break;
				
				case OpCode.Jmp:

					script.instrStream.currentInstr = GetOpAsInstrIndex ( 0 );
					Console.WriteLine ( $"Jump {GetOp ( 0 )}" );
					break;
				
				case OpCode.JE:
				case OpCode.JNE:
				case OpCode.JG:
				case OpCode.JL:
				case OpCode.JGE:
				case OpCode.JLE:

					Op J_op1 = GetOp ( 0 );
					Op J_op2 = GetOp ( 1 );

					bool J_jump = false;
					switch ( J_op1.type ) {
						case OpType.INT:
							if ( currentInstr.opCode == OpCode.JE ) {
								J_jump = J_op1.intLiteral == J_op2.intLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JNE ) {
								J_jump = J_op1.intLiteral != J_op2.intLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JG ) {
								J_jump = J_op1.intLiteral > J_op2.intLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JL ) {
								J_jump = J_op1.intLiteral < J_op2.intLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JGE ) {
								J_jump = J_op1.intLiteral >= J_op2.intLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JLE ) {
								J_jump = J_op1.intLiteral <= J_op2.intLiteral;
							}
							break;
						case OpType.FLOAT:
							if ( currentInstr.opCode == OpCode.JE ) {
								J_jump = J_op1.floatLiteral == J_op2.floatLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JNE ) {
								J_jump = J_op1.floatLiteral != J_op2.floatLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JG ) {
								J_jump = J_op1.floatLiteral > J_op2.floatLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JL ) {
								J_jump = J_op1.floatLiteral < J_op2.floatLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JGE ) {
								J_jump = J_op1.floatLiteral >= J_op2.floatLiteral;
							}
							else if ( currentInstr.opCode == OpCode.JLE ) {
								J_jump = J_op1.floatLiteral <= J_op2.floatLiteral;
							}
							break;
						case OpType.STRING:
							if ( currentInstr.opCode == OpCode.JE ) {
								J_jump = script.stringTable.table [ J_op1.stringTableIndex ] == script.stringTable.table [ J_op2.stringTableIndex ];
							}
							else if ( currentInstr.opCode == OpCode.JNE ) {
								J_jump = script.stringTable.table [ J_op1.stringTableIndex ] != script.stringTable.table [ J_op2.stringTableIndex ];
							}
							break;
					}
					
					Console.WriteLine ( $"{currentInstr.opCode} {J_op1} {J_op2} {GetOpAsInstrIndex ( 2 )}" );

					if ( J_jump ) {
						script.instrStream.currentInstr = GetOpAsInstrIndex ( 2 );
					}
					break;
				
				case OpCode.Call:

					int Call_funcIdx = GetOpAsFuncIndex ( 0 );
					FuncNode Call_destFunc = script.funcTable.table [ Call_funcIdx ];
					
					// 保存当前堆栈帧索引
					int Call_frameIndex = script.stack.frameIndex;

					// 指令索引移动到下一个
					script.instrStream.currentInstr++;
					
					// 向堆栈中压入返回地址
					Op Call_returnAddr = new Op { type = OpType.INSTR_INDEX, instrIndex = script.instrStream.currentInstr };
					script.stack.Push ( Call_returnAddr );
					
					// 压入堆栈帧
					// +1的额外控件是给函数索引的
					Call_destFunc.stackFrameSize = Call_destFunc.localDataSize + 1;
					script.stack.PushFrame ( Call_destFunc.localDataSize + 1 );

					// 把函数索引和原堆栈帧写入堆栈顶部
					Op Call_funcIndexValue = new Op { type = OpType.FUNC_INDEX, funcIndex = Call_funcIdx, offsetIndex = Call_frameIndex };
					
					script.stack.SetStackValue ( script.stack.topIndex - 1, Call_funcIndexValue );
					
					// 设置指令索引到目标函数入口点
					script.instrStream.currentInstr = Call_destFunc.entryPoint;
					
//					Console.WriteLine ( $"Call指令 跳转地址:{script.instrStream.currentInstr} 目标函数表索引:{Call_funcIdx} 堆栈帧:{Call_frameIndex} 返回地址:{Call_returnAddr.instrIndex} 函数信息值:{Call_funcIndexValue} #{Call_funcIndexValue.GetHashCode()}" );
//					Console.WriteLine ( script.stack );s
					break;
				
				case OpCode.Ret:
					
					// 从堆栈顶部取出当前函数索引并引用它去获取相应的函数结构
					Op Ret_funcIndexValue = script.stack.Pop ();
//					Console.WriteLine ( $"Ret指令 当前函数信息值:{Ret_funcIndexValue} #{Ret_funcIndexValue.GetHashCode()}" );
					FuncNode Ret_currentFunc = script.funcTable.table [ Ret_funcIndexValue.funcIndex ];
					int Ret_frameIndex = Ret_funcIndexValue.offsetIndex;
					
					// 从堆栈中读取返回地址的结构，位置在局部数据下面的一个索引中
					Op Ret_returnAddr = script.stack.GetStackValue ( script.stack.topIndex - ( Ret_currentFunc.localDataSize + 1 ) );
					
					// 把堆栈框架联通返回地址一起弹出
					script.stack.PopFrame ( Ret_currentFunc.stackFrameSize );
					
					// 储存之前的堆栈索引
					script.stack.frameIndex = Ret_frameIndex;
					
					// 跳转到返回地址
					script.instrStream.currentInstr = Ret_returnAddr.instrIndex;
					
//					Console.WriteLine ( $"Ret指令 当前函数索引:{Ret_funcIndexValue.funcIndex} 堆栈帧:{Ret_frameIndex} 返回地址:{Ret_returnAddr.instrIndex}" );

					break;
			}

			// 当前指令没有让指针发生跳转，则可以增加
			if ( currentInstrIdx == script.instrStream.currentInstr ) {
				script.instrStream.currentInstr++;
			}
			
			currentInstr = GetCurrentInstr ();
		}
		
		Console.WriteLine ( $"脚本运行结束: {script.stack}" );
	}
	
	public void ResetScript () {
		
		int mainFuncIndex = script.mainFuncIndex;
		if ( script.funcTable.table != null && script.funcTable.table.Length != 0 ) {
			if ( script.isMainFuncPresent ) {
				script.instrStream.currentInstr = script.funcTable.table [ mainFuncIndex ].entryPoint;
//				Console.WriteLine ( $"设置程序入口指令索引:{script.instrStream.currentInstr}" );
			}
		}

		script.stack.topIndex = 0;
		script.stack.frameIndex = 0;

		for ( var i = 0; i < script.stack.elements.Length; ++i ) {
			script.stack.elements [ i ].type = OpType.Null;
		}

		script.isPaused = false;
		
		// 全局变量分配空间
		script.stack.PushFrame ( script.globalDataSize );
		
		// 入口函数分配空间
		script.stack.PushFrame ( script.funcTable.table [ mainFuncIndex ].localDataSize + 1 );
	}

	public Instr GetCurrentInstr () {
		
		if ( script.instrStream.currentInstr >= 0 && script.instrStream.currentInstr < script.instrStream.instrs.Length ) {
			return script.instrStream.instrs [ script.instrStream.currentInstr ];
		}
		
		return null;
	}

	public Op GetOp ( int opIndex ) {

		Op opValue = null;
		
		if ( script.instrStream.currentInstr >= 0 && script.instrStream.currentInstr < script.instrStream.instrs.Length ) {
			
			Instr instr = script.instrStream.instrs [ script.instrStream.currentInstr ];
			if ( opIndex >= 0 && opIndex < instr.opList.Count ) {
				opValue = instr.opList [ opIndex ];
			}
		}

		switch ( opValue.type ) {
			case OpType.ABS_STACK_INDEX:
			case OpType.REL_STACK_INDEX:
				return script.stack.GetStackValue ( opValue.stackIndex );
			case OpType.REG:
				return script._ret_val;
			default:
				return opValue;
		}
	}

	public OpType GetOpType ( int opIndex ) {
		return GetOp ( opIndex ).type;
	}
	
	public int GetOpAsInt ( int opIndex ) {
		return CoerceValueToInt ( GetOp ( opIndex ) );
	}
	
	public float GetOpAsFloat ( int opIndex ) {
		return CoerceValueToFloat ( GetOp ( opIndex ) );
	}
	
	public string GetOpAsString ( int opIndex ) {
		return CoerceValueToString ( GetOp ( opIndex ) );
	}
	
	public int GetOpAsStackIndex ( int opIndex ) {
		return GetOp ( opIndex ).stackIndex;
	}
	
	public int GetOpAsInstrIndex ( int opIndex ) {
		return GetOp ( opIndex ).instrIndex;
	}
	
	public int GetOpAsFuncIndex ( int opIndex ) {
		return GetOp ( opIndex ).funcIndex;
	}
	
	public int GetOpAsHostAPICallIndex ( int opIndex ) {
		return GetOp ( opIndex ).hostAPICallIndex;
	}
	
	public int GetOpAsReg ( int opIndex ) {
		return GetOp ( opIndex ).reg;
	}

	#region CoerceValueConvert
	public int CoerceValueToInt ( Op val ) {
		switch ( val.type ) {
			case OpType.INT: return val.intLiteral;
			case OpType.FLOAT: return (int)val.floatLiteral;
			case OpType.STRING:
				var result = 0;
				int.TryParse ( script.stringTable.table [ val.stringTableIndex ], out result );
				return result;
			default:
				return 0;
		}
	}
	
	public float CoerceValueToFloat ( Op val ) {
		switch ( val.type ) {
			case OpType.INT:   return (float)val.intLiteral;
			case OpType.FLOAT: return val.floatLiteral;
			case OpType.STRING:
				var result = 0f;
				float.TryParse ( script.stringTable.table [ val.stringTableIndex ], out result );
				return result;
			default:
				return 0f;
		}
	}
	
	public string CoerceValueToString ( Op val ) {
		switch ( val.type ) {
			case OpType.INT:   return val.intLiteral.ToString ();
			case OpType.FLOAT: return val.floatLiteral.ToString ();
			case OpType.STRING: return script.stringTable.table [ val.stringTableIndex ];
			default:
				return string.Empty;
		}
	}
	#endregion

	Op ResolveOpPntr ( int opIndex ) {
		
		switch ( GetOpType ( opIndex ) ) {
			case OpType.ABS_STACK_INDEX:
			case OpType.REL_STACK_INDEX:
				int absIdx = GetOpAsStackIndex ( opIndex );
				return script.stack.GetStackValue ( absIdx );
			case OpType.REG:
				return script._ret_val;
		}

		return null;
	}
	
	void CopyValue ( Op dest, Op source ) {
		
		source.CopyTo ( dest );
		
		if ( source.type == OpType.STRING ) {
			string strRef = CoerceValueToString ( source );
			dest.stringTableIndex = script.stringTable.AddString ( string.Copy ( strRef ) );
		}
	}
}
