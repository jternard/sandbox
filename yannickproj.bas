Attribute VB_Name = "Projet"
Sub bondfromaccess()
Dim DB As Database
Dim DBE As DBEngine
Dim nombase As String
Dim datefinvalide, datedebutvalide As Date

Set DBE = New DBEngine
nombase = Application.GetOpenFilename
Set DB = DBE.OpenDatabase(nombase)
sqlStatement = "select * from Obligation"
Set RS = DB.OpenRecordset(sqlStatement)

Dim myBond As Bond

While Not RS.EOF
    Set myBond = New Bond
    myBond.Nom = RS.Fields("Nom").Value
    myBond.FaceValue = RS.Fields("Valeur_Faciale").Value
    myBond.Devise = RS.Fields("Devise").Value
    
    sqlStatement2 = "select * from BondsSchedule where Nom_Obligation = '" & myBond.Nom & "'" & " AND Fin_Periode > #4/4/2017#"
    Set RS2 = DB.OpenRecordset(sqlStatement2)

    Dim schedule As FixedSchedule
    Dim col As Collection
    
    Set col = New Collection
        datefinvalide = RS2.Fields("Fin_Periode").Value
        datedebutvalide = RS2.Fields("Debut_Periode").Value
        While Not RS2.EOF
            Set scheduleSC = New FixedSchedule
            scheduleSC.CashFlow = RS2.Fields("Flux").Value
            scheduleSC.EndDate = RS2.Fields("Fin_Periode").Value
            scheduleSC.StartDate = RS2.Fields("Debut_Periode").Value
            col.Add scheduleSC
            RS2.MoveNext
        Wend
     
    Set myBond.schedule = col
    
    sqlStatement3 = "select * from Taux where Devise='" & myBond.Devise & "'order by Maturite ASC "
    Set RS3 = DB.OpenRecordset(sqlStatement3)
    
    Dim CurvePoint As Point
    Dim Colpoints As New Collection
    
    While Not RS3.EOF
        Set CurvePoint = New Point
        CurvePoint.Tenor = RS3.Fields("Indice_Taux").Value
        CurvePoint.Maturite = RS3.Fields("Maturite").Value
        CurvePoint.rate = RS3.Fields("Dernier_Taux").Value
        Colpoints.Add CurvePoint
        RS3.MoveNext
    Wend

    nomCourbe = RS.Fields("Devise").Value & " " & "Curve"
    nomDevise = RS.Fields("Devise").Value
    nomQuoteType = RS.Fields("Periodicite_Coupons").Value
    nomAssetClass = "oblig"
    
    Dim maCourbe As New Courbe
    
    maCourbe.Nom = nomCourbe
    maCourbe.Devisec = nomDevise
    maCourbe.AssetClass = nomAssetClass
    maCourbe.QuoteType = nomQuoteType
    
    Set maCourbe.Points = Colpoints
    Set myBond.DiscountCurve = maCourbe.Points

    Dim RS4 As Recordset
    Set RS4 = DB.OpenRecordset("projetVBA2")
    RS4.AddNew
    RS4.Fields("Nom_Obligation").Value = myBond.Nom
    RS4.Fields("PrixTheorique").Value = myBond.Price
    RS4.Fields("PrixAnticipe").Value = (myBond.Price + myBond.InterestAccrued(datefinvalide, datedebutvalide)) - myBond.BondModifiedDuration * 0.003 * (myBond.Price + myBond.InterestAccrued(datefinvalide, datedebutvalide)) + (1 / 2) * myBond.BondConvexity * (0.003) ^ 2

    RS4.Update
    
RS.MoveNext
Wend

End Sub

