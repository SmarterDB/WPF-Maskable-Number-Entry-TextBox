
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Globalization
Imports System.Windows.Data
Imports System.Diagnostics
Imports System.Windows.Input

'Namespace SmarterDB.CodeProject.DataEntryVB
#Region "Documentation Tags"
''' <summary>
'''     WPF Maskable TextBox class. Just specify the TextBoxMaskBehavior.Mask and ValueType 
'''     attached property to a TextBox. It protect your TextBox from unwanted non numeric 
'''     symbols and make it easy to modify your numbers.
''' </summary>
''' <remarks>
''' <para>
'''     Class Information:
'''	    <list type="bullet">
'''         <item name="authors">Authors: SmarterDB</item>
'''         <item name="date">February 2012</item>
'''         <item name="originalURL">http://www.smarterdb.com</item>
'''         <item name="authors">Authors: Ruben Hakopian</item>
'''         <item name="date">February 2009</item>
'''         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
'''     </list>
''' </para>
''' </remarks>
#End Region

Public Class TextBoxMaskBehavior
#Region "MinimumValue Property"

    Public Shared Function GetMinimumValue(obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MinimumValueProperty))
    End Function

    Public Shared Sub SetMinimumValue(obj As DependencyObject, value As Double)
        obj.SetValue(MinimumValueProperty, value)
    End Sub

    Public Shared ReadOnly MinimumValueProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("MinimumValue", _
                                            GetType(Double), _
                                            GetType(TextBoxMaskBehavior), _
                                            New FrameworkPropertyMetadata(Double.NaN, _
                                                                          AddressOf MinimumValueChangedCallback))

    Private Shared Sub MinimumValueChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim _this As TextBox = TryCast(d, TextBox)

        Dim vt As ValueTypes = GetValueType(_this)
        Dim min As Double = GetMinimumValue(_this)
        If Not min.Equals(Double.NaN) Then
            Select Case vt
                Case ValueTypes.[Integer]
                    If min < Convert.ToDouble(Int32.MinValue) Then
                        Throw New ArgumentOutOfRangeException("Overflow, minimum: " + Int32.MinValue.ToString())
                    End If
                    'SetMinimumValue(_this, Convert.ToDouble(Int32.MinValue));
                    Exit Select

                Case ValueTypes.[Double]
                    'Egy karakterrel előbb megállunk, hogy ne okozzon exception-t.
                    If min < ([Double].MinValue / 100) Then
                        Throw New ArgumentOutOfRangeException("Overflow, minimum: " + _
                                                              ([Double].MinValue / 100).ToString())
                    End If
                    'SetMinimumValue(_this, (Double.MinValue / 100));
                    Exit Select
            End Select
        End If

        ValidateTextBox(_this)
    End Sub
#End Region

#Region "MaximumValue Property"

    Public Shared Function GetMaximumValue(obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MaximumValueProperty))
    End Function

    Public Shared Sub SetMaximumValue(obj As DependencyObject, value As Double)
        obj.SetValue(MaximumValueProperty, value)
    End Sub

    Public Shared ReadOnly MaximumValueProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("MaximumValue", _
                                            GetType(Double), _
                                            GetType(TextBoxMaskBehavior), _
                                            New FrameworkPropertyMetadata(Double.NaN, _
                                                                          AddressOf MaximumValueChangedCallback))

    Private Shared Sub MaximumValueChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim _this As TextBox = TryCast(d, TextBox)

        Dim vt As ValueTypes = GetValueType(_this)
        Dim max As Double = GetMaximumValue(_this)
        If Not max.Equals(Double.NaN) Then
            Select Case vt
                Case ValueTypes.[Integer]
                    If max > Convert.ToDouble(Int32.MaxValue) Then
                        Throw New ArgumentOutOfRangeException("Overflow, maximum: " + Int32.MaxValue.ToString())
                    End If
                    'SetMinimumValue(_this, Convert.ToDouble(Int32.MinValue));
                    Exit Select

                Case ValueTypes.[Double]
                    'We stop two characters ahead, so as not to cause an exception.
                    If max > ([Double].MinValue / 100) Then
                        Throw New ArgumentOutOfRangeException("Overflow, maximum: " + _
                                                              ([Double].MaxValue / 100).ToString())
                    End If
                    'SetMinimumValue(_this, (Double.MinValue / 100));
                    Exit Select
            End Select
        End If

        ValidateTextBox(_this)
    End Sub
#End Region

