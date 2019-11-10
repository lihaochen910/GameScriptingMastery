using System;
using System.Collections.Generic;


/// <summary>
/// 汇编脚本解析器
/// </summary>
public class XASMParser {
	public XASMLexer      lexer;
	public List < Token > tokenList;

	/// <summary>
	/// 当前在某函数作用域内，目前不支持多层嵌套
	/// </summary>
	private bool isFuncActive;

	private Script script;
	
	private int instrStreamSize;

	private FuncNode currentFunc;
	private int currentParamIndex;
	private InstrLookupTable instrLookupTable;
	
	public XASMParser ( string text ) {
		lexer     = new XASMLexer ( text );
		tokenList = new List < Token > ();
		
		instrLookupTable = new InstrLookupTable ();
		script = new Script ();
		
		InstrLookupTable.InitInstrTable ();
	}

	public Script Compile () {
		
		FirstParse ();
		
		lexer.Reset ();
		script.instrStream.size = instrStreamSize;
		script.instrStream.instrs = new Instr[ instrStreamSize ];
		for ( var i = 0; i < instrStreamSize; ++i ) {
			script.instrStream.instrs [ i ] = new Instr { opList = new List < Op >() };
		}
		
		instrStreamSize = 0;
		isFuncActive = false;
		
		SecondParse ();
		
//		for ( var i = 0; i < script.instrStream.instrs.Length; ++i ) {
//			Console.WriteLine ( $"[{i}] " + script.instrStream.instrs [ i ] );
//		}

		return script;
	}
	
