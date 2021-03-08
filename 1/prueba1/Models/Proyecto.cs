namespace prueba1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Proyecto : IValidatableObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proyecto()
        {
            this.Archivo1 = new HashSet<Archivo1>();
            this.Archivo2 = new HashSet<Archivo2>();
            this.Archivo3 = new HashSet<Archivo3>();
            this.Comentario = new HashSet<Comentario>();
            this.CatalogoEntregable = new HashSet<CatalogoEntregable>();
        }

        public int idProyecto { get; set; }
        public string nombreProyecto { get; set; }
        public Nullable<int> idContacto { get; set; }
        public int idCurso { get; set; }
        public string tecnologia { get; set; }
        public int idProfesor { get; set; }
        public int idGrupo { get; set; }
        public string estadoProyecto { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime fechaInicio { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> fechaFinalizado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Archivo1> Archivo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Archivo2> Archivo2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Archivo3> Archivo3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario { get; set; }
        public virtual Curso Curso { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Grupo Grupo { get; set; }
        public virtual Profesor Profesor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatalogoEntregable> CatalogoEntregable { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (fechaFinalizado < fechaInicio)
            {
                yield return new ValidationResult(
                    errorMessage: "La fecha de finalización no puede ser antes de la fecha de inicio",
                    memberNames: new[] { "fechaFinalizado" }
               );
            }
        }
    }
}
