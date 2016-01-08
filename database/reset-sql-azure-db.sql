declare @n char(1)
set @n = char(10)

declare @stmt nvarchar(max)

-- procedures 
select @stmt = isnull( @stmt + @n, '' ) + 
	'drop procedure [' + name + ']' 
	from sys.procedures
	where is_ms_shipped = 0

-- check constraints 
select @stmt = isnull( @stmt + @n, '' ) +
    'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']' 
	from sys.check_constraints
	where is_ms_shipped = 0

-- functions 
select @stmt = isnull( @stmt + @n, '' ) +
    'drop function [' + name + ']' 
	from sys.objects
	where type in ( 'FN', 'IF', 'TF' )
		and is_ms_shipped = 0

-- views 
select @stmt = isnull( @stmt + @n, '' ) +
    'drop view [' + name + ']' 
	from sys.views
	where is_ms_shipped = 0

-- foreign keys 
select @stmt = isnull( @stmt + @n, '' ) +
    'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']' 
	from sys.foreign_keys
	where is_ms_shipped = 0

-- tables 
select @stmt = isnull( @stmt + @n, '' ) +
    'drop table [' + name + ']' 
	from sys.tables
	where is_ms_shipped = 0

-- user defined types 
select @stmt = isnull( @stmt + @n, '' ) +
    'drop type [' + name + ']' 
	from sys.types
	where is_user_defined = 1

exec sp_executesql @stmt