	private void FirstParse () {
		
		Token token = null;
		
		do {
			
			token = lexer.NextToken ();
			
//			Console.WriteLine ( token );

			switch ( token.type ) {
				
				case TokenType.SET_STACK_SIZE:

					// set_stack_size 只能出现在全局范围内
					if ( isFuncActive ) {
						throw new SyntaxError ( Messages.ERROR_MSG_LOCAL_SETSTACKSIZE, token.span.Start.Line,
							token.span.Start.Column );
					}

					break;

				case TokenType.FUNC:

					var funcNameToken = lexer.NextToken ();

					if ( funcNameToken.type != TokenType.IDENT ) {
						throw new SyntaxError ( Messages.ERROR_MSG_FUNC_NAME_ERROR, token.span.Start.Line,
							token.span.Start.Column );
					}

					isFuncActive = true;
					var funcIdx = script.funcTable.AddFunc ( funcNameToken.value, instrStreamSize );

					if ( funcIdx == FuncTable.ERROR_IDX ) {
						throw new SyntaxError (
							string.Format ( Messages.ERROR_MSG_FUNC_DUPLICATE_DEFINITION,
								funcNameToken.value ) , token.span.Start.Line, token.span.Start.Column );
					}
					
					currentFunc = script.funcTable.GetFuncByName ( funcNameToken.value );

					// 跳过函数名单词后面的可选换行符
					var funcNameNextToken = lexer.NextToken ();
					while ( funcNameNextToken.type == TokenType.NEWLINE ) {
						funcNameNextToken = lexer.NextToken ();
					}

					if ( funcNameNextToken.type != TokenType.OPEN_BRACE ) {
						throw new SyntaxError ( Messages.ERROR_MSG_FUNC_START_MISMATCH, token.span.Start.Line,
							token.span.Start.Column );
					}
					
					// 入口函数检查
					if ( funcNameToken.value == XASMLexer.Keyworkd_Main ) {
						script.isMainFuncPresent = true;
						script.mainFuncIndex     = currentFunc.index;
					}
					
//					lexer.RollingBackToTheTokenStart ( funcNameNextToken );

					instrStreamSize++;
					break;
				
				case TokenType.CLOSE_BRACE:
					
					// 只能出现在函数体结尾
					if ( !isFuncActive ) {
						throw new SyntaxError ( Messages.ERROR_MSG_FUNC_END_MISMATCH, token.span.Start.Line,
							token.span.Start.Column );
					}
					
					script.funcTable.SetFuncInfo ( currentFunc.name, currentFunc.paramCount, currentFunc.localDataSize );
					
					isFuncActive = false;
					break;
				
				/*
				 * var (VarName)([ArraySize])
				 */
				case TokenType.VAR:
					
					var varNameToken = lexer.NextToken ();

					if ( varNameToken.type != TokenType.IDENT ) {
						throw new SyntaxError ( Messages.ERROR_MSG_VAR_NAME_ERROR, token.span.Start.Line,
							token.span.Start.Column );
					}
					
//					Console.WriteLine ( varNameToken );

					int arraySize = 1;
					char varNextChar = lexer.PeekChar ( 1 );
					
					if ( varNextChar == XASMLexer.Open_Bracket ) {
	
						if ( lexer.NextToken ().type != TokenType.OPEN_BRACKET ) {
							throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_START_ERROR, token.span.Start.Line,
								token.span.Start.Column );
						}

						var varArraySizeToken = lexer.NextToken ();
						if ( varArraySizeToken.type != TokenType.INT ) {
							throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_SIZE_FORMAT_ERROR, token.span.Start.Line,
								token.span.Start.Column );
						}
						if ( !int.TryParse ( varArraySizeToken.value, out arraySize ) ) {
							throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_SIZE_FORMAT_ERROR, token.span.Start.Line,
								token.span.Start.Column );
						}
						
						if ( lexer.NextToken ().type != TokenType.CLOSE_BRACKET ) {
							throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_END_ERROR, token.span.Start.Line,
								token.span.Start.Column );
						}
					}

					int stackIndex;
					if ( isFuncActive ) {
						stackIndex = -( 2 + currentFunc.localDataSize );

						if ( script.symbolTable.AddSymbol ( varNameToken.value, arraySize, stackIndex, currentFunc.index ) == -1 ) {
							throw new SyntaxError (
								string.Format ( Messages.ERROR_MSG_FUNC_VAR_DUPLICATE_DEFINITION,
									varNameToken.value ) , token.span.Start.Line, token.span.Start.Column );
						}

						currentFunc.localDataSize += arraySize;
					}
					else {
						stackIndex = script.globalDataSize;
						if ( script.symbolTable.AddSymbol ( varNameToken.value, arraySize, stackIndex, -1 ) == -1 ) {
							throw new SyntaxError (
								string.Format ( Messages.ERROR_MSG_GLOBAL_VAR_DUPLICATE_DEFINITION,
									varNameToken.value ) , token.span.Start.Line, token.span.Start.Column );
						}
						script.globalDataSize += arraySize;
					}
					
					break;
				
				/*
				 * Param (ParameterName)
				 */
				case TokenType.PARAM:
					
					// 只能出现在函数体中
					if ( !isFuncActive ) {
						throw new SyntaxError ( Messages.ERROR_MSG_FUNC_PARAM_WRONG_LOCATION, token.span.Start.Line,
							token.span.Start.Column );
					}
					
					var paramNameToken = lexer.NextToken ();

					if ( paramNameToken.type != TokenType.IDENT ) {
						throw new SyntaxError ( Messages.ERROR_MSG_FUNC_PARAM_WRONG_FORMAT, token.span.Start.Line,
							token.span.Start.Column );
					}

					currentFunc.paramCount++;
					break;
				
				/*
				 * (IdentifierName):
				 */
				case TokenType.IDENT:
					
					// 跳转只能出现在函数体中
					if ( !isFuncActive ) {
						throw new SyntaxError ( Messages.ERROR_MSG_LABEL_WRONG_LOCATION, token.span.Start.Line,
							token.span.Start.Column );
					}

					var labelToken = token;
					var labelNameNextToken = lexer.NextToken ();

					if ( labelNameNextToken.type != TokenType.COLON ) {
						throw new SyntaxError ( Messages.ERROR_MSG_LABEL_WRONG_FORMAT, token.span.Start.Line,
							token.span.Start.Column );
					}

					int targetIndex = instrStreamSize - 1;
					
					var labelIdx = script.labelTable.AddLabel ( labelToken.value, targetIndex, currentFunc.index );

					if ( labelIdx == FuncTable.ERROR_IDX ) {
						throw new SyntaxError (
							string.Format ( Messages.ERROR_MSG_LABEL_DUPLICATE_DEFINITION,
								labelToken.value ) , token.span.Start.Line, token.span.Start.Column );
					}
					break;
				
				/*
				 * 指令定义
				 */
				case TokenType.INSTR:
					
					// 指令只能出现在函数体中
					if ( !isFuncActive ) {
						throw new SyntaxError ( Messages.ERROR_MSG_INSTR_WRONG_LOCATION, token.span.Start.Line,
							token.span.Start.Column );
					}

					instrStreamSize++;

					Token instrEndToken = null;
					do {
						instrEndToken = lexer.NextToken ();
					}
					while ( instrEndToken.type != TokenType.NEWLINE || instrEndToken.type == TokenType.EOF );
					break;
				
				case TokenType.EOF: 
					break;
				
				default:

					if ( token.type != TokenType.NEWLINE ) {
						throw new SyntaxError ( Messages.ERROR_MSG_INVALID_TOKEN + $" {token}", token.span.Start.Line,
							token.span.Start.Column );
					}
					
					break;
			}
			
		} while ( token.type != TokenType.EOF );

		if ( isFuncActive ) {
			throw new SyntaxError ( Messages.ERROR_MSG_FUNC_END_NOT_FOUND, token.span.Start.Line,
				token.span.Start.Column );
		}
		
