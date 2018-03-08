Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Xml.Linq
Imports Microsoft.Office.Interop
Imports Microsoft.Win32

Public Class ViewModelSalida
    Implements ICommand, INotifyPropertyChanged

#Region "Atributos"
    Private _viewModelSalida As ViewModelSalida
    Private _observableSalida As New ObservableCollection(Of vs_salida)
    Private _observableProducto As New ObservableCollection(Of Producto)
    Private _observableProducto1 As New ObservableCollection(Of p_Producto)
    Private _observableCategoria As New ObservableCollection(Of Categoria)
    Private _iqueryableSalida As IQueryable(Of vs_salida)
    Private _iqueryableProducto As IQueryable(Of Producto)
    Private _iqueryableProducto1 As IQueryable(Of p_Producto)
    Private _iqueryableCategoria As IQueryable(Of Categoria)
    Private _idSalidaView As Integer
    Private _fechaView As Date = DateAndTime.Now
    Private _productoView As New Producto
    Private _categoriaView As New Categoria
    Private _cantidadSalidaView As Integer
    Private _total As Double
    Private _totalStock As Integer
    Private _totales As Double
    Private _quetzales As Double
#End Region
#Region "Propiedades"
    Public Property MeViewModelSalida As ViewModelSalida
        Get
            Return _viewModelSalida
        End Get
        Set(value As ViewModelSalida)
            _viewModelSalida = value
        End Set
    End Property
    Public ReadOnly Property ObservableSalida As ObservableCollection(Of vs_salida)
        Get
            CargarDatos()
            Return _observableSalida
        End Get
    End Property
    Public ReadOnly Property ObservableProducto As ObservableCollection(Of Producto)
        Get
            LoadProducto()
            Return _observableProducto
        End Get
    End Property
    Public ReadOnly Property ObservableProducto1 As ObservableCollection(Of p_Producto)
        Get
            LoadProductos()
            Return _observableProducto1
        End Get
    End Property
    Public ReadOnly Property ObservableCategoria As ObservableCollection(Of Categoria)
        Get
            LoadCategoria()
            Return _observableCategoria
        End Get
    End Property
    Public Property IdSalidaView As Integer
        Get
            Return _idSalidaView
        End Get
        Set(value As Integer)
            _idSalidaView = value
            NotificarCambio("IdSalidaView")
        End Set
    End Property
    Public Property FechaSalidaView As Date
        Get
            Return _fechaView
        End Get
        Set(value As Date)
            _fechaView = value
            NotificarCambio("FechaSalidaView")
        End Set
    End Property
    Public Property Fecha As Date
        Get
            Return _fechaView
        End Get
        Set(value As Date)
            _fechaView = value
            NotificarCambio("Fecha")
        End Set
    End Property
    Public Property ProductoView As Producto
        Get
            Return _productoView
        End Get
        Set(value As Producto)
            _productoView = value
            NotificarCambio("ProductoView")
        End Set
    End Property
    Public Property CategoriaView As Categoria
        Get
            Return _categoriaView
        End Get
        Set(value As Categoria)
            _categoriaView = value
            Dim _db As New RegistrosEntities
            _observableProducto1.Clear()
            'Sumas 
            If value.Producto.Count <= 0 Then
                MsgBox("No hay productos en esa categoria")
            Else
                Dim sumaPrecio As Double = (From _data In _db.Producto Where (_data.idCategoria = _categoriaView.idCategoria) And (_data.stock > 0) Select _data.precio).Sum()
                Dim sumaStock As Integer = (From _data In _db.Producto Where _data.Categoria.idCategoria = value.idCategoria Select _data.stock).Sum()
                Dim sumaTotales As Double = (From _data In _db.p_Producto Where _data.idCategoria = value.idCategoria Select _data.totales).Sum()

                _iqueryableProducto1 = (From _pr In _db.p_Producto Where (_pr.idCategoria = _categoriaView.idCategoria) And (_pr.stock > 0) Select _pr)
                TotalPrecio = sumaPrecio
                TotalStock = sumaStock
                Quetzales = sumaTotales

                For Each _producto In _iqueryableProducto1
                    If (_producto.stock < 5) Then
                        Dim _titulo As String = "Stock"
                        MsgBox(Title:=_producto.nombreCategoria, Prompt:="Alerta el producto, codigo No." & _producto.codigo & "  tiene en stock la cantidad de: " & _producto.stock)
                    ElseIf _producto.stock = 0 Then
                        MsgBox("No puede generar salidas en esta categoria, edite sus productos y el stock")
                    End If
                    _observableProducto1.Add(_producto)
                Next
            End If
            'Sumas 
            NotificarCambio("CategoriaView")
        End Set
    End Property
    Public Property CantidadSalidaView As Integer
        Get
            Return _cantidadSalidaView
        End Get
        Set(value As Integer)
            _cantidadSalidaView = value
            NotificarCambio("CantidadSalidaView")
        End Set
    End Property
