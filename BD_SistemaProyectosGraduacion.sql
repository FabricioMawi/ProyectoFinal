create database BD_SistemaProyectosGraduacion

use BD_SistemaProyectosGraduacion
go

create table Empresa (
idContacto		int				identity(1,1) primary key not null,
nombreEmpresa	varchar(30)		null,
nombreContacto	varchar(50)		null,
telefono		int				not null,
email			varchar(90)		not null,
tipoEmpresa		varchar(90)		null);

insert into Empresa values ('Sin Empresa', 'Sin Empresa', 0, 'Sin Empresa', 'Sin Empresa');

select * from Empresa


create table Curso (
idCurso			int				identity(1,1) primary key,
nombreCurso		varchar(MAX)	not null); 

insert into Curso values ('SC-603 Análisis y Modelado de Requerimientos')
insert into Curso values ('SC-702 Diseño y Desarrollo de Sistemas')
insert into Curso values ('SC-803 Implantación de Sistemas')
select * from Curso

create table Profesor (
idProfesor			int				identity(1,1) primary key,
nombreProfesor		varchar(20)		not null,
apellidoProfesor	varchar(30)		not null,
rol					varchar(20)		not null,
contrasena			varchar(500)	not null,
nombreUsuario		varchar(30)		not null,
emailProfesor		varchar(30)		not null,
estado				varchar(9)		not null);

insert into Profesor values('Fabricio','Arce','Profesor','03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4','fau2110','fau1997@gmail.com', 'Activo')
insert into Profesor values('Raquel','Meléndez','Profesor','1234','tengosueño','hacer.esto.aburre@gmail.com', 'Activo')



select * from Profesor


create table Grupo (
idGrupo			int				identity(1,1) primary key,
nombreGrupo varchar (100)		not null,
idProfesor		int				not null,
sede			varchar(50)		null,
idCurso			int				not null,
cuatrimestre	varchar(100)	null,
foreign key (idProfesor) references Profesor(idProfesor),
foreign key (idCurso) references Curso(idCurso));

insert into Grupo values ('Sin Grupo', 1, ' ', 1, ' ');

select * from Grupo


create table Proyecto (
idProyecto			int				identity(1,1) primary key,
nombreProyecto		varchar(100)	not null,
idContacto			int				null,
idCurso				int				not null,
tecnologia			varchar(30)		not null,
idProfesor			int				not null,
idGrupo				int				not null,
estadoProyecto		varchar(20)		not null,
fechaInicio			date			not null,
fechaFinalizado		date			null,
foreign key (idContacto) references Empresa(idContacto),
foreign key (idCurso) references Curso(idCurso),
foreign key (idProfesor) references Profesor(idProfesor),
foreign key (idGrupo) references Grupo(idGrupo));

insert into Proyecto values ('Sin Proyecto', 1, 1, ' ', 1, 1, 'Inexistente', GETDATE(), GETDATE())
select * from Archivo2

select idCurso from Proyecto where idCurso = 2 


create table Comentario (
idComentario	int				identity(1,1) primary key,
fecha			date			not null,
contenido		varchar(400)	not null,
idProyecto		int				not null,
foreign key (idProyecto) references Proyecto(idProyecto));

create table Archivo1 (
idArchivo			int				identity(1,1) primary key,
nombreArchivo		varchar(MAX)	not null,
tipoContenido		varchar(MAX)	not null,
contenidoArchivo	varbinary(MAX)	not null,
idProyecto			int				not null,
documentos			varchar(MAX)    null,
foreign key (idProyecto) references Proyecto(idProyecto));



select * from Proyecto

create table Archivo2 (
idArchivo			int				identity(1,1) primary key,
nombreArchivo		varchar(MAX)	not null,
tipoContenido		varchar(MAX)	not null,
contenidoArchivo	varbinary(MAX)	not null,
idProyecto			int				not null,
documentos			varchar(MAX)	null,
foreign key (idProyecto) references Proyecto(idProyecto));

create table Archivo3 (
idArchivo			int				identity(1,1) primary key,
nombreArchivo		varchar(MAX)	not null,
tipoContenido		varchar(MAX)	not null,
contenidoArchivo	varbinary(MAX)	not null,
idProyecto			int				not null,
documentos			varchar(MAX)	null,
foreign key (idProyecto) references Proyecto(idProyecto));

create table Profesor_Curso (
idProfesor		int		not null,
idCurso			int		not null,
foreign key (idProfesor) references Profesor(idProfesor),
foreign key (idCurso) references Curso(idCurso), 
constraint idProfesorCurso primary key (idProfesor, idCurso));

