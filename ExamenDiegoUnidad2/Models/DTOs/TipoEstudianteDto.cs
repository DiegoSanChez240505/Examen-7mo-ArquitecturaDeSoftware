using System.ComponentModel.DataAnnotations;

namespace ExamenDiegoUnidad2.Models.DTOs
{
    public class TipoEstudianteDto
    {
        public int IdTipoEstudiante { get; set; }
        
        [Required(ErrorMessage = "El nombre del tipo de estudiante es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del tipo no puede exceder 50 caracteres")]
        public string NombreTipo { get; set; } = string.Empty;
        
        public int TotalEstudiantes { get; set; }
    }
    
    public class TipoEstudianteCreateDto
    {
        [Required(ErrorMessage = "El nombre del tipo de estudiante es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del tipo no puede exceder 50 caracteres")]
        public string NombreTipo { get; set; } = string.Empty;
    }
}