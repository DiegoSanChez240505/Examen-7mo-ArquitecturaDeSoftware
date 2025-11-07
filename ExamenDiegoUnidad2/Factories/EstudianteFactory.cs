using ExamenDiegoUnidad2.Data.Models;

namespace ExamenDiegoUnidad2.Factories
{
    // Factory Method Pattern
    public abstract class EstudianteFactory
    {
        public abstract EstudianteBase CrearEstudiante();
        
        // Método template que utiliza el factory method
        public virtual Estudiantes CrearEntidadEstudiante(string matricula, string nombreCompleto, 
            sbyte semestre, decimal promedio, int idCarrera, int idTipoEstudiante)
        {
            var tipoEstudiante = CrearEstudiante();
            
            return new Estudiantes
            {
                Matricula = matricula,
                NombreCompleto = nombreCompleto,
                Semestre = semestre,
                Promedio = promedio,
                IdCarreraFk = idCarrera,
                IdTipoEstudianteFk = idTipoEstudiante
            };
        }
    }
    
    public class EstudianteRegularFactory : EstudianteFactory
    {
        public override EstudianteBase CrearEstudiante()
        {
            return new EstudianteRegular();
        }
    }
    
    public class EstudianteBecadoFactory : EstudianteFactory
    {
        public override EstudianteBase CrearEstudiante()
        {
            return new EstudianteBecado();
        }
    }
    
    public class EstudianteEgresadoFactory : EstudianteFactory
    {
        public override EstudianteBase CrearEstudiante()
        {
            return new EstudianteEgresado();
        }
    }
    
    // Factory Provider - Punto de entrada principal
    public class EstudianteFactoryProvider
    {
        public static EstudianteFactory ObtenerFactory(TipoEstudianteEnum tipoEstudiante)
        {
            return tipoEstudiante switch
            {
                TipoEstudianteEnum.Regular => new EstudianteRegularFactory(),
                TipoEstudianteEnum.Becado => new EstudianteBecadoFactory(),
                TipoEstudianteEnum.Egresado => new EstudianteEgresadoFactory(),
                _ => throw new ArgumentException($"Tipo de estudiante no soportado: {tipoEstudiante}")
            };
        }
        
        public static EstudianteFactory ObtenerFactory(int idTipoEstudiante)
        {
            if (!Enum.IsDefined(typeof(TipoEstudianteEnum), idTipoEstudiante))
            {
                throw new ArgumentException($"ID de tipo de estudiante no válido: {idTipoEstudiante}");
            }
            
            return ObtenerFactory((TipoEstudianteEnum)idTipoEstudiante);
        }
        
        public static EstudianteBase CrearEstudiantePorTipo(int idTipoEstudiante)
        {
            var factory = ObtenerFactory(idTipoEstudiante);
            return factory.CrearEstudiante();
        }
    }
}