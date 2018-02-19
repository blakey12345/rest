Imports System.Reflection
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports RestSharp
Imports System.Security.Cryptography

' (C) Copyright Craig Blake 2018
' This is a VB.Net implementation of the GateIO php library
' I used http://www.jsonutils.com/ to generate the classes for data storage
' The project requires Newtonsoft JSON / RestSharp NuGet packages
' Contributions are welcome and encouraged...

Public Class APIClient
    Public Property APIKey As String
    Public Property APISecret As String

    Private Function unauthenticated_query(query_url As String) As String
        Dim client = New RestClient("https://data.gate.io/api2/")

        Dim request = New RestRequest(query_url, Method.GET)
        Dim response As IRestResponse = client.Execute(request)

        Return response.Content

    End Function

    Private Function authenticated_query(query_url As String, Optional req As List(Of KeyValuePair(Of String, String)) = Nothing)
        Dim client = New RestClient("https://data.gate.io/api2/")
        Dim request = New RestRequest(query_url, Method.POST)

        Dim nonce As Long = (DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds
        If req Is Nothing Then
            req = New List(Of KeyValuePair(Of String, String))
        End If
        req.Add(New KeyValuePair(Of String, String)("nonce", nonce))

        For Each kvp As KeyValuePair(Of String, String) In req
            request.AddParameter(kvp.Key, kvp.Value)
        Next

        Dim postdatastring As String = http_build_query(req)
        Dim sign = SHA512Hasher.HMAC_SHA512(Me.APISecret, postdatastring)

        request.AddHeader("KEY", Me.APIKey)
        request.AddHeader("SIGN", sign)
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8")

        Dim response As IRestResponse = client.Execute(request)

        Return response.Content
    End Function

    Private Function http_build_query(D As List(Of KeyValuePair(Of String, String)))
        Dim returnValue As String = ""
        For Each kvp As KeyValuePair(Of String, String) In D
            returnValue &= kvp.Key & "=" & kvp.Value & "&"
        Next
        returnValue = returnValue.Substring(0, returnValue.Length - 1)
        Return returnValue
    End Function

    '#######################################################################################################################
    'public methods - no authentication needed

    Public Function get_top_rate(ByVal pair As String, Optional ByVal type As String = "BUY") As Decimal
        Dim jsondata As String = unauthenticated_query("1/orderBook/" & pair.ToUpper)
        Dim result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsondata)

        Dim rate As Decimal = 0
        If type = "BUY" Then
            rate = result("bids")(0)(0)
        Else
            rate = result("asks")(0)(0)
        End If
        Return rate
    End Function

    Public Function get_pairs() As Array
        Dim jsondata As String = unauthenticated_query("1/pairs")
        Dim result As JArray = Newtonsoft.Json.JsonConvert.DeserializeObject(jsondata)
        Return result.ToArray
    End Function

    Public Function get_marketinfo() As MarketInfo.DataStructure
        Dim jsondata As String = unauthenticated_query("1/marketinfo")

        Dim result As MarketInfo.DataStructure = JsonConvert.DeserializeObject(Of MarketInfo.DataStructure)(jsondata)

        ' TODO: REWRITE

        Dim newresult As New MarketInfo.DataStructure
        newresult.result = result.result
        newresult.pairs = {New MarketInfo.Pair}

        For Each pair As MarketInfo.Pair In result.pairs
            For Each prop As PropertyInfo In GetType(MarketInfo.Pair).GetProperties
                Dim v As Object = CallByName(pair, prop.Name, CallType.Get, Nothing)
                If Not v Is Nothing Then
                    CallByName(newresult.pairs(0), prop.Name, CallType.Set, v)
                    Exit For
                End If
            Next
        Next

        result = Nothing

        Return newresult
    End Function

    Public Function get_marketlist() As MarketList.DataStructure
        Dim jsondata As String = unauthenticated_query("1/marketlist")

        Dim result As MarketList.DataStructure = JsonConvert.DeserializeObject(Of MarketList.DataStructure)(jsondata)

        Return result
    End Function

    Public Function get_tickers() As Tickers.DataStructure
        Dim jsondata As String = unauthenticated_query("1/tickers")

        Dim result As Tickers.DataStructure = JsonConvert.DeserializeObject(Of Tickers.DataStructure)(jsondata)

        Return result
    End Function

    Public Function get_ticker(ByVal pair As String) As Ticker.DataStructure
        Dim jsondata As String = unauthenticated_query("1/ticker/" & pair.ToUpper)

        Dim result As Ticker.DataStructure = JsonConvert.DeserializeObject(Of Ticker.DataStructure)(jsondata)

        Return result
    End Function

    Public Function get_orderbooks() As OrderBooks.DataStructure
        Dim jsondata As String = unauthenticated_query("1/orderBooks")

        Dim result As OrderBooks.DataStructure = JsonConvert.DeserializeObject(Of OrderBooks.DataStructure)(jsondata)

        Return result
    End Function

    Public Function get_orderbook(ByVal pair As String) As OrderBook.DataStructure
        Dim jsondata As String = unauthenticated_query("1/orderBook/" & pair.ToUpper)

        Dim result As OrderBook.DataStructure = JsonConvert.DeserializeObject(Of OrderBook.DataStructure)(jsondata)

        Return result
    End Function

    Public Function get_trade_history(ByVal pair As String, ByVal tid As String) As TradeHistory.DataStructure
        Dim jsondata As String = unauthenticated_query("1/tradeHistory/" & pair.ToUpper & "/" & tid)

        Dim result As TradeHistory.DataStructure = JsonConvert.DeserializeObject(Of TradeHistory.DataStructure)(jsondata)

        Return result
    End Function

    '#######################################################################################################################
    'private methods - API key & secret needed

    Public Function get_balances() As Balances.DataStructure
        Dim jsondata As String = authenticated_query("1/private/balances")

        Dim result As Balances.DataStructure = JsonConvert.DeserializeObject(Of Balances.DataStructure)(jsondata)

        Return result
    End Function

    Public Function new_adddress(currency As String) As DepositAddress.DataStructure
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("currency", currency))

        Dim jsondata As String = authenticated_query("1/private/newAddress", l)

        Dim result As DepositAddress.DataStructure = JsonConvert.DeserializeObject(Of DepositAddress.DataStructure)(jsondata)

        Return result
    End Function

    Public Function deposit_address(currency As String) As DepositAddress.DataStructure
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("currency", currency))

        Dim jsondata As String = authenticated_query("1/private/depositAddress", l)

        Dim result As DepositAddress.DataStructure = JsonConvert.DeserializeObject(Of DepositAddress.DataStructure)(jsondata)

        Return result
    End Function

    Public Function cancel_all_orders(type As String, pair As String) As CancelOrders.DataStructure
        'order type(0:sell,1:buy,-1:all)

        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("type", type))
        l.Add(New KeyValuePair(Of String, String)("currencyPair", pair))

        Dim jsondata As String = authenticated_query("1/private/cancelAllOrders", l)

        Dim result As CancelOrders.DataStructure = JsonConvert.DeserializeObject(Of CancelOrders.DataStructure)(jsondata)

        Return result
    End Function

    Public Function sell(pair As String, rate As Double, amount As Double) As OrderBuySell.DataStructure
        Return _order("sell", pair, rate, amount)
    End Function

    Public Function buy(pair As String, rate As Double, amount As Double) As OrderBuySell.DataStructure
        Return _order("buy", pair, rate, amount)
    End Function

    Private Function _order(type As String, pair As String, rate As Double, amount As Double) As OrderBuySell.DataStructure
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("currencyPair", pair.ToUpper))
        l.Add(New KeyValuePair(Of String, String)("rate", rate))
        l.Add(New KeyValuePair(Of String, String)("amount", amount))

        Dim jsondata As String = authenticated_query("1/private/" & type, l)

        Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return result
    End Function

    Public Function open_orders() As Orders.DataStructure
        Dim jsondata As String = authenticated_query("1/private/openOrders")

        Dim result As Orders.DataStructure = JsonConvert.DeserializeObject(Of Orders.DataStructure)(jsondata)

        Return result
    End Function

    '#######################################################################################################################

    Public Function get_order_trades(order_number As Integer)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("orderNumber", order_number))

        Dim jsondata As String = authenticated_query("1/private/orderTrades", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function withdraw(pair As String, amount As Double, address As String)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("currencyPair", pair.ToUpper))
        l.Add(New KeyValuePair(Of String, String)("amount", amount))
        l.Add(New KeyValuePair(Of String, String)("address", address))

        Dim jsondata As String = authenticated_query("1/private/withdraw", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function get_order(order_number As Integer)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("orderNumber", order_number))

        Dim jsondata As String = authenticated_query("1/private/getOrder", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function cancel_order(order_number As Integer)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("orderNumber", order_number))

        Dim jsondata As String = authenticated_query("1/private/cancelOrder", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function cancel_orders(orders As List(Of Integer))
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("orders_json", JsonConvert.SerializeObject(orders)))

        Dim jsondata As String = authenticated_query("1/private/cancelOrders", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function get_my_trade_history(pair As String, order_number As Integer)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("currencyPair", pair.ToUpper))
        l.Add(New KeyValuePair(Of String, String)("orderNumber", order_number))

        Dim jsondata As String = authenticated_query("1/private/tradeHistory", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function deposits_withdrawals(unix_time_start As Integer, unix_time_end As Integer)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("start", unix_time_start))
        l.Add(New KeyValuePair(Of String, String)("end", unix_time_end))

        Dim jsondata As String = authenticated_query("1/private/depositsWithdrawals", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function

    Public Function check_username(username As String, phone As String, sign As String)
        Dim l As New List(Of KeyValuePair(Of String, String))
        l.Add(New KeyValuePair(Of String, String)("username", username))
        l.Add(New KeyValuePair(Of String, String)("phone", phone))
        l.Add(New KeyValuePair(Of String, String)("sign", sign))

        Dim jsondata As String = authenticated_query("1/checkUsername", l)

        'Dim result As OrderBuySell.DataStructure = JsonConvert.DeserializeObject(Of OrderBuySell.DataStructure)(jsondata)

        Return jsondata
    End Function



End Class


