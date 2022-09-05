USE [TBD]
GO

/****** Object:  StoredProcedure [dbo].[sp_FT_Web_StatementOfAccount]    Script Date: 08/02/2017 11:23:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER procedure [dbo].[sp_FT_Web_StatementOfAccount]
(
@StatementDate DATE,
@CardCodeFr NVARCHAR(15), 
@CardCodeTo NVARCHAR(15),
@CardType NVARCHAR(1)
)
as

DECLARE @AgeBy		NVARCHAR(10)
DECLARE @ComName	NVARCHAR(100)
DECLARE @ComAddr	NVARCHAR(254)
DECLARE @MainCur	VARCHAR(3)
DECLARE @CurName	NVARCHAR(20)

-- DEBUG ------
--EXEC sp_FT_Web_StatementOfAccount '20170601','RNBE001','ZZZZ001','C'


-- COMPANY INFO
SELECT @MainCur = T.Maincurncy, 
@CurName = C.CurrName, 
@ComName = T.CompnyName, 
@ComAddr = T.CompnyAddr 
FROM OADM T
INNER JOIN OCRN C ON C.CurrCode = T.MainCurncy

-- PARAMETER
SET @AgeBy = 'DUE'	-- DOC, DUE
--SET @CardType = ''	-- C, S

SET @StatementDate = isNull(NullIf(@StatementDate,'19000101'),GETDATE())

IF OBJECT_ID('tempdb..#BPStatement' , 'U') IS NOT NULL
DROP TABLE #BPStatement

SELECT 
	@StatementDate as [StatementDate],
	MAX(CASE WHEN @AgeBy = 'DUE' THEN J1.DueDate ELSE J1.RefDate END) AS [AgeDate],
	MAX(J0.Memo) as [Memo],
	MAX(N0.DocCode) as [DocType],
	MAX(J1.BaseRef) as [DocNo],
	MAX(NULLIF(J1.SourceLine,-99)) as [Instal. No.],
	MAX(J1.RefDate) as [Posting Date],
	MAX(J1.DueDate) as [Due Date],
	MAX(J1.TaxDate) as [Document Date],
	MAX(CASE WHEN J1.DueDate < @StatementDate THEN DATEDIFF(DAY, J1.DueDate, @StatementDate) END) as [Number of Days Outstanding],
	MAX(J1.FCCurrency) as [Currency],	
	-- JE ROW AMOUNT ----------------------------------------------------------------
	MAX(CASE WHEN R0.ReconDate <= @StatementDate OR R0.ReconDate is null  
		THEN (J1.BalDueDeb - J1.BalDueCred) 
		ELSE (CASE WHEN R1.IsCredit = 'C' THEN -1 ELSE 1 END) * (J1.BalDueDeb + R1.ReconSum)
	END) AS [Balance],
		
	MAX(CASE WHEN R0.ReconDate <= @StatementDate OR R0.ReconDate is null 
		THEN (J1.BalFcDeb - J1.BalFcCred) 
		ELSE (CASE WHEN R1.IsCredit = 'C' THEN -1 ELSE 1 END) * (J1.BalFcDeb + R1.ReconSumFC)
	END) AS [BalanceFC],
	-- JE ROW AMOUNT ----------------------------------------------------------------
	J1.TransId,
	J1.Line_ID,
	MAX(J1.Debit) as [Debit],
	MAX(J1.Credit) as [Credit],
	MAX(J1.FCDebit) as [DebitFC],
	MAX(J1.FCCredit) as [CreditFC],	
	MAX(J1.Debit) + MAX(J1.Credit) as [BaseAmt],
	MAX(J1.FCDebit) + MAX(J1.FCCredit) as [BaseAmtFC],
	MAX(J1.SYSDeb) + MAX(J1.SYSCred) as [BaseAmtSys],
	MAX(J1.Project) as [Project Code],
	MAX(J1.Account) as [Control Account],	
	MAX(J1.CreatedBy) as [CreatedBy],
	MAX(J1.LineMemo) as [LineMemo],
	MAX(J1.Indicator) as [Indicator],
	MAX(J1.DunnLevel) as [DunnLevel],
	J1.BPLName as [BPLName]	,		
	--MAX(J1.BalDueCred - J1.BalDueDeb) as [Balance],
	--MAX(CASE WHEN R1.IsCredit = 'D' THEN -1 ELSE 1 END) * (MAX(J1.BalDueDeb) + SUM(R1.ReconSum)) as [BalanceRecon],
	--MAX(J1.BalFcCred - J1.BalFcDeb) as [BalanceFC],
	--MAX(CASE WHEN R1.IsCredit = 'D' THEN -1 ELSE 1 END) * (MAX(J1.BalFcDeb) + SUM(R1.ReconSumFC)) as [BalanceReconFC],
	--MAX(J1.BalScCred - J1.BalScDeb) as [BalanceSC],
	--MAX(CASE WHEN R1.IsCredit = 'D' THEN -1 ELSE 1 END) * (MAX(J1.BalScDeb) + SUM(R1.ReconSumSC)) as [BalanceReconSC],
	-- RECON INFO ------------------------------------------------------
	MAX(R0.ReconDate) as [Recon Date],	
	MAX(isNull(R1.ReconSum,0)) AS [Recon],
	MAX(isNull(R1.ReconSumFC,0)) AS [ReconFC],	
	-- JournalTransSourceView -----------------------------------------
	MAX(J5.CardCode) as [BPCode],
	MAX(J5.CardName) as [BPName],
	MAX(J5.NumAtCard) as [BP Ref. No.],	
	MAX(J5.SlpCode) as [SlpCode],
	MAX(J5.BlockDunn) as [BlockDunn],
	MAX(J5.DunnLevel) as [vwDunnLevel],
	MAX(J5.TransType) as [TransType],
	MAX(J5.IsSales) as [IsSales],
	-- BP INFO ---------------------------------------------------------
	MAX(B0.CardCode) as [CardCode],
	MAX(B0.CardName) as [CardName],		
	MAX(B0.CardType) as [CardType],		
	MAX(B0.Balance) as [BPBalance],
	MAX(B0.Currency) as [BPCurrency],
	MAX(B0.PymCode) as [Payment Method Code],
	MAX(B1.PymntGroup) as [Payment Terms],
	MAX(B0.DunTerm) as [Dunning],
	MAX(B0.CntctPrsn) as [ContactPerson],
	MAX(B0.Phone1) as [Phone1], 
	MAX(B0.Fax) AS [Fax], 
	MAX(B0.CreditLine) as [CreditLine],
	MAX(B0.BillToDef) AS [AddID], 
	MAX(ISNULL(B0.Address,'')) AS [AddL1], 
	MAX(ISNULL(B0.Block,'')) AS [AddL2], 
	MAX(ISNULL(B0.County,'')) AS [AddL3], 
	MAX(ISNULL(B0.City,'')) AS [AddL4]
	-- UDF ----------------------------------------------------------
	--MAX(ISNULL(I0.U_InvGrp, '')) AS [U_InvGrp]
	INTO #BPStatement
FROM JDT1 J1
INNER JOIN OJDT J0 ON J0.TransId = J1.TransId
INNER JOIN OCRD B0 ON B0.CardCode = J1.ShortName
LEFT OUTER JOIN OCTG B1 ON B1.GroupNum = B0.GroupNum
LEFT OUTER JOIN vwObjName N0 ON N0.ObjType = J1.TransType
LEFT OUTER JOIN ITR1 R1 ON R1.TransId = J1.TransId AND R1.TransRowId = J1.Line_ID
LEFT OUTER JOIN OITR R0 ON R0.ReconNum = R1.ReconNum
--LEFT OUTER JOIN OINV I0 ON I0.ObjType = J0.TransType AND I0.DocEntry = J0.CreatedBy
LEFT OUTER JOIN B1_JournalTransSourceView J5 ON J5.ObjType = J1.TransType AND J5.DocEntry = J1.CreatedBy
	AND (J5.TransType <> N'I' 
		OR (J5.TransType = N'I'	AND J5.InstlmntID = J1.SourceLine)
		)
WHERE 	(
		J1.RefDate <= @StatementDate --(@P12)
		AND J1.RefDate <= @StatementDate --(@P13)
		AND (B0.CardType = @CardType OR @CardType='')--(@P14)
		AND (B0.CardCode >= @CardCodeFr OR @CardCodeFr = '') --(@P15)
		AND (B0.CardCode <= @CardCodeTo OR @CardCodeTo = '') --(@P16)		
		AND (
				(
					R0.ReconDate > @StatementDate --(@P17)
					AND R1.IsCredit in ('D','C') --(@P18)
				)
				OR
				(
					(J1.BalDueCred <> J1.BalDueDeb OR J1.BalFcCred <> J1.BalFcDeb)
					AND NOT EXISTS (
						SELECT U0.TransId, U0.TransRowId
						FROM ITR1 U0
						INNER JOIN OITR U1 ON U1.ReconNum = U0.ReconNum	WHERE J1.TransId = U0.TransId
						AND J1.Line_ID = U0.TransRowId	AND U1.ReconDate > @StatementDate --(@P28)
						GROUP BY U0.TransId, U0.TransRowId
						)
				)
		)		
		)
	AND (
		B0.CardCode IS NULL
		OR (
			B0.validFor = 'Y' --(@P19)
			OR (
				B0.frozenFor = 'Y' --(@P20)
				AND (
					B0.frozenFrom IS NOT NULL
					OR B0.frozenTo IS NOT NULL
					)
				)
			OR (
				B0.validFor = 'N' --(@P21)
				AND B0.frozenFor = 'N' --(@P22)
				)
			)
		)
	AND	J0.U_HideStmt <> 'Y'
GROUP BY J1.TransId, J1.Line_ID, J1.BPLName
Order By MAX(B0.CardCode), MAX(J0.CreateDate)

-- CREATE INDEX FOR FAST SEARCHING --
CREATE INDEX [IDX_CardCode] ON #BPStatement (CardCode)


-- TO DISPLAY AGING BAND --
SELECT 
-- DOC INFO --
T.DocType, T.DocNo, T.[Instal. No.], T.TransId,
T.[Posting Date], T.[Due Date], T.[Document Date],
T.[BP Ref. No.], T.Memo, T.[Number of Days Outstanding], 
-- DOC AMOUNT --
T.BaseAmt, isNull(nullIf(T.BaseAmtFC,0),T.BaseAmt) as [BaseAmtBP],
T.Balance, isNull(nullIf(T.BalanceFC,0),0) as [BalanceBP],
T.Recon, isNull(nullIf(T.ReconFC,0),T.Recon) as [ReconBP],
T.Debit, isNull(nullIf(T.DebitFC,0),T.Debit) as [DebitBP],
T.Credit, isNull(nullIf(T.CreditFC,0),T.Credit) as [CreditBP],
T.Currency,
-- AGING BAND BY DAY --
-- Local Currency

T.Balance as [L000],
CASE WHEN T.[AgeDate] > T.[StatementDate] THEN isNull(nullIf(T.Balance,0),0) ELSE 0 END as [L999],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 0 AND 30 THEN T.Balance ELSE 0 END as [L030],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 31 AND 60 THEN T.Balance ELSE 0 END as [L060],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 61 AND 90 THEN T.Balance ELSE 0 END as [L090],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 91 AND 120 THEN T.Balance ELSE 0 END as [L120],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) > 120 THEN T.Balance ELSE 0 END as [L121+],

-- BP Currency
T.BalanceFC as [B000],
CASE WHEN T.[AgeDate] > T.[StatementDate] THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B999],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 0 AND 30 THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B030],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 31 AND 60 THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B060],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 61 AND 90 THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B090],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) BETWEEN 91 AND 120 THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B120],
CASE WHEN DATEDIFF(Day, T.[AgeDate], T.[StatementDate]) > 120 THEN isNull(nullIf(T.BalanceFC,0),0) ELSE 0 END as [B121+],
-- AGING BAND BY MONTH --
--CASE WHEN DATEDIFF(Month, T.[AgeDate], T.[StatementDate]) < 1 THEN isNull(NullIf(T.BalanceFC,0), T.Balance) END as [P0Mon],
--CASE WHEN DATEDIFF(Month, T.[AgeDate], T.[StatementDate]) = 1 THEN isNull(NullIf(T.BalanceFC,0), T.Balance) END as [P1Mon],
--CASE WHEN DATEDIFF(Month, T.[AgeDate], T.[StatementDate]) = 2 THEN isNull(NullIf(T.BalanceFC,0), T.Balance) END as [P2Mon],
--CASE WHEN DATEDIFF(Month, T.[AgeDate], T.[StatementDate]) = 3 THEN isNull(NullIf(T.BalanceFC,0), T.Balance) END as [P3Mon],
--CASE WHEN DATEDIFF(Month, T.[AgeDate], T.[StatementDate]) > 3 THEN isNull(NullIf(T.BalanceFC,0), T.Balance) END as [P4Mon],
T.[Control Account],
-- BP INFO --
T.CardCode, T.CardName, T.CardType, T.BPCurrency, T.[Payment Method Code],  Upper(T.[Payment Terms]) AS [Payment Terms], T.ContactPerson, T.Phone1, T.Fax, T.CreditLine, T.AddID, T.AddL1, T.AddL2, T.AddL3, T.AddL4,
-- COMPANY INFO --
@ComName as [CompanyName], @ComAddr as [CompanyAddress], @MainCur as [MainCurrency], @CurName as [Currency Name], T.StatementDate
FROM #BPStatement T
Order By T.CardCode, T.[Posting Date], T.TransId, T.Line_ID

GO


