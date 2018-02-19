Namespace TradeHistory

    Public Class Datum
        Public Property tradeID As Integer
        Public Property [date] As String
        Public Property timestamp As Integer
        Public Property type As String
        Public Property rate As Double
        Public Property amount As Double
        Public Property total As Double
    End Class

    Public Class DataStructure
        Public Property result As String
        Public Property data As Datum()
        Public Property elapsed As String
    End Class

End Namespace

