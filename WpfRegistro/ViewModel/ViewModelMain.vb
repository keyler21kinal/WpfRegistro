Imports System.ComponentModel

Public Class ViewModelMain
    Implements ICommand, INotifyPropertyChanged
#Region "Atributos"
    Private _viewModelMain As ViewModelMain
#End Region
#Region "Propiedades"
    Public Property MeViewModelMain As ViewModelMain
        Get
            Return _viewModelMain
        End Get
        Set(value As ViewModelMain)
            _viewModelMain = value
        End Set
    End Property
#End Region
#Region "Constructor"
    Public Sub New()
        MeViewModelMain = Me
    End Sub
#End Region
#Region "ICommand"
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        Select Case parameter.ToString
            Case "Producto"
                Producto()
            Case "Listado"
                Listado()
            Case "Entradas"
                Entradas()
            Case "Salidas"
                Salidas()
            Case "ListadoSalidas"
                ListadoSalidas()
            Case "Historial"
                Historial()
        End Select
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
#End Region
#Region "Notificar Cambio"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotificarCamibio(ByVal propiedad As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propiedad))
    End Sub
#End Region
#Region "Ventanas"
    Public Sub Historial()
        Dim _windowHistorial As New WindowHistorial
        _windowHistorial.Show()
    End Sub
    Public Sub Producto()
        Dim _windowProducto As New WindowProducto
        Dim _prompt As String = "Instrucciones: "
        Dim _instruccion1 As String = "Si registraras un nuevo producto, asegurate de que tengas la categoria necesaria, si no ¡Registra una categoria!"
        _windowProducto.Show()
        MsgBox(Title:="Producto", Prompt:=_instruccion1)

    End Sub
    Public Sub Listado()
        Dim _productos As New WindowProductos
        _productos.Show()
    End Sub
    Public Sub Entradas()
        Dim _entrada As New WindowEntrada
        _entrada.Show()
    End Sub
    Public Sub Salidas()
        Dim _salida As New WindowSalida
        _salida.Show()
    End Sub
    Public Sub ListadoSalidas()
        Dim _salidas As New WindowSalidas
        _salidas.Show()
    End Sub
#End Region
End Class
