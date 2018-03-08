Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class ViewModelCategoria
    Implements ICommand, INotifyPropertyChanged
#Region "Atributos"
    Private _viewModelCategoria As ViewModelCategoria
    Private _observableCategoria As New ObservableCollection(Of Categoria)
    Private _iqueryableCategoria As IQueryable(Of Categoria)
    Private _idCategoriaView As Integer
    Private _nombreCategoriaView As String
#End Region
#Region "Propiedades"
    Public Property MeViewModelCategoria As ViewModelCategoria
        Get
            Return _viewModelCategoria
        End Get
        Set(value As ViewModelCategoria)
            _viewModelCategoria = value
        End Set
    End Property
    Public ReadOnly Property ObservableCategoria As ObservableCollection(Of Categoria)
        Get
            CargarDatos()
            Return _observableCategoria
            NotificarCambio("ObservableCategoria")
        End Get
    End Property
    Public Property IdCategoriaView As Integer
        Get
            Return _idCategoriaView
        End Get
        Set(value As Integer)
            value = _idCategoriaView
            NotificarCambio("IdCategoriaView")
        End Set
    End Property
    Public Property NombreCategoriaView As String
        Get
            Return _nombreCategoriaView
        End Get
        Set(value As String)
            _nombreCategoriaView = value
            NotificarCambio("NombreCategoriaView")
        End Set
    End Property
#End Region
#Region "ICommand"
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        Select Case parameter.ToString
            Case "Agregar"
                AgregarCategoria()
        End Select
    End Sub
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
#End Region
#Region "Notificar Cambio"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotificarCambio(ByVal propiedad As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propiedad))
    End Sub
#End Region
#Region "Constructor"
    Public Sub New()
        MeViewModelCategoria = Me
    End Sub
#End Region
#Region "Cargar Datos"
    Public Sub CargarDatos()
        _observableCategoria.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableCategoria = (From _c In _db.Categoria Select _c)
        For Each _categoria In _iqueryableCategoria
            _observableCategoria.Add(_categoria)
        Next
    End Sub
#End Region
#Region "Agregar"
    Public Sub AgregarCategoria()
        Console.WriteLine("Soy Agregar Categoria :D")

        Dim _db As New RegistrosEntities
        Dim _categoria As New Categoria

        _categoria.nombreCategoria = NombreCategoriaView

        If (_categoria.nombreCategoria = "") Then
            MsgBox(Title:="Agregar", Prompt:="Error al agregar, no ha ingresado datos")
        Else
            _db.Categoria.Add(_categoria)
            _db.SaveChanges()
            MsgBox(Title:="Agregar", Prompt:="¡Tienes un nueva categoria")
            NotificarCambio("ObservableProducto")
        End If
    End Sub
#End Region
End Class
