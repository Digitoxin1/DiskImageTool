Imports System.Threading

Public Interface IImageScanner
    Event ProgressChanged(Percent As Double)
    Event ScanCompleted(Cancelled As Boolean, [Error] As Exception)

    ReadOnly Property ItemsRemaining As UInteger

    Sub Scan(ct As CancellationToken)
End Interface