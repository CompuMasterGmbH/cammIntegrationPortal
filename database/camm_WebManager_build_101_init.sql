declare @sgroup int
declare @srv_id int
declare @server_ip varchar(32)
declare @server_proto varchar(5)
declare @server_name varchar(255)
declare @server_port varchar(255)
declare @sgroup_title nvarchar(255)
declare @sgroup_navtitle nvarchar(255)
declare @sgroup_email varchar(255)
declare @comp_formertitle nvarchar(255)
declare @comp_name nvarchar(255)
declare @comp_url varchar(255)

---------------------------
-- Initialization values --
---------------------------

select @server_ip = <%Server_IP%>
select @server_proto = <%Server_Protocol%>
select @server_name = <%Server_Name%>
select @server_port = <%Server_Port%>
select @sgroup_title = <%SGroup_Title%>
select @sgroup_navtitle = <%SGroup_NavTitle%>
select @sgroup_email = <%SGroup_Contact%>
select @comp_url = <%Company_URL%>
select @comp_name = <%Company_Name%>
select @comp_formertitle = <%Company_FormerName%>

------------------------
-- run initialization --
------------------------

exec @sgroup = dbo.AdminPrivate_CreateServerGroup @sgroup_title, @sgroup_email, 1

select top 1 @srv_id = id from dbo.system_servers where servergroup = @sgroup

update dbo.system_servers 
set ip = @server_ip, serverprotocol = @server_proto, enabled = 1, servername = @server_name, serverdescription = @server_name, serverport = @server_port
where id = @srv_id

update dbo.system_servergroups
set useradminserver = @srv_id, areanavtitle = @sgroup_navtitle, areacompanywebsiteurl = @comp_url, areacompanywebsitetitle = @comp_name + ' WebSite', areacompanytitle = @comp_name, areacompanyformertitle = @comp_formertitle
where id = @sgroup

insert into dbo.System_ServerGroupsAndTheirUserAccessLevels (id_servergroup, id_accesslevel)
values (@sgroup, 0)
insert into dbo.System_ServerGroupsAndTheirUserAccessLevels (id_servergroup, id_accesslevel)
values (@sgroup, 1)
insert into dbo.System_ServerGroupsAndTheirUserAccessLevels (id_servergroup, id_accesslevel)
values (@sgroup, 2)

exec dbo.AdminPrivate_CreateAdminServerNavPoints @srv_id, 0, 1
