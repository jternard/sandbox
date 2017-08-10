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
Public FaceValue As Double
Public Schedule As Collection
Public emetteur As String
Public Courbe As Courbe
Public FixedRate As Double
Public IssueDate As Date
Public MaturityDate As Date
Public MarketPrice As Double
Public Currencyi As String

Public Property Get Nom() As String
    Nom = pNom
    'If Nom = "" Then
   ' MsgBox ("Impossible que le nom soit vide")
    'End If
    
End Property

Public Property Let Nom(iNom As String)
    pNom = iNom
End Property

Public Function Price(Optional YTM As Double = 9999)
prix = 0
dateauj = #4/4/2017#
Dim schedline As Schedule
    For Each schedline In Schedule
        If YTM = 9999 Then
            df = Courbe.utilDiscountFactor(schedline.EndDate)
        Else
            df = (1 + YTM) ^ (-(schedline.EndDate - dateauj) / 360)
        End If
        prix = prix + schedline.CashFlow * df
    Next schedline
Price = prix / FaceValue * 100
End Function

Public Function InterestAccrued(Optional Date1 As Date)
dateauj = #4/4/2017#
Dim schedline As Schedule

For Each schedline In Schedule
If dateauj >= schedline.StartDate And dateauj <= schedline.EndDate Then
   couponcouru = ((dateauj - schedline.StartDate) * FixedRate * FaceValue) / 360
      Exit For
Else
   couponcouru = 0
End If

Next schedline

End Function


Function YTMDichotomy(Optional lowerBoundary As Double = 0, Optional upperBoundary As Double = 3, Optional epsilon As Double = 0.001) As Double

compteur = 0
Do While upperBoundary - lowerBoundary > epsilon And compteur < 200
    compteur = compteur + 1
    YTM = (upperBoundary + lowerBoundary) / 2
    TargetFunctionValue = MarketPrice - Price

    UpperTargetFunctionValue = MarketPrice - Price(upperBoundary)

    If TargetFunctionValue * UpperTargetFunctionValue > 0 Then
        upperBoundary = YTM
    Else
        lowerBoundary = YTM
    End If
Loop
YTMDichotomy = YTM
End Function


Function Duration()
dateauj = #4/4/2017#
Dim schedline As Schedule
Duration = 0
For Each schedline In Schedule
YearFraction = (schedline.EndDate - dateauj) / 360
DiscountFactor = 1 / WorksheetFunction.Power(1 + YTMDichotomy, YearFraction)
a = YearFraction * schedline.CashFlow * DiscountFactor
b = schedline.CashFlow * DiscountFactor

Duration = Duration + a / b
Next schedline

End Function

Function ModifiedDuration()
ModifiedDuration = -Duration / (1 + YTMDichotomy)
End Function

Function Convexity()
dateauj = #4/4/2017#
Dim schedline As Schedule
Convexity = 0
For Each schedline In Schedule
YearFraction = (schedline.EndDate - dateauj) / 360
DiscountFactor = 1 / WorksheetFunction.Power(1 + YTMDichotomy, YearFraction)
c = (YearFraction ^ 2) * schedline.CashFlow * DiscountFactor
d = Price(YTMDichotomy)
Convexity = Convexity + c / d
Next schedline
End Function


Function VariationPrix() As Double


VariationTaux = 0.003
VariationPrix = ((Price) / 100) * VariationTaux * ModifiedDuration


End Function