create table CatalogoEntregable (
idEntregable		int				identity(1,1) primary key,
nombreEntregable	varchar(30)		not null,
nombreCurso			varchar(35)		not null);

create table Entregable_Proyecto (
idProyecto		int		not null,
idEntregable	int		not null,
foreign key (idProyecto) references Proyecto(idProyecto),
foreign key (idEntregable) references CatalogoEntregable(idEntregable),
constraint idProyectoEntregable primary key (idProyecto, idEntregable));

create table Estudiante (
idEstudiante		int				identity(1,1) primary key,
nombreEstudiante	varchar(110)	not null,
idGrupo				int				null,
idProfesor			int				null,
nota				int				null,
foreign key (idGrupo) references Grupo(idGrupo),
foreign key (idProfesor) references Profesor(idProfesor));

truncate table Estudiante

select * from Estudiante

create table Auditoria (
idAuditoria		integer			identity(1,1) primary key,
usuario			varchar(25)		not null,
nombreTabla		varchar(90)		not null,			
fecha			datetime		not null,
tipoCambio		varchar(90)		not null);

select * from Auditoria

go
--Triggers
----Para proyecto
create trigger triProyectoInsert on Proyecto
after insert
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Proyecto', getDate(), 'Insert')
end
go

create trigger triProyectoUpdate on Proyecto
after update
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Proyecto', getDate(), 'Update')
end
go

create trigger triProyectoDelete on Proyecto
after delete 
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Proyecto', getDate(), 'Delete')
end
go

----Para profesor
create trigger triProfesorInsert on Profesor
after insert
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Profesor', getDate(), 'Insert')
end
go

create trigger triProfesorUpdate on Profesor
after update
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Profesor', getDate(), 'Update')
end
go

create trigger triProfesorDelete on Profesor
after delete 
as
begin
  insert into Auditoria 
  (usuario, nombreTabla, fecha, tipoCambio) values
  (suser_name(), 'Profesor', getDate(), 'Delete')
end
go

--Procedimientos para reportes de proyecto
create procedure selectProyecto @idProyecto int
as
	select nombreProyecto, tecnologia, estadoProyecto, fechaInicio 
	from Proyecto 
	where idProyecto = @idProyecto;
go

create procedure selectComentarios @idProyecto int
as
	select fecha, contenido 
	from Comentario
	where idProyecto = @idProyecto;
go

create procedure selectProfesor @idProyecto int
as
	select b.nombreProfesor, b.apellidoProfesor, b.emailProfesor
	from Profesor b, Proyecto a
	where a.idProyecto = @idProyecto
	and a.idProfesor = b.idProfesor;
go

create procedure selectGrupo @idProyecto int
as
	select b.nombreGrupo, b.sede, b.cuatrimestre
	from Grupo b, Proyecto a
	where a.idProyecto = @idProyecto
	and a.idGrupo = b.idGrupo;
go

create procedure selectCurso @idProyecto int
as
	select b.nombreCurso
	from Curso b, Proyecto a
	where a.idProyecto = @idProyecto
	and a.idCurso = b.idCurso;
go

--procedimientos para reporte de profesor
create procedure selectProfesor1 @idProfesor int
as
	select nombreProfesor, apellidoProfesor, rol, nombreUsuario,
	emailProfesor, estado
	from Profesor
	where idProfesor = @idProfesor
go

create procedure selectProyecto1 @idProfesor int
as
	select nombreProyecto, tecnologia, fechaInicio, idCurso, 
	idGrupo 
	from Proyecto 
	where idProfesor = @idProfesor
	and estadoProyecto = 'Activo';
go

create procedure selectGrupo1 @idProfesor int
as
	select idGrupo, nombreGrupo, sede, cuatrimestre
	from Grupo
	where idProfesor = @idProfesor;
go

create procedure selectEstudiante @idProfesor int
as
	select nombreEstudiante 
	from Estudiante
	where idProfesor = @idProfesor
go

--procedimientos para reporte de todos los proyectos
create procedure selectProyectoTodo 
as
	select idProyecto, nombreProyecto, estadoProyecto, tecnologia, 
	fechaInicio, idContacto, idCurso, idProfesor
	from Proyecto
go

create procedure selectEmpresaTodo
as
	select idContacto, nombreEmpresa, nombreContacto,
	email, telefono, tipoEmpresa
	from Empresa
go

create procedure selectProfesorTodo 
as
	select idProfesor, nombreProfesor, apellidoProfesor,
	nombreUsuario, emailProfesor
	from Profesor
	where estado = 'Activo';
go

create procedure selectCursoTodo 
as
	select idCurso, nombreCurso
	from Curso 
go

--no ejecutar
drop database BD_SistemaProyectosGraduacion
use master