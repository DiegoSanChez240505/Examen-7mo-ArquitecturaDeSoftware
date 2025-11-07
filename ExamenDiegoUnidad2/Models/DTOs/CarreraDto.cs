using System.ComponentModel.DataAnnotations;

namespace ExamenDiegoUnidad2.Models.DTOs
{
    public class CarreraDto
    {
        public int IdCarrera { get; set; }
        
        [Required(ErrorMessage = "El nombre de la carrera es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la carrera no puede exceder 100 caracteres")]
        public string NombreCarrera { get; set; } = string.Empty;
        
        public int TotalEstudiantes { get; set; }
    }
    
    public class CarreraCreateDto
    {
        [Required(ErrorMessage = "El nombre de la carrera es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la carrera no puede exceder 100 caracteres")]
        public string NombreCarrera { get; set; } = string.Empty;
    }
}