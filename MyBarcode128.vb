Function MyBarcode128 (code_str As String) As String
    'V 2.0.0
	'Parameters : a string
	'Return : * a string which give the bar code when it is dispayed with CODE128.TTF font
	'				 * an empty string if the supplied parameter is no good
	' based on: https://github.com/saper-2/crystalreports-code128
	' My Github repo: https://github.com/simchev/crystalreports-code128
	Dim i As Number
	Dim checksum As Number
	Dim mini As Number
	Dim dummy As Number
	Dim charsetB As Boolean
	'Dim MyBarcode128 as string
	
	MyBarcode128 = ""
	
	If (Len(code_str) > 0) Then
	'Check for valid characters
		For i = 1 To Len(code_str)
			Select Case Asc(Mid(code_str, i, 1))
			Case 32 To 126
			Case Else
				i = 0
				Exit For
			End Select
		Next 'NEXT:FOR i = 1 To Len(code_str)
		'Generate barcode string with optimalization by using table B or C
		MyBarcode128 = ""
		charsetB = True
		If (i > 0) Then
			i = 1 'string index
			Do While (i <= Len(code_str))
				If (charsetB) Then
					' let's check if we can switch to C table, 
					' for this check if first or last 4 chars are digits - if yes then use B charset, if there is 6 digits switch to C charset
					'mini = IIf(i = 1 Or i + 3 = Len(code_str), 4, 6)
					if (i = 1 or (i+3) = Len(code_str)) then
						mini = 4
					else
						mini = 6
					end if 'ENDIF (i = 1 or (i+3) = Len(code_str)) 
					
					'GoSub testnum
                    mini = mini - 1
                	If ((i + mini) <= Len(code_str)) Then
                		Do While (mini >= 0)
                			If Asc(Mid(code_str, i + mini, 1)) < 48 Or Asc(Mid(code_str, i + mini, 1)) > 57 Then Exit Do
                			mini = mini - 1
                		Loop
                	End If
                    ' return gosub
					
					If (mini < 0) Then 'select charset C
						If (i = 1) Then 'if first char from code_str then add START-C code
							MyBarcode128 = Chr(205)
						Else 'if not first char from code_str, then use Code-C switch
							MyBarcode128 = MyBarcode128 & Chr(199)
						End If
						charsetB = False
					Else 'ELSE:(mini < 0)
						' charset B
						If i = 1 Then MyBarcode128 = Chr(204) 'START-B code
					End If 'ENDIF:ELSE:(mini < 0)
				End If 'ENDIF (charsetB)
				If (Not charsetB) Then
					' if we are in charset-C try process 2 digits at once
					mini = 2

					'GoSub testnum
                    mini = mini - 1
                	If ((i + mini) <= Len(code_str)) Then
                		Do While (mini >= 0)
                			If Asc(Mid(code_str, i + mini, 1)) < 48 Or Asc(Mid(code_str, i + mini, 1)) > 57 Then Exit Do
                			mini = mini - 1
                		Loop
                	End If
                    ' return gosub
					If (mini < 0) Then 'tested 2 chars are digit, process them
						dummy = Val(Mid(code_str, i, 2))
						'dummy = IIf(dummy < 95, dummy + 32, dummy + 105)
						if (dummy < 95) then
							dummy = dummy + 32
						else
							dummy = dummy + 100
						end if
						MyBarcode128 = MyBarcode128 & Chr(dummy)
						i = i + 2
					Else 'ELSE:(mini < 0) - dang, we don't have 2 digits, switch to charset-B
						MyBarcode128 = MyBarcode128 & Chr(200)
						charsetB = True
					End If 'ENDDIF:ELSE:(mini < 0)
				End If 'ENDIF:(Not charsetB)
				If (charsetB) Then
					' process one char from input using charset-B
					MyBarcode128 = MyBarcode128 & Mid(code_str, i, 1)
					i = i + 1
				End If 'ENDIF:(charsetB)
			Loop
			'Calc CSM
			For i = 1 To Len(MyBarcode128)
				dummy = Asc(Mid(MyBarcode128, i, 1))
				if (dummy < 127) then
					dummy = dummy - 32
				else
					dummy = dummy - 100
				end if
				
				If i = 1 Then 
					checksum = dummy
				end if
				checksum = (checksum + (i - 1) * dummy) Mod 103
			Next
			'Get the ASCII code for CSM
			'checksum = IIf(checksum < 95, checksum + 32, checksum + 105)
			if (checksum < 95) then
				checksum = checksum + 32
			else
				checksum = checksum + 100
			end if
			'Slap at end function result the CSM char and STOP
			MyBarcode128 = MyBarcode128 & Chr(checksum) & Chr(206)
		End If
	End If 'ENDIF:(Len(code_str) > 0)
	Exit Function
End Function 