using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cnrl.Logica
{
    public static class Constantes
    {
        public const string CONN = "Cnrl";
        public const string WHERE = @"{WHERE}";
        public const string ESTADO_NO_CONFORMADA = @"No Conformada";
        public const string ESTADO_EN_FORMACION = @"En Formación";
        public const string ESTADO_CONFORMADA = @"Conformada";
        public const string ESTADO_PROVISORIA = @"Provisoria";
        public const string SIN_NOMBRE_SERVICIO = @"(SIN NOMBRE)";
        public const string SIN_RESPONSABLE = @"(SIN RESPONSABLE)";
        // Estados de Red
        public const int ID_ESTADO_NO_CONFORMADA = 0;
        public const int ID_ESTADO_EN_FORMACION = 1;
        public const int ID_ESTADO_CONFORMADA = 2;
        public const int ID_ESTADO_PROVISORIA = 3;
        // Red
        public const int ID_RED_NACIONAL = 0;
        public const string RED_NACIONAL = "nac";
        public const string RED_JURISDICCIONAL = "jur";
        // Nodos
        public const string NODO_TIPO_TODO = "";
        public const string NODO_TIPO_INS = "instituto";
        public const string NODO_TIPO_JUR = "jurisdiccion";
        public const string NODO_TIPO_RED = "red";
        public const string NODO_TIPO_EST = "redestado";
        // Varios
        public const int ERROR = -1;
        public const string A_Z = "A-Z";

        public const string COMPLEJIDAD_SELECCIONE = @"-- Seleccione --";
        public const string COMPLEJIDAD_BAJA = @"I";
        public const string COMPLEJIDAD_MEDIABAJA = @"II";
        public const string COMPLEJIDAD_MEDIA = @"III";
        public const string COMPLEJIDAD_MEDIAALTA = @"IV";
        public const string COMPLEJIDAD_ALTA = @"V";

        public const string ESTADO_CUOTA_IMPAGA = @"0";
        public const string ESTADO_CUOTA_PAGA = @"P";
        public const string ESTADO_CUOTA_BAJA = @"B";

        public static string AsQueryString(this IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!parameters.Any())
                return "";

            var builder = new StringBuilder("?");

            var separator = "";
            foreach (var kvp in parameters.Where(kvp => kvp.Value != null))
            {
                builder.AppendFormat("{0}{1}={2}", separator, WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value.ToString()));

                separator = "&";
            }

            return builder.ToString();
        }

        public static string descripcionCompleja(oferta x)
        {
            return string.Format("({0} - {1}) {2} - {3} - {4} [{5} {6} a {7}]", (x.periodolectivo1 != null) ? x.periodolectivo1.anio.ToString() : "",
                                                         (x.periodolectivo1 != null) ? x.periodolectivo1.periodo.ToString() : "",
                                                         (x.curso1 != null && x.curso1.tipocurso1 != null) ? x.curso1.tipocurso1.descripcion : "",
                                                        (x.curso1 != null) ? x.curso1.descripcion : "",
                                                        (x.sede1 != null) ? x.sede1.nombre : "",
                                                        ((x.lunes) ? "LU " : "")
                                                                + ((x.martes) ? "MA " : "")
                                                                + ((x.miercoles) ? "MI " : "")
                                                                + ((x.jueves) ? "JU " : "")
                                                                + ((x.viernes) ? "VI " : "")
                                                                + ((x.sabado) ? "SA " : ""),
                                                        (x.horaDesde != null && x.horaDesde.HasValue) ? x.horaDesde.Value.ToString(@"hh\:mm") : "",
                                                        (x.horaHasta != null && x.horaHasta.HasValue) ? x.horaHasta.Value.ToString(@"hh\:mm") : "");
        }

        public static string descripcionHorario(oferta x)
        {
            return string.Format("{0} {1} a {2}",
                                                        ((x.lunes) ? "LU " : "")
                                                    + ((x.martes) ? "MA " : "")
                                                    + ((x.miercoles) ? "MI " : "")
                                                    + ((x.jueves) ? "JU " : "")
                                                    + ((x.viernes) ? "VI " : "")
                                                    + ((x.sabado) ? "SA " : ""),
                                                         x.horaDesde != null ? x.horaDesde.Value.ToString(@"hh\:mm") : "",
                                                    x.horaHasta != null ? x.horaHasta.Value.ToString(@"hh\:mm") : "");
        }

        public static string descripcionCompleja(curso x)
        {
            return string.Format("({0}) {1}", x.tipocurso1.descripcion, x.descripcion);
        }

        public static string descripcionCompleja(periodolectivo x)
        {
            return string.Format("{0} - {1} ({2})", x.anio, x.periodo, x.tipocurso1.descripcion);
        }

        public static string descripcionCompleja(AspNetUsers x)
        {
            return string.Format("{0}, {1} ({2})", x.Apellido, x.Nombre, x.NroDocumento);
        }
    }

    public enum EstadoRed
    {
        NoConformada = 0,
        EnFormacion,
        Conformada,
        Provisoria
    }

    public enum TiposAlumno
    {
        NoAlumno = 0,
        Alumno,
        Empleado,
        Mayor60,
        Graduado
    }

    public enum EstadoOferta
    {
        Habilitada = 1,
        NoHabilitada = 2
    }

    public enum EstadoCuota
    {
        Paga = 'P',
        Impaga = '0',
        Baja = 'B'
    }

    public enum GrupoEtareo
    {
        Grupo1_10_a_16 = 1,
        Grupo2_17_a_25 = 2,
        Grupo3_26_a_35 = 3,
        Grupo4_Mayor_a_35 = 4,
        //DiezADieciseis = 1,
        //DiesisieteAVeinticinco = 2,
        //VeintiseisATreintaYCinco = 3,
        //MayorDe35 = 4
    }

    public enum Sectores
    {
        Idiomas = 1,
        Empresas = 2,
        Graduados = 3
    }

    public enum Roles
    {
        SuperAdministrador = 1,
        Administrador = 2,
        Extension = 3,
        Ventanilla = 4,
        Docente = 5,
        Alumno = 6,
        coordinadorDocente = 7
    }


    public enum Complejidades
    {
        Baja = 0,
        MediaBaja = 1,
        Media = 2,
        MediaAlta = 3,
        Alta = 4,
    }

    public enum EstadosCursada
    {
        Inscripto = 0,
        Confirmado = 1,
        NoConfirmado = 2,
        Rechazado = 3,
        Nivelacion = 4,
        SolicitudBaja = 5,
        BajaAceptada = 6,
        FormaPago = 7,
        PlanGenerado = 8,
        PreInscripcion = 9,
        Migrado = 10,
        Becado = 12
    }

    public enum Dias
    {
        Lunes = 2,
        Martes = 3,
        Miercoles = 4,
        Jueves = 5,
        Viernes = 6,
        Sabado = 7
    }

    public enum Aprobado
    {
        Si = 0,
        No = 1
    }

}
