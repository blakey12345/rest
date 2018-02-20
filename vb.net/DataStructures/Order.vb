Namespace Order
    Public Class Order
        Inherits Orders.Order

        Public Property feePercentage As Double
        Public Property feeValue As String
        Public Property feeCurrency As String
        Public Property fee As String

        'Shared with Orders.Order
        'Public Property orderNumber As String
        'Public Property status As String
        'Public Property currencyPair As String
        'Public Property type As String
        'Public Property rate As Integer
        'Public Property amount As String
        'Public Property initialRate As Integer
        'Public Property initialAmount As String
        'Public Property filledAmount As Integer
        'Public Property filledRate As Integer
        'Public Property timestamp As String

        'Not shared
        'Public Property feePercentage As Double
        'Public Property feeValue As String
        'Public Property feeCurrency As String
        'Public Property fee As String
    End Class

    Public Class DataStructure
        Public Property result As String
        Public Property order As Order
        Public Property message As String
        Public Property code As Integer
        Public Property elapsed As String
    End Class


End Namespace
