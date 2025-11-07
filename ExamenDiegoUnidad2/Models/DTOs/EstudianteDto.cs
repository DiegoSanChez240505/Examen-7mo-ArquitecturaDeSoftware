using System.ComponentModel.DataAnnotations;
using ExamenDiegoUnidad2.Validators;

namespace ExamenDiegoUnidad2.Models.DTOs
{
    public class EstudianteDto
    {
        public int IdEstudiante { get; set; }
        
        [Required(ErrorMessage = "La matrícula es obligatoria")]
        [StringLength(20, ErrorMessage = "La matrícula no puede exceder 20 caracteres")]
        [MatriculaFormato]
        public string Matricula { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [NombreCompleto]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [SemestreValido]
        public sbyte Semestre { get; set; }
        
        [PromedioValido]
        public decimal Promedio { get; set; }
        
        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int IdCarrera { get; set; }
        
        [Required(ErrorMessage = "El tipo de estudiante es obligatorio")]
        public int IdTipoEstudiante { get; set; }
        
        // Propiedades de navegación para mostrar información adicional
        public string? NombreCarrera { get; set; }
        public string? TipoEstudiante { get; set; }
    }
    
    public class EstudianteCreateDto
    {
        [Required(ErrorMessage = "La matrícula es obligatoria")]
        [StringLength(20, ErrorMessage = "La matrícula no puede exceder 20 caracteres")]
        [MatriculaFormato]
        public string Matricula { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [NombreCompleto]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [SemestreValido]
        public sbyte Semestre { get; set; }
        
        [PromedioValido]
        public decimal Promedio { get; set; }
        
        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int IdCarrera { get; set; }
        
        [Required(ErrorMessage = "El tipo de estudiante es obligatorio")]
        public int IdTipoEstudiante { get; set; }
    }
    
    public class EstudianteUpdateDto
    {
        [Required(ErrorMessage = "La matrícula es obligatoria")]
        [StringLength(20, ErrorMessage = "La matrícula no puede exceder 20 caracteres")]
        [MatriculaFormato]
        public string Matricula { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [NombreCompleto]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [SemestreValido]
        public sbyte Semestre { get; set; }
        
        [PromedioValido]
        public decimal Promedio { get; set; }
        
        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int IdCarrera { get; set; }
        
        [Required(ErrorMessage = "El tipo de estudiante es obligatorio")]
        public int IdTipoEstudiante { get; set; }
    }
}