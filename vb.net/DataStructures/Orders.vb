Namespace Orders

    Public Class Order
        Public Property orderNumber As String
        Public Property type As String
        Public Property rate As Double
        Public Property amount As String
        Public Property total As String
        Public Property initialRate As Double
        Public Property initialAmount As String
        Public Property filledRate As Integer
        Public Property filledAmount As Integer
        Public Property currencyPair As String
        Public Property timestamp As String
        Public Property status As String
    End Class

    Public Class DataStructure
        Public Property result As String
        Public Property orders As Order()
        Public Property message As String
        Public Property code As Integer
        Public Property elapsed As String
    End Class

End Namespace
