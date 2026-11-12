-- Script SQL cho đề thi giữa kỳ Lập trình Web
-- Sinh viên: Trần Quốc Thành
-- MSSV: 24dh111674
-- Ngày sinh: 12/01/2006

USE master;
GO

IF DB_ID(N'Warehouse_24dh111674') IS NOT NULL
BEGIN
    ALTER DATABASE Warehouse_24dh111674 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Warehouse_24dh111674;
END
GO

CREATE DATABASE Warehouse_24dh111674;
GO

USE Warehouse_24dh111674;
GO

CREATE TABLE dbo.Products
(
    ProductID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Category    NVARCHAR(100) NOT NULL,
    Price       DECIMAL(18,2) NOT NULL,
    Quantity    INT NOT NULL
);
GO

INSERT INTO dbo.Products (ProductName, Category, Price, Quantity)
VALUES
    (N'Laptop Dell Inspiron - Trần Quốc Thành', N'Điện tử - Trần Quốc Thành', 15500000.00, 10),
    (N'Chuột không dây Logitech - Trần Quốc Thành', N'Điện tử - Trần Quốc Thành', 350000.00, 4),
    (N'Bàn phím cơ RGB - Trần Quốc Thành', N'Điện tử - Trần Quốc Thành', 1250000.00, 0),
    (N'Giấy A4 Double A - Trần Quốc Thành', N'Văn phòng phẩm - Trần Quốc Thành', 85000.00, 20),
    (N'Bút bi Thiên Long - Trần Quốc Thành', N'Văn phòng phẩm - Trần Quốc Thành', 5000.00, 2),
    (N'Sổ tay bìa da - Trần Quốc Thành', N'Văn phòng phẩm - Trần Quốc Thành', 45000.00, 0),
    (N'Tai nghe Bluetooth - Trần Quốc Thành', N'Phụ kiện - Trần Quốc Thành', 650000.00, 8),
    (N'Cáp sạc USB-C - Trần Quốc Thành', N'Phụ kiện - Trần Quốc Thành', 120000.00, 3);
GO

SELECT * FROM dbo.Products;
GO