#Region "Mask Property"

    Public Shared Function GetMask(obj As DependencyObject) As String
        Return DirectCast(obj.GetValue(MaskProperty), String)
    End Function

    Public Shared Sub SetMask(obj As DependencyObject, value As String)
        obj.SetValue(MaskProperty, value)
    End Sub

    Public Shared ReadOnly MaskProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("Mask", _
                                            GetType(String), _
                                            GetType(TextBoxMaskBehavior), _
                                            New FrameworkPropertyMetadata(AddressOf MaskChangedCallback))

    Private Shared Sub MaskChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If TypeOf e.OldValue Is TextBox Then

            RemoveHandler TryCast(e.OldValue, TextBox).PreviewTextInput, AddressOf TextBox_PreviewTextInput
            RemoveHandler TryCast(e.OldValue, TextBox).TextChanged, AddressOf TextBox_TextChanged
            RemoveHandler TryCast(e.OldValue, TextBox).PreviewKeyDown, AddressOf TextBox_PreviewKeyDown
            RemoveHandler TryCast(e.OldValue, TextBox).GotKeyboardFocus, AddressOf _TextBox_GotKeyboardFocus

            DataObject.RemovePastingHandler(TryCast(e.OldValue, TextBox), _
                                            DirectCast(AddressOf TextBoxPastingEventHandler,  _
                                                       DataObjectPastingEventHandler))
        End If

        Dim _this As TextBox = TryCast(d, TextBox)
        If _this Is Nothing Then
            Return
        End If

        If String.Empty <> DirectCast(e.NewValue, String) Then
            AddHandler _this.PreviewTextInput, AddressOf TextBox_PreviewTextInput
            AddHandler _this.TextChanged, AddressOf TextBox_TextChanged
            AddHandler _this.PreviewKeyDown, AddressOf TextBox_PreviewKeyDown
            AddHandler _this.GotKeyboardFocus, AddressOf _TextBox_GotKeyboardFocus
            DataObject.AddPastingHandler(_this, DirectCast(AddressOf TextBoxPastingEventHandler,  _
                                                           DataObjectPastingEventHandler))
        End If

        ValidateTextBox(_this)
    End Sub

#End Region

#Region "ValueType Property"

    Public Shared Function GetValueType(obj As DependencyObject) As ValueTypes
        Return DirectCast(obj.GetValue(ValueTypeProperty), ValueTypes)
    End Function

    Public Shared Sub SetValueType(obj As DependencyObject, value As ValueTypes)
        obj.SetValue(ValueTypeProperty, value)
    End Sub

    Public Shared ReadOnly ValueTypeProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("ValueType", _
                                            GetType(ValueTypes), _
                                            GetType(TextBoxMaskBehavior), _
                                            New FrameworkPropertyMetadata(AddressOf ValueTypeChangedCallback))

    Private Shared Sub ValueTypeChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim _this As TextBox = TryCast(d, TextBox)
        ValidateTextBox(_this)
    End Sub
#End Region