//		Console.WriteLine ( $"指令流Size: {instrStreamSize}" );
	}

	private void SecondParse () {
		
		Token token = null;

		while ( (token = lexer.NextToken ()).type != TokenType.EOF ) {

//            Console.WriteLine ( token );

            switch ( token.type ) {
				
				case TokenType.FUNC:

					isFuncActive = true;
					currentFunc = script.funcTable.GetFuncByName ( lexer.NextToken ().value );
					currentParamIndex = 0;
					break;
				
				case TokenType.CLOSE_BRACE:
					
					isFuncActive = false;
					if ( currentFunc.name == XASMLexer.Keyworkd_Main ) {
						script.instrStream.instrs [ instrStreamSize ].opCode = OpCode.Exit;
						script.instrStream.instrs [ instrStreamSize ].opCount = 1;
						script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.INT, intLiteral = 0 }  );
					}
					else {
						script.instrStream.instrs [ instrStreamSize ].opCode  = OpCode.Ret;
						script.instrStream.instrs [ instrStreamSize ].opCount = 0;
						script.instrStream.instrs [ instrStreamSize ].opList = null;
					}
					
//					Console.WriteLine ( $"添加 {script.instrStream.instrs [ instrStreamSize ]} {instrStreamSize} {lexer.LexerPosition}" );

					instrStreamSize++;
					break;
				
				case TokenType.PARAM:

					var paramNameToken = lexer.NextToken ();
					int stackIndex = -( currentFunc.localDataSize + 2 + (currentParamIndex + 1) ); // +2 因为从-2 开始(-1保留) +1 因为参数在返回地址的下面

					if ( script.symbolTable.AddSymbol ( paramNameToken.value, 1, stackIndex, currentFunc.index ) == -1 ) {
						throw new SyntaxError (
							string.Format ( Messages.ERROR_MSG_FUNC_PARAM_DUPLICATE_DEFINITION,
								paramNameToken.value ), token.span.Start.Line, token.span.Start.Column );
					}

					currentParamIndex++;
					break;
				
				case TokenType.INSTR:

					InstrLookup currentInstrLookup = null;
					if ( !instrLookupTable.GetInstrByMnemonic ( token.value, out currentInstrLookup ) ) {
						throw new SyntaxError (
							string.Format ( Messages.ERROR_MSG_INSTR_NOT_FOUND,
								token.value ) , token.span.Start.Line, token.span.Start.Column );
					}

					script.instrStream.instrs [ instrStreamSize ].opCode = currentInstrLookup.opCode;
					script.instrStream.instrs [ instrStreamSize ].opCount = currentInstrLookup.opCount;

					for ( int i = 0; i < currentInstrLookup.opCount; i++ ) {
						OpFlagType currentOpFlagType = currentInstrLookup.opList [ i ];
						
						Token instrParamToken = lexer.NextToken ();
//						Console.WriteLine ( $"指令参数{i}: {instrParamToken}" );
						switch ( instrParamToken.type ) {
							
							case TokenType.INT:

								if ( (currentOpFlagType & OpFlagType.INT) == OpFlagType.INT ) {
									script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.INT, intLiteral = int.Parse ( instrParamToken.value ) } );
								}
								else {
									throw new SyntaxError (
										string.Format ( Messages.ERROR_MSG_INSTR_WRONG_PARAM,
											token.value, i, instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
								}
								break;
							
							case TokenType.FLOAT:

								if ( (currentOpFlagType & OpFlagType.FLOAT) == OpFlagType.FLOAT ) {
									script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.FLOAT, floatLiteral = float.Parse ( instrParamToken.value ) } );
								}
								else {
									throw new SyntaxError (
										string.Format ( Messages.ERROR_MSG_INSTR_WRONG_PARAM,
											token.value, i, instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
								}
								break;
							
							case TokenType.STRING:
								
								if ( (currentOpFlagType & OpFlagType.STRING) == OpFlagType.STRING ) {
									
									int stringTableIdx = script.stringTable.AddString ( instrParamToken.value );
									
									script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.STRING, stringTableIndex = stringTableIdx } );
								}
								else {
									throw new SyntaxError (
										string.Format ( Messages.ERROR_MSG_INSTR_WRONG_PARAM,
											token.value, i, instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
								}
								break;
							
							case TokenType.REG_RETVAL:
								
								if ( (currentOpFlagType & OpFlagType.REG) == OpFlagType.REG ) {
									script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.REG, reg = 0 } );
								}
								else {
									throw new SyntaxError (
										string.Format ( Messages.ERROR_MSG_INSTR_WRONG_PARAM,
											token.value, i, instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
								}
								break;
							
							case TokenType.IDENT:
								
								if ( (currentOpFlagType & OpFlagType.FUNC_NAME) == OpFlagType.FUNC_NAME ) {
									
									FuncNode func = script.funcTable.GetFuncByName ( instrParamToken.value );

									if ( func != null ) {
										script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.FUNC_INDEX, funcIndex = func.index } );
									}
									else {
										throw new SyntaxError (
											string.Format ( Messages.ERROR_MSG_FUNC_NOT_FOUND,
												instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
									}
								}
								
								if ( (currentOpFlagType & OpFlagType.LINE_LABEL) == OpFlagType.LINE_LABEL ) {

									LabelNode labelNode = script.labelTable.GetLabelByIdent ( instrParamToken.value, currentFunc.index );

									if ( labelNode != null ) {
										script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.INSTR_INDEX, instrIndex = labelNode.targetIndex } );
									}
									else {
										throw new SyntaxError (
											string.Format ( Messages.ERROR_MSG_LABEL_NOT_FOUND,
												instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
									}
								}
								
								if ( (currentOpFlagType & OpFlagType.MEM_REF) == OpFlagType.MEM_REF ) {

									SymbolNode symbolNode = script.symbolTable.GetSymbolByIdent ( instrParamToken.value, currentFunc.index );

									// 如果该变量不是函数作用域内定义的，则查找全局变量
									if ( symbolNode == null ) {
										symbolNode = script.symbolTable.GetSymbolByIdent ( instrParamToken.value, -1 );
									}
									if ( symbolNode == null ) {
										throw new SyntaxError (
											string.Format ( Messages.ERROR_MSG_VAR_NOT_FOUND,
												instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
									}

									int baseIndex = symbolNode.stackIndex;
									char symbolNextChar = lexer.PeekChar ( 1 );

									if ( symbolNextChar != char.MinValue ) {

										// 非数组成员
										if ( symbolNextChar != XASMLexer.Open_Bracket ) {
											// 验证非数组定义
											if ( symbolNode.size > 1 ) {
												throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_START_ERROR, token.span.Start.Line,
													token.span.Start.Column );
											}
											
											script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.ABS_STACK_INDEX, stackIndex = baseIndex } );
										}
										else {
											// 验证数组定义
											if ( symbolNode.size <= 1 ) {
												throw new SyntaxError (
													string.Format ( Messages.ERROR_MSG_VAR_TYPE_IS_NOT_A_ARRAY,
														instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
											}
											
											// 左中括号
											lexer.NextToken ();
											
											// 数组索引参数
											Token arrayIndexParamToken = lexer.NextToken ();

											// 数组索引是常数
											if ( arrayIndexParamToken.type == TokenType.INT ) {
												int offset = int.Parse ( arrayIndexParamToken.value );
												if ( offset >= symbolNode.size ) {
													throw new SyntaxError (
														string.Format ( Messages.ERROR_MSG_INDEX_OUT_OF_BOUNDS,
															arrayIndexParamToken.value ), token.span.Start.Line, token.span.Start.Column );
												}
												
												script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.ABS_STACK_INDEX, stackIndex = baseIndex >= 0 ? baseIndex + offset : baseIndex - offset } );
											}
											
											// 数组索引是变量
											else if ( arrayIndexParamToken.type == TokenType.IDENT ) {
												
												SymbolNode offsetSymbolNode = script.symbolTable.GetSymbolByIdent ( arrayIndexParamToken.value, currentFunc.index );

												// 如果该变量不是函数作用域内定义的，则查找全局变量
												if ( offsetSymbolNode == null ) {
													symbolNode = script.symbolTable.GetSymbolByIdent ( arrayIndexParamToken.value, -1 );
												}
												if ( offsetSymbolNode == null ) {
													throw new SyntaxError (
														string.Format ( Messages.ERROR_MSG_VAR_NOT_FOUND,
															arrayIndexParamToken.value ), token.span.Start.Line, token.span.Start.Column );
												}

												// 不支持数组作为数组的索引
												if ( offsetSymbolNode.size > 1 ) {
													throw new SyntaxError (
														string.Format ( Messages.ERROR_MSG_VAR_ARRAY_INDEX_TYPE_NOT_SUPPORT,
															arrayIndexParamToken.value ), token.span.Start.Line, token.span.Start.Column );
												}
												
												script.instrStream.instrs [ instrStreamSize ].opList.Add ( new Op { type = OpType.REL_STACK_INDEX, stackIndex = offsetSymbolNode.stackIndex } );
												
												// 右中括号
												if ( lexer.NextToken ().type != TokenType.CLOSE_BRACKET ) {
													throw new SyntaxError ( Messages.ERROR_MSG_VAR_ARRAY_END_ERROR, token.span.Start.Line,
														token.span.Start.Column );
												}
												
											}
											else {
												throw new SyntaxError (
													string.Format ( Messages.ERROR_MSG_VAR_ARRAY_INDEX_TYPE_NOT_SUPPORT,
														arrayIndexParamToken.value ), token.span.Start.Line, token.span.Start.Column );
											}
											
										}
										
									}
									else {
										throw new SyntaxError ( Messages.ERROR_MSG_INVALID_TOKEN, token.span.Start.Line,
											token.span.Start.Column );
									}
								}
								
								break;
							
							default:
								
								throw new SyntaxError (
									string.Format ( Messages.ERROR_MSG_INVALID_PARAM_TYPE,
										instrParamToken.value ), token.span.Start.Line, token.span.Start.Column );
								break;
						}

						// 指令参数之间的逗号分隔符
						if ( i < currentInstrLookup.opCount - 1 ) {
							if ( lexer.NextToken ().type != TokenType.COMMA ) {
								throw new SyntaxError ( Messages.ERROR_MSG_INSTR_PARAM_MISS_COMMA, token.span.Start.Line, token.span.Start.Column );
							}
						}
					}
					
					// 每条指令以换行符结束
					if ( lexer.NextToken ().type != TokenType.NEWLINE ) {
						throw new SyntaxError ( Messages.ERROR_MSG_INSTR_END_MISMATCH, token.span.Start.Line, token.span.Start.Column );
					}
					
//					Console.WriteLine ( $"添加 {script.instrStream.instrs [ instrStreamSize ]} {instrStreamSize} {lexer.LexerPosition}" );

					instrStreamSize++;
					break;	// End Of TokenType.INSTR
			} // End Of switch ( token.Type )
			
			// SkipToNextLine???
			
		}	// End Of While
	}
	
}
