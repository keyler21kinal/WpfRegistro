Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Microsoft.Office.Interop
Imports Microsoft.Win32

Public Class ViewModelProducto
    Implements ICommand, INotifyPropertyChanged

#Region "Atributos"
    Private _viewModelProducto As ViewModelProducto
    Private _viewModelCategoria As ViewModelCategoria
    Private _observableCategoria As New ObservableCollection(Of Categoria)
    Private _observableHistorial As New ObservableCollection(Of hv)
    Private _observableProducto As New ObservableCollection(Of p_Producto)
    Private _iqueryableProducto As IQueryable(Of p_Producto)
    Private _iqueryableHistorial As IQueryable(Of hv)
    Private _iqueryableCategoria As IQueryable(Of Categoria)
    Private _idProductoView As Integer
    Private _codigoView As String
    Private _categoriaView As New Categoria
    Private _descripcionView As String
    Private _precioView As Double
    Private _stockView As Integer
#End Region
#Region "Propiedades"
    Public Property MeViewModelProducto As ViewModelProducto
        Get
            Return _viewModelProducto
        End Get
        Set(value As ViewModelProducto)
            _viewModelProducto = value
        End Set
    End Property
    Public Property MeViewModelCategoria As ViewModelCategoria
        Get
            Return _viewModelCategoria
        End Get
        Set(value As ViewModelCategoria)
            _viewModelCategoria = value
        End Set
    End Property
    Public ReadOnly Property ObservableProducto As ObservableCollection(Of p_Producto)
        Get
            CargarDatos()
            Return _observableProducto
        End Get
    End Property
    Public ReadOnly Property ObservableHistorial As ObservableCollection(Of hv)
        Get
            CargarHistorial()
            Return _observableHistorial
        End Get
    End Property
    Public ReadOnly Property ObservableCategoria As ObservableCollection(Of Categoria)
        Get
            LoadCategorias()
            Return _observableCategoria
            NotificarCambio("ObservableCategoria")
        End Get
    End Property
    Public Property IdProductoView As Integer
        Get
            Return _idProductoView
        End Get
        Set(value As Integer)
            _idProductoView = value
            NotificarCambio("IdProductoView")
        End Set
    End Property
    Public Property CodigoView As String
        Get
            Return _codigoView
        End Get
        Set(value As String)
            _codigoView = value
            NotificarCambio("CodigoView")
        End Set
    End Property
    Public Property CategoriaView As Categoria
        Get
            Return _categoriaView
        End Get
        Set(value As Categoria)
            _categoriaView = value
            NotificarCambio("CategoriaView")
        End Set
    End Property
    Public Property DescripcionView As String
        Get
            Return _descripcionView
        End Get
        Set(value As String)
            _descripcionView = value
            NotificarCambio("DescripcionView")
        End Set
    End Property
    Public Property PrecioView As Double
        Get
            Return _precioView
        End Get
        Set(value As Double)
            _precioView = value
            NotificarCambio("PrecioView")
        End Set
    End Property
    Public Property StockView As Integer
        Get
            Return _stockView
        End Get
        Set(value As Integer)
            _stockView = value
            NotificarCambio("StockView")
        End Set
    End Property
#End Region
#Region "Cargar Datos"
    Public Sub CargarDatos()
        _observableProducto.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableProducto = (From _p In _db.p_Producto Select _p)
        For Each _producto In _iqueryableProducto
            _observableProducto.Add(_producto)
        Next
    End Sub
    Public Sub LoadCategorias()
        _observableCategoria.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableCategoria = (From _c In _db.Categoria Select _c)
        For Each _categoria In _iqueryableCategoria
            _observableCategoria.Add(_categoria)
        Next
    End Sub
    Public Sub CargarHistorial()
        ' Select Case h.fecha, p.codigo, h.stockNuevo FROM Historial h INNER Join Producto p On h.idProducto = p.idProducto 

        _observableHistorial.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableHistorial = (From h In _db.hv Select h)
        For Each _historial In _iqueryableHistorial
            _observableHistorial.Add(_historial)
        Next
    End Sub
#End Region
#Region "Constructor"
    Public Sub New()
        MeViewModelProducto = Me
        Me.MeViewModelCategoria = New ViewModelCategoria
    End Sub
