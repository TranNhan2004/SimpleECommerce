ALTER TABLE [business].[Products]
ADD 
    AverageRating DECIMAL(3,2) NOT NULL DEFAULT 0.00,
    TotalRatings INT NOT NULL DEFAULT 0;
GO