#Region "Static Methods"

    Private Shared Sub _TextBox_GotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        'A tizedesvessző baloldalára áll kezdéskor
        If _this.Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
            _this.CaretIndex = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
        Else
            _this.CaretIndex = _this.Text.Length
        End If
    End Sub

    Private Shared Sub TextBox_PreviewKeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs)
        If (ValueTypes.NoNumeric <> GetValueType(TryCast(sender, TextBox))) AndAlso (Key.Space = e.Key) Then
            'Space is not allowed at number type entry.
            e.Handled = True
            Return
        End If

        If Key.Back = e.Key Then
            'Backspace
            Dim _this As TextBox = TryCast(sender, TextBox)
            If (0 = _this.SelectionLength) AndAlso (0 < _this.CaretIndex) Then
                'If nothing is selected, the cursor is not at the very beginning.
                If NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = _this.Text.Substring(_this.CaretIndex - 1, 1) Then
                    'This does not have to be carried out if we want to delete the separator
                    _this.CaretIndex -= 1
                    e.Handled = True
                    Return
                End If

                If (True = _this.Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) _
                    AndAlso (_this.CaretIndex > _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) + 1) Then
                    'If the cursor is at the decimal value and we delete backwards.
                    Dim caret As Integer = _this.CaretIndex
                    _this.Text = _this.Text.Substring(0, _this.CaretIndex - 1) + _this.Text.Substring(_this.CaretIndex) + "0"
                    _this.CaretIndex = caret - 1
                    e.Handled = True
                    Return
                End If
            End If

            If 0 < _this.CaretIndex Then
                If 0 < _this.SelectionLength Then
                    'If we delete the highlighted part of text.
                    Dim caret As Integer = _this.SelectionStart
                    Dim rcaret As Integer = _this.Text.Length - caret - _this.SelectionLength

                    Dim txtWS As String = _this.Text.Substring(0, caret)
                    Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, _
                                                         [String].Empty)

                    Dim txtSWS As String = _this.Text.Substring(caret, _this.SelectionLength)
                    Dim txtSWOS As String = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, _
                                                           [String].Empty)

                    Dim text As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, _
                                                            [String].Empty)

                    'If the highlighted part contains the decimal separator, we put it back after deleting.
                    text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) + _
                        (If(txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), _
                        NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, _
                        [String].Empty)) + _
                            text.Substring(caret - (txtWS.Length - txtWOS.Length) + _
                                           _this.SelectionLength - (txtSWS.Length - txtSWOS.Length))

                    _this.Text = text

                    If txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                        'If the decimal separator was also selected, then the cursor is put in front 
                        'of the decimal separator.
                        caret = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Else
                        caret = _this.Text.Length - rcaret
                    End If

                    If caret < 0 Then
                        caret = 0
                    End If

                    _this.CaretIndex = caret
                    e.Handled = True
                    Return
                Else
                    'One item is deleted from the left.
                    Dim caret As Integer = _this.CaretIndex
                    Dim rcaret As Integer = _this.Text.Length - caret

                    Dim txtWS As String = _this.Text.Substring(0, caret)
                    Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, _
                                                         [String].Empty)

                    Dim text As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, _
                                                            [String].Empty)

                    text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length) - 1) + _
                        text.Substring(caret - (txtWS.Length - txtWOS.Length))

                    _this.Text = text

                    caret = _this.Text.Length - rcaret

                    If caret < 0 Then
                        caret = 0
                    End If

                    _this.CaretIndex = caret
                    e.Handled = True

                    Return
                End If

            End If
        End If

        If Key.Delete = e.Key Then
            'Del
            Dim _this As TextBox = TryCast(sender, TextBox)
            If (0 = _this.SelectionLength) AndAlso (_this.CaretIndex < _this.Text.Length) Then
                'If nothing is selected, the cursor is not at the very end.
                If NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = _this.Text.Substring(_this.CaretIndex, 1) Then
                    'This does not have to be carried out if we want to delete the separator
                    _this.CaretIndex += 1
                    e.Handled = True
                    Return
                End If

                If (True = _this.Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) _
                    AndAlso (_this.CaretIndex > _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) Then
                    'If the cursor is at the decimal value and we delete.
                    Dim caret As Integer = _this.CaretIndex
                    Dim ind As Integer = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

                    _this.Text = _this.Text.Substring(0, caret) + _this.Text.Substring(caret + 1) + "0"
                    _this.CaretIndex = caret
                    e.Handled = True
                    Return
                End If
            End If

            If 0 < _this.SelectionLength Then
                'If we delete the highlighted part of text.
                Dim caret As Integer = _this.SelectionStart
                Dim rcaret As Integer = _this.Text.Length - caret - _this.SelectionLength

                Dim txtWS As String = _this.Text.Substring(0, caret)
                Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                Dim txtSWS As String = _this.Text.Substring(caret, _this.SelectionLength)
                Dim txtSWOS As String = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                Dim text As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                'If the highlighted part contains the decimal separator, we put it back after deleting.
                text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) + _
                    (If(txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), _
                    NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, _
                    [String].Empty)) + text.Substring(caret - (txtWS.Length - txtWOS.Length) + _this.SelectionLength - (txtSWS.Length - txtSWOS.Length))

                'If there is only one decimal separator, it will be deleted.
                text = (If(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = text, [String].Empty, text))

                _this.Text = text

                If txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                    caret = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                Else
                    caret = _this.Text.Length - rcaret
                End If

                If caret < 0 Then
                    caret = 0
                End If

                _this.CaretIndex = caret
                e.Handled = True
                Return
            Else
                If _this.CaretIndex < _this.Text.Length Then

                    'One item is deleted from the right.
                    Dim caret As Integer = _this.CaretIndex
                    Dim rcaret As Integer = _this.Text.Length - caret - 1

                    If NumberFormatInfo.CurrentInfo.NumberGroupSeparator = _this.Text.Substring(caret, 1) Then
                        rcaret -= 1
                    End If

                    Dim txtWS As String = _this.Text.Substring(0, caret)
                    Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    Dim text As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) + text.Substring(caret - (txtWS.Length - txtWOS.Length) + 1)

                    _this.Text = text
                    caret = _this.Text.Length - rcaret

                    If caret < 0 Then
                        caret = 0
                    End If

                    _this.CaretIndex = caret
                    e.Handled = True
                    Return

                End If


            End If
        End If

        e.Handled = False
    End Sub