#End Region
#Region "Constructor"
    Public Sub New()
        Me.MeViewModelSalida = Me
    End Sub
#End Region
#Region "Cargar Datos"
    Public Sub CargarDatos()
        _observableSalida.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableSalida = (From _salida In _db.vs_salida Select _salida)
        For Each _sali In _iqueryableSalida
            _observableSalida.Add(_sali)
        Next
    End Sub
    Public Sub LoadProducto()
        _observableProducto.Clear()
        Dim _db As New RegistrosEntities

        _iqueryableProducto = (From _pr In _db.Producto Where _pr.stock > 0 Select _pr)
        For Each _producto In _iqueryableProducto
            _observableProducto.Add(_producto)
        Next
    End Sub
    Public Sub LoadProductos()
        _observableProducto1.Clear()
        Dim _db As New RegistrosEntities
        _iqueryableProducto1 = (From _pr In _db.p_Producto Select _pr Where _pr.idCategoria = _categoriaView.idCategoria)
        For Each _pro In _iqueryableProducto1
            _observableProducto1.Add(_pro)
        Next
    End Sub
    Public Sub LoadCategoria()
        _observableCategoria.Clear()
        Dim _db As New RegistrosEntities

        _iqueryableCategoria = (From _c In _db.Categoria Select _c)
        For Each _categoria In _iqueryableCategoria
            _observableCategoria.Add(_categoria)
        Next
    End Sub
#End Region
#Region "ICommand"
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        Select Case parameter.ToString
            Case "Agregar"
                Agregar()
            Case "Eliminar"
                Eliminar()
        End Select
    End Sub
#End Region
#Region "Notificar Cambio"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotificarCambio(ByVal propiedad As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propiedad))
    End Sub
