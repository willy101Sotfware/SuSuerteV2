using SuSuerteV2.Domain.Enumerables;

namespace SuSuerteV2.Domain.ApiService.Models
{
    public class UserDataChance
    {
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public ETypeIdentification TipoIdentificacion { get; set; }
        public string NumIdentificacion { get; set; }
        public char Genero { get; set; }
        public string FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Cel { get; set; } = string.Empty;

    }
}
