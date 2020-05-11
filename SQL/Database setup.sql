ALTER TABLE customers
ADD PRIMARY KEY(customerNumber);

ALTER TABLE payments
ADD PRIMARY KEY(customerNumber, checkNumber);

ALTER TABLE orderdetails
ADD PRIMARY KEY(orderNumber, productCode);

ALTER TABLE products
ADD PRIMARY KEY(productCode);

ALTER TABLE productlines
ADD PRIMARY KEY(productLine);

ALTER TABLE orders
ADD PRIMARY KEY(orderNumber);

ALTER TABLE offices
ADD PRIMARY KEY(officeCode);

ALTER TABLE employees
ADD PRIMARY KEY(employeeNumber);

go

ALTER TABLE employees ADD CONSTRAINT FK_OFFICE_TO_EMPLOYEES
FOREIGN KEY (officeCode) references offices(officeCode);

ALTER TABLE payments ADD CONSTRAINT FK_CUSTOMERS_TO_PAYMENTS
FOREIGN KEY (customerNumber) REFERENCES customers(customerNumber);

ALTER TABLE orders ADD CONSTRAINT FK_CUSTOMERS_TO_ORDERS
FOREIGN KEY (customerNumber) REFERENCES customers(customerNumber);

ALTER TABLE orderdetails ADD CONSTRAINT FK_ORDERS_TO_ORDERDETAILS
FOREIGN KEY (orderNumber) REFERENCES orders(orderNumber);

ALTER TABLE orderdetails ADD CONSTRAINT FK_PRODUCTS_TO_ORDERDETAILS
FOREIGN KEY (productCode) REFERENCES products(productCode);

ALTER TABLE products ADD CONSTRAINT FK_PRODUCTLINES_TO_PRODUCTS 
FOREIGN KEY (productLine) REFERENCES productlines(productLine);

go

CREATE INDEX idx_Orders ON orders(orderNumber, customerNumber);
CREATE INDEX idx_Customer ON customers(customerName);
CREATE INDEX idx_Product ON products(productName, MSRP);

go

SELECT emp.firstName, emp.lastName, ofc.city
FROM employees emp join offices ofc ON emp.officeCode = ofc.officeCode
ORDER BY emp.firstName, emp.lastName;

SELECT *
FROM orders
WHERE orderDate like '2003-01-%';

SELECT cust.customerName, count(ord.orderNumber) as ordersPlaced
FROM customers cust JOIN orders ord ON cust.customerNumber = ord.customerNumber
group by cust.customerName
order by cust.customerName;

select * 
from products 
where productCode not in (select pro.productCode from products pro join orderdetails od on pro.productcode = od.productCode);

go

create view view_CustomerOrderInfo as
select cust.customerName, cust.customerNumber, ord.orderDate, pro.productName
from customers cust join orders ord on cust.customerNumber = ord.customerNumber
join orderdetails od on ord.orderNumber = od.orderNumber
join products pro on od.productCode = pro.productCode;

go

select * from view_CustomerOrderInfo;

go

begin try
	begin transaction
		insert into employees values(1703, 'Smith', 'Sam', '123', 'ssmith@classicmodelcars.com', '7', 1143, 'Sales rep'),
		(1704, 'Jones', 'Tom', '412', 'tjones@classicmodelcars.com', '6', 1143, 'Sales rep');
	commit
end try
begin catch
	raiserror('Transaction Fail', 1, 1);
	rollback
end catch;

select * from employees where employeeNumber = 1703 or employeeNumber = 1704;

go

select * from employees;
select * from offices;

delete from employees where officeCode = 6;
delete from offices where officeCode = 6;

go

create or alter function FUNC_AVG_MSRP()
returns decimal(10,2) as
begin
	declare @AvgReturn decimal(10,2);
	select @AvgReturn = sum(MSRP)/count(productCode) from products;
	return @AvgReturn
end

declare @Result decimal(10,2);
exec @Result = dbo.FUNC_AVG_MSRP;
print N'Average MSRP: ' + CAST(@Result as nvarchar(MAX));

go

create or alter procedure pro_FETCH_EMPLOYEES @OfficeCode as varchar(10)
as
begin transaction
	select * from employees where officeCode = @OfficeCode
commit

exec dbo.pro_FETCH_EMPLOYEES @OfficeCode = '-1';

go

create trigger trig_MSRP_10Percent_Increase
on products after insert
as
begin
	update products
	set MSRP = MSRP * 1.1;
end

insert into products values ('S1337_420', 'Testing Product', 'Ships', '1:100', 'Yes', 'Testing product for DB', 100, 100, 100);

select * from products where productCode = 's1337_420';

go

create or alter trigger trig_PREVENT_TABLE_CREATION
on database for CREATE_TABLE
as
begin
	print 'You are unable to create a table!';
	rollback
end

create table Test(
	TestID int primary key
);