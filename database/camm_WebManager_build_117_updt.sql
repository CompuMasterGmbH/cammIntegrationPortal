-- Activate log analysis 
UPDATE dbo.Applications_CurrentAndInactiveOnes 
SET AppDisabled = 0
WHERE Title = 'System - User Administration - LogAnalysis' AND SystemAppType = 2
