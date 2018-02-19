Imports System.Security.Cryptography
Imports System.Text

Public Class SHA512Hasher
    Private Shared ReadOnly encoding As Encoding = Encoding.UTF8

    Public Shared Function HMAC_SHA512(ByVal key As String, ByVal data As String)
        Dim keyByte = encoding.GetBytes(key)
        Using hmacsha512_ = New HMACSHA512(keyByte)
            hmacsha512_.ComputeHash(encoding.GetBytes(data))
            Return ByteToString(hmacsha512_.Hash)
        End Using
    End Function

    Public Shared Function ByteToString(ByVal buff As Byte()) As String
        Dim sbinary As String = ""
        For i As Integer = 0 To buff.Length - 1
            sbinary += buff(i).ToString("x2")
        Next
        Return sbinary
    End Function

End Class