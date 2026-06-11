-- Check contracts without ContractTemplateId
SELECT Id, ContractCode, Status, ContractTemplateId, CreatedAt
FROM Contracts
WHERE ContractTemplateId IS NULL;

-- Check if default template exists
SELECT * FROM ContractTemplates WHERE IsDefault = 1;

-- Update all contracts without ContractTemplateId to use default template
UPDATE Contracts 
SET ContractTemplateId = (
    SELECT TOP 1 Id FROM ContractTemplates 
    WHERE Code = 'STANDARD' AND IsDefault = 1
)
WHERE ContractTemplateId IS NULL;

-- Verify the update
SELECT Id, ContractCode, Status, ContractTemplateId, CreatedAt
FROM Contracts;
