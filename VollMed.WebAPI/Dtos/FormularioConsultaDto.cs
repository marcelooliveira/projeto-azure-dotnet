namespace VollMed.Web.Dtos
{
    [Serializable]
    public class FormularioConsultaDto
    {
        public ConsultaDto Consulta { get; set; }
        public IEnumerable<MedicoDto> Medicos { get; set; }
    }
}