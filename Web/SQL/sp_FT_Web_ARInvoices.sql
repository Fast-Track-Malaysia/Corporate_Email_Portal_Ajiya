USE [TBD]
GO

/****** Object:  StoredProcedure [dbo].[sp_FT_Web_ARInvoices]    Script Date: 08/02/2017 11:23:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE procedure [dbo].[sp_FT_Web_ARInvoices]
(
@DateFr date, 
@DateTo date, 
@CardCodeFr NVARCHAR(15), 
@CardCodeTo NVARCHAR(15)
)

as


--DECLARE @DateFr SMALLDATETIME = '20160101';
--DECLARE @DateTo SMALLDATETIME = '20170601';
--DECLARE @CardCodeFr NVARCHAR(20) = 'RNBE001';
--DECLARE @CardCodeTo	NVARCHAR(20) = 'ZZZZ001';


SELECT T0.CardCode,
	T0.CardName,	
	T0.DocNum,
	T0.DocDate,
	T0.DocStatus,
	T0.DocType,
	isNull(T0.NumAtCard,'') as [NumAtCard],
	isNull(T0.Address,'') as [Address],
	T0.DocCur,
	T0.DocTotal,
	T0.DocTotalFC,
	T0.VatSum,
	T0.VatSumFC,
	T0.U_USDAmount,	
	T1.VisOrder,
	isNull(T1.ItemCode,'') as [ItemCode],
	T1.Dscription,
	T1.Quantity,
	T1.Price,	
	T1.TotalFrgn,
	T1.VatGroup,
	T1.VatPrcnt,	
	G0.PymntGroup,
	P0.NAME,	
	isNull(T12.CountryB,'') as [CountryB],
	isNull(C0.VatIdUnCmp,'') as [VatIdUnCmp],
	isNull(C0.CardFName,'') as [CardFName],
	ADM.CompnyName,
	ADM.CompnyAddr,
	ADM.TaxIdNum2,
	ADM.Fax,
	ADM.Phone1
FROM OINV T0
INNER JOIN INV1 T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN OCRD C0 ON T0.CardCode = C0.CardCode
LEFT OUTER JOIN OCTG G0 ON G0.GroupNum = T0.GroupNum
LEFT OUTER JOIN OCPR P0 ON P0.CntctCode = T0.CntctCode
LEFT OUTER JOIN INV12 T12 ON T12.DocEntry = T0.DocEntry
LEFT OUTER JOIN OADM ADM ON 1=1
WHERE T0.DocStatus = 'O'
AND T0.DocType = 'I'
AND T0.DocDate between @DateFr and @DateTo
AND T0.CardCode between @CardCodeFr and @CardCodeTo
ORDER BY T0.CardCode,T0.DocNum,T1.VisOrder




GO


