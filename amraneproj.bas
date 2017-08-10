Attribute VB_Name = "ProcédureAccess"
Sub bondfromaccess()
Dim DB As Database
Dim DBE As DBEngine

Set DBE = New DBEngine
Set DB = DBE.OpenDatabase(ThisWorkbook.Path & "/projet.accdb")


Set RS = DB.OpenRecordset("Obligation")

While Not RS.EOF
     Dim myBond As Bond
     Set myBond = New Bond
     myBond.Nom = RS.Fields("Nom").Value
     myBond.FaceValue = RS.Fields("Valeur_Faciale").Value
     myBond.FixedRate = RS.Fields("Taux_Fixe").Value
     myBond.IssueDate = RS.Fields("Date_Emission").Value
     myBond.MaturityDate = RS.Fields("Date_maturite").Value
     myBond.MarketPrice = RS.Fields("PrixMarche").Value
     myBond.Currencyi = RS.Fields("Devise").Value

sqlStatement2 = "select * from BondsSchedule where Nom_Obligation = '" & myBond.Nom & "'" & " AND Fin_Periode > 4/4/2017"
Set RS2 = DB.OpenRecordset(sqlStatement2)

Dim schedule1 As Schedule
Dim col As Collection

Set col = New Collection
    While Not RS2.EOF
       Set schedule1 = New Schedule
       schedule1.CashFlow = RS2.Fields("Flux").Value
       schedule1.EndDate = RS2.Fields("Fin_Periode").Value
       schedule1.StartDate = RS2.Fields("Debut_Periode").Value
       col.Add schedule1
       RS2.MoveNext
     Wend

     Set myBond.Schedule = col

     sqlStatement3 = "select * from Taux where Devise = '" & myBond.Currencyi & "'"
     Set RS3 = DB.OpenRecordset(sqlStatement3)
   
     Dim colPoint As Collection
     Set colPoint = New Collection
     While Not RS3.EOF
    
       
      
        Dim myPoint As Point
        Set myPoint = New Point
        myPoint.Tenor = RS3.Fields("Indice_Taux")
        myPoint.Maturite = RS3.Fields("Maturite")
        myPoint.rate = RS3.Fields("Dernier_Taux")
        colPoint.Add myPoint
        RS3.MoveNext
    Wend
    
    Dim myCurve As Courbe
    Set myCurve = New Courbe
    
    myCurve.Devise = myBond.Currencyi
    myCurve.AssetClass = "IR"
    If myCurve.Devise = "EUR" Then myCurve.Nom = "DF-EURIBOR"
    If myCurve.Devise = "GBP" Then myCurve.Nom = "DF-LIBOR"
    
    myCurve.QuoteType = RS.Fields("Periodicite_Coupons")
    Set myCurve.Points = colPoint
  Set myBond.Courbe = myCurve

Set RS4 = DB.OpenRecordset("projetVBA2")
RS4.AddNew
RS4.Fields("Nom_Obligation").Value = myBond.Nom

RS4.Fields("PrixTheorique").Value = myBond.Price

RS4.Fields("PrixAnticipe").Value = myBond.Price * (1 + myBond.VariationPrix)
Debug.Print RS4.Fields("PrixAnticipe").Value
'RS4.Update


RS.MoveNext
Wend



End Sub