#End Region
#Region "Agregar, Editar, Eliminar, Reporte"
    Public Sub AgregarProducto()
        Console.WriteLine("Soy Agregar Producto :D")

        Dim _db As New RegistrosEntities
        Dim _producto As New Producto

        _producto.codigo = CodigoView
        _producto.idCategoria = CategoriaView.idCategoria.ToString
        _producto.descripcion = DescripcionView
        _producto.precio = PrecioView
        _producto.stock = StockView

        If ((_producto.codigo = "") Or (_producto.idCategoria = 0) Or (_producto.descripcion = "") Or (_producto.precio = 0) Or (_producto.stock = 0)) Then
            MsgBox(Title:="Error al agregar", Prompt:="Por favor ingrese todos los datos y verifique si son correctos")
        Else
            _db.Producto.Add(_producto)
            _db.SaveChanges()
            MsgBox(Title:="Agregar", Prompt:="¡Tienes un nuevo Producto!")
            NotificarCambio("ObservableProducto")
        End If
    End Sub
    Public Sub EditarProducto()
        Dim _db As New RegistrosEntities
        Dim _product = (From _pr In _db.Producto
                        Where _pr.idProducto = IdProductoView
                        Select _pr)
        For Each _produ As Producto In _product
            _produ.codigo = CodigoView
            _produ.idCategoria = CategoriaView.idCategoria.ToString
            _produ.descripcion = DescripcionView
            _produ.stock = StockView
            _produ.precio = PrecioView
        Next
        _db.SaveChanges()
        NotificarCambio("ObservableProducto")
    End Sub
    Public Sub Eliminar()
        Dim _db As New RegistrosEntities
        Dim _producto = (From _p In _db.Producto Where _p.idProducto = IdProductoView Select _p)
        For Each _produ As Producto In _producto
            If Not IsNothing(_produ) Then
                _db.Producto.Remove(_produ)
            End If
        Next
        _db.SaveChanges()
        NotificarCambio("ObservableProducto")
    End Sub
    Public Sub Reporte()
        Dim _db As New RegistrosEntities

        Dim _saveFileDialog As New SaveFileDialog
        _saveFileDialog.Filter = "Excel File|*xlsx"
        _saveFileDialog.Title = "Guardar Archivo Excel"
        _saveFileDialog.ShowDialog()

        If _saveFileDialog.FileName <> "" Then
            Dim _Excel As Excel.Application
            Dim _LibroExcel As Excel.Workbook
            Dim _HojaExcel As Excel.Worksheet
            'INICIALIZACION

            _Excel = New Excel.Application
            _LibroExcel = _Excel.Workbooks.Add
            _HojaExcel = _LibroExcel.Worksheets(1)

            _HojaExcel.Range("A1:D1").Merge()
            _HojaExcel.Range("A1:D1").Value = "Reporte Salidas"
            _HojaExcel.Range("A1:D1").Font.Bold = True
            _HojaExcel.Range("A1:D1").Font.FontStyle = "AR ESSENCE"
            _HojaExcel.Range("A1:D1").Font.Size = 24

            _HojaExcel.Range("A3:B3").Merge()
            _HojaExcel.Range("A3:B3").Value = "Codigo Producto"
            _HojaExcel.Range("A3:B3").Font.Size = 11
            _HojaExcel.Range("A3:B3").Font.Bold = True

            _HojaExcel.Range("C3:E3").Merge()
            _HojaExcel.Range("C3:E3").Value = "Nombre"
            _HojaExcel.Range("C3:E3").Font.Size = 11
            _HojaExcel.Range("C3:E3").Font.Bold = True

            _HojaExcel.Range("F3:I3").Merge()
            _HojaExcel.Range("F3:I3").Value = "Descripcion"
            _HojaExcel.Range("F3:I3").Font.Size = 11
            _HojaExcel.Range("F3:I3").Font.Bold = True

            _HojaExcel.Range("J3:L3").Merge()
            _HojaExcel.Range("J3:L3").Value = "Precio"
            _HojaExcel.Range("J3:L3").Font.Size = 11
            _HojaExcel.Range("J3:L3").Font.Bold = True

            _HojaExcel.Range("M3").Merge()
            _HojaExcel.Range("M3").Value = "Stock"
            _HojaExcel.Range("M3").Font.Size = 11
            _HojaExcel.Range("M3").Font.Bold = True

            _HojaExcel.Range("N3").Merge()
            _HojaExcel.Range("N3").Value = "Totales"
            _HojaExcel.Range("N3").Font.Size = 11
            _HojaExcel.Range("N3").Font.Bold = True


            Dim _fila As Integer = 1
            Dim _cantRegistros = _observableProducto.Count
            Dim _dataContext(0 To _cantRegistros, 0 To 15) As Object

            For Each _producto In _observableProducto
                _dataContext(_fila, 0) = _producto.codigo
                _dataContext(_fila, 1) = _producto.nombreCategoria
                _dataContext(_fila, 5) = _producto.descripcion
                _dataContext(_fila, 9) = _producto.precio
                _dataContext(_fila, 12) = _producto.stock
                _dataContext(_fila, 13) = _producto.totales
                _fila += 1
            Next
            _HojaExcel.Range("A4").Resize(_fila, 15).Value = _dataContext

            _LibroExcel.SaveAs(_saveFileDialog.FileName)
            _Excel.Quit()
            _Excel = Nothing
            MsgBox(Title:="Reporte", Prompt:="El reporte fue generado exitosamente")
        End If
    End Sub

#End Region
#Region "ICommand"
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        Select Case parameter.ToString
            Case "AgregarProducto"
                AgregarProducto()
            Case "Categoria"
                Categoria()
            Case "Editar"
                EditarProducto()
            Case "Eliminar"
                Eliminar()
            Case "Reporte"
                Reporte()
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
#Region "Ventana"
    Public Sub Categoria()
        Dim _categoria As New WindowCategoria
        _categoria.Show()
    End Sub
#End Region
End Class