#End Region
#Region "Agregar"
    Public Sub Agregar()
        Dim _db As New RegistrosEntities
        Dim _salida As New Salida
        Dim _nuevoStock As Integer = 0

        _salida.fechaSalida = FechaSalidaView
        _salida.idProducto = ProductoView.idProducto
        _salida.cantidadSalida = CantidadSalidaView

        Dim _product = (From _pr In _db.Salida Where _pr.idProducto = ProductoView.idProducto Select _pr)
        For Each _produ As Salida In _product
            _nuevoStock = _productoView.stock - _salida.cantidadSalida
            _produ.Producto.stock = _nuevoStock
        Next
        _db.Salida.Add(_salida)
        _db.SaveChanges()
        MsgBox(Title:="Agregar", Prompt:="Has generado una salida")
        NotificarCambio("ObservableProducto1")
    End Sub
    Public Sub Eliminar()
        Dim _db As New RegistrosEntities
        Dim _salida = (From _p In _db.Salida Where _p.idSalida = IdSalidaView Select _p)
        For Each _produ As Salida In _salida
            If Not IsNothing(_produ) Then
                _db.Salida.Remove(_produ)
            End If
        Next
        _db.SaveChanges()
        NotificarCambio("ObservableSalida")
    End Sub
    'Public Sub Reporte()
    '    Dim _db As New RegistrosEntities

    '    Dim _saveFileDialog As New SaveFileDialog
    '    _saveFileDialog.Filter = "Excel File|*xlsx"
    '    _saveFileDialog.Title = "Guardar Archivo Excel"
    '    _saveFileDialog.ShowDialog()

    '    If _saveFileDialog.FileName <> "" Then
    '        Dim _Excel As Excel.Application
    '        Dim _LibroExcel As Excel.Workbook
    '        Dim _HojaExcel As Excel.Worksheet
    '        'INICIALIZACION

    '        _Excel = New Excel.Application
    '        _LibroExcel = _Excel.Workbooks.Add
    '        _HojaExcel = _LibroExcel.Worksheets(1)

    '        _HojaExcel.Range("A1:D1").Merge()
    '        _HojaExcel.Range("A1:D1").Value = "Reporte Salidas"
    '        _HojaExcel.Range("A1:D1").Font.Bold = True
    '        _HojaExcel.Range("A1:D1").Font.FontStyle = "AR ESSENCE"
    '        _HojaExcel.Range("A1:D1").Font.Size = 24

    '        _HojaExcel.Range("A3:B3").Merge()
    '        _HojaExcel.Range("A3:B3").Value = "Fecha Salida"
    '        _HojaExcel.Range("A3:B3").Font.Size = 11
    '        _HojaExcel.Range("A3:B3").Font.Bold = True

    '        _HojaExcel.Range("C3:E3").Merge()
    '        _HojaExcel.Range("C3:E3").Value = "Codigo"
    '        _HojaExcel.Range("C3:E3").Font.Size = 11
    '        _HojaExcel.Range("C3:E3").Font.Bold = True

    '        _HojaExcel.Range("F3:I3").Merge()
    '        _HojaExcel.Range("F3:I3").Value = "Nombre Producto"
    '        _HojaExcel.Range("F3:I3").Font.Size = 11
    '        _HojaExcel.Range("F3:I3").Font.Bold = True

    '        _HojaExcel.Range("J3:L3").Merge()
    '        _HojaExcel.Range("J3:L3").Value = "Precio"
    '        _HojaExcel.Range("J3:L3").Font.Size = 11
    '        _HojaExcel.Range("J3:L3").Font.Bold = True

    '        _HojaExcel.Range("M3").Merge()
    '        _HojaExcel.Range("M3").Value = "Stock"
    '        _HojaExcel.Range("M3").Font.Size = 11
    '        _HojaExcel.Range("M3").Font.Bold = True

    '        _HojaExcel.Range("N3").Merge()
    '        _HojaExcel.Range("N3").Value = "Cantidad Salida"
    '        _HojaExcel.Range("N3").Font.Size = 11
    '        _HojaExcel.Range("N3").Font.Bold = True


    '        Dim _fila As Integer = 1
    '        Dim _cantRegistros = _observableSalida.Count
    '        Dim _dataContext(0 To _cantRegistros, 0 To 15) As Object

    '        For Each _salida In _observableSalida
    '            _dataContext(_fila, 0) = _salida.fechaSalida
    '            _dataContext(_fila, 1) = _salida.Producto.idProducto
    '            _dataContext(_fila, 5) = _salida.Producto.codigo
    '            _dataContext(_fila, 9) = _salida.Producto.Categoria.nombreCategoria
    '            _dataContext(_fila, 12) = _salida.Producto.stock
    '            _dataContext(_fila, 13) = _salida.cantidadSalida
    '            _fila += 1
    '        Next
    '        _HojaExcel.Range("A4").Resize(_fila, 15).Value = _dataContext

    '        _LibroExcel.SaveAs(_saveFileDialog.FileName)
    '        _Excel.Quit()
    '        _Excel = Nothing
    '        MsgBox(Title:="Reporte", Prompt:="El reporte fue generado exitosamente")
    '    End If
    'End Sub
#End Region
#Region "Sumas"
    Public Property TotalPrecio As Double
        Get
            Return _total
        End Get
        Set(value As Double)
            _total = value
            NotificarCambio("TotalPrecio")
        End Set
    End Property

    Public Property TotalStock As Integer
        Get
            Return _totalStock
        End Get
        Set(value As Integer)
            _totalStock = value
            NotificarCambio("TotalStock")
        End Set
    End Property
    Public Property Totales As Double
        Get
            Dim _db As New RegistrosEntities
            _iqueryableProducto = (From _pr In _db.Producto Select _pr.precio * _pr.stock)
            For Each _produ In _iqueryableProducto
                _totales = _produ.precio * _produ.stock
                _observableProducto.Add(_produ)
            Next
            Return _totales
        End Get
        Set(value As Double)
            _totales = value
            NotificarCambio("Totales")
        End Set
    End Property
    Public Property Quetzales As Double
        Get
            Return _quetzales
        End Get
        Set(value As Double)
            _quetzales = value
            NotificarCambio("Quetzales")
        End Set
    End Property
#End Region
End Class
#Disable Warning