Attribute VB_Name = "Interaction_DB"
Sub BondAccess()
Dim DB As database
Dim DBE As DBEngine

Set DBE = New DBEngine
Set DB = DBE.OpenDatabase(ThisWorkbook.Path & "/projet.accdb")

sqlStatement = "select * from Obligation"
Set RS = DB.OpenRecordset(sqlStatement)

Dim myBond As Bond

'Boucle sur les obligations
While Not RS.EOF
    Set myBond = New Bond
    
    'Remplissage d'un objet obligation
    myBond.Name = RS.Fields("Nom").Value
    myBond.FaceValue = RS.Fields("Valeur_Faciale").Value
    myBond.DateMaturite = RS.Fields("Date_Maturite").Value
    myBond.Issues = RS.Fields("Emetteur").Value
    myBond.DateIssues = RS.Fields("Date_Emission").Value
    myBond.Devise = RS.Fields("Devise").Value
    myBond.MarketPrice = RS.Fields("PrixMarche").Value
    myBond.FixedRate = RS.Fields("Taux_Fixe").Value
    
    'requete SQL pour récupérer l'échéancier
    sqlStatement2 = "select * from BondsSchedule where Nom_Obligation = '" & myBond.Name & "'"
    Set RS2 = DB.OpenRecordset(sqlStatement2)
    
    'Boucle sur les lignes de RS2
    Dim schedule As FixedSchedule
    Dim col As Collection
    Set col = New Collection
    While Not RS2.EOF
        'Remplissage de l'objet FixedSchedule et ajout dans une collection
        Set schedule = New FixedSchedule
        schedule.CashFlow = RS2.Fields("Flux").Value
        schedule.DateFin = RS2.Fields("Fin_Periode").Value
        schedule.DateDebut = RS2.Fields("Debut_Periode").Value
        col.Add schedule
        RS2.MoveNext
    Wend
    
    'Ajout de la collection d'objet FixedSchedule dans l'objet obligation
    Set myBond.FixedSchedule = col
    
    'Récupération des points qui ont la même devise que l'obligation
    Dim maCourbe As Courbe
    Set maCourbe = New Courbe

    Dim colPoints As Collection
    Set colPoints = New Collection

    Dim monPoint As Point

    sqlStatement3 = "select * from Taux where Devise = '" & myBond.Devise & "'"
    Set RS3 = DB.OpenRecordset(sqlStatement3)
    
    While Not RS3.EOF
        Set monPoint = New Point
        monPoint.Tenor = RS3.Fields("Indice_Taux").Value
        monPoint.Maturite = RS3.Fields("Maturite").Value
        monPoint.Rate = RS3.Fields("Dernier_Taux").Value
        colPoints.Add monPoint
        RS3.MoveNext
    Wend
    
    'Création de l'objet courbe
    maCourbe.Devise = myBond.Devise
    maCourbe.Nom = myBond.Name
    
    'Collection de points dans l'objet courbe
    Set maCourbe.Points = colPoints

    'Objet courbe dans l'obligation
    Set myBond.Courbe = maCourbe
    
    'Report des prix dans projetVBA2
    Dim RS4 As Recordset
    Set RS4 = DB.OpenRecordset("projetVBA2")
    RS4.AddNew
    RS4.Fields("Nom_Obligation").Value = myBond.Name
    RS4.Fields("PrixTheorique").Value = myBond.Price
    RS4.Fields("PrixAnticipe").Value = (1 - myBond.ModifiedDuration * 30 / 10000 + 0.5 * myBond.Convexity * (30 / 10000) ^ 2) * myBond.Price
    RS4.Update
    
    'Obligation suivante
    RS.MoveNext
Wend

End Sub

