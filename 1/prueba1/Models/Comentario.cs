//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace prueba1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Comentario
    {
        public int idComentario { get; set; }
        [DataType(DataType.Date)] 
        public System.DateTime fecha { get; set; }
        public string contenido { get; set; }
        public int idProyecto { get; set; }
    
        public virtual Proyecto Proyecto { get; set; }
    }
}
