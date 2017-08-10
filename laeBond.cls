VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Bond"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Private pName As String
Private pIssues As String
Private pDateMaturite As Date
Private pDateIssues As Date
Private pFixedRate As Double
Private pCourbe As Courbe
Private pFixedSchedule As Collection
Private pFaceValue As Double
Private pMarketPrice As Double
Private pDevise As String

Public Property Let FaceValue(iFaceValue As Double)
    pFaceValue = iFaceValue
End Property

Public Property Get FaceValue() As Double
    FaceValue = pFaceValue
End Property
Public Property Let Name(iName As String)
    pName = iName
End Property

Public Property Get Name() As String
    Name = pName
End Property

Public Property Let Issues(iIssues As String)
    pIssues = iIssues
End Property

Public Property Get Issues() As String
    Issues = pIssues
End Property

Public Property Let DateMaturite(iDateMaturite As Date)
    pDateMaturite = iDateMaturite
End Property

Public Property Get DateMaturite() As Date
    DateMaturite = pDateMaturite
End Property
Public Property Let DateIssues(iDateIssues As Date)
    pDateIssues = iDateIssues
End Property

Public Property Get DateIssues() As Date
    DateIssues = pDateIssues
End Property

Public Property Let FixedRate(iFixedRate As Double)
    pFixedRate = iFixedRate
End Property

Public Property Get FixedRate() As Double
    FixedRate = pFixedRate
End Property

Public Property Set Courbe(iCourbe As Courbe)
    Set pCourbe = iCourbe
End Property

Public Property Get Courbe() As Courbe
    Set Courbe = pCourbe
End Property

Public Property Set FixedSchedule(iFixedSchedule As Collection)
    Set pFixedSchedule = iFixedSchedule
End Property

Public Property Get FixedSchedule() As Collection
    Set FixedSchedule = pFixedSchedule
End Property

Public Property Let MarketPrice(iMarketPrice As Double)
    pMarketPrice = iMarketPrice
End Property

Public Property Get MarketPrice() As Double
    MarketPrice = pMarketPrice
End Property

Public Property Let Devise(iDevise As String)
    pDevise = iDevise
End Property

Public Property Get Devise() As String
    Devise = pDevise
End Property

Public Function Price(Optional YTM As Double = 9999)
prix = 0

Dim schedline As FixedSchedule
    For Each schedline In FixedSchedule
        If YTM = 9999 Then
            df = Courbe.utilDiscountFactor(schedline.DateFin)
        Else
            df = (1 + YTM) ^ (-(schedline.DateFin - #4/4/2017#) / 360)
        End If
        prix = prix + schedline.CashFlow * df
    Next schedline
Price = prix / FaceValue * 100

End Function

Public Function InterestAccrued(Optional dateInt As Date = 9999)
    If dateInt = 9999 Then
        yf = (DateMaturite - #4/4/2017#) / 360
    Else
        yf = (DateMaturite - dateInt) / 360
    End If
InterestAccrued = FixedRate * FaceValue * yf

End Function

Public Function TargetDichotomy(ByVal YTM1 As Double) As Double
BondYTMPrice = 0
BondFairMarketPrice = MarketPrice * FaceValue / 100 + InterestAccrued

Dim schedline As FixedSchedule
For Each schedline In FixedSchedule
    yf = (schedline.DateFin - #4/4/2017#) / 360
    df = (1 + YTM1) ^ (-yf)
    BondYTMPrice = BondYTMPrice + schedline.CashFlow * df
Next schedline

TargetDichotomy = BondYTMPrice - BondFairMarketPrice
End Function

Public Function YTM()
    upperBoundary = 0.5
    lowerBoundary = -0.5
    epsilon = 0.000001
    compteur = 0
Do While upperBoundary - lowerBoundary > epsilon And compteur < 200
    compteur = compteur + 1
    YTM = (upperBoundary + lowerBoundary) / 2
    TargetFunctionValue = TargetDichotomy(YTM)
    UpperTargetFunctionValue = TargetDichotomy(upperBoundary)
    If TargetFunctionValue * UpperTargetFunctionValue > 0 Then
        upperBoundary = YTM
    Else
        lowerBoundary = YTM
    End If
Loop
YTMDichotomy = YTM
End Function

Public Function Duration()
Numerator = 0
Denominator = 0
Dim schedline As FixedSchedule
For Each schedline In FixedSchedule
    yf = (schedline.DateFin - #4/4/2017#) / 360
    df = (1 + YTM) ^ (-yf)
    Numerator = Numerator + yf * schedline.CashFlow * df
    Denominator = Denominator + schedline.CashFlow * df
Next schedline

Duration = Numerator / Denominator

End Function

Public Function ModifiedDuration()

ModifiedDuration = Duration / (1 + YTM)

End Function

Public Function Convexity()
Numerator = 0
Denominator = 0

Dim schedline As FixedSchedule
For Each schedline In FixedSchedule
    yf = (schedline.DateFin - #4/4/2017#) / 360
    df = (1 + YTM) ^ (-yf)
    Numerator = Numerator + (yf ^ 2) * schedline.CashFlow * df
    Denominator = Denominator + schedline.CashFlow * df
Next schedline

Convexity = Numerator / Denominator

End Function

