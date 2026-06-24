namespace webVentasSaaSV25.Services.Sec;

public sealed class MenuNodo
{
    public int IdModulo { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public int Nivel { get; set; }
    public string? RutaWeb { get; set; }
    public string Estado { get; set; } = "ACTIVO";
    public List<MenuNodo> Hijos { get; set; } = new();

    public bool EsProximo => Estado.Equals("PROXIMO", StringComparison.OrdinalIgnoreCase);
    public bool TieneHijos => Hijos.Count > 0;
    public bool EsHoja => Hijos.Count == 0;
}
