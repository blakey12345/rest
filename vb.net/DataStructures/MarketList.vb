Imports Microsoft.VisualBasic

Namespace MarketList

    Public Class DataStructure
        Public Property result As String
        Public Property data As Data()
    End Class

    Public Class Data
        Public Property no As Integer
        Public Property symbol As String
        Public Property name As String
        Public Property name_en As String
        Public Property name_cn As String
        Public Property pair As String
        Public Property rate As String
        Public Property vol_a As Double
        Public Property vol_b As String
        Public Property curr_a As String
        Public Property curr_b As String
        Public Property curr_suffix As String
        Public Property rate_percent As String
        Public Property trend As String
        Public Property supply As Object
        Public Property marketcap As Object
    End Class

End Namespace
