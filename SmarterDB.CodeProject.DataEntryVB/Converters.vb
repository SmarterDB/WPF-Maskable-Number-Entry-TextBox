Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows.Data
Imports System.Globalization

'Namespace SmarterDB.CodeProject.DataEntryVB
Public Class NumericConverter
    Implements IValueConverter
    Public Function Convert(ByVal value As Object, _
                            ByVal targetType As System.Type, _
                            ByVal parameter As Object, _
                            ByVal culture As System.Globalization.CultureInfo) As Object _
        Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing Then
            Return Nothing
        ElseIf targetType.UnderlyingSystemType = GetType([String]) Then
            Return value.ToString()
        Else
            Return value
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, _
                                ByVal targetType As System.Type, _
                                ByVal parameter As Object, _
                                ByVal culture As System.Globalization.CultureInfo) As Object _
        Implements System.Windows.Data.IValueConverter.ConvertBack
        'We create numbers from formatted text.
        If value Is Nothing Then
            Return Nothing
        Else
            Dim numericTypes As New HashSet(Of Type)({GetType([Byte]), _
                                                      GetType([Decimal]), _
                                                      GetType([Double]), _
                                                      GetType(Int16), _
                                                      GetType(Int32), _
                                                      GetType(Int64), _
                                                      GetType([SByte]), _
                                                      GetType([Single]), _
                                                      GetType(UInt16), _
                                                      GetType(UInt32), _
                                                      GetType(UInt64)})

            If numericTypes.Contains(targetType.UnderlyingSystemType) Then
                Dim intTypes As New HashSet(Of Type)({GetType([Byte]), _
                                                      GetType(Int16), _
                                                      GetType(Int32), _
                                                      GetType(Int64), _
                                                      GetType([SByte]), _
                                                      GetType(UInt16), _
                                                      GetType(UInt32), _
                                                      GetType(UInt64)})
                Dim text As String = value.ToString().Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, [String].Empty)
                If intTypes.Contains(targetType.UnderlyingSystemType) Then
                    Try
                        Return Int32.Parse(text)
                    Catch
                        Return Nothing
                    End Try
                Else
                    Try
                        Return [Double].Parse(text)
                    Catch
                        Return Nothing
                    End Try
                End If
            Else
                Return value
            End If
        End If
    End Function
End Class

Public Class NumericToValueTypeConverter
    Implements IValueConverter
    Public Function Convert(ByVal value As Object, _
                            ByVal targetType As System.Type, _
                            ByVal parameter As Object, _
                            ByVal culture As System.Globalization.CultureInfo) As Object _
        Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing Then
            Return ValueTypes.NoNumeric
        Else
            Dim numericTypes As New HashSet(Of Type)({GetType([Byte]), _
                                                      GetType([Decimal]), _
                                                      GetType([Double]), _
                                                      GetType(Int16), _
                                                      GetType(Int32), _
                                                      GetType(Int64), _
                                                      GetType([SByte]), _
                                                      GetType([Single]), _
                                                      GetType(UInt16), _
                                                      GetType(UInt32), _
                                                      GetType(UInt64)})

            If numericTypes.Contains(value.[GetType]()) Then
                Dim intTypes As New HashSet(Of Type)({GetType([Byte]), _
                                                      GetType(Int16), _
                                                      GetType(Int32), _
                                                      GetType(Int64), _
                                                      GetType([SByte]), _
                                                      GetType(UInt16), _
                                                      GetType(UInt32), _
                                                      GetType(UInt64)})
                If intTypes.Contains(value.[GetType]()) Then
                    Return ValueTypes.[Integer]
                Else
                    Return ValueTypes.[Double]
                End If
            Else
                Return ValueTypes.NoNumeric
            End If
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, _
                                ByVal targetType As System.Type, _
                                ByVal parameter As Object, _
                                ByVal culture As System.Globalization.CultureInfo) As Object _
        Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class

'End Namespace
