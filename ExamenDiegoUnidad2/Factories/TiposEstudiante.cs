using ExamenDiegoUnidad2.Data.Models;

namespace ExamenDiegoUnidad2.Factories
{
    public enum TipoEstudianteEnum
    {
        Regular = 1,
        Becado = 2,
        Egresado = 3
    }
    
    public abstract class EstudianteBase
    {
        public abstract string ObtenerDescripcion();
        public abstract decimal CalcularDescuentoMatricula(decimal montoBase);
        public abstract bool PuedeRecibirBecas();
        public abstract int ObtenerPrioridadInscripcion();
    }
    
    public class EstudianteRegular : EstudianteBase
    {
        public override string ObtenerDescripcion()
        {
            return "Estudiante regular con todas las obligaciones académicas y administrativas vigentes";
        }
        
        public override decimal CalcularDescuentoMatricula(decimal montoBase)
        {
            return montoBase; // Sin descuento
        }
        
        public override bool PuedeRecibirBecas()
        {
            return true;
        }
        
        public override int ObtenerPrioridadInscripcion()
        {
            return 2; // Prioridad normal
        }
    }
    
    public class EstudianteBecado : EstudianteBase
    {
        public override string ObtenerDescripcion()
        {
            return "Estudiante con beca académica o socioeconómica, con beneficios especiales";
        }
        
        public override decimal CalcularDescuentoMatricula(decimal montoBase)
        {
            return montoBase * 0.5m; // 50% de descuento
        }
        
        public override bool PuedeRecibirBecas()
        {
            return false; // Ya tiene beca
        }
        
        public override int ObtenerPrioridadInscripcion()
        {
            return 1; // Prioridad alta
        }
    }
    
    public class EstudianteEgresado : EstudianteBase
    {
        public override string ObtenerDescripcion()
        {
            return "Estudiante que ha completado su programa académico";
        }
        
        public override decimal CalcularDescuentoMatricula(decimal montoBase)
        {
            return 0; // No paga matrícula
        }
        
        public override bool PuedeRecibirBecas()
        {
            return false;
        }
        
        public override int ObtenerPrioridadInscripcion()
        {
            return 3; // Prioridad baja (solo para cursos especiales)
        }
    }
}