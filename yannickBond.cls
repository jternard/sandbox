VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Bond"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Private pNom As String
Private pFaceValue As Double
Private pschedule As Collection
Private pDiscountCurve As Collection
Private pDevise As String

Public Property Let Nom(iNom As String)
    pNom = iNom
End Property

Public Property Get Nom() As String
    Nom = pNom
End Property

Public Property Let FaceValue(iFaceValue As Double)
    pFaceValue = iFaceValue
End Property

Public Property Get FaceValue() As Double
    FaceValue = pFaceValue
End Property

Public Property Set schedule(CollectionSchedule As Collection)
    Set pschedule = CollectionSchedule
End Property

Public Property Get schedule() As Collection
    Set schedule = pschedule
End Property

Public Property Set DiscountCurve(CollectionDiscountCurve As Collection)
    Set pDiscountCurve = CollectionDiscountCurve
End Property

Public Property Get DiscountCurve() As Collection
    Set DiscountCurve = pDiscountCurve
End Property

Public Property Let Devise(iDevise As String)
    pDevise = iDevise
End Property

Public Property Get Devise() As String
    Devise = pDevise
End Property

Public Function Price(Optional YTM1 As Double)
prix = 0
Dim schedline As FixedSchedule
Dim courbe1 As Courbe
    For Each schedline In schedule
        If IsEmpty(YTM1) = True Then
            df = courbe1.utilDiscountFactor(schedline.EndDate)
        Else
            df = (1 + YTM1) ^ (-(schedline.EndDate - #4/4/2017#) / 360)
        End If
        prix = prix + schedline.CashFlow * df
    Next schedline
Price = prix / FaceValue * 100
End Function

Public Function InterestAccrued(datefin, datedebut As Date)

dateactuelle = #4/4/2017#
If datefin > dateactuelle Then
    InterestAccrued = ((dateactuelle - datedebut) * YTMDichotomy * FaceValue) / 360
Else
    InterestAccrued = 0
End If
End Function

Public Function YTMDichotomy() As Double

Dim YTM As Double
Dim upperBoundary As Double
Dim lowerBoundary As Double

upperBoundary = 0.05
lowerBoundary = -0.05
compteur = 0

Do While upperBoundary - lowerBoundary > 0.00000001 And compteur < 200
    compteur = compteur + 1
    YTM = (upperBoundary + lowerBoundary) / 2
    TargetFunctionValue = Price(YTM) - Price
    UpperTargetFunctionValue = Price(upperBoundary) - Price
    If TargetFunctionValue * UpperTargetFunctionValue > 0 Then
        upperBoundary = YTM
    Else
        lowerBoundary = YTM
    End If
Loop
YTMDichotomy = YTM

End Function

Public Function BondDuration() As Double

Dim schedline As FixedSchedule
Dim courbe1 As Courbe
    For Each schedline In schedule
        df = (1 + YTM1) ^ (-(schedline.EndDate - #4/4/2017#) / 360)
        BondDuration = BondDuration + (schedline.EndDate - #4/4/2017#) / 360 * schedline.CashFlow * df
    Next schedline

BondDuration = BondDuration / ((Price * FaceValue) / 100)
    
End Function

Public Function BondModifiedDuration() As Double

    BondModifiedDuration = BondDuration / (1 + YTMDichotomy)
    
End Function

Function BondConvexity() As Double

Dim schedline As FixedSchedule
Dim courbe1 As Courbe
    For Each schedline In schedule
        df = (1 + YTM1) ^ (-(schedline.EndDate - #4/4/2017#) / 360)
        BondConvexity = BondConvexity + ((schedline.EndDate - #4/4/2017#) / 360) ^ 2 * schedline.CashFlow * df
    Next schedline

BondConvexity = BondConvexity / ((Price * FaceValue) / 100)

End Function
