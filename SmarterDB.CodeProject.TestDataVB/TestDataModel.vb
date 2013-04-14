Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.ComponentModel

'Namespace SmarterDB.CodeProject.TestDataVB
Public Class TestDataModel
    Implements INotifyPropertyChanged
#Region "Public Properties"

    Private _TestDataInt As Integer = 15
    Public Property TestDataInt() As Integer
        Get
            Return _TestDataInt
        End Get
        Set(value As Integer)
            If _TestDataInt <> value Then
                _TestDataInt = value
                RaisePropertyChanged("TestDataInt")
            End If
        End Set
    End Property

    Private _TestDataDouble As Double = 25
    Public Property TestDataDouble() As Double
        Get
            Return _TestDataDouble
        End Get
        Set(value As Double)
            If _TestDataDouble <> value Then
                _TestDataDouble = value
                RaisePropertyChanged("TestDataDouble")
            End If
        End Set
    End Property

    Private _TestDataNoNumeric As String = ""
    Public Property TestDataNoNumeric() As String
        Get
            Return _TestDataNoNumeric
        End Get
        Set(value As String)
            If _TestDataNoNumeric <> value Then
                _TestDataNoNumeric = value
                RaisePropertyChanged("TestDataNoNumeric")
            End If
        End Set
    End Property

#End Region

#Region "PropertyChanged"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub RaisePropertyChanged(PropertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(PropertyName))

    End Sub

#End Region

End Class
'End Namespace