#End Region

#Region "Private Static Methods"

    Private Shared Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)

        Dim text As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

        Dim mask As String = GetMask(_this)
        Dim vt As ValueTypes = GetValueType(_this)

        If 0 <> mask.Length Then
            If 0 < _this.Text.Length Then
                If vt.Equals(ValueTypes.[Integer]) Then
                    'todo: TryParse/try-catch
                    _this.Text = [String].Format("{" + mask + "}", Int32.Parse(text))
                    e.Handled = True
                Else
                    _this.Text = [String].Format("{" + mask + "}", [Double].Parse(text))
                    e.Handled = True
                End If
            Else
                _this.Text = "0"
                e.Handled = True

            End If
        End If

    End Sub

    Private Shared Sub ValidateTextBox(_this As TextBox)
        If String.Empty <> GetMask(_this) Then
            _this.Text = ValidateValue(GetMask(_this), GetValueType(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this))
        End If
    End Sub

    Private Shared Sub TextBoxPastingEventHandler(sender As Object, e As DataObjectPastingEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        Dim clipboard As String = TryCast(e.DataObject.GetData(GetType(String)), String)
        clipboard = ValidateValue(GetMask(_this), GetValueType(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this))
        If Not String.IsNullOrEmpty(clipboard) Then
            _this.Text = clipboard
        End If
        e.CancelCommand()
        e.Handled = True
    End Sub

    Private Shared Sub TextBox_PreviewTextInput(sender As Object, e As System.Windows.Input.TextCompositionEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        Dim isValid As Boolean = IsSymbolValid(GetMask(_this), e.Text, GetValueType(_this))
        Dim textInserted As Boolean = False
        Dim toNDS As Boolean = False

        If isValid Then
            'Current content
            Dim txtOld As String = _this.Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)
            'New content
            Dim txtNew As String = [String].Empty
            Dim handled As Boolean = False
            Dim caret As Integer = _this.CaretIndex
            Dim rcaret As Integer = 0

            If e.Text = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator Then
                'If we entered a decimal separator.
                Dim ind As Integer = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) + 1
                rcaret = _this.Text.Length - ind
                'The text doesn't change.
                txtNew = txtOld
                handled = True
            End If

            If (Not handled) AndAlso (e.Text = NumberFormatInfo.CurrentInfo.NegativeSign) Then
                'We entered a negative symbol.
                If _this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign) Then
                    'A negative symbol is already in the text.
                    'As overriding the text initializes the cursor, the present position is remembered.
                    rcaret = _this.Text.Length - caret
                    txtNew = txtOld.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, String.Empty)
                Else
                    'There is no negative symbol in the text.
                    'As overriding the text initializes the cursor, the present position is remembered.
                    rcaret = _this.Text.Length - caret
                    txtNew = NumberFormatInfo.CurrentInfo.NegativeSign + txtOld
                End If
                handled = True
            End If

            If Not handled Then
                textInserted = True
                If 0 < _this.SelectionLength Then
                    'We delete the highlighted text and insert what we have just written.
                    Dim ind As Integer = _this.SelectionStart
                    rcaret = _this.Text.Length - ind - _this.SelectionLength

                    Dim txtWS As String = _this.Text.Substring(0, ind)
                    Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    Dim txtSWS As String = _this.Text.Substring(ind, _this.SelectionLength)
                    Dim txtSWOS As String = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    Dim txtNWS As String = e.Text
                    Dim txtNWOS As String = txtNWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    txtNew = txtOld.Substring(0, ind - (txtWS.Length - txtWOS.Length)) + txtNWOS + _
                        (If(txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), _
                        NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, _
                        [String].Empty)) + txtOld.Substring(ind - (txtWS.Length - txtWOS.Length) + _this.SelectionLength - (txtSWS.Length - txtSWOS.Length))

                    If txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                        'If the decimal separator was also highlighted, then the cursor is put in front of the decimal separator.
                        toNDS = True

                    End If
                Else
                    'We insert the character to the right of the cursor.
                    Dim ind As Integer = _this.CaretIndex
                    rcaret = _this.Text.Length - ind

                    If (0 < rcaret) AndAlso (NumberFormatInfo.CurrentInfo.NumberGroupSeparator = _this.Text.Substring(ind, 1)) Then
                        rcaret -= 1
                    End If

                    Dim txtWS As String = _this.Text.Substring(0, ind)
                    Dim txtWOS As String = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    Dim txtNWS As String = e.Text
                    Dim txtNWOS As String = txtNWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)

                    txtNew = txtOld.Substring(0, ind - (txtWS.Length - txtWOS.Length)) + txtNWOS + txtOld.Substring(ind - (txtWS.Length - txtWOS.Length))
                End If
            End If

            Try
                Dim val As Double = [Double].Parse(txtNew)
                Dim newVal As Double = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val, GetValueType(_this))
                If val <> newVal Then
                    txtNew = newVal.ToString()
                ElseIf val = 0 Then
                    If Not txtNew.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                        txtNew = "0"
                    End If
                End If
            Catch
                txtNew = "0"
            End Try

            _this.Text = txtNew

            If (True = _this.Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) _
                AndAlso (caret > _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) Then
                'If the cursor is at the decimal value, then it moves to the right of the decimal separator, if possible.
                If caret < _this.Text.Length Then
                    If textInserted Then
                        caret += 1
                        rcaret = _this.Text.Length - caret
                    End If
                Else
                    'We are at the very end; it's not possible to enter more characters.
                    If textInserted Then
                        _this.Text = txtOld
                    End If
                End If
            End If

            caret = _this.Text.Length - rcaret

            If caret < 0 Then
                caret = 0
            End If

            If toNDS Then
                _this.CaretIndex = _this.Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
            Else
                _this.CaretIndex = caret

            End If
        End If

        e.Handled = True
    End Sub

    Private Shared Function ValidateValue(mask As String, vt As ValueTypes, value As String, min As Double, max As Double) As String
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If

        value = value.Trim()

        Select Case vt
            Case ValueTypes.[Integer]
                Try
                    value = ValidateLimits(min, max, Int32.Parse(value), vt).ToString()
                    Return value
                Catch
                End Try
                Return String.Empty

            Case ValueTypes.[Double]
                Try
                    value = ValidateLimits(min, max, [Double].Parse(value), vt).ToString()
                    Return value
                Catch
                End Try
                Return String.Empty
        End Select
        Return String.Empty

    End Function

    Private Shared Function ValidateLimits(min As Double, max As Double, value As Double, vt As ValueTypes) As Double
        If Not min.Equals(Double.NaN) Then
            If value < min Then
                Return min
            End If
        Else
            Select Case vt
                Case ValueTypes.[Integer]
                    If value < Convert.ToDouble(Int32.MinValue) Then
                        Return Convert.ToDouble(Int32.MinValue)
                    End If
                    Exit Select

                Case ValueTypes.[Double]
                    'Két karakterrel előbb megállunk, hogy ne okozzon exception-t.
                    If value < ([Double].MinValue / 100) Then
                        Return ([Double].MinValue / 100)
                    End If
                    Exit Select
            End Select
        End If

        If Not max.Equals(Double.NaN) Then
            If value > max Then
                Return max
            End If
        Else
            Select Case vt
                Case ValueTypes.[Integer]
                    If value > Convert.ToDouble(Int32.MaxValue) Then
                        Return Convert.ToDouble(Int32.MaxValue)
                    End If
                    Exit Select

                Case ValueTypes.[Double]
                    'We stop two characters ahead, so as not to cause an exception.
                    If value > ([Double].MaxValue / 100) Then
                        Return ([Double].MaxValue / 100)
                    End If
                    Exit Select
            End Select
        End If
        Return value
    End Function

    Private Shared Function IsSymbolValid(mask As String, str As String, typ As ValueTypes) As Boolean
        Select Case typ
            Case ValueTypes.NoNumeric
                Return True
            Case ValueTypes.[Integer]
                If str = NumberFormatInfo.CurrentInfo.NegativeSign Then
                    Return True
                End If
                Exit Select
            Case ValueTypes.[Double]
                If str = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator OrElse str = NumberFormatInfo.CurrentInfo.NegativeSign Then
                    Return True
                End If
                Exit Select
        End Select

        If typ.Equals(ValueTypes.[Integer]) OrElse typ.Equals(ValueTypes.[Double]) Then
            For Each ch As Char In str
                If Not [Char].IsDigit(ch) Then
                    Return False
                End If
            Next

            Return True
        End If

        Return False

    End Function

#End Region

End Class

Public Enum ValueTypes
    NoNumeric
    [Integer]
    [Double]
End Enum

'End Namespace

